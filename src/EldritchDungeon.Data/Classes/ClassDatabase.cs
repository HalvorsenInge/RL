using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Classes;

public static class ClassDatabase
{
    private static readonly Dictionary<ClassType, ClassDefinition> _classes = new()
    {
        [ClassType.Warrior] = new ClassDefinition
        {
            Type = ClassType.Warrior, Name = "Warrior",
            BaseHp = 40, BaseMana = 10, StartingGold = 50,
            StartingWeapons = new List<string> { "Longsword" },
            StartingArmor = "Chainmail",
            StartingItems = new List<string> { "Healing Potion", "Healing Potion", "Healing Potion" }
        },
        [ClassType.Mage] = new ClassDefinition
        {
            Type = ClassType.Mage, Name = "Mage",
            BaseHp = 25, BaseMana = 40, StartingGold = 30,
            StartingWeapons = new List<string> { "Dagger" },
            StartingArmor = "Robe",
            StartingItems = new List<string> { "Spellbook (Fireball)", "Mana Potion", "Mana Potion" }
        },
        [ClassType.Rogue] = new ClassDefinition
        {
            Type = ClassType.Rogue, Name = "Rogue",
            BaseHp = 30, BaseMana = 15, StartingGold = 75,
            StartingWeapons = new List<string> { "Shortbow", "Dagger" },
            StartingArmor = "Leather",
            StartingItems = new List<string> { "15 Arrows", "Lockpick Set", "Healing Potion", "Healing Potion" }
        },
        [ClassType.Cultist] = new ClassDefinition
        {
            Type = ClassType.Cultist, Name = "Cultist",
            BaseHp = 28, BaseMana = 35, StartingGold = 25,
            StartingWeapons = new List<string> { "Bone Dagger", "Dagger" },
            StartingArmor = "Robe of the Deep",
            StartingItems = new List<string> { "Chalk (Summoning)", "Sanity Potion" }
        },
        [ClassType.Gunslinger] = new ClassDefinition
        {
            Type = ClassType.Gunslinger, Name = "Gunslinger",
            BaseHp = 32, BaseMana = 10, StartingGold = 40,
            StartingWeapons = new List<string> { "Flintlock Pistol", "Dagger" },
            StartingArmor = "Leather Coat",
            StartingItems = new List<string> { "24 Bullets", "Healing Potion" }
        },
        [ClassType.Investigator] = new ClassDefinition
        {
            Type = ClassType.Investigator, Name = "Investigator",
            BaseHp = 30, BaseMana = 20, StartingGold = 60,
            StartingWeapons = new List<string> { "Revolver", "Club" },
            StartingArmor = "Trenchcoat",
            StartingItems = new List<string> { "18 Bullets", "Notebook", "Magnifying Glass" }
        }
    };

    public static ClassDefinition Get(ClassType type) => _classes[type];

    public static IReadOnlyDictionary<ClassType, ClassDefinition> GetAll() => _classes;
}
