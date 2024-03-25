using WordleArena.Api.Hubs.Messages.ClientToServer;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.Game;

namespace WordleArena.Api.Hubs;

public partial class GameHub
{
    public async Task<PracticeGameMakeAGuessResponse> PracticeGameMakeAGuess(MakeAGuessRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var (gameEvent, state) =
            await mediator.Send(new
                MakeAWordGuess(playerId, GameType.Practice, request.GameId, request.Guess));
        return new PracticeGameMakeAGuessResponse((PracticeGamePlayerState)state, (PracticeGameEvent)gameEvent);
    }
}