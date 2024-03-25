using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain.Events.Game;

public enum TempoGameEventType
{
    PlayerEliminated,
    GuessSuccess,
    GuessIncorrect,
    WordFailed,
    GameOver,
    GameStarted,
    Nothing
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class TempoGameEvent(UserId player, TempoGameEventType eventType, string message)
    : GameEvent
{
    [Id(2)] public string Message { get; set; } = message;
    [Id(1)] public TempoGameEventType EventType { get; set; } = eventType;
    [Id(0)] public UserId Player { get; set; } = player;
}