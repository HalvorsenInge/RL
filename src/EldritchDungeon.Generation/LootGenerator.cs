using EldritchDungeon.Core;
using EldritchDungeon.Data.Items;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.World;

namespace EldritchDungeon.Generation;

public class LootGenerator
{
    private readonly Random _random;

    public LootGenerator(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public void PlaceLoot(DungeonMap map, int dungeonLevel)
    {
        var (minTier, maxTier) = LevelTierMapping.GetTierRange(dungeonLevel);

        var weapons = GetWeaponsForTiers(minTier, maxTier);
        var armors = ArmorDatabase.GetByTierRange(minTier, maxTier).ToList();
        var consumables = GetConsumablesForLevel(dungeonLevel);

        // Build combined loot pool
        var lootPool = new List<Item>();
        lootPool.AddRange(weapons);
        lootPool.AddRange(armors);
        lootPool.AddRange(consumables);

        if (lootPool.Count == 0)
            return;

        // Skip room 0 (player spawn)
        for (int i = 1; i < map.Rooms.Count; i++)
        {
            // 40% chance of an item per room
            if (_random.NextDouble() >= 0.4)
                continue;

            int itemCount = _random.Next(1, 3); // 1-2 items per room

            for (int j = 0; j < itemCount; j++)
            {
                var item = CloneItem(lootPool[_random.Next(lootPool.Count)]);

                if (TryPlaceInRoom(map, map.Rooms[i], item))
                {
                    // placed
                }
            }
        }
    }

    private List<Weapon> GetWeaponsForTiers(int minTier, int maxTier)
    {
        var all = WeaponDatabase.GetAll();
        var result = new List<Weapon>();

        // Map weapon categories to tier ranges
        if (minTier <= 2)
            result.AddRange(WeaponDatabase.GetByCategory(WeaponCategory.Medieval));
        if (minTier <= 4 && maxTier >= 3)
            result.AddRange(WeaponDatabase.GetByCategory(WeaponCategory.EarlyModern));
        if (minTier <= 6 && maxTier >= 5)
            result.AddRange(WeaponDatabase.GetByCategory(WeaponCategory.Dieselpunk));
        if (maxTier >= 7)
            result.AddRange(WeaponDatabase.GetByCategory(WeaponCategory.Lovecraftian));

        return result;
    }

    private List<Consumable> GetConsumablesForLevel(int dungeonLevel)
    {
        var result = new List<Consumable>();
        var all = ConsumableDatabase.GetAll();

        // Basic healing/mana always available
        result.Add(all["Healing Potion"]);
        result.Add(all["Mana Potion"]);

        if (dungeonLevel >= 4)
        {
            result.Add(all["Sanity Potion"]);
            result.Add(all["Sanctified Water"]);
            result.Add(all["Elixir of Calm"]);
        }

        if (dungeonLevel >= 8)
        {
            result.Add(all["Deep One Whiskey"]);
            result.Add(all["Mindcrust"]);
        }

        if (dungeonLevel >= 13)
        {
            result.Add(all["Cultist Opium"]);
        }

        if (dungeonLevel >= 19)
        {
            result.Add(all["Void Essence"]);
        }

        return result;
    }

    private bool TryPlaceInRoom(DungeonMap map, Room room, Item item)
    {
        const int maxAttempts = 20;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int x = _random.Next(room.X, room.X + room.Width);
            int y = _random.Next(room.Y, room.Y + room.Height);

            if (map.GetCell(x, y).IsWalkable)
            {
                map.AddItem(item, x, y);
                return true;
            }
        }

        return false;
    }

    private static Item CloneItem(Item source)
    {
        return source switch
        {
            Weapon w => new Weapon
            {
                Name = w.Name,
                Damage = w.Damage,
                CritRangeMin = w.CritRangeMin,
                CritMultiplier = w.CritMultiplier,
                Speed = w.Speed,
                Range = w.Range,
                MaxAmmo = w.MaxAmmo,
                CurrentAmmo = w.CurrentAmmo,
                Category = w.Category,
                Special = w.Special,
                Weight = w.Weight,
                Value = w.Value
            },
            Armor a => new Armor
            {
                Name = a.Name,
                ArmorClass = a.ArmorClass,
                Slot = a.Slot,
                Weight = a.Weight,
                Value = a.Value
            },
            Consumable c => new Consumable
            {
                Name = c.Name,
                HealAmount = c.HealAmount,
                ManaAmount = c.ManaAmount,
                SanityAmount = c.SanityAmount,
                AddictionRisk = c.AddictionRisk,
                Weight = c.Weight,
                Value = c.Value
            },
            _ => new Item
            {
                Name = source.Name,
                Type = source.Type,
                Glyph = source.Glyph,
                Weight = source.Weight,
                Value = source.Value
            }
        };
    }
}
