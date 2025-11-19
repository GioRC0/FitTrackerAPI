using System.Text;
using FitTrackerAPI.Data;
using FitTrackerAPI.Models.UserInfo;
using FitTrackerAPI.Repositories.Achievements;
using FitTrackerAPI.Repositories.Exercises;
using FitTrackerAPI.Repositories.Training;
using FitTrackerAPI.Repositories.Users;
using FitTrackerAPI.Services.Auth;
using FitTrackerAPI.Services.Dashboard;
using FitTrackerAPI.Services.Email;
using FitTrackerAPI.Services.Exercises;
using FitTrackerAPI.Services.Training;
using FitTrackerAPI.Services.UserProfile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuración de Base de Datos y Repositorios ---

// Configurar MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp => 
    new MongoClient(builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString")));

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Registro de Repositorios y Servicios (Inyección de Dependencias)
// NOTA: Debes re-implementar UserRepository y TokenRepository para que usen IMongoDatabase
builder.Services.AddScoped<IUserRepository, UserRepository>(); 

// Servicios 
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>(); 
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();

// Training Sessions
builder.Services.AddScoped<MongoDbContext>();
builder.Services.AddScoped<ITrainingSessionRepository, TrainingSessionRepository>();
builder.Services.AddScoped<ITrainingSessionService, TrainingSessionService>();

// Dashboard
builder.Services.AddScoped<IDashboardService, DashboardService>();

// User Profile & Achievements
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();

// --- 2. Configuración de Autenticación JWT ---

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Solo true en producción
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// --- 3. Otros Servicios ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FitTracker API", Version = "v1" });

    // 1. Definir el esquema de seguridad para JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, introduce tu token JWT.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    // 2. Añadir el requisito de seguridad que usa el esquema Bearer
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.WebHost.UseUrls("http://0.0.0.0:5180");

var app = builder.Build();

// Crear índices de MongoDB al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    var users = db.GetCollection<User>("Users");
    var indexKeysDefinition = Builders<User>.IndexKeys.Ascending(u => u.Username);
    var indexOptions = new CreateIndexOptions { Unique = true };
    var indexModel = new CreateIndexModel<User>(indexKeysDefinition, indexOptions);
    await users.Indexes.CreateOneAsync(indexModel);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
