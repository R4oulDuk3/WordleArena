using MediatR;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Queries;

namespace WordleArena.Domain.Grains;

public interface IMatchmakingPlayerObserverGrain : IGrainWithStringKey
{
    public Task Initialize(UserId userId);
    public Task ObservePlayerMatchmakingState(object _);
}

public class MatchmakingPlayerObserverGrain(IMediator mediator) : Grain, IMatchmakingPlayerObserverGrain
{
    private readonly TimeSpan tickPeriod = TimeSpan.FromMilliseconds(100);
    private IDisposable timer;
    public UserId? UserId;


    public Task Initialize(UserId userId)
    {
        UserId = userId;
        timer = RegisterTimer(ObservePlayerMatchmakingState, null, tickPeriod, tickPeriod);
        return Task.CompletedTask;
    }

    public async Task ObservePlayerMatchmakingState(object _)
    {
        if (UserId != null)
        {
            var userSession = await mediator.Send(new GetUserSessionByUserId(UserId));
            if (userSession.UserState != UserState.InMatchmaking || userSession.IsConnected == false)
            {
                await mediator.Send(new RemoveFromMatchmaking(new List<UserId> { UserId }));
                // Deactivate Grain
                DeactivateOnIdle();
            }
        }
    }

    public override Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        timer?.Dispose();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}