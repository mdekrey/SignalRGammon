using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Linq;
using Microsoft.Azure.Storage.Blob;
using SignalRGame.GameUtilities;
using System.Collections.Generic;
using SignalRGame.Checkers;
using System.Text.Json;
using System.Text;
using SignalRGame.Backgammon;

namespace SignalRGame
{
    public static class GameFunctions
    {
        private const string SignalRHubName = "games";
        private const string BlobStorageContainerName = "games";
        private const string BlobStorageBlobName = "{headers.x-game-id}.json";
        private const string BlobStorageBlobFullName = BlobStorageContainerName + "/" + BlobStorageBlobName;

        public struct BlobData
        {
            public string GameType { get; init; }
            public string State { get; init; }
        }

        private static readonly IReadOnlyDictionary<string, Func<IGameLogic>> gameTypes = new Dictionary<string, Func<IGameLogic>>
        {
            { "checkers", () => new CheckersGame() },
            { "backgammon", () => new BackgammonGame(new DieRoller()) },
        };

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = SignalRHubName, UserId = "{headers.x-gamer-id}")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("Cleanup")]
        public static async Task Cleanup([TimerTrigger("0 0 */4 * * *")] TimerInfo myTimer, ILogger log,
            [Blob(BlobStorageContainerName, FileAccess.Read)] CloudBlobContainer blobContainer)
        {
            BlobContinuationToken? continuationToken = null;
            do
            {
                var response = await blobContainer.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                foreach (var blob in response.Results)
                {
                    if (blob is CloudBlob cloudBlob &&
                        cloudBlob is { Properties: { LastModified: DateTimeOffset lastModified } } &&
                        lastModified < DateTimeOffset.Now.AddHours(-1))
                    {
                        await cloudBlob.DeleteIfExistsAsync();
                    }
                }
            }
            while (continuationToken != null);

        }

        [FunctionName("createGame")]
        public static async Task<IActionResult> CreateGame(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalR(HubName = SignalRHubName)] IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            [SignalR(HubName = SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessages,
            [Blob(BlobStorageContainerName, FileAccess.Read)] CloudBlobContainer blobContainer)
        {
            var gamerId = req.Headers["x-gamer-id"].Single();
            var gameType = await req.ReadAsStringAsync();
            if (!gameTypes.ContainsKey(gameType))
                return new BadRequestResult();

            var gameId = Guid.NewGuid().ToString();

            // save new game
            var gameLogic = gameTypes[gameType]();
            var state = gameLogic.InitialState();

            await blobContainer.CreateIfNotExistsAsync();
            var blob = blobContainer.GetBlockBlobReference(BlobStorageBlobName.Replace("{headers.x-game-id}", gameId));
            blob.Properties.ContentType = "application/json";

            using var write = await blob.OpenWriteAsync();
            await System.Text.Json.JsonSerializer.SerializeAsync(write, new BlobData  { GameType = gameType, State = gameLogic.FromState(state) });

            // add user to the group
            await signalRGroupActions.AddAsync(
                new SignalRGroupAction
                {
                    UserId = gamerId,
                    GroupName = gameId,
                    Action = GroupAction.Add
                });

            await signalRMessages.AddAsync(CreatePublicStateMessage(gameId, state, null, gameLogic));

            // tell the user the new game id
            return new OkObjectResult(System.Text.Json.JsonSerializer.Serialize(gameId));
        }


        [FunctionName("getGameState")]
        public static async Task<IActionResult> GetGame(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [SignalR(HubName = SignalRHubName)] IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            [SignalR(HubName = SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessages,
            [Blob(BlobStorageBlobFullName, FileAccess.Read)] string gameState)
        {
            var gamerId = req.Headers["x-gamer-id"].Single();
            var gameId = req.Headers["x-game-id"].Single();

            // get game
            var blobData = System.Text.Json.JsonSerializer.Deserialize<BlobData>(gameState);
            var gameLogic = gameTypes[blobData.GameType]();
            var state = gameLogic.ToState(blobData.State);

            // add user to the group
            await signalRGroupActions.AddAsync(
                new SignalRGroupAction
                {
                    UserId = gamerId,
                    GroupName = gameId,
                    Action = GroupAction.Add
                });

            await signalRMessages.AddAsync(CreatePublicStateMessage(gameId, state, null, gameLogic, message => message.UserId = gamerId));

            return new OkResult();
        }

        [FunctionName("doAction")]
        public static async Task<IActionResult> DoAction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalR(HubName = SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessages,
            [Blob(BlobStorageBlobFullName, FileAccess.ReadWrite)] CloudBlockBlob blob)
        {
            var gamerId = req.Headers["x-gamer-id"].Single();
            var gameId = req.Headers["x-game-id"].Single();

            // get game
            using var blobReader = await blob.OpenReadAsync();
            var blobData = await System.Text.Json.JsonSerializer.DeserializeAsync<BlobData>(blobReader);
            var gameLogic = gameTypes[blobData.GameType]();
            var state = gameLogic.ToState(blobData.State);

            // get action
            var actionJson = await req.ReadAsStringAsync();
            var action = gameLogic.ToAction(actionJson);

            var (next, valid) = gameLogic.PerformAction(state, action, null);
            if (valid)
            {
                await signalRMessages.AddAsync(CreatePublicStateMessage(gameId, next, action, gameLogic));
                while (gameLogic.GetRecommendedAction(next, null) is (var newAction, true))
                {
                    (next, valid) = gameLogic.PerformAction(next, newAction, null);
                    if (valid)
                    {
                        await signalRMessages.AddAsync(CreatePublicStateMessage(gameId, next, newAction, gameLogic));
                    }
                    else
                    {
                        break;
                    }
                }

                using var write = await blob.OpenWriteAsync();
                await System.Text.Json.JsonSerializer.SerializeAsync(write, new BlobData { GameType = blobData.GameType, State = gameLogic.FromState(next) });
            }

            return new OkObjectResult(valid);
        }

        private static SignalRMessage CreatePublicStateMessage(string gameId, GameState state, GameAction? action, IGameLogic gameLogic, Action<SignalRMessage>? initializer = null)
        {
            initializer ??= message => message.GroupName = gameId;
            var result = new SignalRMessage
            {
                Target = "NewPublicState",
                Arguments = new[] { gameId, gameLogic.ToPublicGameState(state, action, null) }
            };
            initializer(result);
            return result;
        }
    }
}
