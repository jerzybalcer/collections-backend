using Collections.Application.Interfaces;
using Collections.Infrastructure.ImageStorage;
using Collections.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ICollectionsDbContext, CollectionsDbContext>(c => c.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));
        services.AddAzureClients(builder => builder.AddBlobServiceClient(configuration.GetConnectionString("ImageStorageConnection")));
        services.AddScoped<IImageStorageService, ImageStorageService>();

        return services;
    }
}