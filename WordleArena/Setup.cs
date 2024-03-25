using System.Diagnostics;
using LinqToDB.EntityFrameworkCore;
using Medallion.Threading;
using Medallion.Threading.Postgres;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using WordleArena.Api.Hubs;
using WordleArena.Domain.Providers;
using WordleArena.Domain.Repositories;
using WordleArena.Infrastructure;
using WordleArena.Infrastructure.Providers;
using WordleArena.Infrastructure.Repositories;

namespace WordleArena;

public static class Setup
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        SetUpInfrastructureServices(services, configuration);

        // CreateOrleansGrainStorage(configuration);

        //
        // FirebaseApp.Create(new AppOptions()
        // {
        //     Credential = GoogleCredential.GetApplicationDefault(),
        //     ProjectId = "worlde-arena",
        // });
        //

        services.AddSignalR();
        services.AddControllers();

        SetupAuth(services);

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:5173") // Frontend server address
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        SetUpEdgeServices(services);
        SetupArenaServices(services);

        // services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Transient; });


        return services;
    }

    private static void SetUpInfrastructureServices(IServiceCollection services, IConfiguration configuration)
    {
        ConfigureOrleansPostgresStorage(configuration);
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("ArenaDB"));
        var dataSource = dataSourceBuilder.Build();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Wordle Arena v1", Version = "v1" });
            // some other configs
            options.AddSignalRSwaggerGen();
        });
        LinqToDBForEFTools.Initialize();

        services.AddDbContext<ArenaDbContext>(
            options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseNpgsql(
                        dataSource
                    ).UseSnakeCaseNamingConvention();
            }, ServiceLifetime.Transient
        );

        services.AddSingleton<IDistributedLockProvider>(_ => new PostgresDistributedSynchronizationProvider(
            configuration.GetConnectionString("ArenaDB") ?? throw new InvalidOperationException()));
    }

    private static void SetupArenaServices(IServiceCollection services)
    {
        // Document Services
        services.AddTransient<TextProviderStateRepository>();
        services.AddTransient<IWordleWordRepository, WordleWordRepository>();
        services.AddTransient<IConsumedDocumentRepository, ConsumedDocumentRepository>();
        services.AddTransient<ITextDocumentProvider, GutenbergProjectTextDocumentProvider>();
        services.AddTransient<IDefinitionProvider, DictionaryDefinitionProvider>();
    }

    private static void SetUpEdgeServices(IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
    }

    private static void ConfigureOrleansPostgresStorage(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ArenaOrleans");

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            var t = connection.BeginTransaction();

            var storageConfigureQuery = ReadOrleansSqlFile();

            try
            {
                using (var command = new NpgsqlCommand(storageConfigureQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                t.Commit();
            }
            catch (Exception)
            {
                t.Rollback();
                throw;
            }
        }

        Console.WriteLine("Table created successfully.");
    }

    public static string ReadOrleansSqlFile()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "orleans.sql");
        var result = File.ReadAllText(filePath);
        return result;
    }

    private static void SetupAuth(IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var projectId = "worlde-arena";
                options.Authority = $"https://securetoken.google.com/{projectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{projectId}",
                    ValidateAudience = true,
                    ValidAudience = projectId,
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static Task<IApplicationBuilder> Configure(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetService<ArenaDbContext>();
        Debug.Assert(context != null, nameof(context) + " != null");
        context.Database.Migrate();

        app.UseCors("CorsPolicy");
        app.UseRouting();

        // If you are using authentication and authorization, add these as well
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapHub<GameHub>("/signalr"); });

        return Task.FromResult(app);
    }
}