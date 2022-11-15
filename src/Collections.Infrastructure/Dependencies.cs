using Collections.Application.Interfaces;
using Collections.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ICollectionsDbContext, CollectionsDbContext>(c => c.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));

        return services;
    }
}