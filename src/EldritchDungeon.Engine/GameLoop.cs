using EldritchDungeon.Core;
using EldritchDungeon.Data.Spells;
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
    private readonly SpellScreen _spellScreen;
    private readonly ShopScreen _shopScreen;
    private readonly InputHandler _inputHandler;
    private readonly TurnManager _turnManager;
    private Screen? _overlayScreen;

    // Ranged weapon targeting
    private bool _isTargeting;
    private int _targetX;
    private int _targetY;

    // Spell targeting
    private bool _isSpellTargeting;
    private SpellId? _pendingSpell;

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
        _spellScreen = new SpellScreen(renderer);
        _shopScreen  = new ShopScreen(renderer);
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

                // Special case: SpellScreen closed — check for a selected spell
                if (_overlayScreen is SpellScreen ss && result == ScreenResult.Close)
                {
                    _overlayScreen = null;
                    if (ss.SelectedSpell.HasValue)
                        HandleSpellSelected(ss.SelectedSpell.Value);
                    continue;
                }

                HandleScreenResult(result);
            }
            else if (_isSpellTargeting)
            {
                _gameScreen.SetMap(_engine.Map!, _engine.DungeonLevel);
                _gameScreen.SetMessages(_engine.Log.Messages);
                _gameScreen.SetTargetCursor(_targetX, _targetY);
                _gameScreen.Render();

                var keyInfo = Console.ReadKey(true);
                HandleSpellTargetingInput(keyInfo);
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

                // Ranged combat
                if (keyInfo.KeyChar == 'f')
                {
                    TryStartTargeting();
                    continue;
                }

                // Spellbook
                if (keyInfo.KeyChar == 'm')
                {
                    TryOpenSpellbook();
                    continue;
                }

                if (keyInfo.KeyChar == 't')
                {
                    _engine.Log.Add("Throwing is not yet implemented.");
                    continue;
                }

                // Screen-opening keys
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
            SaveManager.DeleteSave();
        else
            SaveManager.Save(_engine);
    }

    // ── Spell flow ──────────────────────────────────────────────────────────

    private void TryOpenSpellbook()
    {
        var player = _engine.Player;
        if (player == null) return;

        if (player.KnownSpells.Count == 0)
        {
            _engine.Log.Add("You know no spells.");
            return;
        }

        _spellScreen.SetPlayer(player);
        _overlayScreen = _spellScreen;
    }

    private void TryOpenShop()
    {
        var player = _engine.Player;
        var map    = _engine.Map;
        if (player == null || map == null) return;

        // Find a shop within 1 tile of the player (Chebyshev distance)
        int shopIdx = map.Shops.FindIndex(
            shop => Math.Max(Math.Abs(shop.X - player.X), Math.Abs(shop.Y - player.Y)) <= 1);

        if (shopIdx < 0)
        {
            _engine.Log.Add("There is no merchant nearby. (approach the $ symbol)");
            return;
        }

        _shopScreen.Set(player, map.Shops[shopIdx].Inventory);
        _overlayScreen = _shopScreen;
    }

    private void HandleSpellSelected(SpellId spellId)
    {
        var player = _engine.Player!;
        var map    = _engine.Map!;
        var spell  = SpellDatabase.Get(spellId);

        if (spell.Target == SpellTarget.Self || spell.Target == SpellTarget.LevelWide)
        {
            bool turnTaken = _engine.MagicSystem.CastSpell(player, spellId, player.X, player.Y, map);
            if (turnTaken)
            {
                _turnManager.ProcessTurn();
                // Open message log automatically after Eye in the Sky
                if (spellId == SpellId.EyeInTheSky)
                {
                    _messageScreen.SetMessages(_engine.Log.Messages);
                    _overlayScreen = _messageScreen;
                }
            }
        }
        else
        {
            // Enter spell targeting mode
            _isSpellTargeting = true;
            _pendingSpell = spellId;
            _targetX = player.X;
            _targetY = player.Y;
            string hint = spell.Target == SpellTarget.SingleTarget ? "Target an enemy" : "Choose location";
            _engine.Log.Add($"[{spell.Name}] {hint} — Arrows:Aim  Enter:Cast  Esc:Cancel");
        }
    }

    private void HandleSpellTargetingInput(ConsoleKeyInfo keyInfo)
    {
        var player = _engine.Player!;
        var map    = _engine.Map!;

        if (keyInfo.Key == ConsoleKey.Escape)
        {
            _isSpellTargeting = false;
            _pendingSpell = null;
            _gameScreen.SetTargetCursor(null, null);
            _engine.Log.Add("Spell cancelled.");
            return;
        }

        if (keyInfo.Key == ConsoleKey.Enter && _pendingSpell.HasValue)
        {
            var spell = SpellDatabase.Get(_pendingSpell.Value);

            // Validate targeting for single-target spells
            if (spell.Target == SpellTarget.SingleTarget)
            {
                var targetMonster = map.GetMonsterAt(_targetX, _targetY);
                if (targetMonster == null)
                {
                    _engine.Log.Add("No enemy there — pick a target.");
                    return;
                }
            }

            // Range check
            int dist = Math.Max(Math.Abs(_targetX - player.X), Math.Abs(_targetY - player.Y));
            if (spell.Range > 0 && dist > spell.Range)
            {
                _engine.Log.Add($"Out of range! ({dist} tiles, max {spell.Range})");
                return;
            }

            var spellToCast = _pendingSpell.Value;
            _isSpellTargeting = false;
            _pendingSpell = null;
            _gameScreen.SetTargetCursor(null, null);

            bool turnTaken = _engine.MagicSystem.CastSpell(player, spellToCast, _targetX, _targetY, map);
            if (turnTaken)
                _turnManager.ProcessTurn();
            return;
        }

        // Move targeting cursor
        var (dx, dy) = GetCursorDelta(keyInfo);
        if (dx != 0 || dy != 0)
        {
            _targetX = Math.Clamp(_targetX + dx, 0, map.Width  - 1);
            _targetY = Math.Clamp(_targetY + dy, 0, map.Height - 1);
        }
    }

    // ── Ranged weapon targeting (unchanged logic) ────────────────────────────

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
            's' => ScreenResult.OpenShop,
            _   => ScreenResult.None
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
            case ScreenResult.OpenSpell:
                TryOpenSpellbook();
                break;
            case ScreenResult.OpenShop:
                TryOpenShop();
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
        else if (_overlayScreen is SpellScreen ss)
            ss.SetPlayer(_engine.Player);
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
