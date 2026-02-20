using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Spells;

public static class SpellDatabase
{
    private static readonly Dictionary<SpellId, SpellDefinition> _spells = new()
    {
        // ── Basic spells ────────────────────────────────────────────────────

        [SpellId.MagicBolt] = new SpellDefinition
        {
            Id          = SpellId.MagicBolt,
            Name        = "Magic Bolt",
            Description = "A quick arcane missile.",
            ManaCost    = 5,
            Range       = 8,
            Radius      = 0,
            Target      = SpellTarget.SingleTarget,
            BaseDamage  = 10,
            DamageType  = DamageType.Void,
            Color       = ConsoleColor.Magenta
        },

        [SpellId.Fireball] = new SpellDefinition
        {
            Id            = SpellId.Fireball,
            Name          = "Fireball",
            Description   = "Hurls a ball of fire. Ignites oil; vaporises water into steam.",
            ManaCost      = 10,
            Range         = 6,
            Radius        = 1,
            Target        = SpellTarget.Area,
            BaseDamage    = 15,
            DamageType    = DamageType.Fire,
            AppliedStatus = StatusEffectType.Burning,
            StatusDuration = 3,
            Color         = ConsoleColor.Red
        },

        [SpellId.FrostBolt] = new SpellDefinition
        {
            Id            = SpellId.FrostBolt,
            Name          = "Frost Bolt",
            Description   = "A bolt of ice. Freezes the target. Extinguishes fire tiles.",
            ManaCost      = 8,
            Range         = 8,
            Radius        = 0,
            Target        = SpellTarget.SingleTarget,
            BaseDamage    = 8,
            DamageType    = DamageType.Cold,
            AppliedStatus = StatusEffectType.Frozen,
            StatusDuration = 3,
            Color         = ConsoleColor.Cyan
        },

        [SpellId.LightningBolt] = new SpellDefinition
        {
            Id          = SpellId.LightningBolt,
            Name        = "Lightning Bolt",
            Description = "Electric strike. Conducts through water, ignites oil.",
            ManaCost    = 8,
            Range       = 10,
            Radius      = 0,
            Target      = SpellTarget.SingleTarget,
            BaseDamage  = 10,
            DamageType  = DamageType.Lightning,
            Color       = ConsoleColor.Yellow
        },

        [SpellId.MageArmor] = new SpellDefinition
        {
            Id            = SpellId.MageArmor,
            Name          = "Mage Armor",
            Description   = "Wraps you in an arcane shield for 20 turns.",
            ManaCost      = 6,
            Range         = 0,
            Radius        = 0,
            Target        = SpellTarget.Self,
            BaseDamage    = 0,
            DamageType    = DamageType.Void,
            AppliedStatus = StatusEffectType.Blessed,
            StatusDuration = 20,
            Color         = ConsoleColor.Blue
        },

        [SpellId.VoidBolt] = new SpellDefinition
        {
            Id          = SpellId.VoidBolt,
            Name        = "Void Bolt",
            Description = "Pure void energy that ignores physical armour.",
            ManaCost    = 20,
            Range       = 8,
            Radius      = 0,
            Target      = SpellTarget.SingleTarget,
            BaseDamage  = 25,
            DamageType  = DamageType.Void,
            Color       = ConsoleColor.DarkMagenta
        },

        [SpellId.Teleport] = new SpellDefinition
        {
            Id          = SpellId.Teleport,
            Name        = "Teleport",
            Description = "Instantly warp to any visible location.",
            ManaCost    = 15,
            Range       = 20,
            Radius      = 0,
            Target      = SpellTarget.Area,
            BaseDamage  = 0,
            DamageType  = DamageType.Void,
            Color       = ConsoleColor.DarkCyan
        },

        [SpellId.CreateWater] = new SpellDefinition
        {
            Id          = SpellId.CreateWater,
            Name        = "Create Water",
            Description = "Conjures water on the target area. Extinguishes fire. Combine with lightning!",
            ManaCost    = 8,
            Range       = 6,
            Radius      = 2,
            Target      = SpellTarget.Area,
            BaseDamage  = 0,
            DamageType  = DamageType.Cold,
            Color       = ConsoleColor.Blue
        },

        [SpellId.DrainLife] = new SpellDefinition
        {
            Id          = SpellId.DrainLife,
            Name        = "Drain Life",
            Description = "Steal 15 HP from an enemy and heal yourself for half.",
            ManaCost    = 12,
            Range       = 6,
            Radius      = 0,
            Target      = SpellTarget.SingleTarget,
            BaseDamage  = 15,
            DamageType  = DamageType.Void,
            Color       = ConsoleColor.DarkRed
        },

        [SpellId.ChainLightning] = new SpellDefinition
        {
            Id          = SpellId.ChainLightning,
            Name        = "Chain Lightning",
            Description = "Strikes primary target then arcs to up to 2 nearby enemies. Conducts through water.",
            ManaCost    = 20,
            Range       = 10,
            Radius      = 0,
            Target      = SpellTarget.SingleTarget,
            BaseDamage  = 12,
            DamageType  = DamageType.Lightning,
            Color       = ConsoleColor.Yellow
        },

        [SpellId.IceStorm] = new SpellDefinition
        {
            Id            = SpellId.IceStorm,
            Name          = "Ice Storm",
            Description   = "Blizzard in a 2-tile radius. Damages and freezes all in range.",
            ManaCost      = 25,
            Range         = 8,
            Radius        = 2,
            Target        = SpellTarget.Area,
            BaseDamage    = 10,
            DamageType    = DamageType.Cold,
            AppliedStatus = StatusEffectType.Frozen,
            StatusDuration = 5,
            Color         = ConsoleColor.Cyan
        },

        [SpellId.Meteor] = new SpellDefinition
        {
            Id            = SpellId.Meteor,
            Name          = "Meteor",
            Description   = "A falling meteorite. Massive fire damage in a 2-tile radius. Leaves fire behind.",
            ManaCost      = 35,
            Range         = 8,
            Radius        = 2,
            Target        = SpellTarget.Area,
            BaseDamage    = 40,
            DamageType    = DamageType.Fire,
            AppliedStatus = StatusEffectType.Burning,
            StatusDuration = 5,
            Color         = ConsoleColor.Red
        },

        // ── Super Spells ────────────────────────────────────────────────────

        [SpellId.EyeInTheSky] = new SpellDefinition
        {
            Id          = SpellId.EyeInTheSky,
            Name        = "Eye in the Sky",
            Description = "Opens an all-seeing eye above the dungeon. Reveals every monster and item on this level.",
            ManaCost    = 50,
            Range       = 0,
            Radius      = 0,
            Target      = SpellTarget.LevelWide,
            BaseDamage  = 0,
            DamageType  = DamageType.Void,
            IsSuperSpell = true,
            Color       = ConsoleColor.White
        }
    };

    public static SpellDefinition Get(SpellId id) => _spells[id];

    public static IReadOnlyDictionary<SpellId, SpellDefinition> GetAll() => _spells;
}
