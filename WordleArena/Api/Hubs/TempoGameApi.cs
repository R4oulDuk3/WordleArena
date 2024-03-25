using WordleArena.Api.Hubs.Messages.ClientToServer;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.Game;
using WordleArena.Domain.Queries;

namespace WordleArena.Api.Hubs;

public partial class GameHub
{
    public async Task<TempoGameMakeAGuessResponse> TempoGameMakeAGuess(MakeAGuessRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var (gameEvent, state) =
            await mediator.Send(new
                MakeAWordGuess(playerId, GameType.Tempo, request.GameId, request.Guess));
        return new TempoGameMakeAGuessResponse((TempoGamePlayerState)state, (TempoGameEvent)gameEvent);
    }

    public async Task<GetTempoGameResultResponse?> GetTempoGameResult(GetTempoGameResultRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var tempoGamePlayerResults = await mediator.Send(new GetTempoGameResult(playerId, request.GameId));
        var playerInfos =
            await mediator.Send(new GetPlayersInfo(tempoGamePlayerResults.Select(pr => pr.UserId).ToList()));

        if (tempoGamePlayerResults.Count == 0) return GetTempoGameResultResponse.NotAllowed(request.GameId);

        if (tempoGamePlayerResults.Count(r => r.UserId.Equals(playerId)) == 0)
            return GetTempoGameResultResponse.NotFound(request.GameId);

        return GetTempoGameResultResponse.Ok(request.GameId, tempoGamePlayerResults, playerInfos);
    }
}