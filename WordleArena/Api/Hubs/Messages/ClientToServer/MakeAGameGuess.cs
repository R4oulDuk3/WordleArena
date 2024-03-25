using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain;
using WordleArena.Domain.Events.Game;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class MakeAGuessRequest(GameId gameId, Guess guess)
{
    public GameId GameId { get; set; } = gameId;
    public Guess Guess { get; set; } = guess;
}

[ExportTsInterface(OutputDir = "messages")]
public class PracticeGameMakeAGuessResponse(PracticeGamePlayerState playerState, PracticeGameEvent gameEvent)
{
    public PracticeGamePlayerState PlayerState { get; set; } = playerState;
    public PracticeGameEvent GameEvent { get; set; } = gameEvent;
}

[ExportTsInterface(OutputDir = "messages")]
public class TempoGameMakeAGuessResponse(TempoGamePlayerState playerState, TempoGameEvent gameEvent)
{
    public TempoGamePlayerState PlayerState { get; set; } = playerState;
    public TempoGameEvent GameEvent { get; set; } = gameEvent;
}