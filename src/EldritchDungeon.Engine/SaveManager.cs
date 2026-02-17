using System.Text.Json;
using System.Text.Json.Serialization;
using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Components;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.World;

namespace EldritchDungeon.Engine;

#region SaveData DTOs

public class SavedStatusEffect
{
    public StatusEffectType Type { get; set; }
    public int RemainingTurns { get; set; }
    public int Magnitude { get; set; }
}

public class SavedItem
{
    public string Name { get; set; } = string.Empty;
    public string ItemKind { get; set; } = string.Empty; // "Weapon", "Armor", "Consumable"

    // Weapon fields
    public int Damage { get; set; }
    public int CritRangeMin { get; set; }
    public int CritMultiplier { get; set; }
    public int Speed { get; set; }
    public int Range { get; set; }
    public int MaxAmmo { get; set; }
    public int CurrentAmmo { get; set; }
    public WeaponCategory Category { get; set; }
    public string Special { get; set; } = string.Empty;

    // Armor fields
    public int ArmorClass { get; set; }
    public EquipmentSlot Slot { get; set; }

    // Consumable fields
    public int HealAmount { get; set; }
    public int ManaAmount { get; set; }
    public int SanityAmount { get; set; }
    public int AddictionRisk { get; set; }

    // Common
    public double Weight { get; set; }
    public int Value { get; set; }

    // Position (for map items)
    public int X { get; set; }
    public int Y { get; set; }
}

public class SavedMonster
{
    public string Name { get; set; } = string.Empty;
    public char Glyph { get; set; }
    public int Tier { get; set; }
    public int Damage { get; set; }
    public int XpValue { get; set; }
    public int SanityDamage { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public class SavedRoom
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class SavedStairs
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsDown { get; set; }
}

public class SavedTile
{
    public TileType Type { get; set; }
    public bool IsExplored { get; set; }
}

public class SaveData
{
    // Player state
    public string PlayerName { get; set; } = string.Empty;
    public RaceType Race { get; set; }
    public ClassType Class { get; set; }
    public int Gold { get; set; }
    public int AddictionLevel { get; set; }
    public int PlayerX { get; set; }
    public int PlayerY { get; set; }

    // Components (flattened)
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int CurrentMana { get; set; }
    public int MaxMana { get; set; }
    public int CurrentSanity { get; set; }
    public int MaxSanity { get; set; }
    public double InsanityResist { get; set; }

    // Stats
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNextLevel { get; set; }

    // Religion
    public GodType? CurrentGod { get; set; }
    public int Favor { get; set; }
    public int Anger { get; set; }

    // Status effects
    public List<SavedStatusEffect> StatusEffects { get; set; } = new();

    // Inventory & Equipment
    public List<SavedItem> InventoryItems { get; set; } = new();
    public Dictionary<string, SavedItem?> EquippedItems { get; set; } = new();

    // Dungeon state
    public int DungeonLevel { get; set; }
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public List<SavedTile> Tiles { get; set; } = new(); // flattened: y * width + x
    public List<SavedRoom> Rooms { get; set; } = new();
    public List<SavedStairs> Stairs { get; set; } = new();
    public List<SavedMonster> Monsters { get; set; } = new();
    public List<SavedItem> MapItems { get; set; } = new();

    // Message log (last N messages)
    public List<string> Messages { get; set; } = new();
}

#endregion

public static class SaveManager
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string GetSavePath()
    {
        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saves");
        return Path.Combine(dir, "save.json");
    }

    public static bool SaveExists()
    {
        return File.Exists(GetSavePath());
    }

    public static void Save(GameEngine engine)
    {
        var data = BuildSaveData(engine);
        var path = GetSavePath();

        var dir = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(data, _jsonOptions);
        File.WriteAllText(path, json);
    }

    public static SaveData? Load()
    {
        var path = GetSavePath();
        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SaveData>(json, _jsonOptions);
    }

    public static void DeleteSave()
    {
        var path = GetSavePath();
        if (File.Exists(path))
            File.Delete(path);
    }

    #region Build SaveData from engine

    private static SaveData BuildSaveData(GameEngine engine)
    {
        var player = engine.Player!;
        var map = engine.Map!;

        var data = new SaveData
        {
            // Player
            PlayerName = player.Name,
            Race = player.Race,
            Class = player.Class,
            Gold = player.Gold,
            AddictionLevel = player.AddictionLevel,
            PlayerX = player.X,
            PlayerY = player.Y,

            // Health/Mana/Sanity
            CurrentHp = player.Health.CurrentHp,
            MaxHp = player.Health.MaxHp,
            CurrentMana = player.Mana.CurrentMana,
            MaxMana = player.Mana.MaxMana,
            CurrentSanity = player.Sanity.CurrentSanity,
            MaxSanity = player.Sanity.MaxSanity,
            InsanityResist = player.Sanity.InsanityResist,

            // Stats
            Strength = player.Stats.Strength,
            Dexterity = player.Stats.Dexterity,
            Constitution = player.Stats.Constitution,
            Intelligence = player.Stats.Intelligence,
            Wisdom = player.Stats.Wisdom,
            Charisma = player.Stats.Charisma,
            Level = player.Stats.Level,
            Experience = player.Stats.Experience,
            ExperienceToNextLevel = player.Stats.ExperienceToNextLevel,

            // Religion
            CurrentGod = player.Religion.CurrentGod,
            Favor = player.Religion.Favor,
            Anger = player.Religion.Anger,

            // Dungeon
            DungeonLevel = engine.DungeonLevel,
            MapWidth = map.Width,
            MapHeight = map.Height,
        };

        // Status effects
        foreach (var effect in player.StatusEffects.ActiveEffects)
        {
            data.StatusEffects.Add(new SavedStatusEffect
            {
                Type = effect.Type,
                RemainingTurns = effect.RemainingTurns,
                Magnitude = effect.Magnitude
            });
        }

        // Inventory
        foreach (var item in player.Inventory.Items)
        {
            data.InventoryItems.Add(SerializeItem(item));
        }

        // Equipment
        foreach (var (slot, item) in player.Equipment.Slots)
        {
            data.EquippedItems[slot.ToString()] = item != null ? SerializeItem(item) : null;
        }

        // Tiles (flattened)
        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                var tile = map.GetTile(x, y);
                data.Tiles.Add(new SavedTile
                {
                    Type = tile.Type,
                    IsExplored = tile.IsExplored
                });
            }
        }

        // Rooms
        foreach (var room in map.Rooms)
        {
            data.Rooms.Add(new SavedRoom
            {
                X = room.X, Y = room.Y,
                Width = room.Width, Height = room.Height
            });
        }

        // Stairs
        foreach (var stairs in map.StairsList)
        {
            data.Stairs.Add(new SavedStairs
            {
                X = stairs.X, Y = stairs.Y, IsDown = stairs.IsDown
            });
        }

        // Monsters
        foreach (var monster in map.Monsters)
        {
            data.Monsters.Add(new SavedMonster
            {
                Name = monster.Name,
                Glyph = monster.Glyph,
                Tier = monster.Tier,
                Damage = monster.Damage,
                XpValue = monster.XpValue,
                SanityDamage = monster.SanityDamage,
                CurrentHp = monster.Health.CurrentHp,
                MaxHp = monster.Health.MaxHp,
                X = monster.X,
                Y = monster.Y
            });
        }

        // Map items
        foreach (var (item, x, y) in map.Items)
        {
            var saved = SerializeItem(item);
            saved.X = x;
            saved.Y = y;
            data.MapItems.Add(saved);
        }

        // Messages (last 50)
        var messages = engine.Log.Messages;
        int start = Math.Max(0, messages.Count - 50);
        for (int i = start; i < messages.Count; i++)
            data.Messages.Add(messages[i]);

        return data;
    }

    private static SavedItem SerializeItem(Item item)
    {
        var saved = new SavedItem
        {
            Name = item.Name,
            Weight = item.Weight,
            Value = item.Value
        };

        switch (item)
        {
            case Weapon w:
                saved.ItemKind = "Weapon";
                saved.Damage = w.Damage;
                saved.CritRangeMin = w.CritRangeMin;
                saved.CritMultiplier = w.CritMultiplier;
                saved.Speed = w.Speed;
                saved.Range = w.Range;
                saved.MaxAmmo = w.MaxAmmo;
                saved.CurrentAmmo = w.CurrentAmmo;
                saved.Category = w.Category;
                saved.Special = w.Special;
                break;
            case Armor a:
                saved.ItemKind = "Armor";
                saved.ArmorClass = a.ArmorClass;
                saved.Slot = a.Slot;
                break;
            case Consumable c:
                saved.ItemKind = "Consumable";
                saved.HealAmount = c.HealAmount;
                saved.ManaAmount = c.ManaAmount;
                saved.SanityAmount = c.SanityAmount;
                saved.AddictionRisk = c.AddictionRisk;
                break;
        }

        return saved;
    }

    #endregion

    #region Restore from SaveData

    public static Item DeserializeItem(SavedItem saved)
    {
        return saved.ItemKind switch
        {
            "Weapon" => new Weapon
            {
                Name = saved.Name,
                Damage = saved.Damage,
                CritRangeMin = saved.CritRangeMin,
                CritMultiplier = saved.CritMultiplier,
                Speed = saved.Speed,
                Range = saved.Range,
                MaxAmmo = saved.MaxAmmo,
                CurrentAmmo = saved.CurrentAmmo,
                Category = saved.Category,
                Special = saved.Special,
                Weight = saved.Weight,
                Value = saved.Value
            },
            "Armor" => new Armor
            {
                Name = saved.Name,
                ArmorClass = saved.ArmorClass,
                Slot = saved.Slot,
                Weight = saved.Weight,
                Value = saved.Value
            },
            "Consumable" => new Consumable
            {
                Name = saved.Name,
                HealAmount = saved.HealAmount,
                ManaAmount = saved.ManaAmount,
                SanityAmount = saved.SanityAmount,
                AddictionRisk = saved.AddictionRisk,
                Weight = saved.Weight,
                Value = saved.Value
            },
            _ => new Item
            {
                Name = saved.Name,
                Weight = saved.Weight,
                Value = saved.Value
            }
        };
    }

    #endregion
}
