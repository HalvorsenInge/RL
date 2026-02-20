namespace EldritchDungeon.Core;

public enum SanityState
{
    Stable,      // 100-51
    Fractured,   // 50-26
    Unraveling,  // 25-10
    Broken       // 9-0
}

public enum DamageType
{
    Physical,
    Mana,
    Sanity,
    Holy,
    Void,
    Fire,
    Cold,
    Lightning
}

public enum SpellId
{
    MagicBolt,
    Fireball,
    FrostBolt,
    LightningBolt,
    MageArmor,
    VoidBolt,
    Teleport,
    CreateWater,
    DrainLife,
    ChainLightning,
    IceStorm,
    Meteor,
    // Super spells
    EyeInTheSky
}

public enum SpellTarget
{
    Self,
    SingleTarget,
    Area,
    LevelWide
}

public enum TileEffect
{
    None,
    Water,
    Fire,
    Steam,
    Oil
}

public enum WeaponCategory
{
    Medieval,
    EarlyModern,
    Dieselpunk,
    Lovecraftian
}

public enum RaceType
{
    Human,
    Elf,
    Dwarf,
    Halfling,
    Orc,
    DeepOneHybrid,
    HalfMad,
    SerpentFolk
}

public enum ClassType
{
    Warrior,
    Mage,
    Rogue,
    Cultist,
    Gunslinger,
    Investigator
}

public enum GodType
{
    Cthulhu,
    Nyarlathotep,
    Azathoth,
    YogSothoth,
    Hastur,
    Dagon
}

public enum StatType
{
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma
}

public enum EquipmentSlot
{
    MainHand,
    OffHand,
    Head,
    Body,
    Feet,
    Ring,
    Amulet
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Scroll,
    Artifact,
    Ammo,
    Tool
}

public enum StatusEffectType
{
    Poison,
    Bleeding,
    Stunned,
    Drunk,
    Hallucinating,
    Addicted,
    Withdrawal,
    Burning,
    Frozen,
    Paralyzed,
    Invisible,
    Blessed,
    Cursed
}

public enum TileType
{
    Wall,
    Floor,
    Door,
    StairsDown,
    StairsUp
}

public enum MonsterAIState
{
    Idle,
    Aggressive
}
