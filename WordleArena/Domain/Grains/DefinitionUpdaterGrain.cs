using MediatR;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Providers;
using WordleArena.Domain.Queries;

namespace WordleArena.Domain.Grains;

public interface IDefinitionUpdaterGrain : IGrainWithStringKey
{
    public Task Start();
    public Task Stop();
    public Task Tick(object _);
}

public class DefinitionUpdaterGrain(IMediator mediator, IDefinitionProvider provider,
    ILogger<DefinitionUpdaterGrain> logger) : Grain, IDefinitionUpdaterGrain
{
    private IDisposable timer;

    public Task Start()
    {
        timer = RegisterTimer(Tick, null, TimeSpan.Zero, TimeSpan.FromSeconds(11));
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    public async Task Tick(object _)
    {
        var words = await mediator.Send(new GetWordWithNoDefinition(15, 5));
        // var words = await mediator.Send(new GetWordWithNoDefinition(15, 5));
        var definitions = new List<WordDefinition>();
        foreach (var word in words)
            try
            {
                var definition = await provider.Provide(word);
                definitions.Add(definition);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed updating word definition for word");
            }

        await mediator.Send(new UpsertWordDefinitions(definitions));
    }

    public override Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        timer?.Dispose();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}