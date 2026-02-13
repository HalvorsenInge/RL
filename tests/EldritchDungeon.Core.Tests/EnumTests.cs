using EldritchDungeon.Core;

namespace EldritchDungeon.Core.Tests;

public class EnumTests
{
    [Fact]
    public void RaceType_Has8Values()
    {
        Assert.Equal(8, Enum.GetValues<RaceType>().Length);
    }

    [Fact]
    public void ClassType_Has6Values()
    {
        Assert.Equal(6, Enum.GetValues<ClassType>().Length);
    }

    [Fact]
    public void GodType_Has6Values()
    {
        Assert.Equal(6, Enum.GetValues<GodType>().Length);
    }

    [Fact]
    public void StatType_Has6Values()
    {
        Assert.Equal(6, Enum.GetValues<StatType>().Length);
    }

    [Fact]
    public void SanityState_Has4Values()
    {
        Assert.Equal(4, Enum.GetValues<SanityState>().Length);
    }

    [Fact]
    public void DamageType_Has5Values()
    {
        Assert.Equal(5, Enum.GetValues<DamageType>().Length);
    }

    [Fact]
    public void WeaponCategory_Has4Values()
    {
        Assert.Equal(4, Enum.GetValues<WeaponCategory>().Length);
    }

    [Fact]
    public void EquipmentSlot_Has7Values()
    {
        Assert.Equal(7, Enum.GetValues<EquipmentSlot>().Length);
    }
}
