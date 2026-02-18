using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;
using RogueSharp;

namespace EldritchDungeon.World;

public class DungeonMap : Map
{
    private Tile[,] _tiles;

    public List<Room> Rooms { get; } = new();
    public List<Stairs> StairsList { get; } = new();
    public List<Monster> Monsters { get; } = new();
    public List<(Item Item, int X, int Y)> Items { get; } = new();
    public Player? Player { get; set; }

    public DungeonMap()
    {
        _tiles = new Tile[0, 0];
    }

    public void InitializeTiles(int width, int height)
    {
        Initialize(width, height);
        _tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile { Type = TileType.Wall };
                SetCellProperties(x, y, false, false);
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        return _tiles[x, y];
    }

    public void SetTile(int x, int y, TileType type)
    {
        _tiles[x, y].Type = type;

        bool isTransparent = type != TileType.Wall;
        bool isWalkable = type != TileType.Wall;
        SetCellProperties(x, y, isTransparent, isWalkable);
    }

    public new bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;

        if (!GetCell(x, y).IsWalkable)
            return false;

        if (Player != null && Player.X == x && Player.Y == y)
            return false;

        if (GetMonsterAt(x, y) != null)
            return false;

        return true;
    }

    public void PlaceActor(Actor actor, int x, int y)
    {
        actor.X = x;
        actor.Y = y;

        if (actor is Player player)
            Player = player;
        else if (actor is Monster monster && !Monsters.Contains(monster))
            Monsters.Add(monster);
    }

    public bool TryMoveActor(Actor actor, int newX, int newY)
    {
        if (!IsWalkable(newX, newY))
            return false;

        actor.X = newX;
        actor.Y = newY;
        return true;
    }

    public Monster? GetMonsterAt(int x, int y)
    {
        return Monsters.FirstOrDefault(m => m.X == x && m.Y == y);
    }

    public List<Item> GetItemsAt(int x, int y)
    {
        return Items.Where(i => i.X == x && i.Y == y).Select(i => i.Item).ToList();
    }

    public void AddItem(Item item, int x, int y)
    {
        Items.Add((item, x, y));
    }

    public bool RemoveItem(Item item, int x, int y)
    {
        var index = Items.FindIndex(i => i.Item == item && i.X == x && i.Y == y);
        if (index < 0) return false;
        Items.RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Returns true if there is an unobstructed line between (x1,y1) and (x2,y2).
    /// Uses Bresenham line and checks that all intermediate cells are transparent.
    /// </summary>
    public bool HasLineOfSight(int x1, int y1, int x2, int y2)
    {
        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;
        int cx = x1, cy = y1;

        while (true)
        {
            // Only check cells that are neither the source nor the destination
            bool isSource = cx == x1 && cy == y1;
            bool isDest   = cx == x2 && cy == y2;

            if (!isSource && !isDest)
            {
                if (cx >= 0 && cx < Width && cy >= 0 && cy < Height)
                {
                    if (!GetCell(cx, cy).IsTransparent)
                        return false;
                }
            }

            if (isDest)
                break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; cx += sx; }
            if (e2 <  dx) { err += dx; cy += sy; }
        }

        return true;
    }

    public void UpdateFov(int x, int y, int radius)
    {
        // Clear previous FOV
        for (int tx = 0; tx < Width; tx++)
        {
            for (int ty = 0; ty < Height; ty++)
            {
                _tiles[tx, ty].IsInFov = false;
            }
        }

        var fov = new FieldOfView(this);
        var visibleCells = fov.ComputeFov(x, y, radius, true);

        foreach (var cell in visibleCells)
        {
            _tiles[cell.X, cell.Y].IsInFov = true;
            _tiles[cell.X, cell.Y].IsExplored = true;
        }
    }
}
