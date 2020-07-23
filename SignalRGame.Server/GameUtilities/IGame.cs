using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SignalRGame.GameUtilities
{
    public interface IGame
    {
        TimeSpan SlidingExpiration { get; }

        IObservable<string> JsonStates { get; }

        Task<bool> Do(string messageJson);
    }


    public interface IGame<TState, TAction> : IGame
    {
        JsonSerializerSettings JsonSettings { get; }

        IObservable<string> IGame.JsonStates =>
            States
                .Select(s => JsonConvert.SerializeObject(new { s.state, s.action }, JsonSettings));

        Task<bool> IGame.Do(string messageJson) => Do(JsonConvert.DeserializeObject<TAction>(messageJson, JsonSettings));

        IObservable<(TState state, TAction action)> States { get; }

        Task<bool> Do(TAction action);
    }
}