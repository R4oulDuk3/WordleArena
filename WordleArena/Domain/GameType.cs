using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsEnum(OutputDir = "domain")]
public enum GameType
{
    Practice,
    Tempo,
    Survival
}