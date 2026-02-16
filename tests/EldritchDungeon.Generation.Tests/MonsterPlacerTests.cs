using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Generation;
using EldritchDungeon.World;

namespace EldritchDungeon.Generation.Tests;

public class MonsterPlacerTests
{
    private DungeonMap CreateMapWithRooms()
    {
        var generator = new MapGenerator(random: new Random(42));
        return generator.Generate();
    }

    [Fact]
    public void PlaceMonsters_DoesNotPlaceMonstersInFirstRoom()
    {
        var map = CreateMapWithRooms();
        var placer = new MonsterPlacer(new Random(42));
        placer.PlaceMonsters(map, 1);

        var firstRoom = map.Rooms[0];
        foreach (var monster in map.Monsters)
        {
            bool inFirstRoom = monster.X >= firstRoom.X
                && monster.X < firstRoom.X + firstRoom.Width
                && monster.Y >= firstRoom.Y
                && monster.Y < firstRoom.Y + firstRoom.Height;
            Assert.False(inFirstRoom, $"Monster {monster.Name} placed in first room at ({monster.X},{monster.Y})");
        }
    }

    [Fact]
    public void PlaceMonsters_CreatesCorrectTierMonsters_Level1()
    {
        var map = CreateMapWithRooms();
        var placer = new MonsterPlacer(new Random(42));
        placer.PlaceMonsters(map, 1);

        foreach (var monster in map.Monsters)
        {
            Assert.InRange(monster.Tier, 1, 2);
        }
    }

    [Fact]
    public void PlaceMonsters_CreatesCorrectTierMonsters_Level10()
    {
        var map = CreateMapWithRooms();
        var placer = new MonsterPlacer(new Random(42));
        placer.PlaceMonsters(map, 10);

        foreach (var monster in map.Monsters)
        {
            Assert.InRange(monster.Tier, 5, 6);
        }
    }

    [Fact]
    public void PlaceMonsters_CreatesCorrectTierMonsters_Level20()
    {
        var map = CreateMapWithRooms();
        var placer = new MonsterPlacer(new Random(42));
        placer.PlaceMonsters(map, 20);

        foreach (var monster in map.Monsters)
        {
            Assert.InRange(monster.Tier, 9, 10);
        }
    }

    [Fact]
    public void PlaceMonsters_MonstersHaveValidPositions()
    {
        var map = CreateMapWithRooms();
        var placer = new MonsterPlacer(new Random(42));
        placer.PlaceMonsters(map, 1);

        foreach (var monster in map.Monsters)
        {
            Assert.InRange(monster.X, 0, map.Width - 1);
            Assert.InRange(monster.Y, 0, map.Height - 1);
            Assert.Equal(TileType.Floor, map.GetTile(monster.X, monster.Y).Type);
        }
    }

    [Fact]
    public void PlaceMonsters_MonstersHaveHpSet()
    {
        var map = CreateMapWithRooms();
        var placer = new MonsterPlacer(new Random(42));
        placer.PlaceMonsters(map, 1);

        Assert.True(map.Monsters.Count > 0, "Expected at least one monster placed");
        foreach (var monster in map.Monsters)
        {
            Assert.True(monster.Health.MaxHp > 0);
            Assert.Equal(monster.Health.MaxHp, monster.Health.CurrentHp);
        }
    }

    [Fact]
    public void PlaceMonsters_DeterministicWithSeed()
    {
        var map1 = CreateMapWithRooms();
        var placer1 = new MonsterPlacer(new Random(99));
        placer1.PlaceMonsters(map1, 5);

        var map2 = CreateMapWithRooms();
        var placer2 = new MonsterPlacer(new Random(99));
        placer2.PlaceMonsters(map2, 5);

        Assert.Equal(map1.Monsters.Count, map2.Monsters.Count);
        for (int i = 0; i < map1.Monsters.Count; i++)
        {
            Assert.Equal(map1.Monsters[i].Name, map2.Monsters[i].Name);
            Assert.Equal(map1.Monsters[i].X, map2.Monsters[i].X);
            Assert.Equal(map1.Monsters[i].Y, map2.Monsters[i].Y);
        }
    }
}
