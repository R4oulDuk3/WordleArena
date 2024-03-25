using Mediator;
using Orleans.EventSourcing;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.UserSession;

namespace WordleArena.Domain.Grains;

public interface IUserSessionGrain : IGrainWithStringKey
{
    Task Start();
    Task Stop();

    public Task Handle(UserSessionEvent @event);
    Task<UserSession> GetUserSession();
}

public class UserSessionGrain(ISender mediator) : JournaledGrain<UserSession>, IUserSessionGrain
{
    private int lastSyncedVersion = -1;
    private DateTime lastSyncTime = DateTime.MinValue;
    private IDisposable timer;

    public Task Start()
    {
        lastSyncedVersion = State.Version;
        timer = RegisterTimer(Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    public async Task Handle(UserSessionEvent @event)
    {
        RaiseEvent(@event);
        await ConfirmEvents();
    }

    public Task<UserSession> GetUserSession()
    {
        return Task.FromResult(State);
    }

    private async Task Tick(object _)
    {
        if (lastSyncedVersion < State.Version ||
            DateTime.UtcNow.Subtract(lastSyncTime).CompareTo(TimeSpan.FromSeconds(5)) >= 0)
        {
            await mediator.Send(new SendMessageToUsers(State, "UserSession", new List<UserId> { State.UserId }));
            lastSyncedVersion = State.Version;
            lastSyncTime = DateTime.UtcNow;
        }
    }

    public override Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        timer?.Dispose();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}