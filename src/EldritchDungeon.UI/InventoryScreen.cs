using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.UI;

public class InventoryScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private Player? _player;
    private int _selectedIndex;
    private int _scrollOffset;
    private const int MaxVisibleItems = 18;

    /// <summary>Set when a ToolItem is used; consumed by GameLoop to trigger the effect.</summary>
    public ToolItem? PendingToolItem { get; private set; }

    public InventoryScreen(ASCIIRenderer renderer)
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

        int row = 0;
        _renderer.WriteString(1, row, "=== Inventory ===", ConsoleColor.Yellow);
        _renderer.WriteString(50, row, $"Carrying: {_player.Inventory.Items.Count}/{_player.Inventory.Capacity}", ConsoleColor.White);
        _renderer.WriteString(70, row, $"Gold: {_player.Gold}", ConsoleColor.DarkYellow);
        row += 2;

        var items = _player.Inventory.Items;
        if (items.Count == 0)
        {
            _renderer.WriteString(1, row, "Your pack is empty.", ConsoleColor.DarkGray);
        }
        else
        {
            // Header
            _renderer.WriteString(1, row, " # ", ConsoleColor.DarkGray);
            _renderer.WriteString(5, row, "Item", ConsoleColor.DarkGray);
            _renderer.WriteString(35, row, "Type", ConsoleColor.DarkGray);
            _renderer.WriteString(50, row, "Details", ConsoleColor.DarkGray);
            row++;

            _renderer.WriteString(1, row, new string('-', 78), ConsoleColor.DarkGray);
            row++;

            int end = Math.Min(_scrollOffset + MaxVisibleItems, items.Count);
            for (int i = _scrollOffset; i < end; i++)
            {
                var item = items[i];
                bool selected = i == _selectedIndex;
                var color = selected ? ConsoleColor.White : ConsoleColor.Gray;
                var prefix = selected ? ">" : " ";

                char letter = (char)('a' + (i - _scrollOffset));
                _renderer.WriteString(1, row, $"{prefix}{letter})", color);
                _renderer.WriteString(5, row, $"{item.Glyph} {Truncate(item.Name, 27)}", color);
                _renderer.WriteString(35, row, item.Type.ToString(), ConsoleColor.DarkCyan);

                string detail = item switch
                {
                    Weapon w => $"Dmg:{w.Damage}" + (w.MagicDamage > 0 ? $"+{w.MagicDamage}m" : "") + $" Spd:{w.Speed} Rng:{w.Range}",
                    Armor a => $"AC:{a.ArmorClass}",
                    Consumable c => FormatConsumable(c),
                    ToolItem t => $"[TOOL] {t.Effect}",
                    _ => $"Val:{item.Value}"
                };
                _renderer.WriteString(50, row, detail, ConsoleColor.DarkYellow);
                row++;
            }

            if (items.Count > MaxVisibleItems)
            {
                row++;
                _renderer.WriteString(1, row, $"-- Showing {_scrollOffset + 1}-{end} of {items.Count} --", ConsoleColor.DarkGray);
            }
        }

        // Selected item detail
        if (items.Count > 0 && _selectedIndex < items.Count)
        {
            row = GameConstants.ScreenHeight - 4;
            _renderer.WriteString(1, row, new string('-', 78), ConsoleColor.DarkGray);
            row++;
            var sel = items[_selectedIndex];
            _renderer.WriteString(1, row, $"{sel.Name}", ConsoleColor.White);
            _renderer.WriteString(30, row, $"Weight: {sel.Weight:F1}  Value: {sel.Value}g", ConsoleColor.Gray);
            row++;
            if (sel is Weapon w2)
            {
                string special = string.IsNullOrEmpty(w2.Special) ? "" : $" | {w2.Special}";
                _renderer.WriteString(1, row,
                    $"Damage: {w2.Damage}  Crit: {w2.CritRangeMin}-20 x{w2.CritMultiplier}  {w2.Category}{special}",
                    ConsoleColor.Gray);
            }
            else if (sel is Armor a2)
            {
                _renderer.WriteString(1, row, $"Armor Class: {a2.ArmorClass}  Slot: {a2.Slot}", ConsoleColor.Gray);
            }
            else if (sel is Consumable c2)
            {
                _renderer.WriteString(1, row, FormatConsumableDetail(c2), ConsoleColor.Gray);
            }
            else if (sel is ToolItem t2)
            {
                string desc = string.IsNullOrEmpty(t2.EffectDescription) ? $"Effect: {t2.Effect}" : t2.EffectDescription;
                _renderer.WriteString(1, row, desc, ConsoleColor.Magenta);
            }
        }

        // Footer
        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[Up/Down] Select  [e] Equip  [d] Drop  [u] Use  [Esc] Back", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (_player == null) return ScreenResult.None;

        var items = _player.Inventory.Items;

        switch (keyInfo.Key)
        {
            case ConsoleKey.Escape:
                return ScreenResult.Close;

            case ConsoleKey.UpArrow:
                if (_selectedIndex > 0)
                {
                    _selectedIndex--;
                    if (_selectedIndex < _scrollOffset)
                        _scrollOffset = _selectedIndex;
                }
                break;

            case ConsoleKey.DownArrow:
                if (_selectedIndex < items.Count - 1)
                {
                    _selectedIndex++;
                    if (_selectedIndex >= _scrollOffset + MaxVisibleItems)
                        _scrollOffset = _selectedIndex - MaxVisibleItems + 1;
                }
                break;
        }

        if (items.Count == 0) return ScreenResult.None;

        // Letter selection (a-r for visible items)
        if (keyInfo.KeyChar >= 'a' && keyInfo.KeyChar <= 'r')
        {
            int index = _scrollOffset + (keyInfo.KeyChar - 'a');
            if (index < items.Count)
                _selectedIndex = index;
        }

        switch (keyInfo.KeyChar)
        {
            case 'd': // Drop
                if (_selectedIndex < items.Count)
                {
                    items.RemoveAt(_selectedIndex);
                    if (_selectedIndex >= items.Count && _selectedIndex > 0)
                        _selectedIndex--;
                }
                break;

            case 'e': // Equip
                if (_selectedIndex < items.Count)
                    TryEquip(items[_selectedIndex]);
                break;

            case 'u': // Use
                if (_selectedIndex < items.Count)
                {
                    var useResult = TryUse(items[_selectedIndex]);
                    if (useResult != ScreenResult.None)
                        return useResult;
                }
                break;
        }

        return ScreenResult.None;
    }

    private void TryEquip(Item item)
    {
        if (_player == null) return;

        EquipmentSlot? slot = item switch
        {
            Weapon => EquipmentSlot.MainHand,
            Armor a => a.Slot,
            _ => null
        };

        if (slot == null) return;

        // Unequip current item to inventory
        var current = _player.Equipment.GetEquipped(slot.Value);
        if (current != null)
        {
            _player.Equipment.Unequip(slot.Value);
            _player.Inventory.AddItem(current);
        }

        _player.Inventory.RemoveItem(item);
        _player.Equipment.Equip(slot.Value, item);

        if (_selectedIndex >= _player.Inventory.Items.Count && _selectedIndex > 0)
            _selectedIndex--;
    }

    private ScreenResult TryUse(Item item)
    {
        if (_player == null) return ScreenResult.None;

        if (item is Consumable c)
        {
            if (c.HealAmount > 0)
                _player.Health.Heal(c.HealAmount);
            if (c.ManaAmount > 0)
                _player.Mana.Restore(c.ManaAmount);
            if (c.SanityAmount > 0)
                _player.Sanity.RestoreSanity(c.SanityAmount);

            if (c.AddictionRisk > 0)
            {
                _player.AddictionLevel += c.AddictionRisk;
                if (_player.AddictionLevel > 100)
                    _player.AddictionLevel = 100;
            }

            _player.Inventory.RemoveItem(item);
            if (_selectedIndex >= _player.Inventory.Items.Count && _selectedIndex > 0)
                _selectedIndex--;
            return ScreenResult.None;
        }

        if (item is ToolItem tool && tool.Effect != ToolEffect.None)
        {
            _player.Inventory.RemoveItem(item);
            if (_selectedIndex >= _player.Inventory.Items.Count && _selectedIndex > 0)
                _selectedIndex--;
            PendingToolItem = tool;
            return ScreenResult.UseTool;
        }

        return ScreenResult.None;
    }

    private static string FormatConsumable(Consumable c)
    {
        var parts = new List<string>();
        if (c.HealAmount > 0) parts.Add($"HP+{c.HealAmount}");
        if (c.ManaAmount > 0) parts.Add($"MP+{c.ManaAmount}");
        if (c.SanityAmount > 0) parts.Add($"San+{c.SanityAmount}");
        return string.Join(" ", parts);
    }

    private static string FormatConsumableDetail(Consumable c)
    {
        var parts = new List<string>();
        if (c.HealAmount > 0) parts.Add($"Heals {c.HealAmount} HP");
        if (c.ManaAmount > 0) parts.Add($"Restores {c.ManaAmount} Mana");
        if (c.SanityAmount > 0) parts.Add($"Restores {c.SanityAmount} Sanity");
        if (c.AddictionRisk > 0) parts.Add($"Addiction Risk: {c.AddictionRisk}%");
        return string.Join("  |  ", parts);
    }

    private static string Truncate(string s, int maxLen)
    {
        return s.Length <= maxLen ? s : s[..(maxLen - 2)] + "..";
    }
}
