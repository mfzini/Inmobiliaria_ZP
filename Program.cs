using inmobiliaria.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<PersonaRepository>();
builder.Services.AddScoped<ImagesRepo>();
builder.Services.AddScoped<InmuebleRepository>();
builder.Services.AddScoped<ReservaRepo>();
builder.Services.AddScoped<TipoInmuebleRepo>();
builder.Services.AddScoped<RepoPagos>();
builder.Services.AddScoped<UsuariosRepo>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout"; 
        options.AccessDeniedPath = "/Home/Restringido"; 
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
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
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
