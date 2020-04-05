using Newtonsoft.Json;
using SignalRGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Checkers
{
    public class CheckersGame : GameBase<CheckersExternalState, CheckersState, CheckersAction?>, IGame
    {
        public CheckersGame() : base(Defaults.DefaultState)
        {
        }

        IObservable<string> IGame.JsonStates =>
            States
                .Select(s => JsonConvert.SerializeObject(new { state = s.state.State, validMovesForCurrentPlayer = s.state.ValidMovesForCurrentPlayer, s.action }, JsonSettings));

        protected override CheckersExternalState GetExternalState(CheckersState state) => new CheckersExternalState(state);
        protected override Task<(CheckersState newState, bool isValid)> ApplyAction(CheckersState state, CheckersAction? action) =>
            Task.FromResult(Rules.ApplyAction(state, action));
        protected override Task CheckAutomaticActions(CheckersState state) =>
            Rules.CheckAutomaticActions(state, Do) ?? Task.CompletedTask;
    }
}
