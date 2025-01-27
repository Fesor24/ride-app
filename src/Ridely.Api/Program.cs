using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Soloride.Application;
using Soloride.Infrastructure;
using SolorideAPI.Extensions;
using SolorideAPI.Middlewares;
using SolorideAPI.OpenApi;

var builder = WebApplication.CreateBuilder(args);

Serilog.ILogger logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Logger = logger;

builder.Host.UseSerilog();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
    "soloride-app-firebase-adminsdk-o64ak-05a4f56e1e.json"))
});

builder.Services
    .RegisterServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

builder.Services.AddControllers();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddAntiforgery();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", pol =>
    {
        pol.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
    });
});

var app = builder.Build();

app.UseCustomMiddleware();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Static Files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Static")),
    RequestPath = "/Static"
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSwagger();

app.UseSwaggerUI(opts =>
{
    var descriptions = app.DescribeApiVersions();

    foreach(var description in descriptions)
    {
        var url = $"/swagger/{description.GroupName}/swagger.json";
        var name = description.GroupName.ToUpperInvariant();

        opts.SwaggerEndpoint(url, name);
    }
});

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(25)
};

app.UseWebSockets(webSocketOptions);

app.UseMiddleware<WebSocketMiddleware>();

app.UseAntiforgery();

app.MapControllers();

app.UseHangfireDashboard();

app.AddBackgroundJobs();

app.Run();

