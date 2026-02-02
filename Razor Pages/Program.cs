using Backend.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Inicializamos los servicios, repos y base de datos del proyecto Backend
builder.Services.AddDatabase();
builder.Services.AddRepositoriesAndServices();

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

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

//Poblamos la base de datos con los datos de prueba
app.SeedDatabase();

app.Run();