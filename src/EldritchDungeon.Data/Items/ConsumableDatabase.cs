using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Data.Items;

public static class ConsumableDatabase
{
    private static readonly Dictionary<string, Consumable> _consumables = new()
    {
        // Standard potions
        ["Healing Potion"] = new Consumable
        {
            Name = "Healing Potion", HealAmount = 25
        },
        ["Mana Potion"] = new Consumable
        {
            Name = "Mana Potion", ManaAmount = 25
        },
        ["Sanity Potion"] = new Consumable
        {
            Name = "Sanity Potion", SanityAmount = 30
        },

        // Sanity healing items (with addiction risk)
        ["Sanctified Water"] = new Consumable
        {
            Name = "Sanctified Water", SanityAmount = 30, AddictionRisk = 10
        },
        ["Elixir of Calm"] = new Consumable
        {
            Name = "Elixir of Calm", SanityAmount = 25, AddictionRisk = 10
        },
        ["Mindcrust"] = new Consumable
        {
            Name = "Mindcrust", SanityAmount = 50, AddictionRisk = 30
        },
        ["Deep One Whiskey"] = new Consumable
        {
            Name = "Deep One Whiskey", SanityAmount = 40, AddictionRisk = 25
        },
        ["Cultist Opium"] = new Consumable
        {
            Name = "Cultist Opium", SanityAmount = 60, AddictionRisk = 40
        },
        ["Void Essence"] = new Consumable
        {
            Name = "Void Essence", SanityAmount = 100, AddictionRisk = 50
        },
    };

    public static Consumable Get(string name) => _consumables[name];

    public static bool TryGet(string name, out Consumable consumable) => _consumables.TryGetValue(name, out consumable!);

    public static IReadOnlyDictionary<string, Consumable> GetAll() => _consumables;
}
