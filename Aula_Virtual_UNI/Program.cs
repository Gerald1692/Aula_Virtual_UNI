using AulaVirtualDAL;

var builder = WebApplication.CreateBuilder(args);

// MVC con serialización JSON en camelCase
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// SQL Server connection factory
builder.Services.AddSingleton<Aula_Virtual_UNI.Data.ISqlConnectionFactory, Aula_Virtual_UNI.Data.SqlConnectionFactory>();
builder.Services.AddScoped<Login>();
builder.Services.AddScoped<ProyectosDAL>();
builder.Services.AddScoped<TareasDAL>();
builder.Services.AddScoped<UsuariosDAL>();
builder.Services.AddScoped<RegistroDAL>();

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

