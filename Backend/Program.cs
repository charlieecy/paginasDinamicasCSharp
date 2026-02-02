// 1. EL BUILDER: Configuramos nuestro contenedor de dependencias (Servicios)

using Backend.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

// 1. EL BUILDER: Configuramos nuestro contenedor de dependencias (Servicios)
// Inyección de Dependencias
builder.Services.AddDatabase();
builder.Services.AddRepositoriesAndServices();
builder.Services.AddMemoryCache();

// 2. CONSTRUCCIÓN DE LA APP
// Una vez que hemos terminado de configurar los servicios, construimos nuestra aplicación
var app = builder.Build();

// 3. INICIALIZACIÓN DE DATOS (Seed)
app.SeedDatabase();

//Para el Storage
app.UseStaticFiles();

app.Run();