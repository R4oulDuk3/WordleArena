using Mediator;

namespace WordleArena.Domain.Commands;

public class AddTempoGamePlayerResult(List<TempoGamePlayerResult> playerResults) : IRequest
{
    public List<TempoGamePlayerResult> PlayerResults = playerResults;
}