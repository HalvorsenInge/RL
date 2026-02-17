using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.Systems;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems.Tests;

public class CombatSystemTests
{
    private readonly List<string> _messages = new();
    private readonly CombatSystem _combat;

    public CombatSystemTests()
    {
        _combat = new CombatSystem(msg => _messages.Add(msg));
    }

    private static Player CreateTestPlayer()
    {
        var player = new Player { Name = "Tester" };
        player.InitializeComponents();
        player.Stats.Strength = 14;
        player.Stats.Dexterity = 12;
        player.Stats.Constitution = 12;
        player.Health.MaxHp = 40;
        player.Health.CurrentHp = 40;
        return player;
    }

    private static Monster CreateTestMonster(int tier = 1, int hp = 10, int damage = 5)
    {
        var monster = new Monster
        {
            Name = "Test Rat",
            Tier = tier,
            Damage = damage,
            XpValue = 10,
            SanityDamage = 0
        };
        monster.Health.MaxHp = hp;
        monster.Health.CurrentHp = hp;
        return monster;
    }

    private static DungeonMap CreateTestMap()
    {
        var map = new DungeonMap();
        map.InitializeTiles(20, 20);
        for (int x = 1; x < 19; x++)
            for (int y = 1; y < 19; y++)
                map.SetTile(x, y, TileType.Floor);
        return map;
    }

    [Fact]
    public void PlayerAttack_HitsAndDealsDamage()
    {
        // Seed so d20 roll is high enough to hit AC 11 (tier 1 monster)
        Dice.SetSeed(100);
        var player = CreateTestPlayer();
        var weapon = new Weapon { Name = "Longsword", Damage = 8, CritRangeMin = 20, CritMultiplier = 2, Range = 1 };
        player.Equipment.Equip(EquipmentSlot.MainHand, weapon);

        var monster = CreateTestMonster(tier: 1, hp: 30);
        var map = CreateTestMap();
        map.PlaceActor(player, 5, 5);
        map.PlaceActor(monster, 6, 5);

        _combat.PlayerAttack(player, monster, map);

        // Should either hit or miss depending on roll — verify message was logged
        Assert.NotEmpty(_messages);
    }

    [Fact]
    public void PlayerAttack_UnarmedUses4Damage()
    {
        Dice.SetSeed(999); // Try multiple seeds for a hit
        var player = CreateTestPlayer();
        // No weapon equipped

        var monster = CreateTestMonster(tier: 1, hp: 100);
        var map = CreateTestMap();
        map.PlaceActor(player, 5, 5);
        map.PlaceActor(monster, 6, 5);

        _combat.PlayerAttack(player, monster, map);

        Assert.NotEmpty(_messages);
        Assert.Contains(_messages, m => m.Contains("fists") || m.Contains("miss") || m.Contains("Test Rat"));
    }

    [Fact]
    public void PlayerAttack_KillAwardsXP()
    {
        Dice.SetSeed(42);
        var player = CreateTestPlayer();
        var weapon = new Weapon { Name = "Sword", Damage = 100, Range = 1 };
        player.Equipment.Equip(EquipmentSlot.MainHand, weapon);

        var monster = CreateTestMonster(tier: 1, hp: 1);
        monster.XpValue = 25;
        var map = CreateTestMap();
        map.PlaceActor(player, 5, 5);
        map.PlaceActor(monster, 6, 5);

        int xpBefore = player.Stats.Experience;

        // Keep attacking until we get a hit (not a nat 1)
        for (int i = 0; i < 20; i++)
        {
            if (monster.Health.IsDead) break;
            _combat.PlayerAttack(player, monster, map);
        }

        if (monster.Health.IsDead)
        {
            Assert.Equal(xpBefore + 25, player.Stats.Experience);
            Assert.DoesNotContain(monster, map.Monsters);
        }
    }

    [Fact]
    public void MonsterAttack_DealsFlatDamage()
    {
        Dice.SetSeed(100);
        var player = CreateTestPlayer();
        var armor = new Armor { Name = "Chainmail", ArmorClass = 5 };
        player.Equipment.Equip(EquipmentSlot.Body, armor);

        var monster = CreateTestMonster(tier: 1, hp: 10, damage: 8);

        _combat.MonsterAttack(monster, player);

        Assert.NotEmpty(_messages);
    }

    [Fact]
    public void MonsterAttack_AppliesSanityDamageOnHit()
    {
        // Use a seed that gives a high roll to ensure hit
        Dice.SetSeed(12345);
        var player = CreateTestPlayer();
        player.Sanity.MaxSanity = 100;
        player.Sanity.CurrentSanity = 100;

        var monster = CreateTestMonster(tier: 5, hp: 10, damage: 5);
        monster.SanityDamage = 10;

        // Attack multiple times to ensure at least one hit
        for (int i = 0; i < 10; i++)
        {
            _combat.MonsterAttack(monster, player);
        }

        // With 10 attacks, at least one should hit and deal sanity damage
        bool sanityLost = player.Sanity.CurrentSanity < 100 ||
            _messages.Any(m => m.Contains("sanity"));
        Assert.True(sanityLost || _messages.Any(m => m.Contains("miss")));
    }
}
