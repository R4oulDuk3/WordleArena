using Mediator;
using Polly;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.Game;

namespace WordleArena.Application.CommandHandlers;

public class MakeAWordGuessHandler(IGrainFactory factory) : IRequestHandler<MakeAWordGuess, (GameEvent eventOccured,
    GamePlayerState
    currentPlayerState)>
{
    public async ValueTask<(GameEvent eventOccured, GamePlayerState currentPlayerState)> Handle(MakeAWordGuess request,
        CancellationToken cancellationToken)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine(
                        $"Retry {retryCount} due to {exception.Message}. Waiting {timeSpan} before retrying.");
                }
            );

        return await retryPolicy.ExecuteAsync(async () =>
        {
            var gameGrain = factory.GetGameGrain(request.GameId, request.GameType);
            var result = await gameGrain.HandleWordGuess(request.UserId, request.Guess);
            return result;
        });
    }
}