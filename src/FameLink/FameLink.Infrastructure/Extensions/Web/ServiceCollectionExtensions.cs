using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using FameLink.Data.Models.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using FameLink.Infrastructure.Options;
using FameLink.Infrastructure.Identity;
using System.Reflection;
using FluentValidation.AspNetCore;

namespace FameLink.Infrastructure.Extensions.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEFCore<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options => options.UseSqlServer(connectionString));
        }

        public static IServiceCollection AddIdentity<TContext>(
            this IServiceCollection services,
                 IConfiguration configuration) where TContext : DbContext
        {
            services.AddIdentity<FameLinkUser, IdentityRole>()
                    .AddEntityFrameworkStores<TContext>()
                    .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            using var provider = services.BuildServiceProvider();
            var jwtTokenHandler = provider.GetService<JwtTokenHandler>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = jwtTokenHandler.GetValidationParameters();
            });

            return services;
        }

        public static IServiceCollection AddMediatR(this IServiceCollection services, params Assembly[] handlersAssembly)
        {
            return services.AddMediatR(assemblies: handlersAssembly);
        }

        public static IServiceCollection AddFluentValidations(this IServiceCollection services, Assembly assembly)
        {
            services
               .AddMvc()
               .AddFluentValidation(cfg =>
               {
                   cfg.RegisterValidatorsFromAssembly(assembly);
               });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "FameLink Api", Version = "v1" });

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
                        new string[] {}
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });
        }

        public static IServiceCollection AddServices<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddHttpContextAccessor();

            services.AddTransient<JwtTokenHandler>();

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            const string Jwt = "Jwt";

            services.Configure<JwtOptions>(configuration.GetSection(Jwt));
            return services;
        }
    }
}
