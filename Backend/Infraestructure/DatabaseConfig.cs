using Backend.DataBase;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infraestructure;

public static class DatabaseConfig
{
    
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        //BBDD de Funkos y categorías
        services.AddDbContext<Context>(options =>
            options.UseInMemoryDatabase("FunkoDatabase"));

        //BBDD de Usuarios
        services.AddIdentity<User, IdentityRole<long>>(options => 
            {
                // Configuraciones opcionales de contraseña, etc.
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders();
        
        return services;
        
        
    }
}