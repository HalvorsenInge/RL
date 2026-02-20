using EldritchDungeon.Core;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.Utilities;
using EldritchDungeon.World;

namespace EldritchDungeon.UI;

public class GameScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private DungeonMap? _map;
    private int _dungeonLevel;
    private IReadOnlyList<string>? _messages;
    private int? _cursorX;
    private int? _cursorY;

    public GameScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public void SetMap(DungeonMap map, int dungeonLevel)
    {
        _map = map;
        _dungeonLevel = dungeonLevel;
    }

    public void SetMessages(IReadOnlyList<string> messages)
    {
        _messages = messages;
    }

    /// <summary>
    /// Sets the targeting cursor position. Pass null to clear.
    /// The cursor is rendered as '*' in yellow on the map.
    /// </summary>
    public void SetTargetCursor(int? x, int? y)
    {
        _cursorX = x;
        _cursorY = y;
    }

    public override void Render()
    {
        if (_map == null)
            return;

        _renderer.Clear();
        RenderMap();
        RenderHud();
        RenderMessages();
        _renderer.Flush();
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        // Input handling delegated to InputHandler in Engine
        return ScreenResult.None;
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
                    ConsoleColor color = GetTileColor(tile.Type);
                    _renderer.Set(x, y, glyph, color);
                }
                else if (tile.IsExplored)
                {
                    char glyph = Glyphs.ForTileType(tile.Type);
                    ConsoleColor color = tile.Type == TileType.Wall
                        ? ColorPalette.ExploredWall
                        : ColorPalette.ExploredFloor;
                    _renderer.Set(x, y, glyph, color);
                }
            }
        }

        // Render tile effects in FOV (over tiles, under entities)
        for (int ex = 0; ex < _map.Width && ex < GameConstants.ScreenWidth; ex++)
        {
            for (int ey = 0; ey < _map.Height && ey < GameConstants.MapHeight; ey++)
            {
                var effectTile = _map.GetTile(ex, ey);
                if (!effectTile.IsInFov) continue;
                if (effectTile.Effect == TileEffect.None) continue;

                var (glyph, color) = effectTile.Effect switch
                {
                    TileEffect.Water => ('~', ConsoleColor.Blue),
                    TileEffect.Fire  => ('^', ConsoleColor.Red),
                    TileEffect.Steam => ('*', ConsoleColor.White),
                    TileEffect.Oil   => ('%', ConsoleColor.DarkYellow),
                    _                => (' ', ConsoleColor.Black)
                };
                _renderer.Set(ex, ey, glyph, color);
            }
        }

        // Render items in FOV
        foreach (var (item, ix, iy) in _map.Items)
        {
            if (ix < _map.Width && iy < GameConstants.MapHeight)
            {
                var itemTile = _map.GetTile(ix, iy);
                if (itemTile.IsInFov)
                {
                    var color = item switch
                    {
                        Weapon => ConsoleColor.Cyan,
                        Armor => ConsoleColor.DarkCyan,
                        Consumable => ConsoleColor.Green,
                        _ => ConsoleColor.White
                    };
                    _renderer.Set(ix, iy, item.Glyph, color);
                }
            }
        }

        // Render shop merchants in FOV
        foreach (var (sx, sy, _) in _map.Shops)
        {
            if (sx < _map.Width && sy < GameConstants.MapHeight)
            {
                var shopTile = _map.GetTile(sx, sy);
                if (shopTile.IsInFov)
                    _renderer.Set(sx, sy, '$', ConsoleColor.Yellow);
                else if (shopTile.IsExplored)
                    _renderer.Set(sx, sy, '$', ConsoleColor.DarkYellow);
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
            _renderer.Set(_map.Player.X, _map.Player.Y,
                _map.Player.Glyph, ColorPalette.Player);
        }

        // Render targeting cursor (drawn last so it's always visible)
        if (_cursorX.HasValue && _cursorY.HasValue
            && _cursorX.Value >= 0 && _cursorX.Value < GameConstants.ScreenWidth
            && _cursorY.Value >= 0 && _cursorY.Value < GameConstants.MapHeight)
        {
            _renderer.Set(_cursorX.Value, _cursorY.Value, '*', ConsoleColor.Yellow);
        }
    }

    private void RenderHud()
    {
        if (_map?.Player == null)
            return;

        var player = _map.Player;
        int hudRow = GameConstants.MapHeight;

        // Row 1: Resources
        _renderer.WriteString(0, hudRow,
            $"HP:{player.Health.CurrentHp}/{player.Health.MaxHp}",
            ConsoleColor.Red);

        _renderer.WriteString(16, hudRow,
            $"MP:{player.Mana.CurrentMana}/{player.Mana.MaxMana}",
            ConsoleColor.Blue);

        var sanityColor = player.Sanity.State switch
        {
            SanityState.Stable => ConsoleColor.Magenta,
            SanityState.Fractured => ConsoleColor.DarkMagenta,
            SanityState.Unraveling => ConsoleColor.DarkRed,
            _ => ConsoleColor.Red
        };
        _renderer.WriteString(30, hudRow,
            $"San:{player.Sanity.CurrentSanity}/{player.Sanity.MaxSanity}",
            sanityColor);

        _renderer.WriteString(48, hudRow,
            $"Lv:{player.Stats.Level} XP:{player.Stats.Experience}/{player.Stats.ExperienceToNextLevel}",
            ConsoleColor.Cyan);

        _renderer.WriteString(74, hudRow,
            $"Dl:{_dungeonLevel}",
            ConsoleColor.White);
    }

    private void RenderMessages()
    {
        if (_messages == null || _messages.Count == 0)
            return;

        int startRow = GameConstants.MapHeight + 1;
        int maxMessages = Math.Min(3, _messages.Count);

        for (int i = 0; i < maxMessages; i++)
        {
            int msgIndex = _messages.Count - maxMessages + i;
            if (msgIndex >= 0 && msgIndex < _messages.Count)
            {
                string msg = _messages[msgIndex];
                if (msg.Length > GameConstants.ScreenWidth)
                    msg = msg[..GameConstants.ScreenWidth];
                _renderer.WriteString(0, startRow + i, msg, ConsoleColor.Gray);
            }
        }
    }

    private static ConsoleColor GetTileColor(TileType type)
    {
        return type switch
        {
            TileType.Wall => ColorPalette.Wall,
            TileType.Floor => ColorPalette.Floor,
            TileType.Door => ColorPalette.Door,
            TileType.StairsDown or TileType.StairsUp => ColorPalette.Stairs,
            _ => ConsoleColor.White
        };
    }
}
