namespace EldritchDungeon.World;

public class Room
{
    public int X { get; init; }
    public int Y { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public int CenterX => X + Width / 2;
    public int CenterY => Y + Height / 2;
    public bool IsShop { get; set; }

    public bool Intersects(Room other)
    {
        return X < other.X + other.Width
            && X + Width > other.X
            && Y < other.Y + other.Height
            && Y + Height > other.Y;
    }
}
