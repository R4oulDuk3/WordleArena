using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class UpdateUserStateRequest(UserState userState)
{
    public UserState UserState { get; set; } = userState;
}