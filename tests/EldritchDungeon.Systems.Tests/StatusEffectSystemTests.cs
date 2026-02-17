using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Systems;

namespace EldritchDungeon.Systems.Tests;

public class StatusEffectSystemTests
{
    private readonly List<string> _messages = new();
    private readonly StatusEffectSystem _system;

    public StatusEffectSystemTests()
    {
        _system = new StatusEffectSystem(msg => _messages.Add(msg));
    }

    private static Actor CreateTestActor()
    {
        var monster = new Monster { Name = "TestMonster" };
        monster.InitializeComponents();
        monster.Health.MaxHp = 100;
        monster.Health.CurrentHp = 100;
        return monster;
    }

    [Fact]
    public void ProcessActor_PoisonDealsDamage()
    {
        var actor = CreateTestActor();
        actor.StatusEffects.AddEffect(StatusEffectType.Poison, 3, 5);

        _system.ProcessActor(actor);

        Assert.Equal(95, actor.Health.CurrentHp);
        Assert.Contains(_messages, m => m.Contains("poison"));
    }

    [Fact]
    public void ProcessActor_BleedingDealsHalfMagnitude()
    {
        var actor = CreateTestActor();
        actor.StatusEffects.AddEffect(StatusEffectType.Bleeding, 3, 6);

        _system.ProcessActor(actor);

        Assert.Equal(97, actor.Health.CurrentHp); // 6/2 = 3 damage
    }

    [Fact]
    public void ProcessActor_BurningDealsDamage()
    {
        var actor = CreateTestActor();
        actor.StatusEffects.AddEffect(StatusEffectType.Burning, 2, 10);

        _system.ProcessActor(actor);

        Assert.Equal(90, actor.Health.CurrentHp);
    }

    [Fact]
    public void ProcessActor_EffectExpires()
    {
        var actor = CreateTestActor();
        actor.StatusEffects.AddEffect(StatusEffectType.Poison, 1, 5);

        _system.ProcessActor(actor);

        Assert.False(actor.StatusEffects.HasEffect(StatusEffectType.Poison));
    }

    [Fact]
    public void ProcessActor_MultipleEffectsApply()
    {
        var actor = CreateTestActor();
        actor.StatusEffects.AddEffect(StatusEffectType.Poison, 3, 5);
        actor.StatusEffects.AddEffect(StatusEffectType.Burning, 3, 10);

        _system.ProcessActor(actor);

        Assert.Equal(85, actor.Health.CurrentHp); // 5 + 10 = 15 damage
    }

    [Fact]
    public void IsIncapacitated_StunnedReturnsTrue()
    {
        var actor = CreateTestActor();
        actor.StatusEffects.AddEffect(StatusEffectType.Stunned, 2);

        Assert.True(StatusEffectSystem.IsIncapacitated(actor));
    }

    [Fact]
    public void IsIncapacitated_ParalyzedReturnsTrue()
    {
        var actor = CreateTestActor();
        actor.StatusEffects.AddEffect(StatusEffectType.Paralyzed, 2);

        Assert.True(StatusEffectSystem.IsIncapacitated(actor));
    }

    [Fact]
    public void IsIncapacitated_NoEffectsReturnsFalse()
    {
        var actor = CreateTestActor();

        Assert.False(StatusEffectSystem.IsIncapacitated(actor));
    }
}
