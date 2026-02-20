using EldritchDungeon.Core;
using EldritchDungeon.Data.Items;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Components;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.Generation;
using EldritchDungeon.Systems;
using EldritchDungeon.World;

namespace EldritchDungeon.Engine;

public class GameEngine
{
    public DungeonMap? Map { get; private set; }
    public Player? Player { get; private set; }
    public int DungeonLevel { get; private set; }
    public MessageLog Log { get; } = new();
    public bool IsRunning { get; private set; } = true;

    public CombatSystem CombatSystem { get; }
    public LevelingSystem LevelingSystem { get; }
    public StatusEffectSystem StatusEffectSystem { get; }
    public SanitySystem SanitySystem { get; }
    public ReligionSystem ReligionSystem { get; }
    public AddictionSystem AddictionSystem { get; }
    public AISystem AISystem { get; }
    public MagicSystem MagicSystem { get; }

    public GameEngine()
    {
        CombatSystem = new CombatSystem(Log.Add);
        LevelingSystem = new LevelingSystem(Log.Add);
        StatusEffectSystem = new StatusEffectSystem(Log.Add);
        SanitySystem = new SanitySystem(Log.Add);
        ReligionSystem = new ReligionSystem(Log.Add);
        AddictionSystem = new AddictionSystem(Log.Add);
        AISystem = new AISystem(CombatSystem);
        MagicSystem = new MagicSystem(Log.Add);
    }

    public void Initialize(Player player)
    {
        Player = player;
        DungeonLevel = 1;
        GenerateLevel();
        Log.Add("Welcome to the Eldritch Dungeon. Tread carefully...");
    }

    public void DescendStairs()
    {
        if (DungeonLevel >= GameConstants.MaxDungeonLevel)
        {
            Log.Add("You have reached the deepest level!");
            return;
        }

        DungeonLevel++;
        GenerateLevel();
        Log.Add($"You descend to dungeon level {DungeonLevel}.");
    }

    public bool PlayerDied { get; private set; }

    public void RequestQuit()
    {
        IsRunning = false;
    }

    public void OnPlayerDeath()
    {
        PlayerDied = true;
        IsRunning = false;
    }

    public void LoadFromSave(SaveData data)
    {
        var player = new Player
        {
            Name = data.PlayerName,
            Race = data.Race,
            Class = data.Class,
            Gold = data.Gold,
            AddictionLevel = data.AddictionLevel,
        };
        player.InitializeComponents();

        // Stats
        player.Stats.Strength = data.Strength;
        player.Stats.Dexterity = data.Dexterity;
        player.Stats.Constitution = data.Constitution;
        player.Stats.Intelligence = data.Intelligence;
        player.Stats.Wisdom = data.Wisdom;
        player.Stats.Charisma = data.Charisma;
        player.Stats.Level = data.Level;
        player.Stats.Experience = data.Experience;
        player.Stats.ExperienceToNextLevel = data.ExperienceToNextLevel;

        // Resources
        player.Health.MaxHp = data.MaxHp;
        player.Health.CurrentHp = data.CurrentHp;
        player.Mana.MaxMana = data.MaxMana;
        player.Mana.CurrentMana = data.CurrentMana;
        player.Sanity.MaxSanity = data.MaxSanity;
        player.Sanity.CurrentSanity = data.CurrentSanity;
        player.Sanity.InsanityResist = data.InsanityResist;

        // Religion
        if (data.CurrentGod != null)
        {
            player.Religion.SetGod(data.CurrentGod.Value);
            player.Religion.AddFavor(data.Favor);
            player.Religion.AddAnger(data.Anger);
        }

        // Status effects
        foreach (var effect in data.StatusEffects)
        {
            player.StatusEffects.AddEffect(effect.Type, effect.RemainingTurns, effect.Magnitude);
        }

        // Known spells
        foreach (var spell in data.KnownSpells)
            player.KnownSpells.Add(spell);

        // Inventory
        foreach (var saved in data.InventoryItems)
        {
            player.Inventory.AddItem(SaveManager.DeserializeItem(saved));
        }

        // Equipment
        foreach (var (slotName, savedItem) in data.EquippedItems)
        {
            if (savedItem != null && Enum.TryParse<EquipmentSlot>(slotName, out var slot))
            {
                player.Equipment.Equip(slot, SaveManager.DeserializeItem(savedItem));
            }
        }

        Player = player;
        DungeonLevel = data.DungeonLevel;

        // Rebuild map
        var map = new DungeonMap();
        map.InitializeTiles(data.MapWidth, data.MapHeight);

        // Restore tiles
        for (int y = 0; y < data.MapHeight; y++)
        {
            for (int x = 0; x < data.MapWidth; x++)
            {
                int index = y * data.MapWidth + x;
                if (index < data.Tiles.Count)
                {
                    var savedTile = data.Tiles[index];
                    map.SetTile(x, y, savedTile.Type);
                    map.GetTile(x, y).IsExplored = savedTile.IsExplored;
                }
            }
        }

        // Restore rooms
        foreach (var savedRoom in data.Rooms)
        {
            map.Rooms.Add(new Room
            {
                X = savedRoom.X,
                Y = savedRoom.Y,
                Width = savedRoom.Width,
                Height = savedRoom.Height
            });
        }

        // Restore stairs
        foreach (var savedStairs in data.Stairs)
        {
            map.StairsList.Add(new Stairs
            {
                X = savedStairs.X,
                Y = savedStairs.Y,
                IsDown = savedStairs.IsDown
            });
        }

        // Place player
        map.PlaceActor(player, data.PlayerX, data.PlayerY);

        // Restore monsters
        foreach (var savedMonster in data.Monsters)
        {
            var monster = new Monster
            {
                Name = savedMonster.Name,
                Glyph = savedMonster.Glyph,
                Tier = savedMonster.Tier,
                Damage = savedMonster.Damage,
                XpValue = savedMonster.XpValue,
                SanityDamage = savedMonster.SanityDamage
            };
            monster.Health.MaxHp = savedMonster.MaxHp;
            monster.Health.CurrentHp = savedMonster.CurrentHp;
            map.PlaceActor(monster, savedMonster.X, savedMonster.Y);
        }

        // Restore map items
        foreach (var savedItem in data.MapItems)
        {
            map.AddItem(SaveManager.DeserializeItem(savedItem), savedItem.X, savedItem.Y);
        }

        // Restore shops
        foreach (var savedShop in data.Shops)
        {
            var inventory = savedShop.Inventory.Select(SaveManager.DeserializeItem).ToList();
            map.Shops.Add((savedShop.X, savedShop.Y, inventory));
        }

        Map = map;

        // Restore FOV
        map.UpdateFov(player.X, player.Y, GameConstants.DefaultFovRadius);

        // Restore messages
        foreach (var msg in data.Messages)
        {
            Log.Add(msg);
        }

        Log.Add("Game loaded. Welcome back.");
    }

    private void GenerateLevel()
    {
        var generator = new MapGenerator();
        Map = generator.Generate(DungeonLevel);

        // Place player in first room
        if (Map.Rooms.Count > 0)
        {
            var startRoom = Map.Rooms[0];
            Map.PlaceActor(Player!, startRoom.CenterX, startRoom.CenterY);
        }

        // Place monsters
        var placer = new MonsterPlacer();
        placer.PlaceMonsters(Map, DungeonLevel);

        // Place loot
        var lootGen = new LootGenerator();
        lootGen.PlaceLoot(Map, DungeonLevel);

        // Populate shop rooms
        foreach (var room in Map.Rooms.Where(r => r.IsShop))
        {
            var stock = ShopDatabase.GenerateStock(DungeonLevel);
            Map.Shops.Add((room.CenterX, room.CenterY, stock));
        }

        // Initial FOV
        Map.UpdateFov(Player!.X, Player.Y, GameConstants.DefaultFovRadius);
    }
}
