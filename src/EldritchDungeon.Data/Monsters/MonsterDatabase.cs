namespace EldritchDungeon.Data.Monsters;

public static class MonsterDatabase
{
    private static readonly Dictionary<string, MonsterDefinition> _monsters = new()
    {
        // Tier 1-2 (Dungeon Levels 1-3)
        ["Rat"] = new MonsterDefinition
        {
            Name = "Rat", Glyph = 'r', Tier = 1,
            HP = 5, Damage = 2, SanityDamage = 0, XpValue = 10,
            Abilities = new List<string> { "Swarm (5+)" }
        },
        ["Giant Bat"] = new MonsterDefinition
        {
            Name = "Giant Bat", Glyph = 'B', Tier = 1,
            HP = 8, Damage = 3, SanityDamage = 0, XpValue = 15,
            Abilities = new List<string> { "Flying" }
        },
        ["Goblin"] = new MonsterDefinition
        {
            Name = "Goblin", Glyph = 'g', Tier = 1,
            HP = 15, Damage = 5, SanityDamage = 0, XpValue = 25,
            Abilities = new List<string> { "Numbers" }
        },
        ["Kobold"] = new MonsterDefinition
        {
            Name = "Kobold", Glyph = 'k', Tier = 1,
            HP = 12, Damage = 4, SanityDamage = 0, XpValue = 20,
            Abilities = new List<string> { "Traps" }
        },
        ["Skeleton"] = new MonsterDefinition
        {
            Name = "Skeleton", Glyph = 's', Tier = 2,
            HP = 20, Damage = 8, SanityDamage = 2, XpValue = 35,
            Abilities = new List<string> { "Undead" }
        },
        ["Zombie"] = new MonsterDefinition
        {
            Name = "Zombie", Glyph = 'z', Tier = 2,
            HP = 25, Damage = 6, SanityDamage = 1, XpValue = 30,
            Abilities = new List<string> { "Slow", "Rot" }
        },
        ["Giant Spider"] = new MonsterDefinition
        {
            Name = "Giant Spider", Glyph = 'S', Tier = 2,
            HP = 18, Damage = 5, SanityDamage = 3, XpValue = 40,
            Abilities = new List<string> { "Poison" }
        },
        ["Cave Spider"] = new MonsterDefinition
        {
            Name = "Cave Spider", Glyph = 'a', Tier = 2,
            HP = 12, Damage = 3, SanityDamage = 2, XpValue = 30,
            Abilities = new List<string> { "Web" }
        },

        // Tier 3-4 (Dungeon Levels 4-7)
        ["Orc"] = new MonsterDefinition
        {
            Name = "Orc", Glyph = 'o', Tier = 3,
            HP = 35, Damage = 12, SanityDamage = 0, XpValue = 75,
            Abilities = new List<string> { "Berserk" }
        },
        ["Dark Elf"] = new MonsterDefinition
        {
            Name = "Dark Elf", Glyph = 'e', Tier = 3,
            HP = 30, Damage = 14, SanityDamage = 2, XpValue = 80,
            Abilities = new List<string> { "Magic (-10 HP)" }
        },
        ["Ghoul"] = new MonsterDefinition
        {
            Name = "Ghoul", Glyph = 'G', Tier = 3,
            HP = 40, Damage = 10, SanityDamage = 5, XpValue = 90,
            Abilities = new List<string> { "Paralysis" }
        },
        ["Ogre"] = new MonsterDefinition
        {
            Name = "Ogre", Glyph = 'O', Tier = 4,
            HP = 60, Damage = 18, SanityDamage = 3, XpValue = 100,
            Abilities = new List<string> { "Throw rocks" }
        },
        ["Dark Cultist"] = new MonsterDefinition
        {
            Name = "Dark Cultist", Glyph = 'c', Tier = 4,
            HP = 35, Damage = 10, SanityDamage = 8, XpValue = 100,
            Abilities = new List<string> { "Summon" }
        },
        ["Worm"] = new MonsterDefinition
        {
            Name = "Worm", Glyph = 'w', Tier = 3,
            HP = 25, Damage = 15, SanityDamage = 4, XpValue = 85,
            Abilities = new List<string> { "Burrow" }
        },
        ["Fire Vampire"] = new MonsterDefinition
        {
            Name = "Fire Vampire", Glyph = 'V', Tier = 4,
            HP = 25, Damage = 15, SanityDamage = 5, XpValue = 95,
            Abilities = new List<string> { "Life drain" }
        },
        ["Shadow"] = new MonsterDefinition
        {
            Name = "Shadow", Glyph = 'h', Tier = 4,
            HP = 20, Damage = 8, SanityDamage = 10, XpValue = 110,
            Abilities = new List<string> { "Invisibility" }
        },

        // Tier 5-6 (Dungeon Levels 8-12)
        ["Deep One"] = new MonsterDefinition
        {
            Name = "Deep One", Glyph = 'D', Tier = 5,
            HP = 50, Damage = 18, SanityDamage = 15, XpValue = 200,
            Abilities = new List<string> { "Amphibious", "Devolve" }
        },
        ["Mi-Go"] = new MonsterDefinition
        {
            Name = "Mi-Go", Glyph = 'M', Tier = 5,
            HP = 55, Damage = 20, SanityDamage = 20, XpValue = 220,
            Abilities = new List<string> { "Flying", "Brain harvest" }
        },
        ["Ghast"] = new MonsterDefinition
        {
            Name = "Ghast", Glyph = 'g', Tier = 5,
            HP = 45, Damage = 22, SanityDamage = 12, XpValue = 200,
            Abilities = new List<string> { "Stench", "Rot" }
        },
        ["Wight"] = new MonsterDefinition
        {
            Name = "Wight", Glyph = 'W', Tier = 6,
            HP = 60, Damage = 15, SanityDamage = 15, XpValue = 210,
            Abilities = new List<string> { "Level drain" }
        },
        ["Phantom"] = new MonsterDefinition
        {
            Name = "Phantom", Glyph = 'P', Tier = 6,
            HP = 35, Damage = 12, SanityDamage = 25, XpValue = 230,
            Abilities = new List<string> { "Possession" }
        },
        ["Byakhee"] = new MonsterDefinition
        {
            Name = "Byakhee", Glyph = 'b', Tier = 5,
            HP = 50, Damage = 18, SanityDamage = 10, XpValue = 200,
            Abilities = new List<string> { "Flying", "Poison" }
        },
        ["Hunting Horror"] = new MonsterDefinition
        {
            Name = "Hunting Horror", Glyph = 'H', Tier = 6,
            HP = 40, Damage = 25, SanityDamage = 30, XpValue = 250,
            Abilities = new List<string> { "Constrict" }
        },
        ["Star-Spawn (Lesser)"] = new MonsterDefinition
        {
            Name = "Star-Spawn (Lesser)", Glyph = '*', Tier = 6,
            HP = 70, Damage = 25, SanityDamage = 20, XpValue = 250,
            Abilities = new List<string> { "Mythos magic" }
        },

        // Tier 7-8 (Dungeon Levels 13-18)
        ["Elder Thing"] = new MonsterDefinition
        {
            Name = "Elder Thing", Glyph = 'E', Tier = 7,
            HP = 80, Damage = 30, SanityDamage = 25, XpValue = 400,
            Abilities = new List<string> { "Sonic attack" }
        },
        ["Shoggeth"] = new MonsterDefinition
        {
            Name = "Shoggeth", Glyph = 'S', Tier = 7,
            HP = 100, Damage = 35, SanityDamage = 35, XpValue = 450,
            Abilities = new List<string> { "Regeneration", "Acid" }
        },
        ["High Cultist"] = new MonsterDefinition
        {
            Name = "High Cultist", Glyph = 'C', Tier = 7,
            HP = 70, Damage = 25, SanityDamage = 30, XpValue = 380,
            Abilities = new List<string> { "Summon", "Sanity blast" }
        },
        ["Chthonian"] = new MonsterDefinition
        {
            Name = "Chthonian", Glyph = 'c', Tier = 8,
            HP = 90, Damage = 28, SanityDamage = 20, XpValue = 400,
            Abilities = new List<string> { "Burrow", "Ambush" }
        },
        ["Fire Elemental"] = new MonsterDefinition
        {
            Name = "Fire Elemental", Glyph = 'F', Tier = 7,
            HP = 70, Damage = 35, SanityDamage = 5, XpValue = 350,
            Abilities = new List<string> { "Fire immunity" }
        },
        ["Vampire Lord"] = new MonsterDefinition
        {
            Name = "Vampire Lord", Glyph = 'V', Tier = 8,
            HP = 120, Damage = 30, SanityDamage = 25, XpValue = 450,
            Abilities = new List<string> { "Domination" }
        },
        ["Flying Polyp"] = new MonsterDefinition
        {
            Name = "Flying Polyp", Glyph = 'P', Tier = 8,
            HP = 80, Damage = 25, SanityDamage = 40, XpValue = 480,
            Abilities = new List<string> { "Telekinesis" }
        },
        ["Serpent of N'kai"] = new MonsterDefinition
        {
            Name = "Serpent of N'kai", Glyph = 'N', Tier = 8,
            HP = 95, Damage = 32, SanityDamage = 35, XpValue = 500,
            Abilities = new List<string> { "Hypnosis" }
        },

        // Tier 9-10 (Dungeon Levels 19-25)
        ["High Priest"] = new MonsterDefinition
        {
            Name = "High Priest", Glyph = 'P', Tier = 9,
            HP = 150, Damage = 45, SanityDamage = 50, XpValue = 1000,
            Abilities = new List<string> { "Summon", "Miracles" }
        },
        ["Star-Spawn (Greater)"] = new MonsterDefinition
        {
            Name = "Star-Spawn (Greater)", Glyph = '*', Tier = 9,
            HP = 180, Damage = 55, SanityDamage = 60, XpValue = 1200,
            Abilities = new List<string> { "Reality warp" }
        },
        ["Dagon's Avatar"] = new MonsterDefinition
        {
            Name = "Dagon's Avatar", Glyph = 'D', Tier = 9,
            HP = 200, Damage = 50, SanityDamage = 55, XpValue = 1300,
            Abilities = new List<string> { "Water control" }
        },
        ["Hastur's Avatar"] = new MonsterDefinition
        {
            Name = "Hastur's Avatar", Glyph = 'H', Tier = 9,
            HP = 220, Damage = 60, SanityDamage = 70, XpValue = 1400,
            Abilities = new List<string> { "Yellow Sign" }
        },
        ["Nyarlathotep's Avatar"] = new MonsterDefinition
        {
            Name = "Nyarlathotep's Avatar", Glyph = 'N', Tier = 10,
            HP = 250, Damage = 65, SanityDamage = 75, XpValue = 1500,
            Abilities = new List<string> { "Chaos magic" }
        },
        ["Shuggoth (Elder)"] = new MonsterDefinition
        {
            Name = "Shuggoth (Elder)", Glyph = 'S', Tier = 10,
            HP = 300, Damage = 70, SanityDamage = 80, XpValue = 1800,
            Abilities = new List<string> { "Hive mind" }
        },
        ["Great Old One"] = new MonsterDefinition
        {
            Name = "Great Old One", Glyph = 'G', Tier = 10,
            HP = 500, Damage = 100, SanityDamage = 100, XpValue = 5000,
            Abilities = new List<string> { "Reality destruction" }
        },
        ["Cthulhu"] = new MonsterDefinition
        {
            Name = "Cthulhu", Glyph = 'C', Tier = 10,
            HP = 666, Damage = 150, SanityDamage = 200, XpValue = 10000,
            Abilities = new List<string> { "Dream revival" }
        },
    };

    public static MonsterDefinition Get(string name) => _monsters[name];

    public static IReadOnlyDictionary<string, MonsterDefinition> GetAll() => _monsters;

    public static IEnumerable<MonsterDefinition> GetByTier(int tier) =>
        _monsters.Values.Where(m => m.Tier == tier);

    public static IEnumerable<MonsterDefinition> GetByTierGroup(int minTier, int maxTier) =>
        _monsters.Values.Where(m => m.Tier >= minTier && m.Tier <= maxTier);
}
