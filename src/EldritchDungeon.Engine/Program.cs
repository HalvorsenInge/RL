using EldritchDungeon.Engine;
using EldritchDungeon.UI;

// Set up console
Console.Title = "Eldritch Dungeon";
Console.CursorVisible = false;

var engine = new GameEngine();
var renderer = new ASCIIRenderer();

// Check for existing save
if (SaveManager.SaveExists())
{
    Console.Clear();
    Console.WriteLine("Eldritch Dungeon");
    Console.WriteLine();
    Console.WriteLine("[C] Continue saved game");
    Console.WriteLine("[N] New game (deletes save!)");
    Console.WriteLine();

    while (true)
    {
        var key = Console.ReadKey(true);
        if (key.KeyChar is 'c' or 'C')
        {
            var saveData = SaveManager.Load();
            if (saveData != null)
            {
                engine.LoadFromSave(saveData);
                break;
            }
            // Fall through to new game if load fails
            Console.WriteLine("Save file corrupted. Starting new game...");
            SaveManager.DeleteSave();
            Thread.Sleep(1500);
            if (!StartNewGame(engine, renderer)) return;
            break;
        }
        else if (key.KeyChar is 'n' or 'N')
        {
            SaveManager.DeleteSave();
            if (!StartNewGame(engine, renderer)) return;
            break;
        }
    }
}
else
{
    Console.Clear();
    if (!StartNewGame(engine, renderer)) return;
}

// Run game
var loop = new GameLoop(engine, renderer);
loop.Run();

Console.CursorVisible = true;
Console.Clear();
Console.WriteLine("Thanks for playing Eldritch Dungeon!");

static bool StartNewGame(GameEngine engine, ASCIIRenderer renderer)
{
    var creation = new CharacterCreationScreen(renderer);
    var player = creation.Run();

    if (player == null)
        return false;

    engine.Initialize(player);
    return true;
}
