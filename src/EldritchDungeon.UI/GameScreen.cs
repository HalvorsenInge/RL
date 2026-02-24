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

    private (int camX, int camY) ComputeCamera()
    {
        int px = _map!.Player?.X ?? 0;
        int py = _map.Player?.Y ?? 0;

        int camX = px - GameConstants.ViewportWidth  / 2;
        int camY = py - GameConstants.ViewportHeight / 2;

        camX = Math.Clamp(camX, 0, Math.Max(0, _map.Width  - GameConstants.ViewportWidth));
        camY = Math.Clamp(camY, 0, Math.Max(0, _map.Height - GameConstants.ViewportHeight));

        return (camX, camY);
    }

    private void RenderMap()
    {
        var (camX, camY) = ComputeCamera();

        // Tiles
        for (int sy = 0; sy < GameConstants.ViewportHeight; sy++)
        {
            int my = sy + camY;
            if (my >= _map!.Height) break;

            for (int sx = 0; sx < GameConstants.ViewportWidth; sx++)
            {
                int mx = sx + camX;
                if (mx >= _map.Width) break;

                var tile = _map.GetTile(mx, my);

                if (tile.IsInFov)
                {
                    char glyph = Glyphs.ForTileType(tile.Type);
                    ConsoleColor color = GetTileColor(tile.Type);
                    _renderer.Set(sx, sy, glyph, color);
                }
                else if (tile.IsExplored)
                {
                    char glyph = Glyphs.ForTileType(tile.Type);
                    ConsoleColor color = tile.Type == TileType.Wall
                        ? ColorPalette.ExploredWall
                        : ColorPalette.ExploredFloor;
                    _renderer.Set(sx, sy, glyph, color);
                }

                // Tile effects (over tiles, under entities)
                if (tile.IsInFov && tile.Effect != TileEffect.None)
                {
                    var (eg, ec) = tile.Effect switch
                    {
                        TileEffect.Water => ('~', ConsoleColor.Blue),
                        TileEffect.Fire  => ('^', ConsoleColor.Red),
                        TileEffect.Steam => ('*', ConsoleColor.White),
                        TileEffect.Oil   => ('%', ConsoleColor.DarkYellow),
                        _                => (' ', ConsoleColor.Black)
                    };
                    _renderer.Set(sx, sy, eg, ec);
                }
            }
        }

        // Items in FOV
        foreach (var (item, mx, my) in _map!.Items)
        {
            int sx = mx - camX;
            int sy = my - camY;
            if (sx < 0 || sx >= GameConstants.ViewportWidth) continue;
            if (sy < 0 || sy >= GameConstants.ViewportHeight) continue;

            var itemTile = _map.GetTile(mx, my);
            if (!itemTile.IsInFov) continue;

            var color = item switch
            {
                Weapon    => ConsoleColor.Cyan,
                Armor     => ConsoleColor.DarkCyan,
                Consumable => ConsoleColor.Green,
                ToolItem   => ConsoleColor.Yellow,
                _          => ConsoleColor.White
            };
            _renderer.Set(sx, sy, item.Glyph, color);
        }

        // Shop merchants
        foreach (var (mx, my, _) in _map.Shops)
        {
            int sx = mx - camX;
            int sy = my - camY;
            if (sx < 0 || sx >= GameConstants.ViewportWidth) continue;
            if (sy < 0 || sy >= GameConstants.ViewportHeight) continue;

            var shopTile = _map.GetTile(mx, my);
            if (shopTile.IsInFov)
                _renderer.Set(sx, sy, '$', ConsoleColor.Yellow);
            else if (shopTile.IsExplored)
                _renderer.Set(sx, sy, '$', ConsoleColor.DarkYellow);
        }

        // Monsters in FOV
        foreach (var monster in _map.Monsters)
        {
            int sx = monster.X - camX;
            int sy = monster.Y - camY;
            if (sx < 0 || sx >= GameConstants.ViewportWidth) continue;
            if (sy < 0 || sy >= GameConstants.ViewportHeight) continue;

            var tile = _map.GetTile(monster.X, monster.Y);
            if (tile.IsInFov)
                _renderer.Set(sx, sy, monster.Glyph, ColorPalette.MonsterByTier(monster.Tier));
        }

        // Player
        if (_map.Player != null)
        {
            int sx = _map.Player.X - camX;
            int sy = _map.Player.Y - camY;
            _renderer.Set(sx, sy, _map.Player.Glyph, ColorPalette.Player);
        }

        // Targeting cursor (map coords → screen coords)
        if (_cursorX.HasValue && _cursorY.HasValue)
        {
            int csx = _cursorX.Value - camX;
            int csy = _cursorY.Value - camY;
            if (csx >= 0 && csx < GameConstants.ViewportWidth
                && csy >= 0 && csy < GameConstants.ViewportHeight)
            {
                _renderer.Set(csx, csy, '*', ConsoleColor.Yellow);
            }
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
