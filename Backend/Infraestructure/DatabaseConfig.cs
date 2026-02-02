using Backend.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infraestructure;

public static class DatabaseConfig
{
    
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<Context>(options =>
            options.UseInMemoryDatabase("FunkoDatabase"));

        return services;
    }
}