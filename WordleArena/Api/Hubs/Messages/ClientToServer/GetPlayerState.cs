using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class GetPlayerStateRequest(GameId gameId, GameType gameType)
{
    public GameId GameId { get; set; } = gameId;
    public GameType GameType { get; set; } = gameType;
}