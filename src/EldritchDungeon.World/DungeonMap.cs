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
    public List<(int X, int Y, List<Item> Inventory)> Shops { get; } = new();
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

    // ── Tile Effect API ──────────────────────────────────────────────────────

    public TileEffect GetTileEffect(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return TileEffect.None;
        return _tiles[x, y].Effect;
    }

    /// <summary>
    /// Sets a tile effect at (x, y). duration=-1 means permanent.
    /// Steam blocks line-of-sight; setting or clearing it updates RogueSharp cell transparency.
    /// </summary>
    public void SetTileEffect(int x, int y, TileEffect effect, int duration = -1)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return;
        var tile = _tiles[x, y];
        if (tile.Type == TileType.Wall) return;

        var oldEffect = tile.Effect;
        tile.Effect = effect;
        tile.EffectDuration = duration;

        // Keep RogueSharp transparency in sync for Steam (blocks FOV/LOS)
        bool wasBlocking = oldEffect == TileEffect.Steam;
        bool nowBlocking = effect == TileEffect.Steam;
        if (wasBlocking != nowBlocking)
        {
            bool transparent = tile.Type != TileType.Wall && !nowBlocking;
            bool walkable = tile.Type != TileType.Wall;
            SetCellProperties(x, y, transparent, walkable);
        }
    }

    /// <summary>
    /// Ticks all tile effects down by one turn and clears expired ones.
    /// Returns a list of (x, y, expiredEffect) for caller to react to.
    /// </summary>
    public List<(int X, int Y, TileEffect Effect)> TickTileEffects()
    {
        var expired = new List<(int, int, TileEffect)>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var tile = _tiles[x, y];
                if (tile.Effect == TileEffect.None || tile.EffectDuration == -1) continue;

                tile.EffectDuration--;
                if (tile.EffectDuration <= 0)
                {
                    var old = tile.Effect;
                    tile.Effect = TileEffect.None;
                    // Restore transparency if Steam expired
                    if (old == TileEffect.Steam)
                        SetCellProperties(x, y, tile.Type != TileType.Wall, tile.Type != TileType.Wall);
                    expired.Add((x, y, old));
                }
            }
        }
        return expired;
    }

    /// <summary>
    /// Returns all tiles reachable from (startX, startY) by orthogonal flood-fill that share the Water effect.
    /// </summary>
    public List<(int X, int Y)> GetConnectedWaterTiles(int startX, int startY)
    {
        var result = new List<(int, int)>();
        if (GetTileEffect(startX, startY) != TileEffect.Water) return result;

        var visited = new HashSet<(int, int)>();
        var queue = new Queue<(int, int)>();
        queue.Enqueue((startX, startY));

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            if (!visited.Add((cx, cy))) continue;
            if (GetTileEffect(cx, cy) != TileEffect.Water) continue;
            result.Add((cx, cy));
            for (int i = 0; i < 4; i++)
                queue.Enqueue((cx + dx[i], cy + dy[i]));
        }
        return result;
    }

    // ── Sanity Mutations ─────────────────────────────────────────────────────

    /// <summary>
    /// Swaps a handful of wall↔floor tiles at random.
    /// Called when the player has been at Broken sanity for several turns.
    /// </summary>
    public string MutateWalls(Random rng)
    {
        int wallsToFloor = rng.Next(1, 7);
        int floorToWalls = rng.Next(1, 7);
        int changed = 0;

        var walls  = new List<(int x, int y)>();
        var floors = new List<(int x, int y)>();

        for (int x = 1; x < Width - 1; x++)
        for (int y = 1; y < Height - 1; y++)
        {
            var t = GetTile(x, y);
            if (t.Type == TileType.Wall)  walls.Add((x, y));
            else if (t.Type == TileType.Floor) floors.Add((x, y));
        }

        for (int i = 0; i < wallsToFloor && i < walls.Count; i++)
        {
            int idx = rng.Next(walls.Count - i) + i;
            (walls[i], walls[idx]) = (walls[idx], walls[i]);
            SetTile(walls[i].x, walls[i].y, TileType.Floor);
            changed++;
        }

        for (int i = 0; i < floorToWalls && i < floors.Count; i++)
        {
            int idx = rng.Next(floors.Count - i) + i;
            (floors[i], floors[idx]) = (floors[idx], floors[i]);
            var (fx, fy) = floors[i];
            if (Player != null && Player.X == fx && Player.Y == fy) continue;
            if (GetMonsterAt(fx, fy) != null) continue;
            if (GetTile(fx, fy).Type == TileType.StairsDown
                || GetTile(fx, fy).Type == TileType.StairsUp) continue;
            SetTile(fx, fy, TileType.Wall);
            changed++;
        }

        return changed > 0
            ? $"[MADNESS] The walls shift — {changed} tiles remade by your unravelling mind."
            : "[MADNESS] The walls pulse... but hold.";
    }

    /// <summary>
    /// Carves a new short passage between two random rooms.
    /// </summary>
    public string MutatePassages(Random rng)
    {
        if (Rooms.Count < 2) return "[MADNESS] Reality flickers.";

        int i = rng.Next(Rooms.Count);
        int j = rng.Next(Rooms.Count);
        while (j == i) j = rng.Next(Rooms.Count);

        var r1 = Rooms[i];
        var r2 = Rooms[j];

        int x1 = Math.Clamp(r1.X + r1.Width / 2, 1, Width - 2);
        int y1 = Math.Clamp(r1.Y + r1.Height / 2, 1, Height - 2);
        int x2 = Math.Clamp(r2.X + r2.Width / 2, 1, Width - 2);
        int y2 = Math.Clamp(r2.Y + r2.Height / 2, 1, Height - 2);

        int minX = Math.Min(x1, x2), maxX = Math.Max(x1, x2);
        int minY = Math.Min(y1, y2), maxY = Math.Max(y1, y2);

        for (int cx = minX; cx <= maxX; cx++)
            if (GetTile(cx, y1).Type == TileType.Wall) SetTile(cx, y1, TileType.Floor);
        for (int cy = minY; cy <= maxY; cy++)
            if (GetTile(x2, cy).Type == TileType.Wall) SetTile(x2, cy, TileType.Floor);

        return "[MADNESS] A passage opens where no passage was. The dungeon is changing.";
    }

    // ── FOV ─────────────────────────────────────────────────────────────────

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
