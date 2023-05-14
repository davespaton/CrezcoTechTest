using Crezco.Infrastructure.Cache;
using Crezco.Infrastructure.Persistence;
using Crezco.Location;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

Log.Logger = new LoggerConfiguration()
    // todo this is configured by file
    .WriteTo.Console()
    .CreateLogger();

try
{
    WebApplicationBuilder builder = SetupBuilder();
    WebApplication app = SetupApp(builder);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

WebApplicationBuilder SetupBuilder()
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Host.UseSerilog();

        builder.Services.AddResponseCaching();
        builder.Services.AddLocationServices();

        IConfigurationRoot config = builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets(typeof(Program).Assembly)
            .Build();

        (DatabaseOptions databaseOptions, CacheOptions cacheOptions) = GetConfigs(config);

        builder.Services.AddStackExchangeRedisCache(opt =>
        {
            opt.InstanceName = cacheOptions.InstanceName;
            opt.Configuration = cacheOptions.Configuration;
        });

        var mongoContext = new MongoContext(Options.Create(databaseOptions));
        builder.Services.AddSingleton<IMongoDatabase>(_ => mongoContext.Database);
    }
    return builder;
}

WebApplication SetupApp(WebApplicationBuilder builder)
{
    WebApplication app = builder.Build();
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseResponseCaching();
        app.UseAuthorization();
        app.MapControllers();
    }
    return app;
}

(DatabaseOptions databaseOptions, CacheOptions cacheOptions) GetConfigs(IConfigurationRoot config)
{
    DatabaseOptions? databaseOptions = config.GetRequiredSection(DatabaseOptions.SectionName).Get<DatabaseOptions>();
    CacheOptions? cacheOptions = config.GetRequiredSection(CacheOptions.SectionName).Get<CacheOptions>();

    if (databaseOptions is null)
    {
        throw new ArgumentNullException(nameof(databaseOptions));
    }

    if (cacheOptions is null)
    {
        throw new ArgumentNullException(nameof(cacheOptions));
    }
    return (databaseOptions, cacheOptions);
}
