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

        monster.Health.TakeDamage(totalDamage);

        if (isCrit)
            _log($"CRITICAL HIT! You strike the {monster.Name} for {totalDamage} damage!");
        else
            _log($"You hit the {monster.Name} for {totalDamage} damage.");

        if (monster.Health.IsDead)
        {
            _log($"The {monster.Name} is slain! (+{monster.XpValue} XP)");
            map.Monsters.Remove(monster);
            player.Stats.Experience += monster.XpValue;
        }
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
