using EldritchDungeon.Core;
using EldritchDungeon.Utilities;
using EldritchDungeon.World;

namespace EldritchDungeon.UI;

public class GameScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private DungeonMap? _map;
    private int _dungeonLevel;

    public GameScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public void SetMap(DungeonMap map, int dungeonLevel)
    {
        _map = map;
        _dungeonLevel = dungeonLevel;
    }

    public override void Render()
    {
        if (_map == null)
            return;

        _renderer.Clear();
        RenderMap();
        RenderHud();
        _renderer.Flush();
    }

    public override void HandleInput(ConsoleKeyInfo keyInfo)
    {
        // Input handling deferred to Phase 4+
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
    }

    private void RenderHud()
    {
        if (_map?.Player == null)
            return;

        var player = _map.Player;
        int hudRow = GameConstants.MapHeight;

        _renderer.WriteString(0, hudRow,
            $"HP: {player.Health.CurrentHp}/{player.Health.MaxHp}",
            ConsoleColor.Red);

        _renderer.WriteString(20, hudRow,
            $"Mana: {player.Mana.CurrentMana}/{player.Mana.MaxMana}",
            ConsoleColor.Blue);

        _renderer.WriteString(42, hudRow,
            $"Sanity: {player.Sanity.CurrentSanity}/{player.Sanity.MaxSanity}",
            ConsoleColor.Magenta);

        _renderer.WriteString(66, hudRow,
            $"Dlvl: {_dungeonLevel}",
            ConsoleColor.White);
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
