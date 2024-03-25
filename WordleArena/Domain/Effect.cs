using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[GenerateSerializer]
[ExportTsEnum(OutputDir = "domain")]
public enum EffectType
{
    Freeze,
    Reflect,
    KeyboardHints
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class Effect(UserId targetPlayer, EffectType type)
{
    [Id(0)] public EffectType Type { get; set; } = type;
    [Id(1)] public UserId TargetPlayer { get; set; } = targetPlayer;
}