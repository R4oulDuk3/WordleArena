using MediatR;
using Polly;
using WordleArena.Domain;
using WordleArena.Domain.Commands;

namespace WordleArena.Application.CommandHandlers;

public class GoToNextWordHandler(IGrainFactory factory) : IRequestHandler<GoToNextWord, GamePlayerState>
{
    public async Task<GamePlayerState> Handle(GoToNextWord request, CancellationToken cancellationToken)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));


        return await retryPolicy.ExecuteAsync(async () =>
        {
            var gameGrain = factory.GetGameGrain(request.GameId, request.GameType);
            return await gameGrain.GoToNextWord(request.UserId);
        });
    }
}