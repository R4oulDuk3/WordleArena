using MediatR;

namespace WordleArena.Domain.Commands;

public class SendMessageToBots(object message, string method, List<UserId> botIds, GameType gameType) : IRequest
{
    public List<UserId> BotIds = botIds;
    public GameType GameType = gameType;
    public object Message = message;
    public string Method = method;
}