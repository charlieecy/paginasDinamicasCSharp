using Backend.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Inyectamos el Backend
builder.Services.AddDatabase();
builder.Services.AddRepositoriesAndServices();

//Para que funcionen las sesiones
builder.Services.AddDistributedMemoryCache();

//Añades el servicio de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

//Para el storage
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

//Para las sesiones
app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Funkos}/{action=Index}/{id?}")
    .WithStaticAssets();

//Poblamos la base de datos con los datos de prueba
app.SeedDatabase();

app.Run();