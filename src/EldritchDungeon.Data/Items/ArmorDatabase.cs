using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Data.Items;

public static class ArmorDatabase
{
    private static readonly Dictionary<string, Armor> _armors = new()
    {
        ["Robe"] = new Armor { Name = "Robe", ArmorClass = 1 },
        ["Robe of the Deep"] = new Armor { Name = "Robe of the Deep", ArmorClass = 2 },
        ["Leather"] = new Armor { Name = "Leather", ArmorClass = 3 },
        ["Leather Coat"] = new Armor { Name = "Leather Coat", ArmorClass = 3 },
        ["Chainmail"] = new Armor { Name = "Chainmail", ArmorClass = 5 },
        ["Trenchcoat"] = new Armor { Name = "Trenchcoat", ArmorClass = 2 },
    };

    public static Armor Get(string name) => _armors[name];

    public static IReadOnlyDictionary<string, Armor> GetAll() => _armors;
}
