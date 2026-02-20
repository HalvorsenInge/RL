using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems;

public class CombatSystem
{
    private readonly Action<string> _log;

    public CombatSystem(Action<string> log)
    {
        _log = log;
    }

    public void PlayerAttack(Player player, Monster monster, DungeonMap map)
    {
        var weapon = player.Equipment.GetEquipped(EquipmentSlot.MainHand) as Weapon;

        int damage;
        int critRangeMin;
        int critMultiplier;
        string weaponName;

        if (weapon != null)
        {
            damage = weapon.Damage;
            critRangeMin = weapon.CritRangeMin;
            critMultiplier = weapon.CritMultiplier;
            weaponName = weapon.Name;
        }
        else
        {
            damage = 4;
            critRangeMin = 20;
            critMultiplier = 2;
            weaponName = "fists";
        }

        bool isMelee = weapon == null || weapon.Range <= 1;
        int statMod = isMelee
            ? player.Stats.GetModifier(StatType.Strength)
            : player.Stats.GetModifier(StatType.Dexterity);

        int naturalRoll = Dice.Roll(1, 20);
        int attackRoll = naturalRoll + statMod;
        int monsterAC = 10 + monster.Tier;

        if (naturalRoll == 1)
        {
            _log($"You swing your {weaponName} at the {monster.Name} and miss badly!");
            return;
        }

        bool isHit = naturalRoll == 20 || attackRoll >= monsterAC;

        if (!isHit)
        {
            _log($"You attack the {monster.Name} with your {weaponName} but miss. ({attackRoll} vs AC {monsterAC})");
            return;
        }

        bool isCrit = naturalRoll >= critRangeMin;
        int totalDamage = damage + statMod;
        if (isCrit)
            totalDamage *= critMultiplier;
        if (totalDamage < 1)
            totalDamage = 1;

        int magicDamage = weapon?.MagicDamage ?? 0;
        monster.Health.TakeDamage(totalDamage + magicDamage);

        string magicSuffix = magicDamage > 0 ? $" (+{magicDamage} {weapon!.EnchantmentName})" : "";
        if (isCrit)
            _log($"CRITICAL HIT! You strike the {monster.Name} for {totalDamage} damage{magicSuffix}!");
        else
            _log($"You hit the {monster.Name} for {totalDamage} damage{magicSuffix}.");

        if (monster.Health.IsDead)
        {
            _log($"The {monster.Name} is slain! (+{monster.XpValue} XP)");
            map.Monsters.Remove(monster);
            player.Stats.Experience += monster.XpValue;
        }
    }

    /// <summary>
    /// Resolves a ranged attack from <paramref name="player"/> toward the given map cell.
    /// Returns true when a full turn was consumed (hit or miss), false when the action
    /// was rejected before spending any turn (wrong weapon, empty target, etc.).
    /// Ammo is only decremented on a successful attack roll attempt.
    /// </summary>
    public bool ShootRanged(Player player, int targetX, int targetY, DungeonMap map)
    {
        var weapon = player.Equipment.GetEquipped(EquipmentSlot.MainHand) as Weapon;

        if (weapon == null || weapon.Range <= 1)
        {
            _log("You have no ranged weapon equipped.");
            return false;
        }

        if (weapon.MaxAmmo > 0 && weapon.CurrentAmmo <= 0)
        {
            _log($"Your {weapon.Name} is out of ammunition!");
            return false;
        }

        // Range check — Chebyshev distance matches diagonal movement cost
        int dist = Math.Max(Math.Abs(targetX - player.X), Math.Abs(targetY - player.Y));
        if (dist > weapon.Range)
        {
            _log($"That target is out of range! (Range: {weapon.Range}, Distance: {dist})");
            return false;
        }

        if (!map.HasLineOfSight(player.X, player.Y, targetX, targetY))
        {
            _log("You don't have a clear line of sight to that target.");
            return false;
        }

        var monster = map.GetMonsterAt(targetX, targetY);
        if (monster == null)
        {
            _log("There is no target there.");
            return false;
        }

        // Consume ammo before rolling (intentional: misfire still uses a round)
        if (weapon.MaxAmmo > 0)
            weapon.CurrentAmmo--;

        int statMod    = player.Stats.GetModifier(StatType.Dexterity);
        int naturalRoll = Dice.Roll(1, 20);
        int attackRoll  = naturalRoll + statMod;
        int monsterAC   = 10 + monster.Tier;

        if (naturalRoll == 1)
        {
            _log($"You fire your {weapon.Name} at the {monster.Name} and miss badly!");
            return true;
        }

        bool isHit = naturalRoll == 20 || attackRoll >= monsterAC;
        if (!isHit)
        {
            _log($"You fire your {weapon.Name} at the {monster.Name} but miss. ({attackRoll} vs AC {monsterAC})");
            return true;
        }

        bool isCrit       = naturalRoll >= weapon.CritRangeMin;
        int  totalDamage  = weapon.Damage + statMod;
        if (isCrit)
            totalDamage *= weapon.CritMultiplier;
        if (totalDamage < 1)
            totalDamage = 1;

        monster.Health.TakeDamage(totalDamage);

        if (isCrit)
            _log($"CRITICAL HIT! You shoot the {monster.Name} for {totalDamage} damage!");
        else
            _log($"You shoot the {monster.Name} with your {weapon.Name} for {totalDamage} damage.");

        if (monster.Health.IsDead)
        {
            _log($"The {monster.Name} is slain! (+{monster.XpValue} XP)");
            map.Monsters.Remove(monster);
            player.Stats.Experience += monster.XpValue;
        }

        return true;
    }

    public void MonsterAttack(Monster monster, Player player)
    {
        int naturalRoll = Dice.Roll(1, 20);
        int attackRoll = naturalRoll + monster.Tier;

        var armor = player.Equipment.GetEquipped(EquipmentSlot.Body) as Armor;
        int armorAC = armor?.ArmorClass ?? 0;
        int dexMod = player.Stats.GetModifier(StatType.Dexterity);
        int playerAC = 10 + armorAC + dexMod;

        if (naturalRoll == 1)
        {
            _log($"The {monster.Name} attacks you and misses badly!");
            return;
        }

        bool isHit = naturalRoll == 20 || attackRoll >= playerAC;

        if (!isHit)
        {
            _log($"The {monster.Name} attacks you but misses. ({attackRoll} vs AC {playerAC})");
            return;
        }

        int damage = Math.Max(1, monster.Damage);
        player.Health.TakeDamage(damage);
        _log($"The {monster.Name} hits you for {damage} damage!");

        if (monster.SanityDamage > 0)
        {
            player.Sanity.LoseSanity(monster.SanityDamage);
            _log($"The {monster.Name}'s attack shakes your sanity! (-{monster.SanityDamage})");
        }
    }
}
