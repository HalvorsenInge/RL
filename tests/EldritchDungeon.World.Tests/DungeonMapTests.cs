using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.World;

namespace EldritchDungeon.World.Tests;

public class DungeonMapTests
{
    private DungeonMap CreateSmallMap()
    {
        var map = new DungeonMap();
        map.InitializeTiles(20, 20);
        // Carve a small room
        for (int x = 1; x < 10; x++)
            for (int y = 1; y < 10; y++)
                map.SetTile(x, y, TileType.Floor);
        return map;
    }

    [Fact]
    public void InitializeTiles_AllWalls()
    {
        var map = new DungeonMap();
        map.InitializeTiles(10, 10);
        Assert.Equal(TileType.Wall, map.GetTile(0, 0).Type);
        Assert.Equal(TileType.Wall, map.GetTile(5, 5).Type);
    }

    [Fact]
    public void SetTile_ChangesType()
    {
        var map = new DungeonMap();
        map.InitializeTiles(10, 10);
        map.SetTile(5, 5, TileType.Floor);
        Assert.Equal(TileType.Floor, map.GetTile(5, 5).Type);
    }

    [Fact]
    public void IsWalkable_WallTile_ReturnsFalse()
    {
        var map = new DungeonMap();
        map.InitializeTiles(10, 10);
        Assert.False(map.IsWalkable(0, 0));
    }

    [Fact]
    public void IsWalkable_FloorTile_ReturnsTrue()
    {
        var map = CreateSmallMap();
        Assert.True(map.IsWalkable(5, 5));
    }

    [Fact]
    public void IsWalkable_OutOfBounds_ReturnsFalse()
    {
        var map = CreateSmallMap();
        Assert.False(map.IsWalkable(-1, 0));
        Assert.False(map.IsWalkable(0, -1));
        Assert.False(map.IsWalkable(20, 0));
        Assert.False(map.IsWalkable(0, 20));
    }

    [Fact]
    public void IsWalkable_OccupiedByPlayer_ReturnsFalse()
    {
        var map = CreateSmallMap();
        var player = new Player { Name = "Test" };
        map.PlaceActor(player, 5, 5);
        Assert.False(map.IsWalkable(5, 5));
    }

    [Fact]
    public void IsWalkable_OccupiedByMonster_ReturnsFalse()
    {
        var map = CreateSmallMap();
        var monster = new Monster { Name = "Rat" };
        map.PlaceActor(monster, 5, 5);
        Assert.False(map.IsWalkable(5, 5));
    }

    [Fact]
    public void PlaceActor_SetsCoordinates()
    {
        var map = CreateSmallMap();
        var player = new Player { Name = "Test" };
        map.PlaceActor(player, 3, 4);
        Assert.Equal(3, player.X);
        Assert.Equal(4, player.Y);
        Assert.Same(player, map.Player);
    }

    [Fact]
    public void PlaceActor_Monster_AddsToList()
    {
        var map = CreateSmallMap();
        var monster = new Monster { Name = "Rat" };
        map.PlaceActor(monster, 2, 2);
        Assert.Single(map.Monsters);
        Assert.Same(monster, map.Monsters[0]);
    }

    [Fact]
    public void TryMoveActor_ValidMove_ReturnsTrue()
    {
        var map = CreateSmallMap();
        var player = new Player { Name = "Test" };
        map.PlaceActor(player, 5, 5);
        Assert.True(map.TryMoveActor(player, 6, 5));
        Assert.Equal(6, player.X);
        Assert.Equal(5, player.Y);
    }

    [Fact]
    public void TryMoveActor_IntoWall_ReturnsFalse()
    {
        var map = CreateSmallMap();
        var player = new Player { Name = "Test" };
        map.PlaceActor(player, 1, 1);
        Assert.False(map.TryMoveActor(player, 0, 0));
        Assert.Equal(1, player.X);
        Assert.Equal(1, player.Y);
    }

    [Fact]
    public void GetMonsterAt_ReturnsCorrectMonster()
    {
        var map = CreateSmallMap();
        var monster = new Monster { Name = "Goblin" };
        map.PlaceActor(monster, 3, 3);
        Assert.Same(monster, map.GetMonsterAt(3, 3));
        Assert.Null(map.GetMonsterAt(4, 4));
    }
}
