using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.Utilities;
using EldritchDungeon.World;

namespace EldritchDungeon.UI;

public class LookScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private DungeonMap? _map;
    private int _dungeonLevel;
    private int _cursorX;
    private int _cursorY;

    public LookScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public void SetMap(DungeonMap map, int dungeonLevel)
    {
        _map = map;
        _dungeonLevel = dungeonLevel;
        if (map.Player != null)
        {
            _cursorX = map.Player.X;
            _cursorY = map.Player.Y;
        }
    }

    public override void Render()
    {
        if (_map == null) return;

        _renderer.Clear();

        // Render the map (same as GameScreen)
        RenderMap();

        // Render cursor as blinking X
        _renderer.Set(_cursorX, _cursorY, 'X', ConsoleColor.White);

        // Info panel at bottom
        RenderInfo();

        _renderer.Flush();
    }

    private void RenderMap()
    {
        for (int x = 0; x < _map!.Width && x < GameConstants.ScreenWidth; x++)
        {
            for (int y = 0; y < _map.Height && y < GameConstants.MapHeight; y++)
            {
                var tile = _map.GetTile(x, y);
                if (tile.IsInFov)
                {
                    char glyph = Glyphs.ForTileType(tile.Type);
                    ConsoleColor color = tile.Type switch
                    {
                        TileType.Wall => ColorPalette.Wall,
                        TileType.Floor => ColorPalette.Floor,
                        TileType.Door => ColorPalette.Door,
                        TileType.StairsDown or TileType.StairsUp => ColorPalette.Stairs,
                        _ => ConsoleColor.White
                    };
                    _renderer.Set(x, y, glyph, color);
                }
                else if (tile.IsExplored)
                {
                    char glyph = Glyphs.ForTileType(tile.Type);
                    ConsoleColor color = tile.Type == TileType.Wall
                        ? ColorPalette.ExploredWall : ColorPalette.ExploredFloor;
                    _renderer.Set(x, y, glyph, color);
                }
            }
        }

        // Render monsters in FOV
        foreach (var monster in _map.Monsters)
        {
            var tile = _map.GetTile(monster.X, monster.Y);
            if (tile.IsInFov)
            {
                _renderer.Set(monster.X, monster.Y, monster.Glyph,
                    ColorPalette.MonsterByTier(monster.Tier));
            }
        }

        // Render player
        if (_map.Player != null)
        {
            _renderer.Set(_map.Player.X, _map.Player.Y, _map.Player.Glyph, ColorPalette.Player);
        }
    }

    private void RenderInfo()
    {
        int infoRow = GameConstants.MapHeight;

        // Describe what's at the cursor
        var tile = _map!.GetTile(_cursorX, _cursorY);

        if (!tile.IsInFov && !tile.IsExplored)
        {
            _renderer.WriteString(1, infoRow, "You can't see that area.", ConsoleColor.DarkGray);
        }
        else
        {
            // Check for player
            if (_map.Player != null && _map.Player.X == _cursorX && _map.Player.Y == _cursorY)
            {
                _renderer.WriteString(1, infoRow, $"You ({_map.Player.Name}, {_map.Player.Race} {_map.Player.Class})", ConsoleColor.Yellow);
                infoRow++;
                _renderer.WriteString(1, infoRow,
                    $"HP: {_map.Player.Health.CurrentHp}/{_map.Player.Health.MaxHp}  " +
                    $"Level: {_map.Player.Stats.Level}", ConsoleColor.White);
            }
            // Check for monster
            else if (tile.IsInFov)
            {
                var monster = _map.GetMonsterAt(_cursorX, _cursorY);
                if (monster != null)
                {
                    _renderer.WriteString(1, infoRow,
                        $"{monster.Name} (Tier {monster.Tier})", ColorPalette.MonsterByTier(monster.Tier));
                    infoRow++;

                    string hpBar = $"HP: {monster.Health.CurrentHp}/{monster.Health.MaxHp}";
                    string threat = monster.SanityDamage > 0 ? $"  Sanity Damage: {monster.SanityDamage}" : "";
                    _renderer.WriteString(1, infoRow, hpBar + threat, ConsoleColor.White);
                    infoRow++;

                    if (monster.StatusEffects.ActiveEffects.Count > 0)
                    {
                        string effects = string.Join(", ", monster.StatusEffects.ActiveEffects.Select(e => e.Type.ToString()));
                        _renderer.WriteString(1, infoRow, $"Status: {effects}", ConsoleColor.DarkYellow);
                    }
                }
                else
                {
                    _renderer.WriteString(1, infoRow, DescribeTile(tile), ConsoleColor.Gray);
                }
            }
            else
            {
                _renderer.WriteString(1, infoRow, $"{DescribeTile(tile)} (remembered)", ConsoleColor.DarkGray);
            }
        }

        // Coordinates
        _renderer.WriteString(60, GameConstants.MapHeight,
            $"({_cursorX},{_cursorY})", ConsoleColor.DarkGray);

        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[Arrow keys] Move cursor  [Esc] Back", ConsoleColor.DarkGray);
    }

    private static string DescribeTile(Tile tile)
    {
        return tile.Type switch
        {
            TileType.Wall => "A solid wall.",
            TileType.Floor => "Stone floor.",
            TileType.Door => "A door.",
            TileType.StairsDown => "Stairs leading down.",
            TileType.StairsUp => "Stairs leading up.",
            _ => "Unknown."
        };
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Escape)
            return ScreenResult.Close;

        var (dx, dy) = keyInfo.Key switch
        {
            ConsoleKey.UpArrow => (0, -1),
            ConsoleKey.DownArrow => (0, 1),
            ConsoleKey.LeftArrow => (-1, 0),
            ConsoleKey.RightArrow => (1, 0),
            _ => (0, 0)
        };

        int newX = _cursorX + dx;
        int newY = _cursorY + dy;

        if (newX >= 0 && newX < GameConstants.ScreenWidth && newY >= 0 && newY < GameConstants.MapHeight)
        {
            _cursorX = newX;
            _cursorY = newY;
        }

        return ScreenResult.None;
    }
}
