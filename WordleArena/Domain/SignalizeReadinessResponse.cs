using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsEnum(OutputDir = "domain")]
[GenerateSerializer]
public enum SignalizeReadinessResponse
{
    Accepted,
    RejectedNotParticipant,
    RejectedNotInProgress
}