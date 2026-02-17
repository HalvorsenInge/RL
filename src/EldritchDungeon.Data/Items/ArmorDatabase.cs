using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Data.Items;

public static class ArmorDatabase
{
    private static readonly Dictionary<string, Armor> _armors = new()
    {
        // Tier 1-2: Basic armor (Levels 1-3)
        ["Robe"] = new Armor { Name = "Robe", ArmorClass = 1, Value = 5 },
        ["Leather"] = new Armor { Name = "Leather", ArmorClass = 3, Value = 15 },
        ["Studded Leather"] = new Armor { Name = "Studded Leather", ArmorClass = 4, Value = 25 },
        ["Chainmail"] = new Armor { Name = "Chainmail", ArmorClass = 5, Value = 50 },

        // Tier 3-4: Early Modern (Levels 4-7)
        ["Leather Coat"] = new Armor { Name = "Leather Coat", ArmorClass = 3, Value = 20 },
        ["Trenchcoat"] = new Armor { Name = "Trenchcoat", ArmorClass = 2, Value = 15 },
        ["Reinforced Coat"] = new Armor { Name = "Reinforced Coat", ArmorClass = 5, Value = 60 },
        ["Brigandine"] = new Armor { Name = "Brigandine", ArmorClass = 6, Value = 80 },

        // Tier 5-6: Dieselpunk (Levels 8-12)
        ["Robe of the Deep"] = new Armor { Name = "Robe of the Deep", ArmorClass = 2, Value = 30 },
        ["Clockwork Plate"] = new Armor { Name = "Clockwork Plate", ArmorClass = 8, Value = 150 },
        ["Steam-Forged Mail"] = new Armor { Name = "Steam-Forged Mail", ArmorClass = 7, Value = 120 },
        ["Alchemist's Vest"] = new Armor { Name = "Alchemist's Vest", ArmorClass = 6, Value = 100 },

        // Tier 7-8: Late Dieselpunk/Lovecraftian (Levels 13-18)
        ["Void-Touched Plate"] = new Armor { Name = "Void-Touched Plate", ArmorClass = 10, Value = 250 },
        ["Deep One Scale"] = new Armor { Name = "Deep One Scale", ArmorClass = 9, Value = 200 },
        ["Elder Ward"] = new Armor { Name = "Elder Ward", ArmorClass = 8, Value = 180 },

        // Tier 9-10: Endgame (Levels 19-25)
        ["Star-Metal Carapace"] = new Armor { Name = "Star-Metal Carapace", ArmorClass = 12, Value = 500 },
        ["Shoggoth Hide"] = new Armor { Name = "Shoggoth Hide", ArmorClass = 11, Value = 400 },
    };

    private static readonly Dictionary<string, int[]> _armorTiers = new()
    {
        ["Robe"] = [1, 2],
        ["Leather"] = [1, 2],
        ["Studded Leather"] = [1, 2],
        ["Chainmail"] = [2, 3],
        ["Leather Coat"] = [3, 4],
        ["Trenchcoat"] = [3, 4],
        ["Reinforced Coat"] = [3, 4],
        ["Brigandine"] = [4, 5],
        ["Robe of the Deep"] = [5, 6],
        ["Clockwork Plate"] = [5, 6],
        ["Steam-Forged Mail"] = [5, 6],
        ["Alchemist's Vest"] = [5, 6],
        ["Void-Touched Plate"] = [7, 8],
        ["Deep One Scale"] = [7, 8],
        ["Elder Ward"] = [7, 8],
        ["Star-Metal Carapace"] = [9, 10],
        ["Shoggoth Hide"] = [9, 10],
    };

    public static Armor Get(string name) => _armors[name];

    public static IReadOnlyDictionary<string, Armor> GetAll() => _armors;

    public static IEnumerable<Armor> GetByTierRange(int minTier, int maxTier)
    {
        return _armorTiers
            .Where(kv => kv.Value[0] >= minTier && kv.Value[0] <= maxTier
                      || kv.Value[1] >= minTier && kv.Value[1] <= maxTier)
            .Select(kv => _armors[kv.Key]);
    }
}
