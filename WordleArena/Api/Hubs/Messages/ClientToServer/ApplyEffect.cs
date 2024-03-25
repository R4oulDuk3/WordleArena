using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class ApplyEffectRequest(Effect effect, GameType gameType, GameId gameId)
{
    public Effect Effect { get; set; } = effect;
    public GameId GameId { get; set; } = gameId;
    public GameType GameType { get; set; } = gameType;
}