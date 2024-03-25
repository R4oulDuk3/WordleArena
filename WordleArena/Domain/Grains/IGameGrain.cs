using WordleArena.Domain.Events.Game;

namespace WordleArena.Domain.Grains;

public interface IGameGrain : IGrainWithStringKey
{
    Task Initialize(GameId gameId, IList<PlayerInfo> participants);
    Task Start();
    Task End();

    Task Fail();
    Task<bool> IsPlayerParticipant(UserId userId);

    Task<SignalizeReadinessResponse> HandlePlayerReady(UserId userId);

    Task HandlePlayerLeft(UserId userId);

    Task<WordleWord> GetCurrentPlayerWord(UserId userId);
    Task<GamePlayerState> GetPlayerState(UserId userId);

    Task<(GameEvent @event, GamePlayerState playerState)> HandleWordGuess(UserId userId, Guess guess);

    Task<GamePlayerState> GoToNextWord(UserId userId);

    Task BroadcastSharedPlayerState();
    Task<GamePlayerState> ApplyEffect(UserId sender, Effect effect);
    Task<GameStatus> GetStatus();
}