using SignalRGame.Backgammon;
using SignalRGame.GameUtilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BackgammonServicesExtensions
    {
        public static IServiceCollection AddBackgammon(this IServiceCollection services) =>
            services.AddTransient<ISingleGameFactory, BackgammonGameFactory>();
    }
}
