using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Guardian.Domain.User.Entities;
using System.Text;
using Guardian.Application.Accounts.Contracts;
using Microsoft.AspNetCore.Http;
using Guardian.Domain.Settings;
using Newtonsoft.Json;
using Guardian.Application.Wrappers;
using Guardian.Infrastructure.Data.Contexts;


namespace Infrastructure.Identity
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password Options
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                // User Options
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            #region Services

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();

            #endregion

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                };
                o.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = ctx =>
                    {
                        ctx.Request.Cookies.TryGetValue("access_token", out var accessToken);
                        if (!string.IsNullOrEmpty(accessToken))
                            ctx.Token = accessToken;
                        return Task.CompletedTask;
                    },

                    // Isco => removido pois estava dando erro geral na aplicação, foi implementado um Middleware para tratamento
                    //         de erros internos em: NavegaMA.WebApi/Middlewares/ErrorHandlerMiddleware.cs

                    //OnAuthenticationFailed = context =>
                    //{
                    //    if (!context.Response.HasStarted)
                    //    {
                    //        context.NoResult();
                    //        context.Response.ContentType = "application/json";

                    //        var result = JsonConvert.SerializeObject(Response<string>.Failure(
                    //            errors: ["Erro interno no servidor. Entre em contato com o Desenvolvimento."]
                    //        ));

                    //        return context.Response.WriteAsync(result);
                    //    }

                    //    return context.Response.WriteAsync(string.Empty);
                    //},

                    OnChallenge = context =>
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            var result = JsonConvert.SerializeObject(Response<string>.Failure(
                                errors: ["Você não está autorizado."]
                            ));

                            return context.Response.WriteAsync(result);
                        }

                        return context.Response.WriteAsync(string.Empty);
                    },
                    OnForbidden = context =>
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";

                            var result = JsonConvert.SerializeObject(Response<string>.Failure(
                                errors: ["Você não tem permissão para acessar esse recurso."]
                            ));

                            return context.Response.WriteAsync(result);
                        }

                        return context.Response.WriteAsync(string.Empty);
                    }
                };
            });

            return services;
        }
    }
}