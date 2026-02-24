using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.World;
using RogueSharp;

namespace EldritchDungeon.Systems;

public class AISystem : GameSystem
{
    private readonly CombatSystem? _combatSystem;
    private readonly Action<string>? _log;
    private readonly Random _random = new();

    public AISystem() { }

    public AISystem(CombatSystem combatSystem, Action<string>? log = null)
    {
        _combatSystem = combatSystem;
        _log = log;
    }

    public override void Update(DungeonMap map)
    {
        if (map.Player == null)
            return;

        foreach (var monster in map.Monsters.ToList())
        {
            if (StatusEffectSystem.IsIncapacitated(monster))
                continue;

            UpdateMonster(map, monster);
        }
    }

    private void UpdateMonster(DungeonMap map, Monster monster)
    {
        var fov = new FieldOfView(map);
        fov.ComputeFov(monster.X, monster.Y, GameConstants.DefaultFovRadius, true);

        if (monster.IsSummoned)
        {
            switch (monster.Disposition)
            {
                case SummonedDisposition.Friendly:
                    ActFriendly(map, monster, fov);
                    return;
                case SummonedDisposition.Neutral:
                    ActNeutral(map, monster);
                    return;
            }
            // Hostile falls through to normal behavior
        }

        bool canSeePlayer = fov.IsInFov(map.Player!.X, map.Player.Y);
        if (canSeePlayer)
            MoveTowardPlayer(map, monster);
    }

    // Friendly: find nearest enemy monster and attack it; wander if none visible
    private void ActFriendly(DungeonMap map, Monster monster, FieldOfView fov)
    {
        var target = map.Monsters
            .Where(m => m != monster)
            .Where(m => !m.IsSummoned || m.Disposition == SummonedDisposition.Hostile)
            .Where(m => fov.IsInFov(m.X, m.Y))
            .OrderBy(m => Math.Abs(m.X - monster.X) + Math.Abs(m.Y - monster.Y))
            .FirstOrDefault();

        if (target == null)
        {
            ActNeutral(map, monster);
            return;
        }

        if (IsAdjacent(monster.X, monster.Y, target.X, target.Y))
        {
            int dmg = Math.Max(1, monster.Damage);
            target.Health.TakeDamage(dmg);
            _log?.Invoke($"The {monster.Name} attacks the {target.Name} for {dmg} damage!");
            if (target.Health.IsDead)
            {
                _log?.Invoke($"The {target.Name} is destroyed!");
                map.Monsters.Remove(target);
            }
            return;
        }

        MoveTowardCell(map, monster, target.X, target.Y);
    }

    // Neutral: random walk
    private void ActNeutral(DungeonMap map, Monster monster)
    {
        if (_random.Next(3) == 0) return; // 1/3 chance to stay still

        int[] dxs = { -1, 1, 0, 0, -1, -1, 1, 1 };
        int[] dys = { 0, 0, -1, 1, -1, 1, -1, 1 };
        var dirs = Enumerable.Range(0, 8).OrderBy(_ => _random.Next()).ToList();
        foreach (int i in dirs)
        {
            int nx = monster.X + dxs[i];
            int ny = monster.Y + dys[i];
            if (nx < 0 || nx >= map.Width || ny < 0 || ny >= map.Height) continue;
            if (map.IsWalkable(nx, ny) && map.GetMonsterAt(nx, ny) == null)
            {
                map.TryMoveActor(monster, nx, ny);
                break;
            }
        }
    }

    private void MoveTowardPlayer(DungeonMap map, Monster monster)
    {
        var player = map.Player!;

        if (IsAdjacent(monster.X, monster.Y, player.X, player.Y))
        {
            _combatSystem?.MonsterAttack(monster, player);
            return;
        }

        MoveTowardCell(map, monster, player.X, player.Y);
    }

    private void MoveTowardCell(DungeonMap map, Monster monster, int targetX, int targetY)
    {
        try
        {
            var pathFinder = new PathFinder(map, 1.41);
            var path = pathFinder.ShortestPath(
                map.GetCell(monster.X, monster.Y),
                map.GetCell(targetX, targetY));

            if (path.Length > 1)
            {
                var nextStep = path.StepForward();
                if (nextStep.X == monster.X && nextStep.Y == monster.Y)
                    nextStep = path.StepForward();

                if (nextStep.X == targetX && nextStep.Y == targetY)
                    return;

                map.TryMoveActor(monster, nextStep.X, nextStep.Y);
            }
        }
        catch (PathNotFoundException)
        {
            // No path available
        }
    }

    private static bool IsAdjacent(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1;
    }
}
