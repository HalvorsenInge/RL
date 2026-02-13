using EldritchDungeon.Core;

namespace EldritchDungeon.Entities;

public class Player : Actor
{
    public RaceType Race { get; set; }
    public ClassType Class { get; set; }
    public int Gold { get; set; }
    public int AddictionLevel { get; set; }

    public Player()
    {
        Glyph = GameConstants.PlayerGlyph;
    }

    /// <summary>
    /// Creates and initializes a player with rolled stats and racial/class data.
    /// </summary>
    /// <param name="name">Character name</param>
    /// <param name="race">Race type</param>
    /// <param name="classType">Class type</param>
    /// <param name="racialModifiers">STR, DEX, CON, INT, WIS, CHA modifiers</param>
    /// <param name="hpMultiplier">Racial HP multiplier</param>
    /// <param name="manaMultiplier">Racial Mana multiplier</param>
    /// <param name="sanityMultiplier">Racial Sanity multiplier</param>
    /// <param name="baseHp">Class base HP</param>
    /// <param name="baseMana">Class base Mana</param>
    /// <param name="startingGold">Class starting gold</param>
    public static Player CreateCharacter(
        string name,
        RaceType race,
        ClassType classType,
        int[] racialModifiers,
        double hpMultiplier,
        double manaMultiplier,
        double sanityMultiplier,
        int baseHp,
        int baseMana,
        int startingGold)
    {
        var player = new Player
        {
            Name = name,
            Race = race,
            Class = classType,
            Gold = startingGold
        };

        player.InitializeComponents();

        // Roll and apply stats
        var scores = Dice.RollAbilityScores();
        player.Stats.SetBaseScores(scores);
        player.Stats.ApplyRacialModifiers(
            racialModifiers[0], racialModifiers[1], racialModifiers[2],
            racialModifiers[3], racialModifiers[4], racialModifiers[5]);

        // Set resource pools
        int conMod = player.Stats.GetModifier(StatType.Constitution);
        int intMod = player.Stats.GetModifier(StatType.Intelligence);
        int wisMod = player.Stats.GetModifier(StatType.Wisdom);

        player.Health.SetMaxHp(baseHp, conMod, hpMultiplier);
        player.Mana.SetMaxMana(baseMana, intMod, manaMultiplier);
        player.Sanity.SetMaxSanity(wisMod, sanityMultiplier);

        return player;
    }
}
