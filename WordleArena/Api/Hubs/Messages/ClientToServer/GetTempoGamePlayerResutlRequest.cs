using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class GetTempoGameResultRequest(GameId gameId)
{
    public GameId GameId { get; set; } = gameId;
}

[ExportTsEnum(OutputDir = "messages")]
public enum GetTempoGameResultResponseCode
{
    NotFound,
    NotAllowed,
    Ok
}

[ExportTsInterface(OutputDir = "messages")]
public class GetTempoGameResultResponse
{
    private GetTempoGameResultResponse(GameId gameId, GetTempoGameResultResponseCode code,
        List<TempoGamePlayerResult> results, List<PlayerInfo> playerInfos)
    {
        GameId = gameId;
        Code = code;
        TempoGamePlayerResultDtos = (from result in results
            let playerInfo =
                playerInfos.FirstOrDefault(i => i.UserId.Equals(result.UserId),
                    new PlayerInfo(result.UserId, "Unknown"))
            select new TempoGamePlayerResultDto(playerInfo, result.UserId, result.ResultInfo)).ToList();
    }

    public GameId GameId { get; set; }
    public GetTempoGameResultResponseCode Code { get; set; }
    public List<TempoGamePlayerResultDto> TempoGamePlayerResultDtos { get; set; }

    public static GetTempoGameResultResponse NotAllowed(GameId gameId)
    {
        return new GetTempoGameResultResponse(gameId, GetTempoGameResultResponseCode.NotAllowed,
            new List<TempoGamePlayerResult>(), new List<PlayerInfo>());
    }

    public static GetTempoGameResultResponse NotFound(GameId gameId)
    {
        return new GetTempoGameResultResponse(gameId, GetTempoGameResultResponseCode.NotFound,
            new List<TempoGamePlayerResult>(), new List<PlayerInfo>());
    }

    public static GetTempoGameResultResponse Ok(GameId gameId, List<TempoGamePlayerResult> results,
        List<PlayerInfo> playerInfos)
    {
        return new GetTempoGameResultResponse(gameId, GetTempoGameResultResponseCode.Ok,
            results, playerInfos);
    }
}

[ExportTsInterface(OutputDir = "messages")]
public class TempoGamePlayerResultDto(PlayerInfo playerInfo, UserId userId, TempoPlayerResultInfo resultInfo)
{
    public UserId UserId { get; set; } = userId;
    public TempoPlayerResultInfo ResultInfo { get; set; } = resultInfo;

    public PlayerInfo PlayerInfo { get; set; } = playerInfo;
}