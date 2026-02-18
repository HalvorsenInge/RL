using EldritchDungeon.Core;
using EldritchDungeon.Data.Classes;
using EldritchDungeon.Data.Items;
using EldritchDungeon.Data.Races;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.UI;

public class CharacterCreationScreen
{
    private enum Step { Name, Race, Class, Stats, Confirm }

    private readonly ASCIIRenderer _renderer;

    private Step _step = Step.Name;
    private string _name = "";
    private int _raceIndex;
    private int _classIndex;
    private int[] _rolledStats = Array.Empty<int>();

    private readonly RaceType[] _raceTypes;
    private readonly ClassType[] _classTypes;

    public CharacterCreationScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
        _raceTypes = Enum.GetValues<RaceType>();
        _classTypes = Enum.GetValues<ClassType>();
    }

    /// <summary>
    /// Runs the character creation flow. Returns a fully initialized Player, or null if cancelled.
    /// </summary>
    public Player? Run()
    {
        while (true)
        {
            Render();
            var key = Console.ReadKey(true);

            switch (_step)
            {
                case Step.Name:
                    if (!HandleNameInput(key)) return null;
                    break;
                case Step.Race:
                    if (!HandleRaceInput(key)) return null;
                    break;
                case Step.Class:
                    HandleClassInput(key);
                    break;
                case Step.Stats:
                    HandleStatsInput(key);
                    break;
                case Step.Confirm:
                    var result = HandleConfirmInput(key);
                    if (result != null) return result;
                    break;
            }
        }
    }

    private void Render()
    {
        switch (_step)
        {
            case Step.Name: RenderNameStep(); break;
            case Step.Race: RenderRaceStep(); break;
            case Step.Class: RenderClassStep(); break;
            case Step.Stats: RenderStatsStep(); break;
            case Step.Confirm: RenderConfirmStep(); break;
        }
    }

    // === NAME ENTRY ===

    private void RenderNameStep()
    {
        _renderer.Clear();
        int row = 1;
        _renderer.WriteString(1, row, "=== Eldritch Dungeon - Character Creation ===", ConsoleColor.Yellow);
        row += 3;

        _renderer.WriteString(1, row, "What is your name, adventurer?", ConsoleColor.White);
        row += 2;

        _renderer.WriteString(3, row, "> " + _name + "_", ConsoleColor.Cyan);
        row += 4;

        _renderer.WriteString(1, row, "Type your name and press [Enter] to continue.", ConsoleColor.Gray);
        row++;
        _renderer.WriteString(1, row, "[Esc] Quit", ConsoleColor.DarkGray);

        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "Step 1 of 4", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    private bool HandleNameInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
            return false;

        if (key.Key == ConsoleKey.Enter)
        {
            if (_name.Length > 0)
                _step = Step.Race;
            return true;
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (_name.Length > 0)
                _name = _name[..^1];
            return true;
        }

        if (_name.Length < 20 && !char.IsControl(key.KeyChar))
        {
            _name += key.KeyChar;
        }

        return true;
    }

    // === RACE SELECTION ===

    private void RenderRaceStep()
    {
        _renderer.Clear();
        int row = 1;
        _renderer.WriteString(1, row, "=== Choose Your Race ===", ConsoleColor.Yellow);
        row += 2;

        // Race list on the left
        for (int i = 0; i < _raceTypes.Length; i++)
        {
            var race = RaceDatabase.Get(_raceTypes[i]);
            bool selected = i == _raceIndex;
            var color = selected ? ConsoleColor.Cyan : ConsoleColor.Gray;
            string marker = selected ? "> " : "  ";

            _renderer.WriteString(1, row + i, marker + race.Name.PadRight(18), color);

            // Stat modifiers inline
            string mods = FormatMod(race.StrMod) + " "
                        + FormatMod(race.DexMod) + " "
                        + FormatMod(race.ConMod) + " "
                        + FormatMod(race.IntMod) + " "
                        + FormatMod(race.WisMod) + " "
                        + FormatMod(race.ChaMod);
            _renderer.WriteString(21, row + i, mods, selected ? ConsoleColor.White : ConsoleColor.DarkGray);
        }

        // Column headers for stat mods
        _renderer.WriteString(21, row - 1, "STR  DEX  CON  INT  WIS  CHA", ConsoleColor.DarkCyan);

        // Detail panel on the right for selected race
        var selectedRace = RaceDatabase.Get(_raceTypes[_raceIndex]);
        int detailCol = 54;
        int detailRow = row;

        _renderer.WriteString(detailCol, detailRow, selectedRace.Name, ConsoleColor.Cyan);
        detailRow += 2;

        _renderer.WriteString(detailCol, detailRow, "HP Mult:     x" + selectedRace.HpMultiplier.ToString("0.0"), ConsoleColor.White);
        detailRow++;
        _renderer.WriteString(detailCol, detailRow, "Mana Mult:   x" + selectedRace.ManaMultiplier.ToString("0.0"), ConsoleColor.White);
        detailRow++;
        _renderer.WriteString(detailCol, detailRow, "Sanity Mult: x" + selectedRace.SanityMultiplier.ToString("0.0"), ConsoleColor.White);

        // Navigation hints
        row = GameConstants.ScreenHeight - 1;
        _renderer.WriteString(1, row,
            "[Up/Down] Select   [Enter] Choose   [Esc] Back", ConsoleColor.DarkGray);
        _renderer.WriteString(60, row, "Step 2 of 4", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    private bool HandleRaceInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
        {
            _step = Step.Name;
            return true;
        }

        if (key.Key == ConsoleKey.UpArrow || key.KeyChar == 'k')
            _raceIndex = (_raceIndex - 1 + _raceTypes.Length) % _raceTypes.Length;
        else if (key.Key == ConsoleKey.DownArrow || key.KeyChar == 'j')
            _raceIndex = (_raceIndex + 1) % _raceTypes.Length;
        else if (key.Key == ConsoleKey.Enter)
            _step = Step.Class;

        return true;
    }

    // === CLASS SELECTION ===

    private void RenderClassStep()
    {
        _renderer.Clear();
        int row = 1;
        _renderer.WriteString(1, row, "=== Choose Your Class ===", ConsoleColor.Yellow);
        row += 2;

        // Class list
        for (int i = 0; i < _classTypes.Length; i++)
        {
            var cls = ClassDatabase.Get(_classTypes[i]);
            bool selected = i == _classIndex;
            var color = selected ? ConsoleColor.Cyan : ConsoleColor.Gray;
            string marker = selected ? "> " : "  ";

            string info = $"HP:{cls.BaseHp,-4} Mana:{cls.BaseMana,-4} Gold:{cls.StartingGold}";
            _renderer.WriteString(1, row + i, marker + cls.Name.PadRight(14), color);
            _renderer.WriteString(17, row + i, info, selected ? ConsoleColor.White : ConsoleColor.DarkGray);
        }

        // Detail panel for selected class
        var selectedClass = ClassDatabase.Get(_classTypes[_classIndex]);
        int detailCol = 1;
        int detailRow = row + _classTypes.Length + 2;

        _renderer.WriteString(detailCol, detailRow, "Starting Equipment:", ConsoleColor.DarkCyan);
        detailRow++;

        foreach (var weapon in selectedClass.StartingWeapons)
        {
            _renderer.WriteString(detailCol + 2, detailRow, "- " + weapon, ConsoleColor.White);
            detailRow++;
        }

        if (!string.IsNullOrEmpty(selectedClass.StartingArmor))
        {
            _renderer.WriteString(detailCol + 2, detailRow, "- " + selectedClass.StartingArmor, ConsoleColor.White);
            detailRow++;
        }

        foreach (var item in selectedClass.StartingItems)
        {
            _renderer.WriteString(detailCol + 2, detailRow, "- " + item, ConsoleColor.Gray);
            detailRow++;
        }

        // Navigation
        row = GameConstants.ScreenHeight - 1;
        _renderer.WriteString(1, row,
            "[Up/Down] Select   [Enter] Choose   [Esc] Back", ConsoleColor.DarkGray);
        _renderer.WriteString(60, row, "Step 3 of 4", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    private void HandleClassInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
        {
            _step = Step.Race;
            return;
        }

        if (key.Key == ConsoleKey.UpArrow || key.KeyChar == 'k')
            _classIndex = (_classIndex - 1 + _classTypes.Length) % _classTypes.Length;
        else if (key.Key == ConsoleKey.DownArrow || key.KeyChar == 'j')
            _classIndex = (_classIndex + 1) % _classTypes.Length;
        else if (key.Key == ConsoleKey.Enter)
        {
            _rolledStats = Dice.RollAbilityScores();
            _step = Step.Stats;
        }
    }

    // === STAT ROLLING ===

    private void RenderStatsStep()
    {
        _renderer.Clear();
        var race = RaceDatabase.Get(_raceTypes[_raceIndex]);
        var cls = ClassDatabase.Get(_classTypes[_classIndex]);
        var mods = race.GetStatModifiers();

        int row = 1;
        _renderer.WriteString(1, row, "=== Roll Your Stats ===", ConsoleColor.Yellow);
        row += 2;

        // Character summary on the right
        _renderer.WriteString(50, row, "Name:  " + _name, ConsoleColor.White);
        _renderer.WriteString(50, row + 1, "Race:  " + race.Name, ConsoleColor.White);
        _renderer.WriteString(50, row + 2, "Class: " + cls.Name, ConsoleColor.White);

        // Stat display
        string[] statNames = { "STR", "DEX", "CON", "INT", "WIS", "CHA" };
        _renderer.WriteString(1, row, "Stat   Base  Racial  Final  Mod", ConsoleColor.DarkCyan);
        row++;
        _renderer.WriteString(1, row, "----   ----  ------  -----  ---", ConsoleColor.DarkGray);
        row++;

        for (int i = 0; i < 6; i++)
        {
            int baseVal = _rolledStats[i];
            int racialMod = mods[i];
            int finalVal = baseVal + racialMod;
            int mod = Dice.GetModifier(finalVal);

            _renderer.WriteString(1, row + i, statNames[i], ConsoleColor.White);
            _renderer.WriteString(8, row + i, baseVal.ToString().PadLeft(3), ConsoleColor.White);
            var modColor = racialMod > 0 ? ConsoleColor.Green : racialMod < 0 ? ConsoleColor.Red : ConsoleColor.DarkGray;
            _renderer.WriteString(14, row + i, FormatMod(racialMod).PadLeft(4), modColor);
            _renderer.WriteString(22, row + i, finalVal.ToString().PadLeft(4), ConsoleColor.Cyan);
            _renderer.WriteString(28, row + i, $"({FormatMod(mod)})", ConsoleColor.DarkGray);
        }

        // Derived stats
        int conMod = Dice.GetModifier(_rolledStats[2] + mods[2]);
        int intMod = Dice.GetModifier(_rolledStats[3] + mods[3]);
        int wisMod = Dice.GetModifier(_rolledStats[4] + mods[4]);

        int maxHp = (int)((cls.BaseHp + conMod * 5) * race.HpMultiplier);
        int maxMana = (int)((cls.BaseMana + intMod * 5) * race.ManaMultiplier);
        int maxSanity = (int)((GameConstants.MaxSanity + wisMod * 10) * race.SanityMultiplier);

        int derivedRow = row + 8;
        _renderer.WriteString(1, derivedRow, "Derived Stats:", ConsoleColor.DarkCyan);
        derivedRow++;
        _renderer.WriteString(3, derivedRow, $"HP:     {maxHp}", ConsoleColor.Red);
        derivedRow++;
        _renderer.WriteString(3, derivedRow, $"Mana:   {maxMana}", ConsoleColor.Blue);
        derivedRow++;
        _renderer.WriteString(3, derivedRow, $"Sanity: {maxSanity}", ConsoleColor.Magenta);
        derivedRow++;
        _renderer.WriteString(3, derivedRow, $"Gold:   {cls.StartingGold}", ConsoleColor.Yellow);

        // Navigation
        row = GameConstants.ScreenHeight - 1;
        _renderer.WriteString(1, row,
            "[R] Reroll   [Enter] Accept   [Esc] Back", ConsoleColor.DarkGray);
        _renderer.WriteString(60, row, "Step 4 of 4", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    private void HandleStatsInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
        {
            _step = Step.Class;
            return;
        }

        if (key.KeyChar is 'r' or 'R')
            _rolledStats = Dice.RollAbilityScores();
        else if (key.Key == ConsoleKey.Enter)
            _step = Step.Confirm;
    }

    // === CONFIRMATION ===

    private void RenderConfirmStep()
    {
        _renderer.Clear();
        var race = RaceDatabase.Get(_raceTypes[_raceIndex]);
        var cls = ClassDatabase.Get(_classTypes[_classIndex]);
        var mods = race.GetStatModifiers();

        int row = 1;
        _renderer.WriteString(1, row, "=== Confirm Your Character ===", ConsoleColor.Yellow);
        row += 2;

        _renderer.WriteString(1, row, _name, ConsoleColor.Cyan);
        _renderer.WriteString(25, row, race.Name + " " + cls.Name, ConsoleColor.White);
        row += 2;

        // Stats in two columns
        string[] statNames = { "STR", "DEX", "CON", "INT", "WIS", "CHA" };
        for (int i = 0; i < 6; i++)
        {
            int finalVal = _rolledStats[i] + mods[i];
            int mod = Dice.GetModifier(finalVal);
            int col = i < 3 ? 1 : 25;
            int r = row + (i % 3);
            _renderer.WriteString(col, r, $"{statNames[i]}: {finalVal,2} ({FormatMod(mod)})", ConsoleColor.White);
        }
        row += 4;

        // Resources
        int conMod = Dice.GetModifier(_rolledStats[2] + mods[2]);
        int intMod = Dice.GetModifier(_rolledStats[3] + mods[3]);
        int wisMod = Dice.GetModifier(_rolledStats[4] + mods[4]);

        int maxHp = (int)((cls.BaseHp + conMod * 5) * race.HpMultiplier);
        int maxMana = (int)((cls.BaseMana + intMod * 5) * race.ManaMultiplier);
        int maxSanity = (int)((GameConstants.MaxSanity + wisMod * 10) * race.SanityMultiplier);

        _renderer.WriteString(1, row, $"HP: {maxHp}", ConsoleColor.Red);
        _renderer.WriteString(14, row, $"Mana: {maxMana}", ConsoleColor.Blue);
        _renderer.WriteString(28, row, $"Sanity: {maxSanity}", ConsoleColor.Magenta);
        _renderer.WriteString(44, row, $"Gold: {cls.StartingGold}", ConsoleColor.Yellow);
        row += 2;

        // Equipment
        _renderer.WriteString(1, row, "Starting Equipment:", ConsoleColor.DarkCyan);
        row++;
        foreach (var w in cls.StartingWeapons)
        {
            _renderer.WriteString(3, row, "- " + w, ConsoleColor.White);
            row++;
        }
        if (!string.IsNullOrEmpty(cls.StartingArmor))
        {
            _renderer.WriteString(3, row, "- " + cls.StartingArmor, ConsoleColor.White);
            row++;
        }
        foreach (var item in cls.StartingItems)
        {
            _renderer.WriteString(3, row, "- " + item, ConsoleColor.Gray);
            row++;
        }

        // Navigation
        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[Enter] Begin Adventure   [Esc] Start Over", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    private Player? HandleConfirmInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
        {
            _step = Step.Stats;
            return null;
        }

        if (key.Key == ConsoleKey.Enter)
            return CreatePlayer();

        return null;
    }

    // === PLAYER CREATION ===

    private Player CreatePlayer()
    {
        var race = RaceDatabase.Get(_raceTypes[_raceIndex]);
        var cls = ClassDatabase.Get(_classTypes[_classIndex]);

        var player = Player.CreateCharacter(
            _name,
            race.Type,
            cls.Type,
            race.GetStatModifiers(),
            race.HpMultiplier,
            race.ManaMultiplier,
            race.SanityMultiplier,
            cls.BaseHp,
            cls.BaseMana,
            cls.StartingGold,
            _rolledStats);

        // Equip starting weapons (first goes to MainHand, second to OffHand)
        for (int i = 0; i < cls.StartingWeapons.Count; i++)
        {
            try
            {
                var weapon = WeaponDatabase.Get(cls.StartingWeapons[i]);
                var slot = i == 0 ? EquipmentSlot.MainHand : EquipmentSlot.OffHand;
                player.Equipment.Equip(slot, weapon);
            }
            catch (KeyNotFoundException)
            {
                // Weapon not in database yet, skip
            }
        }

        // Equip starting armor
        if (!string.IsNullOrEmpty(cls.StartingArmor))
        {
            try
            {
                var armor = ArmorDatabase.Get(cls.StartingArmor);
                player.Equipment.Equip(EquipmentSlot.Body, armor);
            }
            catch (KeyNotFoundException)
            {
                // Armor not in database yet, skip
            }
        }

        return player;
    }

    private static string FormatMod(int mod)
    {
        return mod >= 0 ? $"+{mod}" : mod.ToString();
    }
}
