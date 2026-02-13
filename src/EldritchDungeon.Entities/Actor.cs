using EldritchDungeon.Entities.Components;

namespace EldritchDungeon.Entities;

public abstract class Actor
{
    public string Name { get; set; } = string.Empty;
    public char Glyph { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public StatsComponent Stats { get; set; } = new();
    public HealthComponent Health { get; set; } = new();
    public ManaComponent Mana { get; set; } = new();
    public SanityComponent Sanity { get; set; } = new();
    public InventoryComponent Inventory { get; set; } = new();
    public EquipmentComponent Equipment { get; set; } = new();
    public StatusEffectsComponent StatusEffects { get; set; } = new();
    public ReligionComponent Religion { get; set; } = new();

    public void InitializeComponents()
    {
        Stats.Initialize();
        Health.Initialize();
        Mana.Initialize();
        Sanity.Initialize();
        Inventory.Initialize();
        Equipment.Initialize();
        StatusEffects.Initialize();
        Religion.Initialize();
    }
}
