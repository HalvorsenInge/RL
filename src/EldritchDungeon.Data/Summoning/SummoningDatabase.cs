namespace EldritchDungeon.Data.Summoning;

public static class SummoningDatabase
{
    public static readonly List<SummonableEntry> Entries = new()
    {
        // ── Mundane Objects (the absurd tier) ────────────────────────────────

        new SummonableEntry
        {
            Name = "Angry Fork", Glyph = 'f', HP = 4, Damage = 2, Tier = 1,
            HostileWeight = 8, NeutralWeight = 2, FriendlyWeight = 0,
            SummonMessage = "The chalk lines glow. A fork materializes from the void.",
            HostileMessage = "It hates you. Deeply, personally, and specifically you.",
            NeutralMessage = "It spins in place, simmering with unspent rage.",
        },
        new SummonableEntry
        {
            Name = "Resentful Shoe", Glyph = 's', HP = 6, Damage = 3, Tier = 1,
            HostileWeight = 5, NeutralWeight = 5, FriendlyWeight = 1,
            SummonMessage = "The chalk burns cold. A shoe drops from nowhere.",
            HostileMessage = "The shoe holds a grudge you cannot explain and cannot escape.",
            NeutralMessage = "The shoe regards your feet with deep suspicion.",
            FriendlyMessage = "Against all reason, the shoe has decided to protect you.",
        },
        new SummonableEntry
        {
            Name = "Disappointed Spoon", Glyph = 'o', HP = 3, Damage = 1, Tier = 1,
            HostileWeight = 1, NeutralWeight = 8, FriendlyWeight = 2,
            SummonMessage = "Reality shudders. A spoon appears.",
            HostileMessage = "The spoon sighs heavily and moves to attack. It expected more from you.",
            NeutralMessage = "The spoon is present. It expected this to go differently.",
            FriendlyMessage = "The spoon is disappointed in your enemies and will fight them.",
        },
        new SummonableEntry
        {
            Name = "Bitter Cabbage", Glyph = 'C', HP = 4, Damage = 2, Tier = 1,
            HostileWeight = 2, NeutralWeight = 8, FriendlyWeight = 1,
            SummonMessage = "Eldritch energies coalesce into... a cabbage.",
            HostileMessage = "The cabbage is absolutely furious. The leaves are sharp.",
            NeutralMessage = "The cabbage sits here, silently judging everything.",
            FriendlyMessage = "The cabbage nods at you. You feel strangely validated.",
        },
        new SummonableEntry
        {
            Name = "Vengeful Boot", Glyph = 'b', HP = 8, Damage = 4, Tier = 1,
            HostileWeight = 9, NeutralWeight = 1, FriendlyWeight = 0,
            SummonMessage = "Something falls through the chalk circle. It is a boot.",
            HostileMessage = "The boot has decided you are the enemy. It advances.",
            NeutralMessage = "The boot paces back and forth, working something out.",
        },
        new SummonableEntry
        {
            Name = "Confused Hat", Glyph = 'H', HP = 3, Damage = 1, Tier = 1,
            HostileWeight = 1, NeutralWeight = 9, FriendlyWeight = 2,
            SummonMessage = "A hat drifts through the portal, slowly rotating.",
            HostileMessage = "The hat attacks! It is confused but committed.",
            NeutralMessage = "The hat isn't sure where it is. Neither are you.",
            FriendlyMessage = "The hat settles on your side. It will fight for you, confusedly.",
        },
        new SummonableEntry
        {
            Name = "Unreasonably Angry Sock", Glyph = '(', HP = 2, Damage = 2, Tier = 1,
            HostileWeight = 10, NeutralWeight = 0, FriendlyWeight = 0,
            SummonMessage = "A sock rockets out of the portal at alarming velocity.",
            HostileMessage = "It is unreasonably angry. It is coming for you.",
        },
        new SummonableEntry
        {
            Name = "Passive-Aggressive Broom", Glyph = '/', HP = 7, Damage = 3, Tier = 1,
            HostileWeight = 2, NeutralWeight = 5, FriendlyWeight = 4,
            SummonMessage = "A broom sweeps through the portal, tidying as it comes.",
            HostileMessage = "The broom sweeps toward you with pointed passive aggression.",
            NeutralMessage = "The broom begins cleaning the dungeon floor. It is making a point.",
            FriendlyMessage = "The broom has allied with you. It will sweep your enemies away. Literally.",
        },
        new SummonableEntry
        {
            Name = "Philosophical Chair", Glyph = 'T', HP = 12, Damage = 5, Tier = 1,
            HostileWeight = 1, NeutralWeight = 9, FriendlyWeight = 1,
            SummonMessage = "A chair materializes. It sits there, contemplating existence.",
            HostileMessage = "The chair questions the nature of violence by committing it.",
            NeutralMessage = "The chair considers the dungeon and finds it lacking.",
            FriendlyMessage = "The chair stands with you. Philosophically.",
        },
        new SummonableEntry
        {
            Name = "A Loaf of Bread", Glyph = '%', HP = 5, Damage = 1, Tier = 1,
            HostileWeight = 1, NeutralWeight = 7, FriendlyWeight = 4,
            SummonDuration = 15,
            SummonMessage = "The summoning circle sputters. A loaf of bread materializes.",
            HostileMessage = "Somehow the bread is trying to fight you. A crust bounces off your shin.",
            NeutralMessage = "It sits there. Smelling faintly of wheat.",
            FriendlyMessage = "The bread is on your side. For all the good that does.",
        },
        new SummonableEntry
        {
            Name = "Rogue Spoon", Glyph = 'O', HP = 2, Damage = 3, Tier = 1,
            HostileWeight = 7, NeutralWeight = 2, FriendlyWeight = 1,
            SummonMessage = "A spoon tears through the chalk circle with murderous intent.",
            HostileMessage = "It has gone rogue. It has always been rogue. It comes for you.",
            NeutralMessage = "The rogue spoon eyes you and decides you aren't worth it. Today.",
            FriendlyMessage = "The rogue spoon turns its murderous intent outward. Useful.",
        },

        // ── Small Creatures ──────────────────────────────────────────────────

        new SummonableEntry
        {
            Name = "Spectral Rat", Glyph = 'r', HP = 5, Damage = 2, Tier = 1, XpValue = 5,
            HostileWeight = 4, NeutralWeight = 4, FriendlyWeight = 2,
            SummonMessage = "A translucent rat scurries through the portal.",
            HostileMessage = "The rat has decided you are food.",
            NeutralMessage = "The rat looks for something to gnaw on. Not you. Probably.",
            FriendlyMessage = "The rat sits on your shoulder. It will warn you of danger. In its way.",
        },
        new SummonableEntry
        {
            Name = "Ethereal Cat", Glyph = 'q', HP = 8, Damage = 3, Tier = 1, XpValue = 10,
            HostileWeight = 2, NeutralWeight = 7, FriendlyWeight = 3,
            SummonMessage = "A ghostly cat steps through the portal and immediately looks away.",
            HostileMessage = "The cat has decided you are interesting. That is bad for you.",
            NeutralMessage = "The cat acknowledges neither you nor your enemies. It is a cat.",
            FriendlyMessage = "The cat is helping. In a cat way. You don't fully understand how.",
        },
        new SummonableEntry
        {
            Name = "Void Imp", Glyph = 'i', HP = 10, Damage = 5, Tier = 2, XpValue = 20,
            SanityDamage = 1,
            HostileWeight = 6, NeutralWeight = 2, FriendlyWeight = 2,
            SummonMessage = "Something small and wrong scuttles through the portal.",
            HostileMessage = "It turns its empty eyes on you and begins to advance.",
            NeutralMessage = "It ignores you. Perhaps it is waiting for the right moment.",
            FriendlyMessage = "The imp bows. You have its service. You are unsure if this is good.",
        },
        new SummonableEntry
        {
            Name = "Ghost Butler", Glyph = 'G', HP = 15, Damage = 3, Tier = 2, XpValue = 25,
            HostileWeight = 0, NeutralWeight = 2, FriendlyWeight = 8,
            SummonMessage = "A distinguished ghost in formal wear steps through the portal.",
            NeutralMessage = "The butler awaits further instruction. Unfortunately you cannot instruct it.",
            FriendlyMessage = "The butler bows and turns toward your enemies. 'Most disagreeable,' it says.",
        },
        new SummonableEntry
        {
            Name = "Helpful Skeleton", Glyph = 'S', HP = 12, Damage = 5, Tier = 2, XpValue = 20,
            HostileWeight = 0, NeutralWeight = 1, FriendlyWeight = 9,
            SummonMessage = "A skeleton clatters through the portal and gives you a thumbs up.",
            NeutralMessage = "The skeleton mills about. It wants to help but lacks direction.",
            FriendlyMessage = "The skeleton grins and advances on your enemies. It is very helpful.",
        },

        // ── Eldritch Entities ────────────────────────────────────────────────

        new SummonableEntry
        {
            Name = "Void Tendril", Glyph = 't', HP = 15, Damage = 8, Tier = 3, XpValue = 40,
            SanityDamage = 2,
            HostileWeight = 7, NeutralWeight = 2, FriendlyWeight = 1,
            SummonMessage = "A writhing appendage tears through the chalk circle from outside reality.",
            HostileMessage = "It turns toward you. It is hungry in a way that has no name.",
            NeutralMessage = "The tendril writhes in place, tasting the air. Waiting.",
            FriendlyMessage = "The tendril coils around your arm briefly, then seeks your enemies.",
        },
        new SummonableEntry
        {
            Name = "Tiny Shoggoth", Glyph = 'S', HP = 20, Damage = 7, Tier = 3, XpValue = 50,
            SanityDamage = 2,
            HostileWeight = 8, NeutralWeight = 2, FriendlyWeight = 0,
            SummonMessage = "A fist-sized amorphous blob of screaming flesh oozes through the portal.",
            HostileMessage = "It screams continuously and flows toward you.",
            NeutralMessage = "It screams continuously but doesn't seem to have chosen a direction yet.",
        },
        new SummonableEntry
        {
            Name = "Dream-Eater", Glyph = 'D', HP = 25, Damage = 9, Tier = 4, XpValue = 70,
            SanityDamage = 3,
            HostileWeight = 8, NeutralWeight = 2, FriendlyWeight = 0,
            SummonMessage = "Something that feeds on conscious thought drifts through the portal.",
            HostileMessage = "It turns its attention toward the largest mind it sees. That is yours.",
            NeutralMessage = "It drifts, tasting ambient thoughts. Yours taste like fear.",
        },
        new SummonableEntry
        {
            Name = "Spectral Hound", Glyph = 'h', HP = 18, Damage = 8, Tier = 3, XpValue = 45,
            HostileWeight = 4, NeutralWeight = 2, FriendlyWeight = 4,
            SummonMessage = "A ghostly hound phases through the portal, nose working furiously.",
            HostileMessage = "It has caught your scent. It does not approve.",
            NeutralMessage = "The hound circles, establishing a perimeter. For whom is unclear.",
            FriendlyMessage = "The hound sits at your heel and bares its spectral teeth at your enemies.",
        },
        new SummonableEntry
        {
            Name = "Elder Fish Thing", Glyph = 'F', HP = 20, Damage = 8, Tier = 3, XpValue = 55,
            SanityDamage = 2,
            HostileWeight = 7, NeutralWeight = 3, FriendlyWeight = 0,
            SummonMessage = "Something that should not be anywhere near this dungeon flops through the portal.",
            HostileMessage = "It gurgles what might be words. None of them are friendly.",
            NeutralMessage = "It gurgles. It circles. Its eyes contain the depths of the sea.",
        },
        new SummonableEntry
        {
            Name = "Screaming Void", Glyph = 'V', HP = 30, Damage = 12, Tier = 5, XpValue = 100,
            SanityDamage = 5,
            HostileWeight = 10, NeutralWeight = 0, FriendlyWeight = 0,
            SummonMessage = "Pure darkness tears through the portal. It screams. It is always screaming.",
            HostileMessage = "It screams. It comes for you. The screaming gets louder.",
        },
        new SummonableEntry
        {
            Name = "Spectral Knight", Glyph = 'K', HP = 35, Damage = 14, Tier = 4, XpValue = 90,
            HostileWeight = 3, NeutralWeight = 2, FriendlyWeight = 5,
            SummonMessage = "An armored ghost steps through the portal. Its visor is down. You cannot read it.",
            HostileMessage = "The knight levels its ghostly blade at you. It has chosen.",
            NeutralMessage = "The knight stands at attention. It awaits a worthy cause.",
            FriendlyMessage = "The knight salutes. It will defend your honor with its unlife.",
        },
    };

    private static readonly Random _random = new();

    public static SummonableEntry GetRandom()
    {
        return Entries[_random.Next(Entries.Count)];
    }
}
