using SignalRGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRGame.Controllers
{
    public class GameController : GameApiControllerBase
    {
        private readonly ISingleGameFactory singleGameFactory;
        private readonly IReadOnlyList<string> types;

        public GameController(ISingleGameFactory singleGameFactory)
        {
            this.singleGameFactory = singleGameFactory;
            this.types = new[] { singleGameFactory.Type };
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
