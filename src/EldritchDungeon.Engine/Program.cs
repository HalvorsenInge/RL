using EldritchDungeon.Core;
using EldritchDungeon.Data.Classes;
using EldritchDungeon.Data.Items;
using EldritchDungeon.Data.Races;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.Engine;
using EldritchDungeon.UI;

// Set up console
Console.Title = "Eldritch Dungeon";
Console.CursorVisible = false;

var engine = new GameEngine();

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
            StartNewGame(engine);
            break;
        }
        else if (key.KeyChar is 'n' or 'N')
        {
            SaveManager.DeleteSave();
            StartNewGame(engine);
            break;
        }
    }
}
else
{
    StartNewGame(engine);
}

// Set up rendering
var renderer = new ASCIIRenderer();

// Run game
var loop = new GameLoop(engine, renderer);
loop.Run();

Console.CursorVisible = true;
Console.Clear();
Console.WriteLine("Thanks for playing Eldritch Dungeon!");

static void StartNewGame(GameEngine engine)
{
    // Create test character: Human Warrior
    var raceDef = RaceDatabase.Get(RaceType.Human);
    var classDef = ClassDatabase.Get(ClassType.Warrior);

    var player = Player.CreateCharacter(
        "Adventurer",
        raceDef.Type,
        classDef.Type,
        raceDef.GetStatModifiers(),
        raceDef.HpMultiplier,
        raceDef.ManaMultiplier,
        raceDef.SanityMultiplier,
        classDef.BaseHp,
        classDef.BaseMana,
        classDef.StartingGold);

    // Equip starting gear
    var longsword = WeaponDatabase.Get("Longsword");
    if (longsword != null)
        player.Equipment.Equip(EquipmentSlot.MainHand, longsword);

    var chainmail = ArmorDatabase.Get("Chainmail");
    if (chainmail != null)
        player.Equipment.Equip(EquipmentSlot.Body, chainmail);

    // Set up engine
    engine.Initialize(player);
}
