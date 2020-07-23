using Microsoft.Extensions.DependencyInjection.Extensions;
using SignalRGame.GameUtilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameServicesExtensions
    {
        public static IServiceCollection AddGameFactory(this IServiceCollection services)
        {
            services.TryAddTransient<IGameFactory, GameFactory>();
            return services;
        }
    }
}
