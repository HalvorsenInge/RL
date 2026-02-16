using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Systems;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems.Tests;

public class AISystemTests
{
    private DungeonMap CreateOpenMap(int width = 20, int height = 20)
    {
        var map = new DungeonMap();
        map.InitializeTiles(width, height);
        // All floor except borders
        for (int x = 1; x < width - 1; x++)
            for (int y = 1; y < height - 1; y++)
                map.SetTile(x, y, TileType.Floor);
        return map;
    }

    [Fact]
    public void Update_MonsterChasesVisiblePlayer()
    {
        var map = CreateOpenMap();
        var player = new Player { Name = "Hero" };
        map.PlaceActor(player, 10, 10);
        var monster = new Monster { Name = "Rat", Glyph = 'r', Tier = 1 };
        map.PlaceActor(monster, 5, 10);

        var ai = new AISystem();
        int initialDistance = Math.Abs(monster.X - player.X) + Math.Abs(monster.Y - player.Y);

        ai.Update(map);

        int newDistance = Math.Abs(monster.X - player.X) + Math.Abs(monster.Y - player.Y);
        Assert.True(newDistance < initialDistance, "Monster should move closer to player");
    }

    [Fact]
    public void Update_MonsterStopsWhenAdjacentToPlayer()
    {
        var map = CreateOpenMap();
        var player = new Player { Name = "Hero" };
        map.PlaceActor(player, 10, 10);
        var monster = new Monster { Name = "Rat", Glyph = 'r', Tier = 1 };
        map.PlaceActor(monster, 11, 10); // Adjacent

        var ai = new AISystem();
        ai.Update(map);

        // Should not have moved onto the player
        Assert.False(monster.X == player.X && monster.Y == player.Y);
        // Should still be adjacent
        Assert.True(Math.Abs(monster.X - player.X) <= 1 && Math.Abs(monster.Y - player.Y) <= 1);
    }

    [Fact]
    public void Update_MonsterIdleWhenPlayerNotVisible()
    {
        var map = new DungeonMap();
        map.InitializeTiles(30, 30);
        // Create two separate rooms with no connection
        for (int x = 1; x < 5; x++)
            for (int y = 1; y < 5; y++)
                map.SetTile(x, y, TileType.Floor);
        for (int x = 20; x < 25; x++)
            for (int y = 20; y < 25; y++)
                map.SetTile(x, y, TileType.Floor);

        var player = new Player { Name = "Hero" };
        map.PlaceActor(player, 2, 2);
        var monster = new Monster { Name = "Rat", Glyph = 'r', Tier = 1 };
        map.PlaceActor(monster, 22, 22);

        int origX = monster.X;
        int origY = monster.Y;

        var ai = new AISystem();
        ai.Update(map);

        Assert.Equal(origX, monster.X);
        Assert.Equal(origY, monster.Y);
    }

    [Fact]
    public void Update_NoPlayer_DoesNotThrow()
    {
        var map = CreateOpenMap();
        var monster = new Monster { Name = "Rat", Glyph = 'r', Tier = 1 };
        map.PlaceActor(monster, 5, 5);

        var ai = new AISystem();
        ai.Update(map); // Should not throw
    }

    [Fact]
    public void Update_MonsterDoesNotMoveOntoPlayer()
    {
        var map = CreateOpenMap();
        var player = new Player { Name = "Hero" };
        map.PlaceActor(player, 10, 10);
        var monster = new Monster { Name = "Rat", Glyph = 'r', Tier = 1 };
        map.PlaceActor(monster, 12, 10);

        var ai = new AISystem();
        // Run multiple turns
        for (int i = 0; i < 5; i++)
            ai.Update(map);

        Assert.False(monster.X == player.X && monster.Y == player.Y,
            "Monster should never occupy player's cell");
    }
}
