using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Spells;

public class SpellDefinition
{
    public SpellId Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int ManaCost { get; init; }

    /// <summary>Max range in tiles. 0 = self / level-wide.</summary>
    public int Range { get; init; }

    /// <summary>AoE radius around the target tile. 0 = single tile.</summary>
    public int Radius { get; init; }

    public SpellTarget Target { get; init; }
    public int BaseDamage { get; init; }
    public DamageType DamageType { get; init; }

    /// <summary>Optional status effect applied to struck enemies.</summary>
    public StatusEffectType? AppliedStatus { get; init; }

    /// <summary>Duration (turns) for the applied status effect.</summary>
    public int StatusDuration { get; init; }

    /// <summary>Super spells have special, dramatic, level-wide effects.</summary>
    public bool IsSuperSpell { get; init; }

    public ConsoleColor Color { get; init; } = ConsoleColor.Magenta;
}
