using Microsoft.Extensions.DependencyInjection;
using WebApi.Application.Services;

namespace WebApi.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<EntityService>();
        return services;
    }
}