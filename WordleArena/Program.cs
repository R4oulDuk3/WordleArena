using WordleArena;
using WordleArena.Domain;
using WordleArena.Domain.Grains;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseDashboard();
    siloBuilder.UseLocalhostClustering();
    siloBuilder.UseAdoNetClustering(options =>
    {
        options.Invariant = "Npgsql";
        options.ConnectionString = builder.Configuration.GetConnectionString("ArenaOrleans");
    });
    siloBuilder.ConfigureLogging(logging => logging.AddConsole());
    // siloBuilder.AddMemoryGrainStorage("wordleArena");
    siloBuilder.AddAdoNetGrainStorage("wordleArena", options =>
    {
        options.Invariant = "Npgsql";
        options.ConnectionString = builder.Configuration.GetConnectionString("ArenaOrleans");
    });
    siloBuilder.ConfigureServices(services =>
    {
        services.ConfigureServices(builder.Configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    });
    siloBuilder.AddLogStorageBasedLogConsistencyProvider().AddMemoryGrainStorageAsDefault();
    siloBuilder.AddStartupTask(async (services, cancellation) =>
    {
        // Use the service provider to get the grain factory.
        var grainFactory = services.GetRequiredService<IGrainFactory>();

        // Get a reference to a grain and call a method on it.
        var grain = grainFactory.GetGrain<IMatchmakingGrain>("0");
        await grain.Initialize(new List<GameType> { GameType.Practice, GameType.Tempo },
            new SimpleMatchmakingStrategy());
        var definitionUpdaterGrain = grainFactory.GetGrain<IDefinitionUpdaterGrain>("0");
        await definitionUpdaterGrain.Start();

        var wordGeneratorGrain = grainFactory.GetGrain<IWordGenerationGrain>("0");
        await wordGeneratorGrain.Start();
    });
});


var app = builder.Build();

await app.Configure();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();