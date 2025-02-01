var builder = DistributedApplication.CreateBuilder(args);

var ridelyDb = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume(isReadOnly: false)
    .AddDatabase("DefaultConnection");

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.Ridely_Api>("ridely-api")
    .WithReference(ridelyDb)
    .WaitFor(ridelyDb)
    .WithReference(redis);

builder.Build().Run();
