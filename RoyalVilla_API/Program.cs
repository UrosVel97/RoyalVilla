using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RoyalVilla_API.Data;
using RoyalVIlla.DTO;
using RoyalVilla_API.Models;
using RoyalVilla_API.Services;
using Scalar.AspNetCore;
using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtSettings")["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

using var tempProvider = builder.Services.BuildServiceProvider();
var buildProvider = tempProvider.GetRequiredService<IApiVersionDescriptionProvider>();

foreach (var descrription in buildProvider.ApiVersionDescriptions)
{
    var versionName = descrription.GroupName;
    var versionNumber = descrription.ApiVersion.ToString();
    var displayName = $"Demo API -- {versionNumber}";

    builder.Services.AddOpenApi(versionName, options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new OpenApiInfo
            {
                Title = displayName,
                Version = versionNumber,
                Description = displayName,
                Contact = new OpenApiContact
                {
                    Name = "Uros Velickovic",
                    Email = "uros.velickovic@example.com"
                }
            };

            document.Components ??= new();
            document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter JWT Bearer token"
                }
            };

            document.Security =
            [
                new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"), new List<string>()
                }
            }
            ];

            return Task.CompletedTask;

        });
    });
}


builder.Services.AddCors();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAutoMapper(o =>
{
    o.CreateMap<Villa, VillaCreateDTO>().ReverseMap();
    o.CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
    o.CreateMap<Villa, VillaDTO>().ReverseMap();
    o.CreateMap<User, UserDTO>().ReverseMap();
    o.CreateMap<VillaAmenities, VillaAmenitiesDTO>()
        .ForMember(dest => dest.VillaName, opt => opt.MapFrom(src => src.Villa != null ? src.Villa.Name : null))
        .ReverseMap()
        .ForMember(dest => dest.Villa, opt => opt.Ignore());
    o.CreateMap<VillaAmenities, VillaAmenitiesCreateDTO>().ReverseMap();
    o.CreateMap<VillaAmenitiesDTO, VillaAmenitiesCreateDTO>().ReverseMap();
    o.CreateMap<VillaAmenities, VillaAmenitiesUpdateDTO>().ReverseMap();

});

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

await SeedDataAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();


    app.MapScalarApiReference(options =>
    {
        options.Title = "Demo - Royal Villa API";

        var sortedVersion = provider.ApiVersionDescriptions.OrderBy(v => v.ApiVersion).ToList();

        foreach (var description in sortedVersion)
        {
            var versionName = description.GroupName;
            var versionNumber = description.ApiVersion.ToString();
            var displayName = $"Demo API {versionNumber}";

            var isDefault = description.ApiVersion.Equals(new ApiVersion(2, 0));

            options.AddDocument(versionName, displayName, $"/openapi/{versionName}.json", isDefault);
        }


    });

    app.MapGet("/", () => Results.Redirect("/scalar"));
}

app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("*"));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();

}