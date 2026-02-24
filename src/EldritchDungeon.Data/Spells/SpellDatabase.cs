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

        // ── New standard spells ────────────────────────────────────────────

        [SpellId.Blink] = new SpellDefinition
        {
            Id = SpellId.Blink, Name = "Blink",
            Description = "Teleport to a random adjacent walkable tile. Great escape.",
            ManaCost = 6, Range = 1, Radius = 0,
            Target = SpellTarget.Self, BaseDamage = 0, DamageType = DamageType.Void,
            Color = ConsoleColor.DarkCyan
        },

        [SpellId.PhaseStep] = new SpellDefinition
        {
            Id = SpellId.PhaseStep, Name = "Phase Step",
            Description = "Pass through walls for 5 turns. Cannot end your turn inside a wall.",
            ManaCost = 14, Range = 0, Radius = 0,
            Target = SpellTarget.Self, BaseDamage = 0, DamageType = DamageType.Void,
            AppliedStatus = StatusEffectType.Blessed, StatusDuration = 5,
            Color = ConsoleColor.DarkCyan
        },

        [SpellId.MirrorImage] = new SpellDefinition
        {
            Id = SpellId.MirrorImage, Name = "Mirror Image",
            Description = "Create 3 illusory copies. Enemies attack copies at random.",
            ManaCost = 12, Range = 0, Radius = 0,
            Target = SpellTarget.Self, BaseDamage = 0, DamageType = DamageType.Void,
            AppliedStatus = StatusEffectType.Blessed, StatusDuration = 12,
            Color = ConsoleColor.White
        },

        [SpellId.RaiseDead] = new SpellDefinition
        {
            Id = SpellId.RaiseDead, Name = "Raise Dead",
            Description = "Animate the nearest slain monster as a temporary ally (15 turns).",
            ManaCost = 15, Range = 5, Radius = 0,
            Target = SpellTarget.Area, BaseDamage = 0, DamageType = DamageType.Void,
            Color = ConsoleColor.DarkGreen
        },

        [SpellId.BoneSpear] = new SpellDefinition
        {
            Id = SpellId.BoneSpear, Name = "Bone Spear",
            Description = "A piercing bone projectile that hits every enemy in a line.",
            ManaCost = 9, Range = 10, Radius = 0,
            Target = SpellTarget.SingleTarget, BaseDamage = 22, DamageType = DamageType.Physical,
            Color = ConsoleColor.Gray
        },

        [SpellId.Petrify] = new SpellDefinition
        {
            Id = SpellId.Petrify, Name = "Petrify",
            Description = "Turn an enemy to stone for 5 turns (stunned, takes +50% damage). Non-boss only.",
            ManaCost = 16, Range = 7, Radius = 0,
            Target = SpellTarget.SingleTarget, BaseDamage = 0, DamageType = DamageType.Void,
            AppliedStatus = StatusEffectType.Frozen, StatusDuration = 5,
            Color = ConsoleColor.DarkGray
        },

        [SpellId.GravityWell] = new SpellDefinition
        {
            Id = SpellId.GravityWell, Name = "Gravity Well",
            Description = "Pull all enemies within 4 tiles toward the target point.",
            ManaCost = 14, Range = 8, Radius = 4,
            Target = SpellTarget.Area, BaseDamage = 8, DamageType = DamageType.Physical,
            Color = ConsoleColor.DarkMagenta
        },

        [SpellId.BloodBoil] = new SpellDefinition
        {
            Id = SpellId.BloodBoil, Name = "Blood Boil",
            Description = "Internal damage over 4 turns. Bypasses armour entirely.",
            ManaCost = 18, Range = 7, Radius = 0,
            Target = SpellTarget.SingleTarget, BaseDamage = 35, DamageType = DamageType.Void,
            AppliedStatus = StatusEffectType.Poison, StatusDuration = 4,
            Color = ConsoleColor.DarkRed
        },

        [SpellId.Wither] = new SpellDefinition
        {
            Id = SpellId.Wither, Name = "Wither",
            Description = "Reduce target's max HP by 20% permanently. Stacks.",
            ManaCost = 12, Range = 7, Radius = 0,
            Target = SpellTarget.SingleTarget, BaseDamage = 0, DamageType = DamageType.Void,
            AppliedStatus = StatusEffectType.Cursed, StatusDuration = 999,
            Color = ConsoleColor.DarkGreen
        },

        [SpellId.EldritchDrain] = new SpellDefinition
        {
            Id = SpellId.EldritchDrain, Name = "Eldritch Drain",
            Description = "Steal 15 HP and 5 sanity from target. You absorb both.",
            ManaCost = 10, Range = 6, Radius = 0,
            Target = SpellTarget.SingleTarget, BaseDamage = 15, DamageType = DamageType.Sanity,
            Color = ConsoleColor.DarkRed
        },

        [SpellId.SwarmOfRats] = new SpellDefinition
        {
            Id = SpellId.SwarmOfRats, Name = "Swarm of Rats",
            Description = "Summon 2d4 rats that harry enemies for 8 turns.",
            ManaCost = 10, Range = 5, Radius = 2,
            Target = SpellTarget.Area, BaseDamage = 0, DamageType = DamageType.Physical,
            Color = ConsoleColor.DarkYellow
        },

        [SpellId.StaticField] = new SpellDefinition
        {
            Id = SpellId.StaticField, Name = "Static Field",
            Description = "Electrify yourself for 5 turns — zaps all adjacent enemies for 8 lightning/turn.",
            ManaCost = 14, Range = 0, Radius = 0,
            Target = SpellTarget.Self, BaseDamage = 8, DamageType = DamageType.Lightning,
            AppliedStatus = StatusEffectType.Blessed, StatusDuration = 5,
            Color = ConsoleColor.Yellow
        },

        [SpellId.Silence] = new SpellDefinition
        {
            Id = SpellId.Silence, Name = "Silence",
            Description = "Prevent all spellcasting in a 3-tile radius for 6 turns. Affects you too.",
            ManaCost = 8, Range = 6, Radius = 3,
            Target = SpellTarget.Area, BaseDamage = 0, DamageType = DamageType.Void,
            Color = ConsoleColor.DarkGray
        },

        [SpellId.ShadowStep] = new SpellDefinition
        {
            Id = SpellId.ShadowStep, Name = "Shadow Step",
            Description = "Teleport directly behind target enemy. Perfect backstab setup.",
            ManaCost = 10, Range = 8, Radius = 0,
            Target = SpellTarget.SingleTarget, BaseDamage = 0, DamageType = DamageType.Void,
            Color = ConsoleColor.DarkGray
        },

        [SpellId.Entangle] = new SpellDefinition
        {
            Id = SpellId.Entangle, Name = "Entangle",
            Description = "Root all enemies in a 2-tile radius for 4 turns.",
            ManaCost = 9, Range = 6, Radius = 2,
            Target = SpellTarget.Area, BaseDamage = 0, DamageType = DamageType.Physical,
            AppliedStatus = StatusEffectType.Frozen, StatusDuration = 4,
            Color = ConsoleColor.Green
        },

        [SpellId.SummonHorror] = new SpellDefinition
        {
            Id = SpellId.SummonHorror, Name = "Summon Horror",
            Description = "Summon an uncontrolled Lovecraftian creature (tier 4-6). It may attack you.",
            ManaCost = 25, Range = 4, Radius = 0,
            Target = SpellTarget.Area, BaseDamage = 0, DamageType = DamageType.Sanity,
            Color = ConsoleColor.DarkMagenta
        },

        [SpellId.DeathWord] = new SpellDefinition
        {
            Id = SpellId.DeathWord, Name = "Death Word",
            Description = "Instantly kills an enemy below 20% HP. Does nothing above that.",
            ManaCost = 20, Range = 8, Radius = 0,
            Target = SpellTarget.SingleTarget, BaseDamage = 0, DamageType = DamageType.Void,
            Color = ConsoleColor.DarkRed
        },

        [SpellId.Polymorph] = new SpellDefinition
        {
            Id = SpellId.Polymorph, Name = "Polymorph",
            Description = "Transform an enemy into a harmless creature for 10 turns.",
            ManaCost = 22, Range = 7, Radius = 0,
            Target = SpellTarget.SingleTarget, BaseDamage = 0, DamageType = DamageType.Void,
            AppliedStatus = StatusEffectType.Frozen, StatusDuration = 10,
            Color = ConsoleColor.Green
        },

        [SpellId.TimeEcho] = new SpellDefinition
        {
            Id = SpellId.TimeEcho, Name = "Time Echo",
            Description = "Rewind your last move (1/floor). Restores HP/position to one turn ago.",
            ManaCost = 30, Range = 0, Radius = 0,
            Target = SpellTarget.Self, BaseDamage = 0, DamageType = DamageType.Void,
            Color = ConsoleColor.Cyan
        },

        // ── Super Spells ────────────────────────────────────────────────────

        [SpellId.EyeInTheSky] = new SpellDefinition
        {
            Id = SpellId.EyeInTheSky, Name = "Eye in the Sky",
            Description = "Opens an all-seeing eye. Reveals every monster and item on this level.",
            ManaCost = 50, Range = 0, Radius = 0,
            Target = SpellTarget.LevelWide, BaseDamage = 0, DamageType = DamageType.Void,
            IsSuperSpell = true, Color = ConsoleColor.White
        },

        [SpellId.ArmageddonRain] = new SpellDefinition
        {
            Id = SpellId.ArmageddonRain, Name = "Armageddon Rain",
            Description = "Meteors hammer every room. 40-80 fire damage to all enemies. Sets floor tiles alight.",
            ManaCost = 60, Range = 0, Radius = 0,
            Target = SpellTarget.LevelWide, BaseDamage = 60, DamageType = DamageType.Fire,
            IsSuperSpell = true, Color = ConsoleColor.Red
        },

        [SpellId.MassPetrification] = new SpellDefinition
        {
            Id = SpellId.MassPetrification, Name = "Mass Petrification",
            Description = "Every non-boss enemy on the floor is frozen in stone for 8 turns.",
            ManaCost = 55, Range = 0, Radius = 0,
            Target = SpellTarget.LevelWide, BaseDamage = 0, DamageType = DamageType.Void,
            AppliedStatus = StatusEffectType.Frozen, StatusDuration = 8,
            IsSuperSpell = true, Color = ConsoleColor.Gray
        },

        [SpellId.RealityFracture] = new SpellDefinition
        {
            Id = SpellId.RealityFracture, Name = "Reality Fracture",
            Description = "Shatter all walls in a 6-tile radius. Spawn 1d4 random monsters. Chaos.",
            ManaCost = 65, Range = 0, Radius = 6,
            Target = SpellTarget.Self, BaseDamage = 0, DamageType = DamageType.Void,
            IsSuperSpell = true, Color = ConsoleColor.DarkMagenta
        },

        [SpellId.TheDreamingWord] = new SpellDefinition
        {
            Id = SpellId.TheDreamingWord, Name = "The Dreaming Word",
            Description = "A psychic scream. All enemies on the floor lose 1d8×5 sanity. Weaker enemies may go mad.",
            ManaCost = 70, Range = 0, Radius = 0,
            Target = SpellTarget.LevelWide, BaseDamage = 0, DamageType = DamageType.Sanity,
            IsSuperSpell = true, Color = ConsoleColor.DarkMagenta
        },

        [SpellId.SummonHorde] = new SpellDefinition
        {
            Id = SpellId.SummonHorde, Name = "Summon Horde",
            Description = "Raise 3d6 undead allies (tier 1-3). They fight until killed or floor changes.",
            ManaCost = 50, Range = 0, Radius = 0,
            Target = SpellTarget.LevelWide, BaseDamage = 0, DamageType = DamageType.Void,
            IsSuperSpell = true, Color = ConsoleColor.DarkGreen
        },

        [SpellId.VoidCollapse] = new SpellDefinition
        {
            Id = SpellId.VoidCollapse, Name = "Void Collapse",
            Description = "Open a singularity at target. Pulls all within 5 tiles (10 void/turn × 5 turns), then explodes for 80 AoE.",
            ManaCost = 80, Range = 8, Radius = 5,
            Target = SpellTarget.Area, BaseDamage = 80, DamageType = DamageType.Void,
            IsSuperSpell = true, Color = ConsoleColor.DarkMagenta
        },
    };

    public static SpellDefinition Get(SpellId id) => _spells[id];

    public static IReadOnlyDictionary<SpellId, SpellDefinition> GetAll() => _spells;
}
