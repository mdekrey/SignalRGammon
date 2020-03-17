using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SignalRGammon
{
    public interface IGame
    {
        TimeSpan SlidingExpiration { get; }

        IObservable<string> States { get; }

        Task<bool> Do(string messageJson);
    }


    public interface IGame<TState, TAction> : IGame
    {
        JsonSerializerSettings JsonSettings { get; }

        IObservable<string> IGame.States => States.Select(s => JsonConvert.SerializeObject(s, JsonSettings));

        Task<bool> IGame.Do(string messageJson) => Do(JsonConvert.DeserializeObject<TAction>(messageJson, JsonSettings));

        new IObservable<TState> States { get; }

        Task<bool> Do(TAction action);
    }
}