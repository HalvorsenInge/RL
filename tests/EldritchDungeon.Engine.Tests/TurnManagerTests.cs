using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Engine;

namespace EldritchDungeon.Engine.Tests;

public class TurnManagerTests
{
    private static Player CreateTestPlayer()
    {
        Dice.SetSeed(42);
        var player = Player.CreateCharacter(
            "Test", RaceType.Human, ClassType.Warrior,
            new[] { 0, 0, 0, 0, 0, 0 }, 1.0, 1.0, 1.0, 40, 10, 50);
        return player;
    }

    [Fact]
    public void ProcessTurn_PoisonedPlayerTakesDamage()
    {
        Dice.SetSeed(42);
        var engine = new GameEngine();
        var player = CreateTestPlayer();
        engine.Initialize(player);

        int hpBefore = player.Health.CurrentHp;
        player.StatusEffects.AddEffect(StatusEffectType.Poison, 3, 5);

        var turnManager = new TurnManager(engine);
        turnManager.ProcessTurn();

        Assert.True(player.Health.CurrentHp < hpBefore);
    }

    [Fact]
    public void ProcessTurn_UpdatesFov()
    {
        Dice.SetSeed(42);
        var engine = new GameEngine();
        var player = CreateTestPlayer();
        engine.Initialize(player);

        var turnManager = new TurnManager(engine);
        turnManager.ProcessTurn();

        var tile = engine.Map!.GetTile(player.X, player.Y);
        Assert.True(tile.IsInFov);
    }
}
