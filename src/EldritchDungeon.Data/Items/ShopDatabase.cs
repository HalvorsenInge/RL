using EldritchDungeon.Core;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Data.Items;

public static class ShopDatabase
{
    // Weapon pools indexed by shop tier (0=early, 1=mid, 2=late, 3=deep, 4=endgame)
    private static readonly string[][] _weaponPools =
    [
        ["Dagger", "Shortsword", "Longsword", "Mace", "Spear", "Longbow"],
        ["Rapier", "Flintlock Pistol", "Pepperbox", "Bayonet", "Sword-Cane"],
        ["Clockwork Pistol", "Voltaic Staff", "Thunder Hammer", "Gyrojet Rifle"],
        ["Bone Dagger", "Star-Metal Blade", "Ichor Spear", "Dream Whip"],
        ["Forbidden Tome", "Star-Visor", "Heart of Cthulhu"],
    ];

    // (prefix, magic damage, description suffix for Special)
    private static readonly (string Prefix, int MagicDamage, string Suffix)[] _enchantments =
    [
        ("Flaming",    8,  "+8 fire"),
        ("Frozen",     7,  "+7 cold"),
        ("Shocking",  10,  "+10 lightning"),
        ("Venomous",   6,  "+6 poison"),
        ("Arcane",    12,  "+12 arcane"),
        ("Blessed",    9,  "+9 holy"),
        ("Cursed",    15,  "+15 void, -5 SAN"),
    ];

    private static readonly string[] _consumableStock =
    [
        "Healing Potion",
        "Mana Potion",
        "Sanity Potion",
        "Sanctified Water",
        "Elixir of Calm",
    ];

    /// <summary>
    /// Generates shop inventory appropriate for <paramref name="dungeonLevel"/>.
    /// Stock includes regular weapons from the current and next tier, 2-3 enchanted
    /// variants, a few consumables, and optional armor pieces.
    /// </summary>
    public static List<Item> GenerateStock(int dungeonLevel, Random? rng = null)
    {
        rng ??= new Random();
        var stock = new List<Item>();

        // Current weapon tier and one tier ahead (shop sells better stuff)
        int currentTier = Math.Clamp((dungeonLevel - 1) / 2, 0, 4);
        int nextTier    = Math.Min(4, currentTier + 1);

        var currentPool = _weaponPools[currentTier];
        var nextPool    = _weaponPools[nextTier];

        // 2 weapons from current tier
        AddRandomWeapons(stock, currentPool, 2, rng);
        // 2 weapons from next tier (preview of what's ahead)
        AddRandomWeapons(stock, nextPool, 2, rng);

        // 2-3 enchanted weapon variants
        int enchantCount = rng.Next(2, 4);
        for (int i = 0; i < enchantCount; i++)
        {
            string[] pool = rng.Next(2) == 0 ? currentPool : nextPool;
            string baseName = pool[rng.Next(pool.Length)];
            var baseWeapon = WeaponDatabase.Get(baseName);
            var enchanted = CreateEnchanted(baseWeapon, rng);
            stock.Add(enchanted);
        }

        // 2-3 consumables
        int consumableCount = rng.Next(2, 4);
        var consumableNames = _consumableStock.OrderBy(_ => rng.Next()).Take(consumableCount).ToList();
        foreach (var name in consumableNames)
        {
            var c = ConsumableDatabase.Get(name);
            stock.Add(CloneConsumable(c));
        }

        return stock;
    }

    private static void AddRandomWeapons(List<Item> stock, string[] pool, int count, Random rng)
    {
        var picked = pool.OrderBy(_ => rng.Next()).Take(Math.Min(count, pool.Length));
        foreach (var name in picked)
        {
            stock.Add(CloneWeapon(WeaponDatabase.Get(name)));
        }
    }

    private static Weapon CreateEnchanted(Weapon base_, Random rng)
    {
        var (prefix, magicDmg, suffix) = _enchantments[rng.Next(_enchantments.Length)];

        var w = CloneWeapon(base_);
        w.Name = $"{prefix} {base_.Name}";
        w.EnchantmentName = prefix;
        w.MagicDamage = magicDmg;
        w.Special = string.IsNullOrEmpty(base_.Special)
            ? suffix
            : $"{base_.Special} | {suffix}";
        w.Value = base_.Value > 0 ? base_.Value * 4 : 100;

        return w;
    }

    // Deep-copy helpers (databases return shared instances)
    private static Weapon CloneWeapon(Weapon src) => new()
    {
        Name            = src.Name,
        Glyph           = src.Glyph,
        Type            = src.Type,
        Weight          = src.Weight,
        Value           = src.Value > 0 ? src.Value : DefaultWeaponValue(src),
        Damage          = src.Damage,
        CritRangeMin    = src.CritRangeMin,
        CritMultiplier  = src.CritMultiplier,
        Speed           = src.Speed,
        Range           = src.Range,
        MaxAmmo         = src.MaxAmmo,
        CurrentAmmo     = src.CurrentAmmo,
        Category        = src.Category,
        Special         = src.Special,
        MagicDamage     = src.MagicDamage,
        EnchantmentName = src.EnchantmentName,
    };

    private static Consumable CloneConsumable(Consumable src) => new()
    {
        Name         = src.Name,
        Glyph        = src.Glyph,
        Type         = src.Type,
        Weight       = src.Weight,
        Value        = src.Value > 0 ? src.Value : DefaultConsumableValue(src),
        HealAmount   = src.HealAmount,
        ManaAmount   = src.ManaAmount,
        SanityAmount = src.SanityAmount,
        AddictionRisk = src.AddictionRisk,
    };

    private static int DefaultWeaponValue(Weapon w) => w.Damage * 5;

    private static int DefaultConsumableValue(Consumable c)
    {
        int val = c.HealAmount * 2 + c.ManaAmount * 2 + c.SanityAmount * 3;
        return Math.Max(val, 15);
    }
}
