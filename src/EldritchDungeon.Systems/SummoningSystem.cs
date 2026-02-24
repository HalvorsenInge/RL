using EldritchDungeon.Core;
using EldritchDungeon.Data.Summoning;
using EldritchDungeon.Entities;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems;

public class SummoningSystem
{
    private readonly Action<string> _log;
    private readonly Random _random = new();

    public SummoningSystem(Action<string> log)
    {
        _log = log;
    }

    public void Summon(Player player, DungeonMap map)
    {
        var entry = SummoningDatabase.GetRandom();
        var disposition = RollDisposition(entry);

        // Find a walkable tile near player
        int sx = -1, sy = -1;
        for (int attempt = 0; attempt < 30; attempt++)
        {
            int tx = player.X + _random.Next(-3, 4);
            int ty = player.Y + _random.Next(-3, 4);
            if (tx < 0 || tx >= map.Width || ty < 0 || ty >= map.Height) continue;
            if (!map.IsWalkable(tx, ty)) continue;
            if (map.GetMonsterAt(tx, ty) != null) continue;
            sx = tx; sy = ty;
            break;
        }

        if (sx < 0)
        {
            _log("The chalk circle flares — but there is no room for what you called.");
            return;
        }

        var monster = new Monster
        {
            Name = entry.Name,
            Glyph = entry.Glyph,
            Tier = entry.Tier,
            Damage = entry.Damage,
            XpValue = entry.XpValue,
            SanityDamage = entry.SanityDamage,
            IsSummoned = true,
            Disposition = disposition,
            SummonDurationLeft = entry.SummonDuration,
        };
        monster.Health.MaxHp = entry.HP;
        monster.Health.CurrentHp = entry.HP;

        map.PlaceActor(monster, sx, sy);

        _log(entry.SummonMessage);

        string dispositionMessage = disposition switch
        {
            SummonedDisposition.Hostile  => entry.HostileMessage,
            SummonedDisposition.Neutral  => entry.NeutralMessage,
            SummonedDisposition.Friendly => entry.FriendlyMessage,
            _ => entry.NeutralMessage
        };

        string tag = disposition switch
        {
            SummonedDisposition.Hostile  => "[HOSTILE]",
            SummonedDisposition.Neutral  => "[NEUTRAL]",
            SummonedDisposition.Friendly => "[FRIENDLY]",
            _ => "[NEUTRAL]"
        };

        _log($"{tag} {dispositionMessage}");

        if (entry.SummonDuration > 0)
            _log($"(It will vanish in {entry.SummonDuration} turns.)");
    }

    private SummonedDisposition RollDisposition(SummonableEntry entry)
    {
        int total = entry.HostileWeight + entry.NeutralWeight + entry.FriendlyWeight;
        if (total <= 0) return SummonedDisposition.Neutral;

        int roll = _random.Next(total);

        if (roll < entry.HostileWeight)
            return SummonedDisposition.Hostile;
        if (roll < entry.HostileWeight + entry.NeutralWeight)
            return SummonedDisposition.Neutral;
        return SummonedDisposition.Friendly;
    }
}
