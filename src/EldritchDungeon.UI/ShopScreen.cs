using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.UI;

public class ShopScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private Player? _player;
    private List<Item>? _shopInventory;

    // 0 = shop pane, 1 = player pane
    private int _activePane;
    private int _shopIndex;
    private int _playerIndex;

    private const int MaxVisible = 14;
    private const int ShopPaneX  = 1;
    private const int PlayerPaneX = 42;

    public ShopScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public void Set(Player player, List<Item> shopInventory)
    {
        _player       = player;
        _shopInventory = shopInventory;
        _activePane   = 0;
        _shopIndex    = 0;
        _playerIndex  = 0;
    }

    public override void Render()
    {
        if (_player == null || _shopInventory == null) return;

        _renderer.Clear();

        // Header
        _renderer.WriteString(ShopPaneX, 0, "=== SHOP ===", ConsoleColor.Yellow);
        _renderer.WriteString(PlayerPaneX, 0, "=== YOUR PACK ===", ConsoleColor.Cyan);
        _renderer.WriteString(62, 0, $"Gold: {_player.Gold}g", ConsoleColor.DarkYellow);

        // Column headers
        _renderer.WriteString(ShopPaneX, 1,  "Item                       Price", ConsoleColor.DarkGray);
        _renderer.WriteString(PlayerPaneX, 1, "Item                       Sell", ConsoleColor.DarkGray);
        _renderer.WriteString(ShopPaneX, 2,   new string('-', 38), ConsoleColor.DarkGray);
        _renderer.WriteString(PlayerPaneX, 2, new string('-', 38), ConsoleColor.DarkGray);

        RenderPane(_shopInventory, _shopIndex, _activePane == 0, ShopPaneX, isBuyPane: true);
        RenderPane(_player.Inventory.Items, _playerIndex, _activePane == 1, PlayerPaneX, isBuyPane: false);

        // Detail row for selected item
        RenderDetail();

        // Footer
        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[Tab] Switch Pane  [b] Buy  [v] Sell  [Up/Down] Navigate  [Esc] Leave",
            ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    private void RenderPane(IList<Item> items, int selectedIdx, bool isActive, int startX, bool isBuyPane)
    {
        var headerColor = isActive ? ConsoleColor.White : ConsoleColor.DarkGray;
        int startRow = 3;

        if (items.Count == 0)
        {
            _renderer.WriteString(startX, startRow, "(empty)", ConsoleColor.DarkGray);
            return;
        }

        int end = Math.Min(MaxVisible, items.Count);
        for (int i = 0; i < end; i++)
        {
            var item = items[i];
            bool selected = (i == selectedIdx) && isActive;
            var color  = selected ? ConsoleColor.White : ConsoleColor.Gray;
            var prefix = selected ? ">" : " ";

            char letter = (char)('a' + i);
            string nameStr = Truncate($"{item.Glyph} {item.Name}", 24);
            int price = isBuyPane ? item.Value : Math.Max(1, item.Value / 2);
            string priceStr = $"{price}g";

            _renderer.WriteString(startX, startRow + i, $"{prefix}{letter}) {nameStr,-24} {priceStr,5}", color);

            // Highlight enchanted weapons
            if (item is Weapon w && w.MagicDamage > 0)
                _renderer.WriteString(startX + 4, startRow + i, $"{item.Glyph} {Truncate(item.Name, 24)}", ConsoleColor.Magenta);
        }

        if (items.Count > MaxVisible)
            _renderer.WriteString(startX, startRow + MaxVisible, $"(+{items.Count - MaxVisible} more)", ConsoleColor.DarkGray);
    }

    private void RenderDetail()
    {
        if (_player == null || _shopInventory == null) return;

        Item? sel = null;
        bool isBuyPane = _activePane == 0;

        if (isBuyPane && _shopIndex < _shopInventory.Count)
            sel = _shopInventory[_shopIndex];
        else if (!isBuyPane && _playerIndex < _player.Inventory.Items.Count)
            sel = _player.Inventory.Items[_playerIndex];

        if (sel == null) return;

        int detailRow = GameConstants.ScreenHeight - 4;
        _renderer.WriteString(1, detailRow, new string('-', 78), ConsoleColor.DarkGray);
        detailRow++;

        int price = isBuyPane ? sel.Value : Math.Max(1, sel.Value / 2);
        string priceLabel = isBuyPane ? "Buy" : "Sell";
        _renderer.WriteString(1, detailRow, $"{sel.Name}", ConsoleColor.White);
        _renderer.WriteString(45, detailRow, $"{priceLabel}: {price}g  Wt:{sel.Weight:F1}", ConsoleColor.Gray);
        detailRow++;

        string detail = sel switch
        {
            Weapon w  => $"Dmg:{w.Damage}" + (w.MagicDamage > 0 ? $" +{w.MagicDamage} {w.EnchantmentName}" : "") +
                         $"  Crit:{w.CritRangeMin}-20x{w.CritMultiplier}  Spd:{w.Speed}  Rng:{w.Range}" +
                         (string.IsNullOrEmpty(w.Special) ? "" : $"  [{w.Special}]"),
            Armor  a  => $"AC:{a.ArmorClass}  Slot:{a.Slot}",
            Consumable c => FormatConsumable(c),
            _           => $"Value:{sel.Value}g",
        };
        _renderer.WriteString(1, detailRow, Truncate(detail, 78), ConsoleColor.Gray);
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (_player == null || _shopInventory == null) return ScreenResult.Close;

        switch (keyInfo.Key)
        {
            case ConsoleKey.Escape:
                return ScreenResult.Close;

            case ConsoleKey.Tab:
                _activePane = 1 - _activePane;
                return ScreenResult.None;

            case ConsoleKey.UpArrow:
                if (_activePane == 0 && _shopIndex > 0)  _shopIndex--;
                if (_activePane == 1 && _playerIndex > 0) _playerIndex--;
                return ScreenResult.None;

            case ConsoleKey.DownArrow:
                if (_activePane == 0 && _shopIndex < _shopInventory.Count - 1)   _shopIndex++;
                if (_activePane == 1 && _playerIndex < _player.Inventory.Items.Count - 1) _playerIndex++;
                return ScreenResult.None;
        }

        switch (keyInfo.KeyChar)
        {
            case 'b':
                TryBuy();
                break;
            case 'v':
                TrySell();
                break;
        }

        return ScreenResult.None;
    }

    private void TryBuy()
    {
        if (_player == null || _shopInventory == null) return;
        if (_shopIndex >= _shopInventory.Count) return;

        var item = _shopInventory[_shopIndex];

        if (_player.Gold < item.Value)
            return; // Can't afford — no log here, UI is self-explanatory via the price display

        if (!_player.Inventory.AddItem(item))
            return; // Inventory full

        _player.Gold -= item.Value;
        _shopInventory.RemoveAt(_shopIndex);
        if (_shopIndex >= _shopInventory.Count && _shopIndex > 0)
            _shopIndex--;
    }

    private void TrySell()
    {
        if (_player == null) return;
        var items = _player.Inventory.Items;
        if (_playerIndex >= items.Count) return;

        var item  = items[_playerIndex];
        int price = Math.Max(1, item.Value / 2);

        _player.Gold += price;
        _player.Inventory.RemoveItem(item);
        _shopInventory!.Add(item); // Merchant buys it back

        if (_playerIndex >= items.Count && _playerIndex > 0)
            _playerIndex--;
    }

    private static string FormatConsumable(Consumable c)
    {
        var parts = new List<string>();
        if (c.HealAmount > 0)    parts.Add($"Heals {c.HealAmount} HP");
        if (c.ManaAmount > 0)    parts.Add($"Restores {c.ManaAmount} MP");
        if (c.SanityAmount > 0)  parts.Add($"Restores {c.SanityAmount} SAN");
        if (c.AddictionRisk > 0) parts.Add($"Addiction {c.AddictionRisk}%");
        return string.Join("  |  ", parts);
    }

    private static string Truncate(string s, int maxLen)
        => s.Length <= maxLen ? s : s[..(maxLen - 2)] + "..";
}
