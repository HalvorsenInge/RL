using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.UI;

public class CharacterScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private Player? _player;
    private int _dungeonLevel;

    public CharacterScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public void SetPlayer(Player player, int dungeonLevel)
    {
        _player = player;
        _dungeonLevel = dungeonLevel;
    }

    public override void Render()
    {
        if (_player == null) return;

        _renderer.Clear();

        int row = 0;
        var p = _player;

        // Title
        string title = $"=== {p.Name} the {p.Race} {p.Class} ===";
        int titleX = (GameConstants.ScreenWidth - title.Length) / 2;
        _renderer.WriteString(titleX, row++, title, ConsoleColor.Yellow);
        row++;

        // Level & XP
        _renderer.WriteString(1, row, $"Level: {p.Stats.Level}", ConsoleColor.White);
        _renderer.WriteString(20, row, $"XP: {p.Stats.Experience}/{p.Stats.ExperienceToNextLevel}", ConsoleColor.Cyan);
        _renderer.WriteString(50, row, $"Dungeon Level: {_dungeonLevel}", ConsoleColor.White);
        row++;
        _renderer.WriteString(1, row, $"Gold: {p.Gold}", ConsoleColor.DarkYellow);
        row += 2;

        // Stats
        _renderer.WriteString(1, row, "-- Stats --", ConsoleColor.DarkCyan);
        _renderer.WriteString(40, row, "-- Resources --", ConsoleColor.DarkCyan);
        row++;

        RenderStat(1, row, "STR", p.Stats.Strength, p.Stats.GetModifier(StatType.Strength));
        _renderer.WriteString(40, row, $"HP:     {p.Health.CurrentHp,3}/{p.Health.MaxHp}", ConsoleColor.Red);
        row++;

        RenderStat(1, row, "DEX", p.Stats.Dexterity, p.Stats.GetModifier(StatType.Dexterity));
        _renderer.WriteString(40, row, $"Mana:   {p.Mana.CurrentMana,3}/{p.Mana.MaxMana}", ConsoleColor.Blue);
        row++;

        RenderStat(1, row, "CON", p.Stats.Constitution, p.Stats.GetModifier(StatType.Constitution));
        string sanityColor = p.Sanity.State switch
        {
            SanityState.Stable => "Stable",
            SanityState.Fractured => "Fractured",
            SanityState.Unraveling => "Unraveling",
            _ => "BROKEN"
        };
        var sanConsoleColor = p.Sanity.State switch
        {
            SanityState.Stable => ConsoleColor.Magenta,
            SanityState.Fractured => ConsoleColor.DarkMagenta,
            SanityState.Unraveling => ConsoleColor.DarkRed,
            _ => ConsoleColor.Red
        };
        _renderer.WriteString(40, row, $"Sanity: {p.Sanity.CurrentSanity,3}/{p.Sanity.MaxSanity} ({sanityColor})", sanConsoleColor);
        row++;

        RenderStat(1, row, "INT", p.Stats.Intelligence, p.Stats.GetModifier(StatType.Intelligence));
        row++;

        RenderStat(1, row, "WIS", p.Stats.Wisdom, p.Stats.GetModifier(StatType.Wisdom));
        if (p.AddictionLevel > 0)
        {
            var addColor = p.AddictionLevel >= GameConstants.AddictionThreshold
                ? ConsoleColor.Red : ConsoleColor.DarkYellow;
            string addLabel = p.AddictionLevel >= GameConstants.AddictionThreshold ? "ADDICTED" : "At Risk";
            _renderer.WriteString(40, row, $"Addiction: {p.AddictionLevel}/{GameConstants.MaxAddiction} ({addLabel})", addColor);
        }
        row++;

        RenderStat(1, row, "CHA", p.Stats.Charisma, p.Stats.GetModifier(StatType.Charisma));
        row += 2;

        // Equipment
        _renderer.WriteString(1, row, "-- Equipment --", ConsoleColor.DarkCyan);
        row++;
        foreach (EquipmentSlot slot in Enum.GetValues<EquipmentSlot>())
        {
            var item = p.Equipment.GetEquipped(slot);
            string slotName = $"{slot,-9}";
            if (item != null)
            {
                string detail = item switch
                {
                    Weapon w => $"{item.Name} (Dmg: {w.Damage}, Spd: {w.Speed})",
                    Armor a => $"{item.Name} (AC: {a.ArmorClass})",
                    _ => item.Name
                };
                _renderer.WriteString(1, row, $"  {slotName}: {detail}", ConsoleColor.White);
            }
            else
            {
                _renderer.WriteString(1, row, $"  {slotName}: (empty)", ConsoleColor.DarkGray);
            }
            row++;
        }
        row++;

        // Status Effects
        if (p.StatusEffects.ActiveEffects.Count > 0)
        {
            _renderer.WriteString(1, row, "-- Status Effects --", ConsoleColor.DarkCyan);
            row++;
            foreach (var effect in p.StatusEffects.ActiveEffects)
            {
                var color = effect.Type switch
                {
                    StatusEffectType.Blessed or StatusEffectType.Invisible => ConsoleColor.Green,
                    StatusEffectType.Poison or StatusEffectType.Bleeding or StatusEffectType.Burning => ConsoleColor.Red,
                    _ => ConsoleColor.DarkYellow
                };
                _renderer.WriteString(1, row, $"  {effect.Type} ({effect.RemainingTurns} turns)", color);
                row++;
            }
            row++;
        }

        // Religion
        if (p.Religion.CurrentGod.HasValue)
        {
            _renderer.WriteString(1, row, "-- Religion --", ConsoleColor.DarkCyan);
            row++;
            _renderer.WriteString(1, row, $"  Worshipping: {p.Religion.CurrentGod}", ConsoleColor.Magenta);
            row++;
            _renderer.WriteString(1, row, $"  Favor: {p.Religion.Favor}/{GameConstants.MaxFavor}  (Tier {p.Religion.PowerTier})", ConsoleColor.Green);
            _renderer.WriteString(40, row, $"Anger: {p.Religion.Anger}/{GameConstants.MaxAnger}",
                p.Religion.Anger > 50 ? ConsoleColor.Red : ConsoleColor.DarkYellow);
            row++;
        }

        // Footer
        _renderer.WriteString(1, GameConstants.ScreenHeight - 1, "[Esc] Back  [i] Inventory  [r] Religion  [?] Help", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.KeyChar switch
        {
            'i' => ScreenResult.OpenInventory,
            'r' => ScreenResult.OpenReligion,
            '?' => ScreenResult.OpenHelp,
            _ => keyInfo.Key == ConsoleKey.Escape ? ScreenResult.Close : ScreenResult.None
        };
    }

    private void RenderStat(int x, int y, string label, int value, int modifier)
    {
        string modStr = modifier >= 0 ? $"+{modifier}" : $"{modifier}";
        _renderer.WriteString(x, y, $"  {label}: {value,2} ({modStr})", ConsoleColor.White);
    }
}
