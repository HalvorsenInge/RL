using EldritchDungeon.Core;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Data.Items;

public static class WeaponDatabase
{
    private static readonly Dictionary<string, Weapon> _weapons = new()
    {
        // Medieval Weapons (Tiers 1-3)
        ["Dagger"] = new Weapon
        {
            Name = "Dagger", Damage = 4, CritRangeMin = 18, CritMultiplier = 2,
            Speed = 1, Range = 1, Category = WeaponCategory.Medieval,
            Special = "Backstab +50%"
        },
        ["Shortsword"] = new Weapon
        {
            Name = "Shortsword", Damage = 8, CritRangeMin = 19, CritMultiplier = 2,
            Speed = 2, Range = 1, Category = WeaponCategory.Medieval
        },
        ["Longsword"] = new Weapon
        {
            Name = "Longsword", Damage = 12, CritRangeMin = 19, CritMultiplier = 2,
            Speed = 2, Range = 1, Category = WeaponCategory.Medieval,
            Special = "Parry +1"
        },
        ["Battleaxe"] = new Weapon
        {
            Name = "Battleaxe", Damage = 15, CritRangeMin = 20, CritMultiplier = 3,
            Speed = 3, Range = 1, Category = WeaponCategory.Medieval,
            Special = "Armor pierce 5"
        },
        ["Mace"] = new Weapon
        {
            Name = "Mace", Damage = 10, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 1, Category = WeaponCategory.Medieval,
            Special = "Ignore armor"
        },
        ["Spear"] = new Weapon
        {
            Name = "Spear", Damage = 10, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 2, Category = WeaponCategory.Medieval,
            Special = "Set against charge"
        },
        ["Warhammer"] = new Weapon
        {
            Name = "Warhammer", Damage = 14, CritRangeMin = 20, CritMultiplier = 3,
            Speed = 3, Range = 1, Category = WeaponCategory.Medieval,
            Special = "Concussive"
        },
        ["Halberd"] = new Weapon
        {
            Name = "Halberd", Damage = 14, CritRangeMin = 19, CritMultiplier = 2,
            Speed = 3, Range = 2, Category = WeaponCategory.Medieval,
            Special = "Hook"
        },
        ["Longbow"] = new Weapon
        {
            Name = "Longbow", Damage = 8, CritRangeMin = 20, CritMultiplier = 3,
            Speed = 2, Range = 10, Category = WeaponCategory.Medieval,
            Special = "Volley"
        },
        ["Crossbow"] = new Weapon
        {
            Name = "Crossbow", Damage = 12, CritRangeMin = 19, CritMultiplier = 2,
            Speed = 3, Range = 15, Category = WeaponCategory.Medieval,
            Special = "Precise"
        },

        // Early Modern Weapons (Tiers 3-5)
        ["Flintlock Pistol"] = new Weapon
        {
            Name = "Flintlock Pistol", Damage = 20, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 8, MaxAmmo = 6, CurrentAmmo = 6,
            Category = WeaponCategory.EarlyModern, Special = "Misfire 10%"
        },
        ["Blunderbuss"] = new Weapon
        {
            Name = "Blunderbuss", Damage = 25, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 3, Range = 4, MaxAmmo = 4, CurrentAmmo = 4,
            Category = WeaponCategory.EarlyModern, Special = "Spread (AoE)"
        },
        ["Musket"] = new Weapon
        {
            Name = "Musket", Damage = 22, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 3, Range = 12, MaxAmmo = 1, CurrentAmmo = 1,
            Category = WeaponCategory.EarlyModern, Special = "High damage"
        },
        ["Rapier"] = new Weapon
        {
            Name = "Rapier", Damage = 10, CritRangeMin = 18, CritMultiplier = 3,
            Speed = 1, Range = 1, Category = WeaponCategory.EarlyModern,
            Special = "Very fast"
        },
        ["Bayonet"] = new Weapon
        {
            Name = "Bayonet", Damage = 14, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 1, Category = WeaponCategory.EarlyModern,
            Special = "Attached to musket"
        },
        ["Grenade"] = new Weapon
        {
            Name = "Grenade", Damage = 30, CritRangeMin = 20, CritMultiplier = 1,
            Speed = 2, Range = 5, MaxAmmo = 3, CurrentAmmo = 3,
            Category = WeaponCategory.EarlyModern, Special = "AoE, self-damage"
        },
        ["Pepperbox"] = new Weapon
        {
            Name = "Pepperbox", Damage = 18, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 8, MaxAmmo = 6, CurrentAmmo = 6,
            Category = WeaponCategory.EarlyModern, Special = "Fast reload"
        },
        ["Sword-Cane"] = new Weapon
        {
            Name = "Sword-Cane", Damage = 8, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 1, Category = WeaponCategory.EarlyModern,
            Special = "Hidden, +5 CHA"
        },

        // Dieselpunk Weapons (Tiers 5-7)
        ["Clockwork Pistol"] = new Weapon
        {
            Name = "Clockwork Pistol", Damage = 28, CritRangeMin = 20, CritMultiplier = 3,
            Speed = 2, Range = 10, MaxAmmo = 8, CurrentAmmo = 8,
            Category = WeaponCategory.Dieselpunk, Special = "No misfire"
        },
        ["Steam-Cannon"] = new Weapon
        {
            Name = "Steam-Cannon", Damage = 45, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 3, Range = 6, MaxAmmo = 2, CurrentAmmo = 2,
            Category = WeaponCategory.Dieselpunk, Special = "Slow, stationary"
        },
        ["Alchemical Bomb"] = new Weapon
        {
            Name = "Alchemical Bomb", Damage = 50, CritRangeMin = 20, CritMultiplier = 1,
            Speed = 2, Range = 6, MaxAmmo = 3, CurrentAmmo = 3,
            Category = WeaponCategory.Dieselpunk, Special = "Elemental AoE"
        },
        ["Voltaic Staff"] = new Weapon
        {
            Name = "Voltaic Staff", Damage = 25, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 4, Category = WeaponCategory.Dieselpunk,
            Special = "5 MP/shock"
        },
        ["Gyrojet Rifle"] = new Weapon
        {
            Name = "Gyrojet Rifle", Damage = 35, CritRangeMin = 20, CritMultiplier = 3,
            Speed = 2, Range = 15, MaxAmmo = 5, CurrentAmmo = 5,
            Category = WeaponCategory.Dieselpunk, Special = "Explosive rounds"
        },
        ["Automaton Fist"] = new Weapon
        {
            Name = "Automaton Fist", Damage = 40, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 1, Category = WeaponCategory.Dieselpunk,
            Special = "Arm-mounted"
        },
        ["Chemthrower"] = new Weapon
        {
            Name = "Chemthrower", Damage = 30, CritRangeMin = 20, CritMultiplier = 1,
            Speed = 2, Range = 6, MaxAmmo = 4, CurrentAmmo = 4,
            Category = WeaponCategory.Dieselpunk, Special = "Burning damage"
        },
        ["Thunder Hammer"] = new Weapon
        {
            Name = "Thunder Hammer", Damage = 35, CritRangeMin = 20, CritMultiplier = 3,
            Speed = 3, Range = 2, Category = WeaponCategory.Dieselpunk,
            Special = "Stun chance"
        },

        // Lovecraftian Weapons (Tiers 7-10)
        ["Bone Dagger"] = new Weapon
        {
            Name = "Bone Dagger", Damage = 20, CritRangeMin = 20, CritMultiplier = 3,
            Speed = 1, Range = 1, Category = WeaponCategory.Lovecraftian,
            Special = "+15 sanity drain"
        },
        ["Star-Metal Blade"] = new Weapon
        {
            Name = "Star-Metal Blade", Damage = 35, CritRangeMin = 19, CritMultiplier = 3,
            Speed = 2, Range = 1, Category = WeaponCategory.Lovecraftian,
            Special = "Ignores armor"
        },
        ["Dream Whip"] = new Weapon
        {
            Name = "Dream Whip", Damage = 15, CritRangeMin = 20, CritMultiplier = 4,
            Speed = 2, Range = 3, Category = WeaponCategory.Lovecraftian,
            Special = "+50 sanity damage"
        },
        ["Shoggoth Slime"] = new Weapon
        {
            Name = "Shoggoth Slime", Damage = 30, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 1, Category = WeaponCategory.Lovecraftian,
            Special = "Acid (5 dmg/turn)"
        },
        ["Forbidden Tome"] = new Weapon
        {
            Name = "Forbidden Tome", Damage = 10, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 1, Category = WeaponCategory.Lovecraftian,
            Special = "Cast spells on hit"
        },
        ["Ichor Spear"] = new Weapon
        {
            Name = "Ichor Spear", Damage = 28, CritRangeMin = 20, CritMultiplier = 2,
            Speed = 2, Range = 2, Category = WeaponCategory.Lovecraftian,
            Special = "Poison (10 dmg/turn)"
        },
        ["Star-Visor"] = new Weapon
        {
            Name = "Star-Visor", Damage = 0, CritRangeMin = 20, CritMultiplier = 1,
            Speed = 2, Range = 0, Category = WeaponCategory.Lovecraftian,
            Special = "See invisible, +20 sanity dmg"
        },
        ["Heart of Cthulhu"] = new Weapon
        {
            Name = "Heart of Cthulhu", Damage = 100, CritRangeMin = 20, CritMultiplier = 5,
            Speed = 2, Range = 1, Category = WeaponCategory.Lovecraftian,
            Special = "Legend tier"
        },
    };

    public static Weapon Get(string name) => _weapons[name];

    public static IReadOnlyDictionary<string, Weapon> GetAll() => _weapons;

    public static IEnumerable<Weapon> GetByCategory(WeaponCategory category) =>
        _weapons.Values.Where(w => w.Category == category);
}
