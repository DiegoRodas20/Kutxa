using Kutxa.Domain.Users;
using Kutxa.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Kutxa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, UserRepository>();

        return services;
    }
}
