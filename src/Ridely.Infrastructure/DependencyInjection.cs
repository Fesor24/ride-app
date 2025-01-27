using System.Reflection;
using Asp.Versioning;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soloride.Application.Abstractions.Authentication;
using Soloride.Application.Abstractions.Data;
using Soloride.Application.Abstractions.Location;
using Soloride.Application.Abstractions.Notifications;
using Soloride.Application.Abstractions.Payment;
using Soloride.Application.Abstractions.Referral;
using Soloride.Application.Abstractions.Rides;
using Soloride.Application.Abstractions.Security;
using Soloride.Application.Abstractions.Storage;
using Soloride.Application.Abstractions.VoiceCall;
using Soloride.Application.Abstractions.Websocket;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Call;
using Soloride.Domain.Common;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Services;
using Soloride.Domain.Transactions;
using Soloride.Domain.Users;
using Soloride.Infrastructure.Authentication;
using Soloride.Infrastructure.Consumers;
using Soloride.Infrastructure.Data;
using Soloride.Infrastructure.Location;
using Soloride.Infrastructure.Messaging;
using Soloride.Infrastructure.Notifications;
using Soloride.Infrastructure.Outbox;
using Soloride.Infrastructure.Payments;
using Soloride.Infrastructure.Referral;
using Soloride.Infrastructure.Repositories;
using Soloride.Infrastructure.Rides;
using Soloride.Infrastructure.Route;
using Soloride.Infrastructure.Security;
using Soloride.Infrastructure.Services;
using Soloride.Infrastructure.Store;
using Soloride.Infrastructure.VoiceCall;
using StackExchange.Redis;

namespace Soloride.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this  IServiceCollection services, 
        IConfiguration config)
    {
        AddPersistence(services, config);

        AddBackgroundJobs(services, config);

        ConfigureOptions(services, config);

        AddAuthentication(services, config);

        AddServices(services);

        AddApiVersioning(services);

        AddLocationService(services);

        AddRouteService(services, config);

        AddNotificationService(services, config);

        AddPaymentService(services);

        AddRideService(services);

        AddObjectStorage(services, config);

        AddReferralService(services);

        AddMassTransit(services, config);

        return services;
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration config)
    {
        var rabbitMqOptions = config.GetSection("RabbitMq").Get<RabbitMqOptions>();

        services.AddMassTransit(config =>
        {
            config.AddConsumer<RideRequestedConsumer>();

            // main api...
            config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("main", false));

            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rabbitMqOptions.Host, "/", hst =>
                {
                    hst.Username(rabbitMqOptions.Username);
                    hst.Password(rabbitMqOptions.Password);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ICacheService, CacheService>();
        services.AddSingleton<IWebSocketManager, WebSocketManager>();
        services.AddScoped<IVoiceService, VoiceService>();
    }

    public static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AgoraCredentials>(configuration.GetSection("Agora"));
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration config)
    {
        string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
            config.GetConnectionString("Database");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseNpgsql(connectionString,
                migration => migration.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
        });

        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IDriverReferrersRepository, DriverReferrersRepository>();
        services.AddScoped<IDriverTransactionHistoryRepository, DriverTransactionHistoryRepository>();
        services.AddScoped<IDriverWalletRepository, DriverWalletRepository>();
        services.AddScoped<IRiderRepository, RiderRepository>();
        services.AddScoped<IRiderReferrersRepository, RiderReferrersRepository>();
        services.AddScoped<IRiderTransactionHistoryRepository, RiderTransactionHistoryRepository>();
        services.AddScoped<IRiderWalletRepository, RiderWalletRepository>();
        services.AddScoped<IRideRepository, RideRepository>();
        services.AddScoped<IRideLogRepository, RideLogRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IRatingsRepository, RatingsRepository>();
        services.AddScoped<IPaymentCardRepository, PaymentCardRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<ICallLogRepository, CallLogRepository>();
        services.AddScoped<ITransactionLogRepository, TransactionLogRepository>();
        services.AddScoped<ITransactionReferenceMapRepository, TransactionReferenceMapRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICabRepository, CabRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IBankAccountRepository, BankAccountRepository>();
        services.AddScoped<IBankRepository, BankRepository>();
        services.AddScoped<ISavedLocationRepository, SavedLocationRepository>();


        services.AddScoped<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        string? redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ??
            config.GetConnectionString("Redis");

        if (string.IsNullOrWhiteSpace(redisConnectionString))
            throw new ArgumentNullException(nameof(redisConnectionString));

        services.AddSingleton<IConnectionMultiplexer>(opt =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnectionString, true);

            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddBackgroundJobs(IServiceCollection services, IConfiguration config)
    {
        string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
            config.GetConnectionString("Database");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        services.AddHangfire(opt =>
        {
            opt.UsePostgreSqlStorage(opts =>
            {
                opts.UseNpgsqlConnection(connectionString);
            });

            opt.UseSerializerSettings(new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            });
        });

        services.AddHangfireServer(serverOpts =>
        {
            serverOpts.Queues = ["high", "default", "low"];
        });

        services.Configure<OutboxOptions>(config.GetSection("Outbox"));

        services.AddScoped<ProcessOutboxMessagesJob>();
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
    }

    private static void AddLocationService(IServiceCollection services)
    {
        services.AddScoped<ILocationService, LocationService>();
    }

    private static void AddRouteService(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<RouteService>();

        services.Configure<GoogleRouteOptions>(
            configuration.GetSection("GoogleRoute"));
    }

    private static void AddNotificationService(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDeviceNotificationService, DeviceNotificationService>();
        services.AddHttpClient<TermiiService>();
        services.AddScoped<ISmsService, SmsService>();

        services.Configure<TermiiOptions>(configuration.GetSection("Termii"));
    }

    private static void AddRideService(IServiceCollection services)
    {
        services.AddScoped<IRideService, RideService>();
    }

    private static void AddPaymentService(IServiceCollection services)
    {
        services.AddTransient<IPaystackService, PaystackService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<PricingService>();
    }

    private static void AddObjectStorage(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IObjectStoreService, ObjectStoreService>();
        services.Configure<S3Options>(configuration.GetSection("AWSS3"));
    }

    private static void AddReferralService(IServiceCollection services)
    {
        services.AddScoped<IReferralService, ReferralService>();
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.Configure<Infrastructure.Authentication.AuthenticationOptions>(
            configuration.GetSection("Authentication"));

        services.AddScoped<IHashService, HashService>();
        services.AddScoped<IJwtService, JwtService>();
    }
}
