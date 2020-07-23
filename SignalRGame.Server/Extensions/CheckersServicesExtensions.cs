using SignalRGame.Checkers;
using SignalRGame.GameUtilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CheckersServicesExtensions
    {
        public static IServiceCollection AddCheckers(this IServiceCollection services) =>
            services.AddTransient<ISingleGameFactory, CheckersGameFactory>();
    }
}
