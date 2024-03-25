using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class InGameNotification(string text, double showNotificationDuration)
{
    [Id(0)] public string Text { get; set; } = text;
    [Id(1)] public double ShowNotificationDuration { get; set; } = showNotificationDuration;
}