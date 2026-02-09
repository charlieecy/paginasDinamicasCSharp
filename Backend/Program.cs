// 1. EL BUILDER: Configuramos nuestro contenedor de dependencias (Servicios)


using Backend.Infraestructure;


var builder = WebApplication.CreateBuilder(args);


// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.LogoutPath = "/Logout";
});


// EL BUILDER: Configuramos nuestro contenedor de dependencias (Servicios)
// Inyección de Dependencias
builder.Services.AddDatabase();
builder.Services.AddRepositoriesAndServices();
builder.Services.AddMemoryCache();

// CONSTRUCCIÓN DE LA APP
// Una vez que hemos terminado de configurar los servicios, construimos nuestra aplicación
var app = builder.Build();

// INICIALIZACIÓN DE DATOS (Seed)
app.SeedDatabase();

//Para el Storage
app.UseStaticFiles();

app.Run();