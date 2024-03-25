using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public abstract class GamePlayerState(UserId userId)
{
    [Id(0)] public UserId UserId { get; set; } = userId;
    [Id(1)] public int Version { get; set; }

    public void IncreaseVersion()
    {
        Version++;
    }
}