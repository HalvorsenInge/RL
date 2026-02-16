using EldritchDungeon.World;

namespace EldritchDungeon.World.Tests;

public class RoomTests
{
    [Fact]
    public void CenterX_ReturnsCorrectCenter()
    {
        var room = new Room { X = 10, Y = 5, Width = 8, Height = 6 };
        Assert.Equal(14, room.CenterX);
    }

    [Fact]
    public void CenterY_ReturnsCorrectCenter()
    {
        var room = new Room { X = 10, Y = 5, Width = 8, Height = 6 };
        Assert.Equal(8, room.CenterY);
    }

    [Fact]
    public void Intersects_OverlappingRooms_ReturnsTrue()
    {
        var room1 = new Room { X = 0, Y = 0, Width = 10, Height = 10 };
        var room2 = new Room { X = 5, Y = 5, Width = 10, Height = 10 };
        Assert.True(room1.Intersects(room2));
    }

    [Fact]
    public void Intersects_NonOverlappingRooms_ReturnsFalse()
    {
        var room1 = new Room { X = 0, Y = 0, Width = 5, Height = 5 };
        var room2 = new Room { X = 10, Y = 10, Width = 5, Height = 5 };
        Assert.False(room1.Intersects(room2));
    }

    [Fact]
    public void Intersects_AdjacentRooms_ReturnsFalse()
    {
        var room1 = new Room { X = 0, Y = 0, Width = 5, Height = 5 };
        var room2 = new Room { X = 5, Y = 0, Width = 5, Height = 5 };
        Assert.False(room1.Intersects(room2));
    }

    [Fact]
    public void Intersects_IsSymmetric()
    {
        var room1 = new Room { X = 0, Y = 0, Width = 10, Height = 10 };
        var room2 = new Room { X = 5, Y = 5, Width = 10, Height = 10 };
        Assert.Equal(room1.Intersects(room2), room2.Intersects(room1));
    }
}
