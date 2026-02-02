using Backend.Repository;
using Backend.Service;
using Backend.Storage;

namespace Backend.Infraestructure;

public static class DependencyInjectionConfig
{

    public static IServiceCollection AddRepositoriesAndServices(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<IFunkoRepository, FunkoRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        
        // Servicios
        services.AddScoped<IFunkoService, FunkoService>();
        services.AddScoped<ICategoryService, CategoryService>();
        
        // Storage
        services.AddScoped<IFunkoStorage, FunkoStorageService>();

        return services;
    }
}