using SignalRGame.Clients;
using SignalRGame.Discovery;
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
            var server = await gameServerDetails.FindGameServerAsync(id, type);

            return server switch {
                ServerDetails { PublicUrl: var serverUrl } => TypeSafeGetGameByIdResult.StatusCode200(new ServerDefinition(serverUrl)),
                _ => TypeSafeGetGameByIdResult.StatusCode404()
            };
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

        public override async Task<TypeSafeGetGameTypesResult> GetGameTypesTypeSafe()
        {
            return TypeSafeGetGameTypesResult.StatusCode200(await gameServerDetails.GetAllGameTypesAsync());

        }
    }
}
