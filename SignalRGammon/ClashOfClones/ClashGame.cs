using Newtonsoft.Json;
using SignalRGammon.GameUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Clash
{
    public class ClashGame : GameBase<ClashState, ClashState, ClashAction?>, IGame
    {

        public ClashGame() : base(Defaults.DefaultState)
        {
        }

        protected override ClashState GetExternalState(ClashState state) => state;
        protected override Task<(ClashState newState, bool isValid)> ApplyAction(ClashState state, ClashAction? action) =>
            Task.FromResult(Rules.ApplyAction(state, action));
        protected override Task CheckAutomaticActions(ClashState state) =>
            Rules.CheckAutomaticActions(state, Do);
    }
}
