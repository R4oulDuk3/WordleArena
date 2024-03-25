using MediatR;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.UserSession;

namespace WordleArena.Domain.Grains;

public interface IConnectedUsersCountGrain : IGrainWithGuidKey
{
    Task<int> Get();
    Task Handle(UserConnected amount);
    Task Handle(UserDisconnected amount);

    Task SendOnlinePlayerCountUpdate(object state);
}

public class ConnectedUsersCountGrain : Grain, IConnectedUsersCountGrain
{
    private readonly IMediator mediator;
    private int playerCount;
    private IDisposable timer;

    public ConnectedUsersCountGrain(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Task<int> Get()
    {
        return Task.FromResult(playerCount);
    }

    public Task Handle(UserConnected @event)
    {
        playerCount += 1;
        return Task.CompletedTask;
    }

    public Task Handle(UserDisconnected @event)
    {
        playerCount -= 1;
        return Task.CompletedTask;
    }

    public async Task SendOnlinePlayerCountUpdate(object state)
    {
        await mediator.Send(new BroadcastMessage("PlayerCountUpdate", playerCount));
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        timer = RegisterTimer(SendOnlinePlayerCountUpdate, null, TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(500));

        return base.OnActivateAsync(cancellationToken);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        // Dispose of the timer when the grain deactivates
        timer?.Dispose();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}