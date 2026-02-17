using EldritchDungeon.Core;
using EldritchDungeon.Data.Gods;
using EldritchDungeon.Entities;

namespace EldritchDungeon.UI;

public class ReligionScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private Player? _player;
    private int _selectedGodIndex;
    private bool _viewingPantheon; // true = browsing gods, false = viewing current god

    public ReligionScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public override void Render()
    {
        if (_player == null) return;

        _renderer.Clear();

        if (_player.Religion.CurrentGod.HasValue)
            RenderCurrentGod();
        else
            RenderGodSelection();

        _renderer.Flush();
    }

    private void RenderCurrentGod()
    {
        var p = _player!;
        var god = GodDatabase.Get(p.Religion.CurrentGod!.Value);
        int row = 0;

        _renderer.WriteString(1, row, $"=== Devotion to {god.Name} ===", ConsoleColor.Yellow);
        row += 2;

        _renderer.WriteString(1, row, $"Domain: {god.Domain}", ConsoleColor.Cyan);
        row++;
        _renderer.WriteString(1, row, $"Favor Bonus: {god.FavorBonus}", ConsoleColor.Green);
        row++;
        _renderer.WriteString(1, row, $"Anger Trigger: {god.AngerTrigger}", ConsoleColor.Red);
        row += 2;

        // Favor/Anger bars
        _renderer.WriteString(1, row, "Favor:", ConsoleColor.White);
        RenderBar(9, row, p.Religion.Favor, GameConstants.MaxFavor, ConsoleColor.Green);
        _renderer.WriteString(45, row, $"{p.Religion.Favor}/{GameConstants.MaxFavor}  (Tier {p.Religion.PowerTier})", ConsoleColor.Green);
        row++;

        _renderer.WriteString(1, row, "Anger:", ConsoleColor.White);
        RenderBar(9, row, p.Religion.Anger, GameConstants.MaxAnger, ConsoleColor.Red);
        _renderer.WriteString(45, row, $"{p.Religion.Anger}/{GameConstants.MaxAnger}", ConsoleColor.Red);
        row += 2;

        // Powers
        _renderer.WriteString(1, row, "-- Divine Powers --", ConsoleColor.DarkCyan);
        row++;

        foreach (var power in god.Powers)
        {
            bool unlocked = p.Religion.Favor >= power.FavorRequired;
            var color = unlocked ? ConsoleColor.White : ConsoleColor.DarkGray;
            string lockStr = unlocked ? " [UNLOCKED]" : $" [Requires {power.FavorRequired} favor]";

            _renderer.WriteString(1, row, $"  Tier {power.Tier}: {power.Name}{lockStr}", color);
            row++;
            _renderer.WriteString(5, row, power.Description, unlocked ? ConsoleColor.Gray : ConsoleColor.DarkGray);
            row++;
        }

        row++;
        if (_viewingPantheon)
        {
            RenderPantheonOverlay(row);
        }

        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[Tab] View All Gods  [p] Pray  [Esc] Back", ConsoleColor.DarkGray);
    }

    private void RenderGodSelection()
    {
        int row = 0;
        _renderer.WriteString(1, row, "=== Choose Your God ===", ConsoleColor.Yellow);
        row++;
        _renderer.WriteString(1, row, "You serve no deity. Select a god to worship.", ConsoleColor.Gray);
        row += 2;

        var gods = GodDatabase.GetAll();
        int i = 0;
        foreach (var (type, god) in gods)
        {
            bool selected = i == _selectedGodIndex;
            var color = selected ? ConsoleColor.White : ConsoleColor.Gray;
            string prefix = selected ? "> " : "  ";

            _renderer.WriteString(1, row, $"{prefix}{god.Name} - {god.Domain}", color);
            row++;

            if (selected)
            {
                _renderer.WriteString(5, row, $"Bonus: {god.FavorBonus}", ConsoleColor.Green);
                row++;
                _renderer.WriteString(5, row, $"Anger: {god.AngerTrigger}", ConsoleColor.Red);
                row++;
                _renderer.WriteString(5, row, "Powers:", ConsoleColor.DarkCyan);
                row++;
                foreach (var power in god.Powers)
                {
                    _renderer.WriteString(7, row, $"T{power.Tier}: {power.Name} - {power.Description}", ConsoleColor.DarkGray);
                    row++;
                }
            }
            row++;
            i++;
        }

        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[Up/Down] Select  [Enter] Worship  [Esc] Back", ConsoleColor.DarkGray);
    }

    private void RenderPantheonOverlay(int startRow)
    {
        int row = startRow;
        _renderer.WriteString(1, row, "-- All Gods --", ConsoleColor.DarkCyan);
        row++;

        foreach (var (type, god) in GodDatabase.GetAll())
        {
            bool current = _player!.Religion.CurrentGod == type;
            var color = current ? ConsoleColor.Yellow : ConsoleColor.Gray;
            _renderer.WriteString(1, row, $"  {god.Name,-15} Domain: {god.Domain,-12} Bonus: {god.FavorBonus}", color);
            row++;
            if (row >= GameConstants.ScreenHeight - 2) break;
        }
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (_player == null) return ScreenResult.None;

        if (keyInfo.Key == ConsoleKey.Escape)
        {
            if (_viewingPantheon)
            {
                _viewingPantheon = false;
                return ScreenResult.None;
            }
            return ScreenResult.Close;
        }

        if (_player.Religion.CurrentGod.HasValue)
        {
            // Viewing current god
            if (keyInfo.Key == ConsoleKey.Tab)
                _viewingPantheon = !_viewingPantheon;
            return ScreenResult.None;
        }

        // God selection mode
        int godCount = GodDatabase.GetAll().Count;
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                if (_selectedGodIndex > 0) _selectedGodIndex--;
                break;
            case ConsoleKey.DownArrow:
                if (_selectedGodIndex < godCount - 1) _selectedGodIndex++;
                break;
            case ConsoleKey.Enter:
                var godType = GodDatabase.GetAll().Keys.ElementAt(_selectedGodIndex);
                _player.Religion.SetGod(godType);
                break;
        }

        return ScreenResult.None;
    }

    private void RenderBar(int x, int y, int value, int max, ConsoleColor color)
    {
        int barWidth = 30;
        int filled = max > 0 ? (value * barWidth) / max : 0;
        _renderer.WriteString(x, y, "[", ConsoleColor.DarkGray);
        for (int i = 0; i < barWidth; i++)
        {
            _renderer.Set(x + 1 + i, y, i < filled ? '=' : '-',
                i < filled ? color : ConsoleColor.DarkGray);
        }
        _renderer.WriteString(x + barWidth + 1, y, "]", ConsoleColor.DarkGray);
    }
}
