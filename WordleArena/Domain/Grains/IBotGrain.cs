using WordleArena.Domain.Events.Game;

namespace WordleArena.Domain.Grains;

public interface IBotGrain : IGrainWithStringKey
{
    public Task Start();
    public Task End();
    public Task<(GameEvent @event, GamePlayerState playerState)> MakeAWordGuess(GameId gameId, Guess guess);
    public Task<GamePlayerState> GoToNextWord(UserId userId);
    public Task<GamePlayerState> GetState(UserId userId);
    public Task HandleMessage(string method, object message);
    public Task Initialize(GameId gameId, UserId userId);

    public Task Tick(object o);

    public Task<WordleWord> GetCurrentWord();
    public Task<Guess> GenerateGuess(WordleWord word);
}