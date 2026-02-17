using EldritchDungeon.Core;
using EldritchDungeon.Engine;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Components;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.World;

namespace EldritchDungeon.Engine.Tests;

public class SaveManagerTests
{
    [Fact]
    public void SerializeAndDeserialize_Weapon_RoundTrips()
    {
        var saved = new SavedItem
        {
            Name = "Longsword",
            ItemKind = "Weapon",
            Damage = 12,
            CritRangeMin = 19,
            CritMultiplier = 2,
            Speed = 2,
            Range = 1,
            Category = WeaponCategory.Medieval,
            Special = "Parry +1",
            Value = 50
        };

        var item = SaveManager.DeserializeItem(saved);

        Assert.IsType<Weapon>(item);
        var weapon = (Weapon)item;
        Assert.Equal("Longsword", weapon.Name);
        Assert.Equal(12, weapon.Damage);
        Assert.Equal(19, weapon.CritRangeMin);
        Assert.Equal(2, weapon.CritMultiplier);
        Assert.Equal(2, weapon.Speed);
        Assert.Equal(1, weapon.Range);
        Assert.Equal(WeaponCategory.Medieval, weapon.Category);
        Assert.Equal("Parry +1", weapon.Special);
        Assert.Equal(50, weapon.Value);
    }

    [Fact]
    public void SerializeAndDeserialize_Armor_RoundTrips()
    {
        var saved = new SavedItem
        {
            Name = "Chainmail",
            ItemKind = "Armor",
            ArmorClass = 5,
            Slot = EquipmentSlot.Body,
            Value = 50
        };

        var item = SaveManager.DeserializeItem(saved);

        Assert.IsType<Armor>(item);
        var armor = (Armor)item;
        Assert.Equal("Chainmail", armor.Name);
        Assert.Equal(5, armor.ArmorClass);
        Assert.Equal(EquipmentSlot.Body, armor.Slot);
    }

    [Fact]
    public void SerializeAndDeserialize_Consumable_RoundTrips()
    {
        var saved = new SavedItem
        {
            Name = "Mindcrust",
            ItemKind = "Consumable",
            SanityAmount = 50,
            AddictionRisk = 30
        };

        var item = SaveManager.DeserializeItem(saved);

        Assert.IsType<Consumable>(item);
        var consumable = (Consumable)item;
        Assert.Equal("Mindcrust", consumable.Name);
        Assert.Equal(50, consumable.SanityAmount);
        Assert.Equal(30, consumable.AddictionRisk);
    }

    [Fact]
    public void SaveData_ContainsAllPlayerFields()
    {
        var data = new SaveData
        {
            PlayerName = "TestHero",
            Race = RaceType.Dwarf,
            Class = ClassType.Warrior,
            Gold = 100,
            AddictionLevel = 25,
            PlayerX = 10,
            PlayerY = 5,
            CurrentHp = 30,
            MaxHp = 50,
            CurrentMana = 10,
            MaxMana = 20,
            CurrentSanity = 80,
            MaxSanity = 100,
            InsanityResist = 0.1,
            Strength = 16,
            Dexterity = 14,
            Constitution = 15,
            Intelligence = 10,
            Wisdom = 12,
            Charisma = 8,
            Level = 3,
            Experience = 250,
            ExperienceToNextLevel = 400,
            CurrentGod = GodType.Dagon,
            Favor = 30,
            Anger = 5,
            DungeonLevel = 4,
            MapWidth = 80,
            MapHeight = 21
        };

        Assert.Equal("TestHero", data.PlayerName);
        Assert.Equal(RaceType.Dwarf, data.Race);
        Assert.Equal(ClassType.Warrior, data.Class);
        Assert.Equal(100, data.Gold);
        Assert.Equal(25, data.AddictionLevel);
        Assert.Equal(GodType.Dagon, data.CurrentGod);
        Assert.Equal(4, data.DungeonLevel);
    }

    [Fact]
    public void SaveData_StatusEffects_PreservedCorrectly()
    {
        var data = new SaveData();
        data.StatusEffects.Add(new SavedStatusEffect
        {
            Type = StatusEffectType.Poison,
            RemainingTurns = 5,
            Magnitude = 3
        });
        data.StatusEffects.Add(new SavedStatusEffect
        {
            Type = StatusEffectType.Blessed,
            RemainingTurns = 10,
            Magnitude = 0
        });

        Assert.Equal(2, data.StatusEffects.Count);
        Assert.Equal(StatusEffectType.Poison, data.StatusEffects[0].Type);
        Assert.Equal(5, data.StatusEffects[0].RemainingTurns);
        Assert.Equal(StatusEffectType.Blessed, data.StatusEffects[1].Type);
    }

    [Fact]
    public void SaveData_Tiles_FlattenedCorrectly()
    {
        var data = new SaveData
        {
            MapWidth = 3,
            MapHeight = 2
        };

        // Flatten a 3x2 grid
        data.Tiles.Add(new SavedTile { Type = TileType.Wall, IsExplored = false });
        data.Tiles.Add(new SavedTile { Type = TileType.Floor, IsExplored = true });
        data.Tiles.Add(new SavedTile { Type = TileType.Wall, IsExplored = false });
        data.Tiles.Add(new SavedTile { Type = TileType.Floor, IsExplored = true });
        data.Tiles.Add(new SavedTile { Type = TileType.Door, IsExplored = true });
        data.Tiles.Add(new SavedTile { Type = TileType.Floor, IsExplored = false });

        Assert.Equal(6, data.Tiles.Count);
        // Index = y * width + x, so (1, 0) is index 1, (2, 1) is index 5
        Assert.Equal(TileType.Floor, data.Tiles[1].Type);
        Assert.True(data.Tiles[1].IsExplored);
        Assert.Equal(TileType.Floor, data.Tiles[5].Type);
    }

    [Fact]
    public void SaveData_Equipment_SerializesSlotNames()
    {
        var data = new SaveData();
        data.EquippedItems["MainHand"] = new SavedItem
        {
            Name = "Dagger",
            ItemKind = "Weapon",
            Damage = 4
        };
        data.EquippedItems["Body"] = new SavedItem
        {
            Name = "Leather",
            ItemKind = "Armor",
            ArmorClass = 3
        };
        data.EquippedItems["Head"] = null;

        Assert.Equal(3, data.EquippedItems.Count);
        Assert.NotNull(data.EquippedItems["MainHand"]);
        Assert.Equal("Dagger", data.EquippedItems["MainHand"]!.Name);
        Assert.Null(data.EquippedItems["Head"]);
    }

    [Fact]
    public void SaveData_MapItems_IncludePosition()
    {
        var data = new SaveData();
        data.MapItems.Add(new SavedItem
        {
            Name = "Healing Potion",
            ItemKind = "Consumable",
            HealAmount = 25,
            X = 15,
            Y = 8
        });

        Assert.Single(data.MapItems);
        Assert.Equal(15, data.MapItems[0].X);
        Assert.Equal(8, data.MapItems[0].Y);
        Assert.Equal("Healing Potion", data.MapItems[0].Name);
    }

    [Fact]
    public void SaveData_Monsters_PreservedWithHp()
    {
        var data = new SaveData();
        data.Monsters.Add(new SavedMonster
        {
            Name = "Goblin",
            Glyph = 'g',
            Tier = 1,
            Damage = 3,
            XpValue = 10,
            SanityDamage = 0,
            CurrentHp = 5,
            MaxHp = 8,
            X = 20,
            Y = 10
        });

        Assert.Single(data.Monsters);
        Assert.Equal("Goblin", data.Monsters[0].Name);
        Assert.Equal(5, data.Monsters[0].CurrentHp);
        Assert.Equal(8, data.Monsters[0].MaxHp);
    }
}
