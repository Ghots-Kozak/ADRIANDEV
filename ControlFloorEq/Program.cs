using Abixe_SapServiceLayer;
using Context;
using Control_Piso.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using PdfSharp.Charting;
using Microsoft.Extensions.Configuration;
using ControlFloor.Models;
using ControlFloor.Middleware;
using ControlFloor.Services;
using Microsoft.AspNetCore.SignalR;
using ControlFloor.Hubs;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
var cultureInfo = new CultureInfo("es-MX");
cultureInfo.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
// Agregar servicios al contenedor.
builder.Services.AddSignalR();  // Aquí se agrega SignalR
// Acceder a la configuración
Microsoft.Extensions.Configuration.ConfigurationManager configuration = builder.Configuration;


var sapSettings = configuration.GetSection("SAP");

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ApplicationDbContext>(db => new ApplicationDbContext
{
    ServidorDB = sapSettings.GetValue<string>("ServerDB"),
    Tipo = (ContextDB.eType)sapSettings.GetValue<int>("TypeDB"),
    DB = sapSettings.GetValue<string>("DbApp"),
    UserDB = sapSettings.GetValue<string>("UserDB"),
    PwdDB = sapSettings.GetValue<string>("PwdDB"),
    Abi_Key = sapSettings.GetValue<string>("LicAbikey"),
    Abi_UrlServerRPT = sapSettings.GetValue<string>("ServicesRPT")
});

builder.Services.AddSingleton(new cCoDeEnc(sapSettings.GetValue<string>("LicAbikey")));

builder.Services.AddTransient<SapServiceLayer>(provider =>
{
    var currentCompany = sapSettings.GetSection("Companys").Get<List<ServLayerCompanys>>()[0];

    // Configurar el servicio SAP Service Layer con WebRequest
    return new SapServiceLayer(
        currentCompany.Srvl_Url,
        currentCompany.Company,
        currentCompany.UserName,
        new cCoDeEnc(sapSettings.GetValue<string>("LicAbikey")).Decrypt(currentCompany.Password)
    );
});

// Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Tiempo de expiración de la cookie
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        // Expiración deslizante
        options.SlidingExpiration = true;
        // Redirección cuando no esté autenticado
        options.LoginPath = "/Accounts/Login";  // Asegúrate que esto coincida con la ruta del controlador
        options.LogoutPath = "/Accounts/Logout";
        options.Cookie.HttpOnly = true;  // Asegura que la cookie solo sea accesible a través de HTTP (no en JS)
        options.Cookie.SameSite = SameSiteMode.Strict;  // Protege contra ataques CSRF
    });

// Configuración de sesiones
builder.Services.AddSession(options =>
{
    // Tiempo de expiración de la sesión
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true; // Hace la cookie solo accesible por el servidor
    options.Cookie.IsEssential = true; // Marca la cookie como esencial para la funcionalidad
});

builder.Services.AddDataProtection();
// Registro de servicios adicionales
builder.Services.AddScoped<TokenService>();
builder.Services.AddTransient<SapProductionOrderService>();
builder.Services.AddTransient<SapPallets>();
builder.Services.AddTransient<SapDecrease>();
builder.Services.AddTransient<SapServiceCall>();

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();


var app = builder.Build();

var supportedCultures = new[] { cultureInfo };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});
// Habilitar archivos estáticos
app.UseStaticFiles();

app.UseMiddleware<GlobalLoggingMiddleware>();
// Middleware de manejo de errores
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Fuerza el uso de HTTPS
}
else
{
    app.UseDeveloperExceptionPage(); // Muestra errores detallados en desarrollo
}

app.UseHttpsRedirection();
app.UseRouting();

// Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Habilita el uso de controladores
    endpoints.MapHub<MessageHub>("/basculahub");  // Aquí mapeas tu Hub SignalR
});


// Habilitar sesiones
app.UseSession(); // Importante si usas sesiones

// Redirige la raíz ("/") a "/Accounts/Login" si no está autenticado
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" && !context.User.Identity.IsAuthenticated)
    {
        // Si no está autenticado, redirige a la página de login
        context.Response.Redirect("/Accounts/Login");
        return; // No continuar con el siguiente middleware
    }
    else if (context.Request.Path == "/" && context.User.Identity.IsAuthenticated)
    {
        // Si está autenticado, redirige a la página principal
        context.Response.Redirect("/Home/Index");
        return; // No continuar con el siguiente middleware
    }

    await next(); // Continua si la condición no se cumple
});

// Configurar rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Accounts}/{action=Login}/{id?}");

app.Run();

