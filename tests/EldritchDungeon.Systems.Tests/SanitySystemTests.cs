using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Systems;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems.Tests;

public class SanitySystemTests
{
    private readonly List<string> _messages = new();
    private readonly SanitySystem _system;

    public SanitySystemTests()
    {
        Dice.SetSeed(42);
        _system = new SanitySystem(msg => _messages.Add(msg));
    }

    private static (DungeonMap map, Player player) CreateTestSetup()
    {
        var map = new DungeonMap();
        map.InitializeTiles(20, 20);
        for (int x = 1; x < 19; x++)
            for (int y = 1; y < 19; y++)
                map.SetTile(x, y, TileType.Floor);

        var player = new Player { Name = "Tester" };
        player.InitializeComponents();
        player.Sanity.MaxSanity = 100;
        player.Sanity.CurrentSanity = 100;
        map.PlaceActor(player, 5, 5);
        map.UpdateFov(5, 5, GameConstants.DefaultFovRadius);

        return (map, player);
    }

    [Fact]
    public void Update_FirstSightCausesSanityLoss()
    {
        var (map, player) = CreateTestSetup();
        var monster = new Monster
        {
            Name = "Eldritch Horror",
            SanityDamage = 15,
            Tier = 3
        };
        monster.InitializeComponents();
        monster.Health.MaxHp = 20;
        monster.Health.CurrentHp = 20;
        map.PlaceActor(monster, 6, 5);
        map.UpdateFov(5, 5, GameConstants.DefaultFovRadius);

        _system.Update(map);

        Assert.True(player.Sanity.CurrentSanity < 100);
        Assert.Contains(_messages, m => m.Contains("Eldritch Horror"));
    }

    [Fact]
    public void Update_NoRepeatDamageForSameMonsterType()
    {
        var (map, player) = CreateTestSetup();
        var monster = new Monster
        {
            Name = "Eldritch Horror",
            SanityDamage = 15,
            Tier = 3
        };
        monster.InitializeComponents();
        monster.Health.MaxHp = 20;
        monster.Health.CurrentHp = 20;
        map.PlaceActor(monster, 6, 5);
        map.UpdateFov(5, 5, GameConstants.DefaultFovRadius);

        _system.Update(map);
        int sanityAfterFirst = player.Sanity.CurrentSanity;

        _system.Update(map);
        Assert.Equal(sanityAfterFirst, player.Sanity.CurrentSanity);
    }

    [Fact]
    public void Update_NoEffectForZeroSanityDamageMonsters()
    {
        var (map, player) = CreateTestSetup();
        var monster = new Monster
        {
            Name = "Rat",
            SanityDamage = 0,
            Tier = 1
        };
        monster.InitializeComponents();
        monster.Health.MaxHp = 5;
        monster.Health.CurrentHp = 5;
        map.PlaceActor(monster, 6, 5);
        map.UpdateFov(5, 5, GameConstants.DefaultFovRadius);

        _system.Update(map);

        Assert.Equal(100, player.Sanity.CurrentSanity);
    }

    [Fact]
    public void Update_MonstersOutOfFovDontCauseDamage()
    {
        var (map, player) = CreateTestSetup();
        var monster = new Monster
        {
            Name = "Far Horror",
            SanityDamage = 20,
            Tier = 5
        };
        monster.InitializeComponents();
        monster.Health.MaxHp = 20;
        monster.Health.CurrentHp = 20;
        // Place far away, behind walls
        map.PlaceActor(monster, 18, 18);
        // Don't update FOV for monster position

        _system.Update(map);

        Assert.Equal(100, player.Sanity.CurrentSanity);
    }

    [Fact]
    public void Update_InsanityResistReducesDamage()
    {
        var (map, player) = CreateTestSetup();
        player.Sanity.InsanityResist = 0.5; // 50% resist

        var monster = new Monster
        {
            Name = "Scary Thing",
            SanityDamage = 20,
            Tier = 3
        };
        monster.InitializeComponents();
        monster.Health.MaxHp = 20;
        monster.Health.CurrentHp = 20;
        map.PlaceActor(monster, 6, 5);
        map.UpdateFov(5, 5, GameConstants.DefaultFovRadius);

        _system.Update(map);

        Assert.Equal(90, player.Sanity.CurrentSanity); // 20 * 0.5 = 10 effective
    }
}
