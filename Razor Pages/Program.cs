using Backend.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Inicializamos los servicios, repos y base de datos del proyecto Backend
builder.Services.AddDatabase();
builder.Services.AddRepositoriesAndServices();

//Sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".FunkoWorld.Session";
});

//Login
builder.Services.ConfigureApplicationCookie(options =>
{
    // Si el usuario no está autenticado, va aquí:
    options.LoginPath = "/Login"; 
    
    // Si el usuario está autenticado pero no tiene permisos (ej: es User e intenta borrar), va aquí:
    options.AccessDeniedPath = "/Login"; 
    
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

//Roles
builder.Services.AddAuthorization(options =>
{
    // Política Simple: Solo requiere un Rol
    options.AddPolicy("EsAdmin", policy => policy.RequireRole("Admin"));
    
    options.AddPolicy("EsUser", policy => policy.RequireRole("User"));
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Para que al lanzar un notFound, nos redirija a /Error
app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

//Poblamos la base de datos con los datos de prueba
//Funkos y categorías:
app.SeedDatabase();
//Usuarios y roles:
await IdentitySeeder.SeedIdentityAsync(app);

app.Run();