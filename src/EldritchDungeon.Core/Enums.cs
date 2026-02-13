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
    Void
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
