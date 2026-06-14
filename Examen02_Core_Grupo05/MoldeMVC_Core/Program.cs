using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;
using MoldeMVC_Core.Roles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Registrar contexto del catálogo académico
builder.Services.AddDbContext<CatalogoMateriaBdCoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogoConnection")));

// Sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
// .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Desactivar los requisitos de la contrase�a
    options.Password.RequireDigit = false; // No requiere d�gitos
    options.Password.RequireLowercase = false; // No requiere min�sculas
    options.Password.RequireUppercase = false; // No requiere may�sculas
    options.Password.RequireNonAlphanumeric = false; // No requiere caracteres especiales
    options.Password.RequiredLength = 3; // Establecer una longitud m�nima de 3 caracteres

    // Requerimos la confirmaci�n de cuenta para iniciar sesi�n
    options.SignIn.RequireConfirmedAccount = false;

    // Configuramos la validaci�n de correo
    options.User.RequireUniqueEmail = false;
})
    //Soporte para roles
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();

// ********************************************** Crea la pol�tica *******************************************
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloUsuariosAdmins", policy =>
        policy.RequireAssertion(context =>
            context.User.Identity != null &&
            (context.User.Identity.Name == "Administrador"
             || context.User.Identity.Name == "SuperAdmin")
        ));
});
//************************************************************************************************************

var app = builder.Build();

// **Aplicar Migraciones Autom�ticamente**
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Aplica migraciones pendientes
    }
    catch (Exception ex)
    {
        // Registra errores en el log en caso de que falle
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Inicializar Roles y Usuarios
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RolesUsuarios.InitializeRoles(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

#if NET9_0_OR_GREATER
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();
#else
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
#endif

app.Run();
