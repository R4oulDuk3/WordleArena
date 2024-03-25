using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public abstract class SharedPlayerState
{
    [Id(0)] public int Version { get; set; }

    public void IncreaseVersion()
    {
        Version++;
    }
}