using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.UI;

namespace EldritchDungeon.UI.Tests;

public class InventoryScreenTests
{
    private static Player CreateTestPlayer()
    {
        Dice.SetSeed(42);
        return Player.CreateCharacter(
            "Test", RaceType.Human, ClassType.Warrior,
            new[] { 0, 0, 0, 0, 0, 0 }, 1.0, 1.0, 1.0, 40, 10, 50);
    }

    [Fact]
    public void DropItem_RemovesFromInventory()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        var sword = new Weapon { Name = "Dagger", Damage = 5 };
        player.Inventory.AddItem(sword);
        screen.SetPlayer(player);

        // Press 'd' to drop selected item (index 0)
        screen.HandleInput(new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false));

        Assert.Empty(player.Inventory.Items);
    }

    [Fact]
    public void EquipItem_MovesWeaponToMainHand()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        var sword = new Weapon { Name = "Dagger", Damage = 5 };
        player.Inventory.AddItem(sword);
        screen.SetPlayer(player);

        // Press 'e' to equip selected item
        screen.HandleInput(new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false));

        Assert.Empty(player.Inventory.Items);
        Assert.Equal("Dagger", player.Equipment.GetEquipped(EquipmentSlot.MainHand)?.Name);
    }

    [Fact]
    public void EquipItem_SwapsWithExisting()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        var oldSword = new Weapon { Name = "OldSword", Damage = 3 };
        var newSword = new Weapon { Name = "NewSword", Damage = 8 };
        player.Equipment.Equip(EquipmentSlot.MainHand, oldSword);
        player.Inventory.AddItem(newSword);
        screen.SetPlayer(player);

        // Equip the new sword
        screen.HandleInput(new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false));

        Assert.Equal("NewSword", player.Equipment.GetEquipped(EquipmentSlot.MainHand)?.Name);
        Assert.Single(player.Inventory.Items);
        Assert.Equal("OldSword", player.Inventory.Items[0].Name);
    }

    [Fact]
    public void UseConsumable_HealsPlayer()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        player.Health.TakeDamage(20);
        int hpBefore = player.Health.CurrentHp;

        var potion = new Consumable { Name = "Healing Potion", HealAmount = 15 };
        player.Inventory.AddItem(potion);
        screen.SetPlayer(player);

        // Press 'u' to use
        screen.HandleInput(new ConsoleKeyInfo('u', ConsoleKey.U, false, false, false));

        Assert.Equal(hpBefore + 15, player.Health.CurrentHp);
        Assert.Empty(player.Inventory.Items);
    }

    [Fact]
    public void UseConsumable_RestoresMana()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        player.Mana.Spend(5);
        int manaBefore = player.Mana.CurrentMana;

        var potion = new Consumable { Name = "Mana Potion", ManaAmount = 10 };
        player.Inventory.AddItem(potion);
        screen.SetPlayer(player);

        screen.HandleInput(new ConsoleKeyInfo('u', ConsoleKey.U, false, false, false));

        Assert.True(player.Mana.CurrentMana > manaBefore);
        Assert.Empty(player.Inventory.Items);
    }

    [Fact]
    public void UseConsumable_RestoresSanity()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        player.Sanity.LoseSanity(30);
        int sanBefore = player.Sanity.CurrentSanity;

        var tonic = new Consumable { Name = "Sanity Tonic", SanityAmount = 20 };
        player.Inventory.AddItem(tonic);
        screen.SetPlayer(player);

        screen.HandleInput(new ConsoleKeyInfo('u', ConsoleKey.U, false, false, false));

        Assert.Equal(sanBefore + 20, player.Sanity.CurrentSanity);
        Assert.Empty(player.Inventory.Items);
    }

    [Fact]
    public void NavigateDown_MovesSelection()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        player.Inventory.AddItem(new Weapon { Name = "Item1" });
        player.Inventory.AddItem(new Weapon { Name = "Item2" });
        screen.SetPlayer(player);

        // Move down, then drop — should drop "Item2" (index 1)
        screen.HandleInput(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        screen.HandleInput(new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false));

        Assert.Single(player.Inventory.Items);
        Assert.Equal("Item1", player.Inventory.Items[0].Name);
    }

    [Fact]
    public void EscapeReturnsClose()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        screen.SetPlayer(player);

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }

    [Fact]
    public void EquipArmor_GoesToCorrectSlot()
    {
        var renderer = new ASCIIRenderer();
        var screen = new InventoryScreen(renderer);
        var player = CreateTestPlayer();
        var armor = new Armor { Name = "Chainmail", ArmorClass = 4, Slot = EquipmentSlot.Body };
        player.Inventory.AddItem(armor);
        screen.SetPlayer(player);

        screen.HandleInput(new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false));

        Assert.Empty(player.Inventory.Items);
        Assert.Equal("Chainmail", player.Equipment.GetEquipped(EquipmentSlot.Body)?.Name);
    }
}
