using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WebApplication6.Context;
using WebApplication6.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur
builder.Services.AddControllers();

// Ajouter AutoMapper au conteneur de services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Pour scanner tous les profils AutoMapper

// Ajouter Swagger et personnalisation de la documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MoviesApi",
        Description = "My first API",
        TermsOfService = new Uri("https://www.google.com"),
        Contact = new OpenApiContact
        {
            Name = "Your Company Name",
            Email = "test@domain.com",
            Url = new Uri("https://www.google.com")
        },
        License = new OpenApiLicense
        {
            Name = "My License",
            Url = new Uri("https://www.google.com")
        }
    });

    // Ajouter la définition de sécurité pour l'authentification Bearer (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then " +
                      "your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
    });

    // Ajouter la demande de sécurité pour les requêtes utilisant le JWT
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Services personnalisés
builder.Services.AddTransient<IGenresService, GenresService>();
builder.Services.AddTransient<IMoviesService, MoviesService>();

// Connexion à la base de données
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configurer le pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
