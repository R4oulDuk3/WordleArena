using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class GetUsernameResponse
{
    public GetUsernameResponse(string username)
    {
        Username = username;
    }

    public string Username { get; set; }
}