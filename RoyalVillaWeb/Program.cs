using Microsoft.AspNetCore.Authentication.Cookies;
using RoyalVIlla.DTO;
using RoyalVillaWeb.Services;
using RoyalVillaWeb.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddAutoMapper(o =>
{
    o.CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
    o.CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/auth/accessdenied";
    });

var villaApiUrl = builder.Configuration["ServiceUrls:VillaAPI"]
    ?? throw new InvalidOperationException("ServiceUrls:VillaAPI is not configured.");

if (!Uri.TryCreate(villaApiUrl, UriKind.Absolute, out var villaApiUri))
{
    throw new InvalidOperationException("ServiceUrls:VillaAPI must be an absolute URL.");
}

builder.Services.AddHttpClient("RoyalVillaAPI", client =>
{
    client.BaseAddress = villaApiUri;

    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<IVillaService, VillaService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
