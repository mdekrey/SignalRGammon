using SignalRGame.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Controllers
{
    public class GameDiscoveryController : GameApiControllerBase
    {
        private readonly IGameServers gameServerDetails;

        public GameDiscoveryController(IGameServers gameServerDetails)
        {
            this.gameServerDetails = gameServerDetails;
        }

        public override async Task<TypeSafeGetGameByIdResult> GetGameByIdTypeSafe(string id, string type)
        {
            var serverUrl = await gameServerDetails.FindGameServerAsync(id, type);

            if (serverUrl == null)
                return TypeSafeGetGameByIdResult.StatusCode404();
            return TypeSafeGetGameByIdResult.StatusCode200(new ServerDefinition(serverUrl));
        }

        public override async Task<TypeSafeCreateGameResult> CreateGameTypeSafe(string gameType)
        {
            var maybeGame = await gameServerDetails.CreateGameAsync(gameType);

            return maybeGame switch
            {
                null => TypeSafeCreateGameResult.StatusCode400(),
                GameDetails game => TypeSafeCreateGameResult.StatusCode200(new Game(
                    game.Id,
                    gameType,
                    new ServerDefinition(game.ServerUrl)
                ))
            };
        }

        public override async Task<TypeSafeGetGamesResult> GetGamesTypeSafe()
        {
            return TypeSafeGetGamesResult.StatusCode200(await gameServerDetails.GetAllGameTypesAsync());

        }
    }
}
