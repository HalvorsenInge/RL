using EldritchDungeon.Core;
using EldritchDungeon.Data.Spells;
using EldritchDungeon.Entities;

namespace EldritchDungeon.UI;

public class SpellScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private Player? _player;

    /// <summary>Set after the player picks a spell. Null if cancelled.</summary>
    public SpellId? SelectedSpell { get; private set; }

    public SpellScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
        SelectedSpell = null;
    }

    public override void Render()
    {
        _renderer.Clear();
        int row = 0;

        _renderer.WriteString(1, row, "=== Spellbook ===", ConsoleColor.Yellow);
        row += 2;

        if (_player == null || _player.KnownSpells.Count == 0)
        {
            _renderer.WriteString(1, row, "You know no spells.", ConsoleColor.DarkGray);
        }
        else
        {
            _renderer.WriteString(1, row,
                $"{"Key",-4} {"Spell",-18} {"MP",4}  {"Range",-8} {"Effect"}",
                ConsoleColor.DarkCyan);
            row++;
            _renderer.WriteString(1, row,
                new string('-', 70),
                ConsoleColor.DarkGray);
            row++;

            for (int i = 0; i < _player.KnownSpells.Count; i++)
            {
                var spellId = _player.KnownSpells[i];
                var spell   = SpellDatabase.Get(spellId);

                char key = (char)('a' + i);
                bool canCast = _player.Mana.CurrentMana >= spell.ManaCost;

                string rangeStr = spell.Target switch
                {
                    SpellTarget.Self      => "Self",
                    SpellTarget.LevelWide => "Level",
                    _                     => $"Rng:{spell.Range}" + (spell.Radius > 0 ? $" AoE:{spell.Radius}" : "")
                };

                string superTag = spell.IsSuperSpell ? " [SUPER]" : "";
                string descLine = $"{spell.Description}{superTag}";
                if (descLine.Length > 40) descLine = descLine[..37] + "...";

                var color = canCast ? spell.Color : ConsoleColor.DarkGray;

                _renderer.WriteString(1, row,
                    $"{key})  {spell.Name,-18} {spell.ManaCost,3}MP  {rangeStr,-9} {descLine}",
                    color);
                row++;

                if (row >= GameConstants.ScreenHeight - 3) break;
            }
        }

        // MP bar
        if (_player != null)
        {
            _renderer.WriteString(1, GameConstants.ScreenHeight - 2,
                $"MP: {_player.Mana.CurrentMana}/{_player.Mana.MaxMana}",
                ConsoleColor.Blue);
        }

        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[a-z] Select spell   [Esc] Cancel",
            ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Escape)
        {
            SelectedSpell = null;
            return ScreenResult.Close;
        }

        if (_player == null) return ScreenResult.None;

        char c = char.ToLower(keyInfo.KeyChar);
        if (c >= 'a' && c <= 'z')
        {
            int index = c - 'a';
            if (index < _player.KnownSpells.Count)
            {
                SelectedSpell = _player.KnownSpells[index];
                return ScreenResult.Close;
            }
        }

        return ScreenResult.None;
    }
}
