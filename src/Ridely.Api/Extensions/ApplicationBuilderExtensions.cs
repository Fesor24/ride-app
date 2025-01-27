using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Ridely.Infrastructure.Outbox;
using RidelyAPI.Middlewares;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RidelyAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration config)
    {
        AddSwagger(services);

        return services;
    }

    public static void UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void AddBackgroundJobs(this IApplicationBuilder app)
    {
        var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

        using var scope = app.ApplicationServices.CreateScope();

        var processOutboxMessageJob = scope.ServiceProvider.GetRequiredService<ProcessOutboxMessagesJob>();

        recurringJobManager.AddOrUpdate("ProcessOutboxMessages", 
            () => processOutboxMessageJob.Execute(),
            "* * * * *"); //every minute...
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(opts =>
        {
            opts.CustomSchemaIds(type => type.ToString());

            JwtBearer(opts);
        });
    }

    private static void JwtBearer(SwaggerGenOptions opts)
    {
        opts.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });

        opts.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Description = "x-api-key",
            In = ParameterLocation.Header,
            Name = "x-api-key",
            Type = SecuritySchemeType.ApiKey
        });

        opts.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme,
                    },
                },
                new List<string>()
            }
        });

        opts.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    new string[] {}
                }
            });
    }
}
