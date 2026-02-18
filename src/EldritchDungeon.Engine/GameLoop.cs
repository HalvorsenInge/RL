using EldritchDungeon.Core;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.UI;

namespace EldritchDungeon.Engine;

public class GameLoop
{
    private readonly GameEngine _engine;
    private readonly ASCIIRenderer _renderer;
    private readonly GameScreen _gameScreen;
    private readonly CharacterScreen _characterScreen;
    private readonly InventoryScreen _inventoryScreen;
    private readonly ReligionScreen _religionScreen;
    private readonly MessageScreen _messageScreen;
    private readonly HelpScreen _helpScreen;
    private readonly LookScreen _lookScreen;
    private readonly InputHandler _inputHandler;
    private readonly TurnManager _turnManager;
    private Screen? _overlayScreen;

    // Targeting mode state
    private bool _isTargeting;
    private int _targetX;
    private int _targetY;

    public GameLoop(GameEngine engine, ASCIIRenderer renderer)
    {
        _engine = engine;
        _renderer = renderer;
        _gameScreen = new GameScreen(renderer);
        _characterScreen = new CharacterScreen(renderer);
        _inventoryScreen = new InventoryScreen(renderer);
        _religionScreen = new ReligionScreen(renderer);
        _messageScreen = new MessageScreen(renderer);
        _helpScreen = new HelpScreen(renderer);
        _lookScreen = new LookScreen(renderer);
        _inputHandler = new InputHandler(engine);
        _turnManager = new TurnManager(engine);
    }

    public void Run()
    {
        int previousDungeonLevel = _engine.DungeonLevel;

        while (_engine.IsRunning)
        {
            if (_overlayScreen != null)
            {
                UpdateOverlayData();
                _overlayScreen.Render();

                var keyInfo = Console.ReadKey(true);
                var result = _overlayScreen.HandleInput(keyInfo);
                HandleScreenResult(result);
            }
            else if (_isTargeting)
            {
                _gameScreen.SetMap(_engine.Map!, _engine.DungeonLevel);
                _gameScreen.SetMessages(_engine.Log.Messages);
                _gameScreen.SetTargetCursor(_targetX, _targetY);
                _gameScreen.Render();

                var keyInfo = Console.ReadKey(true);
                HandleTargetingInput(keyInfo);
            }
            else
            {
                _gameScreen.SetTargetCursor(null, null);
                _gameScreen.SetMap(_engine.Map!, _engine.DungeonLevel);
                _gameScreen.SetMessages(_engine.Log.Messages);
                _gameScreen.Render();

                var keyInfo = Console.ReadKey(true);

                // Ranged combat keys
                if (keyInfo.KeyChar == 'f')
                {
                    TryStartTargeting();
                    continue;
                }
                if (keyInfo.KeyChar == 't')
                {
                    _engine.Log.Add("Throwing is not yet implemented.");
                    continue;
                }

                // Check for screen-opening keys first
                var screenResult = CheckScreenKeys(keyInfo);
                if (screenResult != ScreenResult.None)
                {
                    HandleScreenResult(screenResult);
                    continue;
                }

                bool turnTaken = _inputHandler.HandleInput(keyInfo);

                if (turnTaken && _engine.IsRunning)
                {
                    _turnManager.ProcessTurn();
                }

                // Auto-save on stair descent
                if (_engine.DungeonLevel != previousDungeonLevel)
                {
                    SaveManager.Save(_engine);
                    previousDungeonLevel = _engine.DungeonLevel;
                }
            }
        }

        // On exit: ironman save on quit, delete on death
        if (_engine.PlayerDied)
        {
            SaveManager.DeleteSave();
        }
        else
        {
            SaveManager.Save(_engine);
        }
    }

    private ScreenResult CheckScreenKeys(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.KeyChar switch
        {
            'c' => ScreenResult.OpenCharacter,
            'i' => ScreenResult.OpenInventory,
            'r' => ScreenResult.OpenReligion,
            'M' => ScreenResult.OpenMessages,
            '?' => ScreenResult.OpenHelp,
            'x' => ScreenResult.OpenLook,
            _ => ScreenResult.None
        };
    }

    private void HandleScreenResult(ScreenResult result)
    {
        switch (result)
        {
            case ScreenResult.Close:
                _overlayScreen = null;
                break;
            case ScreenResult.OpenCharacter:
                _overlayScreen = _characterScreen;
                break;
            case ScreenResult.OpenInventory:
                _overlayScreen = _inventoryScreen;
                break;
            case ScreenResult.OpenReligion:
                _overlayScreen = _religionScreen;
                break;
            case ScreenResult.OpenMessages:
                _overlayScreen = _messageScreen;
                break;
            case ScreenResult.OpenHelp:
                _overlayScreen = _helpScreen;
                break;
            case ScreenResult.OpenLook:
                _overlayScreen = _lookScreen;
                break;
        }
    }

    private void UpdateOverlayData()
    {
        if (_engine.Player == null || _engine.Map == null) return;

        if (_overlayScreen is CharacterScreen cs)
            cs.SetPlayer(_engine.Player, _engine.DungeonLevel);
        else if (_overlayScreen is InventoryScreen inv)
            inv.SetPlayer(_engine.Player);
        else if (_overlayScreen is ReligionScreen rs)
            rs.SetPlayer(_engine.Player);
        else if (_overlayScreen is MessageScreen ms)
            ms.SetMessages(_engine.Log.Messages);
        else if (_overlayScreen is LookScreen ls)
            ls.SetMap(_engine.Map, _engine.DungeonLevel);
    }

    private void TryStartTargeting()
    {
        var player = _engine.Player;
        if (player == null) return;

        var weapon = player.Equipment.GetEquipped(EquipmentSlot.MainHand) as Weapon;
        if (weapon == null || weapon.Range <= 1)
        {
            _engine.Log.Add("You have no ranged weapon equipped.");
            return;
        }

        _isTargeting = true;
        _targetX = player.X;
        _targetY = player.Y;

        string ammoInfo = weapon.MaxAmmo > 0 ? $" Ammo:{weapon.CurrentAmmo}/{weapon.MaxAmmo}" : "";
        _engine.Log.Add($"[{weapon.Name} Range:{weapon.Range}{ammoInfo}] Arrows:Aim  Enter:Fire  Esc:Cancel");
    }

    private void HandleTargetingInput(ConsoleKeyInfo keyInfo)
    {
        var player = _engine.Player!;
        var map    = _engine.Map!;

        if (keyInfo.Key == ConsoleKey.Escape)
        {
            _isTargeting = false;
            _gameScreen.SetTargetCursor(null, null);
            _engine.Log.Add("Targeting cancelled.");
            return;
        }

        if (keyInfo.Key == ConsoleKey.Enter)
        {
            // Capture the monster reference before shooting so we can call kill hooks
            var targetMonster = map.GetMonsterAt(_targetX, _targetY);

            bool turnTaken = _engine.CombatSystem.ShootRanged(player, _targetX, _targetY, map);

            _isTargeting = false;
            _gameScreen.SetTargetCursor(null, null);

            if (turnTaken)
            {
                if (targetMonster != null && targetMonster.Health.IsDead)
                {
                    _engine.ReligionSystem.OnKill(player, targetMonster);
                    _engine.LevelingSystem.CheckLevelUp(player);
                }

                _turnManager.ProcessTurn();
            }
            return;
        }

        // Move cursor with the same keys as movement
        var (dx, dy) = GetCursorDelta(keyInfo);
        if (dx != 0 || dy != 0)
        {
            _targetX = Math.Clamp(_targetX + dx, 0, map.Width  - 1);
            _targetY = Math.Clamp(_targetY + dy, 0, map.Height - 1);
        }
    }

    private static (int dx, int dy) GetCursorDelta(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.UpArrow    => (0, -1),
            ConsoleKey.DownArrow  => (0,  1),
            ConsoleKey.LeftArrow  => (-1, 0),
            ConsoleKey.RightArrow => (1,  0),
            ConsoleKey.NumPad8    => (0, -1),
            ConsoleKey.NumPad2    => (0,  1),
            ConsoleKey.NumPad4    => (-1, 0),
            ConsoleKey.NumPad6    => (1,  0),
            ConsoleKey.NumPad7    => (-1, -1),
            ConsoleKey.NumPad9    => (1, -1),
            ConsoleKey.NumPad1    => (-1,  1),
            ConsoleKey.NumPad3    => (1,   1),
            _ => GetViCursorDelta(keyInfo.KeyChar)
        };
    }

    private static (int dx, int dy) GetViCursorDelta(char c)
    {
        return c switch
        {
            'k' => (0, -1),
            'j' => (0,  1),
            'h' => (-1, 0),
            'l' => (1,  0),
            'y' => (-1, -1),
            'u' => (1, -1),
            'b' => (-1,  1),
            'n' => (1,   1),
            _ => (0, 0)
        };
    }
}
