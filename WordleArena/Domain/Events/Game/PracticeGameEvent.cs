using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain.Events.Game;

[ExportTsEnum(OutputDir = "domain")]
public enum PracticeGameEventType
{
    GuessFailed,
    GuessSuccess,
    WordFailed
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class PracticeGameEvent(UserId player, PracticeGameEventType eventType, string message)
    : GameEvent
{
    [Id(1)] public PracticeGameEventType EventType { get; set; } = eventType;
    [Id(0)] public UserId Player { get; set; } = player;

    [Id(2)] public string Message { get; set; } = message;
}