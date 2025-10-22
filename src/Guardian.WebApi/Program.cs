using Application;
using Guardian.Application.Accounts.Contracts;
using Guardian.Application.Wrappers;
using Guardian.WebApi.Attributes;
using Guardian.WebApi.Helpers;
using Guardian.WebApi.Middlewares;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Guardian.WebApi.Services;
using System.Net;
using System.Threading.RateLimiting;
using WebApi.Helpers;
using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Shared;


ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

EnvironmentHelper.Load();

var builder = WebApplication.CreateBuilder(args);

// Implementando rate limiter para evitar muitas requisições enviadas desnecessariamente
// Evitar ataques DDOs também é um benefício dessa implementação
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(5);
        options.PermitLimit = 25;
        options.QueueLimit = 0;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = c =>
    {
        IEnumerable<string> errors = c.ModelState.Values.Where(v => v.Errors.Count > 0)
          .SelectMany(v => v.Errors)
          .Select(v => v.ErrorMessage);

        var response = Response<string>.Failure(
            errors: [.. errors]
        );

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<LogActionAttribute>();

#region Services

builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddDataInfrastructure(builder.Configuration);
builder.Services.AddSharedServices(builder.Configuration);
builder.Services.AddApplicationLayer();

#endregion

builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#region CORS 
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));
#endregion

var app = builder.Build();

#region Database Setup

string autoSeedEnabled = builder.Configuration["DatabaseSettings:AutoSeed"];
string autoMigrateEnabled = builder.Configuration["DatabaseSettings:AutoMigrate"];

if (autoMigrateEnabled == "true")
{
    app.Migrate(builder.Configuration);
}
if (autoSeedEnabled == "true")
{
    await app.SeedDatabaseAsync();
}

#endregion

#region Middlewares

app.UseCors("corspolicy");

app.UseAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Homolog")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<InternalErrorMiddleware>();

app.UseStaticFiles();

app.UseRateLimiter();

app.MapControllers();

#endregion

app.Run();