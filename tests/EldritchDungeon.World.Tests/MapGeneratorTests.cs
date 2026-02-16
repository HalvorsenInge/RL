using EldritchDungeon.Core;
using EldritchDungeon.World;

namespace EldritchDungeon.World.Tests;

public class MapGeneratorTests
{
    [Fact]
    public void Generate_CreatesRooms()
    {
        var generator = new MapGenerator(random: new Random(42));
        var map = generator.Generate();
        Assert.True(map.Rooms.Count > 0);
    }

    [Fact]
    public void Generate_RoomsDoNotExceedMaxRooms()
    {
        var generator = new MapGenerator(random: new Random(42));
        var map = generator.Generate();
        Assert.True(map.Rooms.Count <= GameConstants.MaxRooms);
    }

    [Fact]
    public void Generate_PlacesStairs()
    {
        var generator = new MapGenerator(random: new Random(42));
        var map = generator.Generate();
        Assert.Equal(2, map.StairsList.Count);
        Assert.Contains(map.StairsList, s => !s.IsDown);
        Assert.Contains(map.StairsList, s => s.IsDown);
    }

    [Fact]
    public void Generate_StairsUpInFirstRoom()
    {
        var generator = new MapGenerator(random: new Random(42));
        var map = generator.Generate();
        var stairsUp = map.StairsList.First(s => !s.IsDown);
        var firstRoom = map.Rooms[0];
        Assert.Equal(firstRoom.CenterX, stairsUp.X);
        Assert.Equal(firstRoom.CenterY, stairsUp.Y);
    }

    [Fact]
    public void Generate_StairsDownInLastRoom()
    {
        var generator = new MapGenerator(random: new Random(42));
        var map = generator.Generate();
        var stairsDown = map.StairsList.First(s => s.IsDown);
        var lastRoom = map.Rooms[^1];
        Assert.Equal(lastRoom.CenterX, stairsDown.X);
        Assert.Equal(lastRoom.CenterY, stairsDown.Y);
    }

    [Fact]
    public void Generate_DeterministicWithSeed()
    {
        var gen1 = new MapGenerator(random: new Random(123));
        var gen2 = new MapGenerator(random: new Random(123));
        var map1 = gen1.Generate();
        var map2 = gen2.Generate();

        Assert.Equal(map1.Rooms.Count, map2.Rooms.Count);
        for (int i = 0; i < map1.Rooms.Count; i++)
        {
            Assert.Equal(map1.Rooms[i].X, map2.Rooms[i].X);
            Assert.Equal(map1.Rooms[i].Y, map2.Rooms[i].Y);
        }
    }

    [Fact]
    public void Generate_HasFloorTiles()
    {
        var generator = new MapGenerator(random: new Random(42));
        var map = generator.Generate();

        bool hasFloor = false;
        for (int x = 0; x < map.Width && !hasFloor; x++)
            for (int y = 0; y < map.Height && !hasFloor; y++)
                if (map.GetTile(x, y).Type == TileType.Floor)
                    hasFloor = true;

        Assert.True(hasFloor);
    }

    [Fact]
    public void Generate_CorrectDimensions()
    {
        var generator = new MapGenerator(random: new Random(42));
        var map = generator.Generate();
        Assert.Equal(GameConstants.MapWidth, map.Width);
        Assert.Equal(GameConstants.MapHeight, map.Height);
    }
}
