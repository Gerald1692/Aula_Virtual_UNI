using AulaVirtualDAL;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// SQL Server connection factory
builder.Services.AddSingleton<Aula_Virtual_UNI.Data.ISqlConnectionFactory, Aula_Virtual_UNI.Data.SqlConnectionFactory>();
builder.Services.AddScoped<Login>();
builder.Services.AddScoped<ProyectosDAL>();

builder.Services.AddScoped(_ => new TareasDal(builder.Configuration.GetConnectionString("ConexionDB")
    ?? throw new InvalidOperationException("Connection string 'ConexionDB' not found.")));

// 🔹 Habilitar sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "AulaVirtual.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 Agregar manejo de sesión
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();

