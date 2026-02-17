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
            else
            {
                _gameScreen.SetMap(_engine.Map!, _engine.DungeonLevel);
                _gameScreen.SetMessages(_engine.Log.Messages);
                _gameScreen.Render();

                var keyInfo = Console.ReadKey(true);

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
}
