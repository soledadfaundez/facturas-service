using Facturas.Application.Ports;
using Facturas.Application.Services;
using Facturas.Infrastructure.Persistence;
using Facturas.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Facturas.Infrastructure.Seeders;
using System.Text.Json;
using Facturas.Domain.Aggregates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??=
        new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        // 1. Definir el esquema de autenticación Bearer
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header usando el esquema Bearer."
        };

        document.Components.SecuritySchemes.Add("Bearer", securityScheme);

        // 2. Aplicar el requerimiento de seguridad global corregido para .NET 10
        var securityRequirement = new OpenApiSecurityRequirement
        {
            // Usamos una lista vacía para cumplir con el tipo requerido (List<string>)
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        };

        document.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
        
        // Al usar referencias internas en .NET 10, es una buena práctica asegurar el host del documento
        document.SetReferenceHostDocument(); 
        
        return Task.CompletedTask;
    });
});

builder.Services.AddScoped<IFacturaService, FacturaService>();

builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();

builder.Services.AddSingleton(
    new SqliteConnectionFactory(
        builder.Configuration.GetConnectionString("Sqlite")!));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

InitializeDatabase(app);

await SeedData(app); // ESFC: Cargar datos de prueba para desarrollo

app.Run();

static void InitializeDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var factory =
        scope.ServiceProvider
            .GetRequiredService<SqliteConnectionFactory>();

    using var connection = factory.CreateConnection();

    connection.Open();

    var schemaPath =
    Path.Combine(
        AppContext.BaseDirectory,
        "Data",
        "schema.sql");

    var sql = File.ReadAllText(schemaPath);

    using var command = connection.CreateCommand();

    command.CommandText = sql;

    command.ExecuteNonQuery();
}

static async Task SeedData(
    WebApplication app)
{
    using var scope =
        app.Services.CreateScope();

    var seeder =
        scope.ServiceProvider
            .GetRequiredService<DataSeeder>();

    var jsonPath =
        Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "facturas.json");

    var json =
        await File.ReadAllTextAsync(jsonPath);

    // ESFC: Esto es momentáneo, hay que preguntar porqué va con cero: LIMPIAR .0
    json = json.Replace(".0,", ",")
               .Replace(".0}", "}");

        var options =
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var facturas =
            JsonSerializer.Deserialize<List<Factura>>(
                json,
                options);
 

    Console.WriteLine(jsonPath);

    Console.WriteLine(File.Exists(jsonPath));

    if (facturas is null || !facturas.Any())
    {
        Console.WriteLine("No se encontraron facturas para cargar.");
        return;
    }

    await seeder.SeedAsync(facturas);
}