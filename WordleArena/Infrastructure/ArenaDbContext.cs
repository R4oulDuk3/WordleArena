using Microsoft.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Infrastructure.Providers;

namespace WordleArena.Infrastructure;

public class ArenaDbContext : DbContext
{
    public ArenaDbContext(DbContextOptions<ArenaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<DocumentProviderSerializedState> DocumentProviderStates { get; set; }

    public DbSet<WordleWord> WordleWords { get; set; }

    public DbSet<Hash> Hashes { get; set; }

    public DbSet<Bot> Bots { get; set; }
    public DbSet<WordDefinition> WordDefinitions { get; set; }

    public DbSet<TempoGamePlayerResult> TempoGamePlayerResults { get; set; }

    public DbSet<PlayerMatchmakingInfo> PlayerMatchmakingInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(user => user.UserId);
        modelBuilder.Entity<User>().Property(user => user.UserId).HasConversion(
            playerId => playerId.Id,
            id => new UserId(id)
        );
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DocumentProviderSerializedState>().HasKey(state => state.Provider);
        modelBuilder.Entity<DocumentProviderSerializedState>().Property(s => s.Version).IsRowVersion();
        modelBuilder.Entity<Hash>().HasKey(hash => hash.Value);


        modelBuilder.Entity<WordleWord>().HasKey(word => word.TargetWord);


        modelBuilder.Entity<WordDefinition>().HasKey(wd => wd.Word);
        modelBuilder.Entity<WordDefinition>()
            .HasOne<WordleWord>()
            .WithOne()
            .HasForeignKey<WordDefinition>(wd => wd.Word);
        modelBuilder.Entity<WordDefinition>().OwnsOne(wd => wd.PossibleMeanings, pm =>
        {
            pm.ToJson();
            pm.OwnsMany(pme => pme.Meanings);
        });

        modelBuilder.Entity<PlayerMatchmakingInfo>().HasKey(user => user.UserId);
        modelBuilder.Entity<PlayerMatchmakingInfo>().Property(info => info.UserId).HasConversion(
            playerId => playerId.Id,
            id => new UserId(id)
        );

        modelBuilder.Entity<PlayerMatchmakingInfo>().Property(info => info.Type).HasConversion(
            type => type.ToString(), str => Enum.Parse<GameType>(str));

        modelBuilder.Entity<Bot>().HasKey(user => user.UserId);
        modelBuilder.Entity<Bot>().Property(info => info.UserId).HasConversion(
            playerId => playerId.Id,
            id => new UserId(id)
        );


        modelBuilder.Entity<TempoGamePlayerResult>().HasKey(tgpr => new { tgpr.UserId, tgpr.GameId });
        modelBuilder.Entity<TempoGamePlayerResult>().Property(tgpr => tgpr.UserId).HasConversion(
            playerId => playerId.Id,
            id => new UserId(id)
        );
        modelBuilder.Entity<TempoGamePlayerResult>().Property(tgpr => tgpr.GameId).HasConversion(
            gameId => gameId.Id,
            id => new GameId(id)
        );

        modelBuilder.Entity<TempoGamePlayerResult>().OwnsOne(tgr => tgr.ResultInfo, info =>
        {
            info.ToJson();
            info.OwnsMany(inf => inf.GuessDistributions);
        });
    }
}