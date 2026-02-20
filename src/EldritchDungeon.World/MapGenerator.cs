using EldritchDungeon.Core;

namespace EldritchDungeon.World;

public class MapGenerator
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _maxRooms;
    private readonly int _minRoomSize;
    private readonly int _maxRoomSize;
    private readonly Random _random;

    public MapGenerator(
        int width = GameConstants.MapWidth,
        int height = GameConstants.MapHeight,
        int maxRooms = GameConstants.MaxRooms,
        int minRoomSize = GameConstants.MinRoomSize,
        int maxRoomSize = GameConstants.MaxRoomSize,
        Random? random = null)
    {
        _width = width;
        _height = height;
        _maxRooms = maxRooms;
        _minRoomSize = minRoomSize;
        _maxRoomSize = maxRoomSize;
        _random = random ?? new Random();
    }

    public DungeonMap Generate(int dungeonLevel = 1)
    {
        var map = new DungeonMap();
        map.InitializeTiles(_width, _height);

        for (int i = 0; i < _maxRooms; i++)
        {
            int roomWidth = _random.Next(_minRoomSize, _maxRoomSize + 1);
            int roomHeight = _random.Next(_minRoomSize, _maxRoomSize + 1);
            int x = _random.Next(1, _width - roomWidth - 1);
            int y = _random.Next(1, _height - roomHeight - 1);

            var newRoom = new Room { X = x, Y = y, Width = roomWidth, Height = roomHeight };

            bool overlaps = false;
            foreach (var room in map.Rooms)
            {
                if (newRoom.Intersects(room))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps)
                continue;

            CarveRoom(map, newRoom);

            if (map.Rooms.Count > 0)
            {
                var prevRoom = map.Rooms[^1];
                CreateCorridor(map, prevRoom.CenterX, prevRoom.CenterY, newRoom.CenterX, newRoom.CenterY);
            }

            map.Rooms.Add(newRoom);
        }

        if (map.Rooms.Count > 0)
        {
            // Stairs up in first room
            var firstRoom = map.Rooms[0];
            map.SetTile(firstRoom.CenterX, firstRoom.CenterY, TileType.StairsUp);
            map.StairsList.Add(new Stairs { X = firstRoom.CenterX, Y = firstRoom.CenterY, IsDown = false });

            // Stairs down in last room
            var lastRoom = map.Rooms[^1];
            map.SetTile(lastRoom.CenterX, lastRoom.CenterY, TileType.StairsDown);
            map.StairsList.Add(new Stairs { X = lastRoom.CenterX, Y = lastRoom.CenterY, IsDown = true });

            // Designate one room in the middle as a shop (not the first or last room)
            if (map.Rooms.Count >= 3)
            {
                int shopIndex = map.Rooms.Count / 2;
                map.Rooms[shopIndex].IsShop = true;
            }
        }

        return map;
    }

    private void CarveRoom(DungeonMap map, Room room)
    {
        for (int x = room.X; x < room.X + room.Width; x++)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                map.SetTile(x, y, TileType.Floor);
            }
        }
    }

    private void CreateCorridor(DungeonMap map, int x1, int y1, int x2, int y2)
    {
        // L-shaped corridor: horizontal first, then vertical
        if (_random.Next(2) == 0)
        {
            CarveHorizontalTunnel(map, x1, x2, y1);
            CarveVerticalTunnel(map, y1, y2, x2);
        }
        else
        {
            CarveVerticalTunnel(map, y1, y2, x1);
            CarveHorizontalTunnel(map, x1, x2, y2);
        }
    }

    private void CarveHorizontalTunnel(DungeonMap map, int x1, int x2, int y)
    {
        int minX = Math.Min(x1, x2);
        int maxX = Math.Max(x1, x2);
        for (int x = minX; x <= maxX; x++)
        {
            map.SetTile(x, y, TileType.Floor);
        }
    }

    private void CarveVerticalTunnel(DungeonMap map, int y1, int y2, int x)
    {
        int minY = Math.Min(y1, y2);
        int maxY = Math.Max(y1, y2);
        for (int y = minY; y <= maxY; y++)
        {
            map.SetTile(x, y, TileType.Floor);
        }
    }
}
