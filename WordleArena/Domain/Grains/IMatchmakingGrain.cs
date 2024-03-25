namespace WordleArena.Domain.Grains;

public interface IMatchmakingGrain : IGrainWithStringKey
{
    public Task Initialize(List<GameType> gameTypes, IMatchmakingStrategy strategy);
    public Task MatchPlayers(object _);
}