using System.Reflection;
using Asp.Versioning;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ridely.Application.Abstractions.Authentication;
using Ridely.Application.Abstractions.Data;
using Ridely.Application.Abstractions.Location;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Abstractions.Referral;
using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Abstractions.Security;
using Ridely.Application.Abstractions.Settings;
using Ridely.Application.Abstractions.Storage;
using Ridely.Application.Abstractions.VoiceCall;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Call;
using Ridely.Domain.Common;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Domain.Transactions;
using Ridely.Domain.Users;
using Ridely.Infrastructure.Authentication;
using Ridely.Infrastructure.Cache;
using Ridely.Infrastructure.Consumers;
using Ridely.Infrastructure.Data;
using Ridely.Infrastructure.Location;
using Ridely.Infrastructure.Messaging;
using Ridely.Infrastructure.Notifications;
using Ridely.Infrastructure.Outbox;
using Ridely.Infrastructure.Payments;
using Ridely.Infrastructure.Referral;
using Ridely.Infrastructure.Repositories;
using Ridely.Infrastructure.Rides;
using Ridely.Infrastructure.Route;
using Ridely.Infrastructure.Security;
using Ridely.Infrastructure.Services;
using Ridely.Infrastructure.Store;
using Ridely.Infrastructure.VoiceCall;
using Ridely.Infrastructure.WebSockets;
using StackExchange.Redis;

namespace Ridely.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this  IServiceCollection services, 
        IConfiguration config)
    {
        AddDefaultHttpResilience(services);

        AddHealthChecks(services, config);

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

    private static void AddDefaultHttpResilience(IServiceCollection services)
    {
        services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler(options =>
            {
                options.Retry.Delay = TimeSpan.FromSeconds(1);
                options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
                options.Retry.MaxRetryAttempts = 4;
                options.Retry.UseJitter = true;

                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);

                options.CircuitBreaker.FailureRatio = 0.2;
            });
        });
    }

    private static void AddHealthChecks(IServiceCollection services, IConfiguration config)
    {
        services.AddHealthChecks()
            .AddNpgSql(config.GetConnectionString("Database")!)
            .AddRedis(config.GetConnectionString("Redis")!);
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
        //services.AddSingleton<IWebSocketManager, WebSocketManager>();
        services.AddScoped<IVoiceService, VoiceService>();
    }

    public static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AgoraCredentials>(configuration.GetSection("Agora"));
        services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
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

            opt.ConfigureWarnings(cfg =>
            {
                cfg.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
            });
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
        services.AddScoped<IPaymentDetailRepository, PaymentDetailRepository>();
        services.AddScoped<IDriverDiscountRepository, DriverDiscountRepository>();
        services.AddScoped<IWaitTimeRepository, WaitTimeRepository>();


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
        
        services.Configure<MapboxOptions>(configuration.GetSection("Mapbox"));
    }

    private static void AddNotificationService(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IPushNotificationService, PushNotificationService>();
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
