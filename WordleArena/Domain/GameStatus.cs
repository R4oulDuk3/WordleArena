using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[GenerateSerializer]
[ExportTsEnum(OutputDir = "domain")]
public enum GameStatus
{
    NotInitialized,
    InProgress,
    Finished
}