using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContextPool<ApplicationDbContext>(builder =>
        {
            builder.UseNpgsql(ApplicationDbContext.ConnectionString);
        });
        
        services.AddPooledDbContextFactory<ApplicationDbContext>(builder =>
        {
            builder.UseNpgsql(ApplicationDbContext.ConnectionString);
        });
        
        return services;
    }
}