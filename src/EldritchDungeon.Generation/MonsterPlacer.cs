using EldritchDungeon.Core;
using EldritchDungeon.Data.Monsters;
using EldritchDungeon.Entities;
using EldritchDungeon.World;

namespace EldritchDungeon.Generation;

public class MonsterPlacer
{
    private readonly Random _random;

    public MonsterPlacer(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public void PlaceMonsters(DungeonMap map, int dungeonLevel)
    {
        var (minTier, maxTier) = LevelTierMapping.GetTierRange(dungeonLevel);
        var candidates = MonsterDatabase.GetByTierGroup(minTier, maxTier).ToList();

        if (candidates.Count == 0)
            return;

        // Skip room 0 (player spawn)
        for (int i = 1; i < map.Rooms.Count; i++)
        {
            int monsterCount = _random.Next(
                GameConstants.MonstersPerRoomMin,
                GameConstants.MonstersPerRoomMax + 1);

            for (int j = 0; j < monsterCount; j++)
            {
                var definition = candidates[_random.Next(candidates.Count)];
                var monster = CreateMonster(definition);

                if (TryPlaceInRoom(map, map.Rooms[i], monster))
                {
                    map.PlaceActor(monster, monster.X, monster.Y);
                }
            }
        }
    }

    private Monster CreateMonster(MonsterDefinition definition)
    {
        var monster = new Monster
        {
            Name = definition.Name,
            Glyph = definition.Glyph,
            Tier = definition.Tier,
            Damage = definition.Damage,
            XpValue = definition.XpValue,
            SanityDamage = definition.SanityDamage
        };

        monster.Health.MaxHp = definition.HP;
        monster.Health.CurrentHp = definition.HP;

        return monster;
    }

    private bool TryPlaceInRoom(DungeonMap map, Room room, Monster monster)
    {
        const int maxAttempts = 20;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int x = _random.Next(room.X, room.X + room.Width);
            int y = _random.Next(room.Y, room.Y + room.Height);

            if (map.IsWalkable(x, y))
            {
                monster.X = x;
                monster.Y = y;
                return true;
            }
        }

        return false;
    }
}
