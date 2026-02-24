using System.Diagnostics.CodeAnalysis;
using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Systems;
using EldritchDungeon.World;

namespace EldritchDungeon.Engine;

public class InputHandler
{
    private readonly GameEngine _engine;

    public InputHandler(GameEngine engine)
    {
        _engine = engine;
    }

    public bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        var player = _engine.Player;
        var map = _engine.Map;

        if (player == null || map == null)
            return false;

        if (StatusEffectSystem.IsIncapacitated(player))
        {
            _engine.Log.Add("You are unable to act!");
            return true;
        }

        if (_engine.SanitySystem.ShouldRandomAction(player))
        {
            _engine.Log.Add("Madness overtakes you! You stumble randomly.");
            int dx = Dice.Roll(1, 3) - 2;
            int dy = Dice.Roll(1, 3) - 2;
            TryMoveOrAttack(player, map, dx, dy);
            return true;
        }

        var (dx2, dy2) = GetMovementDelta(keyInfo);
        if (dx2 != 0 || dy2 != 0)
        {
            TryMoveOrAttack(player, map, dx2, dy2);
            return true;
        }

        switch (keyInfo.KeyChar)
        {
            case '>':
                return TryDescend(player, map);
            case '<':
                return TryAscend(player, map);
            case 'g':
                return TryPickup(player, map);
            case 'p':
                return true; // Wait (prayer removed; gods react automatically)
            case 'S':
                SaveManager.Save(_engine);
                _engine.Log.Add("Game saved.");
                return false;
            case '.':
            case ' ':
                return true; // Wait
        }

        if (keyInfo.Key == ConsoleKey.Escape)
        {
            _engine.RequestQuit();
            return false;
        }

        if (keyInfo.Key == ConsoleKey.Spacebar)
            return true; // Wait

        return false;
    }

    private void TryMoveOrAttack(Player player, DungeonMap map, int dx, int dy)
    {
        int newX = player.X + dx;
        int newY = player.Y + dy;

        var monster = map.GetMonsterAt(newX, newY);
        if (monster != null)
        {
            _engine.CombatSystem.PlayerAttack(player, monster, map);

            if (monster.Health.IsDead)
            {
                _engine.ReligionSystem.OnKill(player, monster);
                _engine.LevelingSystem.CheckLevelUp(player);
            }
            return;
        }

        if (map.TryMoveActor(player, newX, newY))
        {
            // Check for items at new position
            var itemsHere = map.GetItemsAt(player.X, player.Y);
            if (itemsHere.Count == 1)
                _engine.Log.Add($"You see {itemsHere[0].Name} here.");
            else if (itemsHere.Count > 1)
                _engine.Log.Add($"You see {itemsHere.Count} items here.");
        }
    }

    private bool TryPickup(Player player, DungeonMap map)
    {
        var items = map.GetItemsAt(player.X, player.Y);
        if (items.Count == 0)
        {
            _engine.Log.Add("There is nothing here to pick up.");
            return false;
        }

        var item = items[0];
        if (!player.Inventory.AddItem(item))
        {
            _engine.Log.Add("Your inventory is full!");
            return false;
        }

        map.RemoveItem(item, player.X, player.Y);
        _engine.Log.Add($"You pick up the {item.Name}.");
        return true;
    }

    private bool TryDescend(Player player, DungeonMap map)
    {
        var stairs = map.StairsList.FirstOrDefault(s => s.IsDown && s.X == player.X && s.Y == player.Y);
        if (stairs != null)
        {
            _engine.DescendStairs();
            return true;
        }
        _engine.Log.Add("There are no stairs down here.");
        return false;
    }

    private bool TryAscend(Player player, DungeonMap map)
    {
        Stairs stairs = map.StairsList.FirstOrDefault(s => !s.IsDown && s.X == player.X && s.Y == player.Y);
        if (stairs != null)
        {
            _engine.Log.Add("You cannot ascend further. Press on!");
            return false;
        }
        _engine.Log.Add("There are no stairs up here.");
        return false;
    }

    private static (int dx, int dy) GetMovementDelta(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            // Arrow keys
            ConsoleKey.UpArrow => (0, -1),
            ConsoleKey.DownArrow => (0, 1),
            ConsoleKey.LeftArrow => (-1, 0),
            ConsoleKey.RightArrow => (1, 0),
            // Numpad
            ConsoleKey.NumPad8 => (0, -1),
            ConsoleKey.NumPad2 => (0, 1),
            ConsoleKey.NumPad4 => (-1, 0),
            ConsoleKey.NumPad6 => (1, 0),
            ConsoleKey.NumPad7 => (-1, -1),
            ConsoleKey.NumPad9 => (1, -1),
            ConsoleKey.NumPad1 => (-1, 1),
            ConsoleKey.NumPad3 => (1, 1),
            _ => GetViKeyDelta(keyInfo.KeyChar)
        };
    }

    private static (int dx, int dy) GetViKeyDelta(char c)
    {
        return c switch
        {
            'k' => (0, -1),
            'j' => (0, 1),
            'h' => (-1, 0),
            'l' => (1, 0),
            'y' => (-1, -1),
            'u' => (1, -1),
            'b' => (-1, 1),
            'n' => (1, 1),
            _ => (0, 0)
        };
    }
}
