using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain.Events.Game;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public abstract class GameEvent
{
}