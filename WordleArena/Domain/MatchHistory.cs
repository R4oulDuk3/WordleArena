using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
public record MatchHistoryRecord(UserId UserId, GameId GameId, DateTime FinishedAt, int Place, double Score);

[ExportTsInterface(OutputDir = "domain")]
public record MatchHistory(List<MatchHistoryRecord> Records, int Page, int Total);