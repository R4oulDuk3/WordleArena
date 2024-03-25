using WordleArena.Api.Hubs.Messages.ClientToServer;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Queries;

namespace WordleArena.Api.Hubs;

public partial class GameHub
{
    public async Task<GamePlayerState> GetGamePlayerState(
        GetPlayerStateRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var practiceState = await mediator.Send(new GetPlayerState(
            request.GameId, playerId, request.GameType));

        return practiceState;
    }

    public async Task<SignalizeReadinessResponse> SignalizeReadiness(SignalizeReadinessRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        return await mediator.Send(new SignalizeReadiness(request.GameId, playerId, request.GameType));
    }


    public async Task<GamePlayerState> GoToNextWord(GoToNextWordRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var practiceState =
            await mediator.Send(new GoToNextWord(playerId, request.GameId, request.GameType));
        return practiceState;
    }

    public async Task<GamePlayerState> ApplyEffect(ApplyEffectRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        return await mediator.Send(new ApplyEffect(playerId, request.Effect, request.GameType, request.GameId));
    }

    public async Task<GameStatus> GetGameStatus(GetGameStatusRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        return await mediator.Send(new GetGameStatus(request.GameId, request.GameType));
    }
}