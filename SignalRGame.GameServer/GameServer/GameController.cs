using SignalRGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRGame.GameServer
{
    public class GameController : GameApiControllerBase
    {
        private readonly ILocalizedGameFactory singleGameFactory;
        private readonly IReadOnlyList<GameType> types;

        public GameController(ILocalizedGameFactory singleGameFactory)
        {
            this.singleGameFactory = singleGameFactory;
            this.types = new[]
            {
                new GameType(singleGameFactory.IconUrl, singleGameFactory.DisplayName, singleGameFactory.Type)
            };
        }

        public override Task<TypeSafeCreateGameResult> CreateGameTypeSafe(string type)
        {
            throw new NotImplementedException();
        }

        public override Task<TypeSafeGetGameIdExistsResult> GetGameIdExistsTypeSafe(string id)
        {
            throw new NotImplementedException();
        }

        public override Task<TypeSafeGetGameTypesResult> GetGameTypesTypeSafe()
        {
            return Task.FromResult(
                TypeSafeGetGameTypesResult.StatusCode200(types)
            );
        }
    }
}
