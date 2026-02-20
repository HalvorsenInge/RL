using EldritchDungeon.Core;

namespace EldritchDungeon.UI;

public class HelpScreen : Screen
{
    private readonly ASCIIRenderer _renderer;

    public HelpScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Render()
    {
        _renderer.Clear();

        int row = 0;
        _renderer.WriteString(1, row, "=== Eldritch Dungeon - Help ===", ConsoleColor.Yellow);
        row += 2;

        // Movement
        _renderer.WriteString(1, row, "-- Movement --", ConsoleColor.DarkCyan);
        _renderer.WriteString(40, row, "-- Actions --", ConsoleColor.DarkCyan);
        row++;

        _renderer.WriteString(1, row, "  Arrow keys / Numpad  Move", ConsoleColor.White);
        _renderer.WriteString(40, row, "  >         Descend stairs", ConsoleColor.White);
        row++;

        _renderer.WriteString(1, row, "  y k u    7 8 9", ConsoleColor.Gray);
        _renderer.WriteString(40, row, "  <         Ascend stairs", ConsoleColor.White);
        row++;

        _renderer.WriteString(1, row, "  h . l    4 5 6       Diagonals", ConsoleColor.Gray);
        _renderer.WriteString(40, row, "  .  Space  Wait a turn", ConsoleColor.White);
        row++;

        _renderer.WriteString(1, row, "  b j n    1 2 3", ConsoleColor.Gray);
        _renderer.WriteString(40, row, "  p         Pray to your god", ConsoleColor.White);
        row++;

        _renderer.WriteString(40, row, "  Bump      Attack (walk into enemy)", ConsoleColor.White);
        row++;

        _renderer.WriteString(40, row, "  f         Fire / Shoot (ranged weapon)", ConsoleColor.White);
        row++;

        _renderer.WriteString(40, row, "  m         Spellbook (cast spells)", ConsoleColor.White);
        row++;

        _renderer.WriteString(40, row, "  t         Throw          (not yet impl.)", ConsoleColor.DarkGray);
        row++;

        _renderer.WriteString(40, row, "  g         Pick Up / Get", ConsoleColor.White);
        row += 2;

        // Screens
        _renderer.WriteString(1, row, "-- Screens --", ConsoleColor.DarkCyan);
        row++;
        _renderer.WriteString(1, row, "  c         Character sheet", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  i         Inventory", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  r         Religion", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  M         Message log", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  x         Examine (look mode)", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  s         Shop (near $ merchant)", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  ?         This help screen", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  Escape    Close screen / Quit game", ConsoleColor.White);
        row += 2;

        // Sanity
        _renderer.WriteString(1, row, "-- Sanity States --", ConsoleColor.DarkCyan);
        row++;
        _renderer.WriteString(1, row, "  Stable    (100-51)  Normal gameplay", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  Fractured ( 50-26)  Unreliable messages, hallucinations", ConsoleColor.DarkYellow);
        row++;
        _renderer.WriteString(1, row, "  Unraveling( 25-10)  Fake monsters, stat shifts", ConsoleColor.Red);
        row++;
        _renderer.WriteString(1, row, "  Broken    (  9- 0)  Random actions, heart attack risk", ConsoleColor.DarkRed);
        row += 2;

        // Spell interactions
        _renderer.WriteString(1, row, "-- Spell Interactions --", ConsoleColor.DarkCyan);
        row++;
        _renderer.WriteString(1, row, "  Fire + Water  -> Steam cloud (blocks sight, 5 turns)", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  Fire + Oil    -> Explosion  (35 dmg, large AoE)", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  Lightning + Water -> Electric shock (all connected water)", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  Lightning + Oil   -> Ignite (creates fire)", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, "  Cold + Fire   -> Extinguish (removes fire tile)", ConsoleColor.White);
        row++;

        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[Esc] Back", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key == ConsoleKey.Escape ? ScreenResult.Close : ScreenResult.None;
    }
}
