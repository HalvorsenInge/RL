using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.World;
using RogueSharp;

namespace EldritchDungeon.Systems;

public class AISystem : GameSystem
{
    public override void Update(DungeonMap map)
    {
        if (map.Player == null)
            return;

        foreach (var monster in map.Monsters.ToList())
        {
            UpdateMonster(map, monster);
        }
    }

    private void UpdateMonster(DungeonMap map, Monster monster)
    {
        var fov = new FieldOfView(map);
        fov.ComputeFov(monster.X, monster.Y, GameConstants.DefaultFovRadius, true);

        bool canSeePlayer = fov.IsInFov(map.Player!.X, map.Player.Y);

        if (canSeePlayer)
        {
            MoveTowardPlayer(map, monster);
        }
        // else: idle, do nothing
    }

    private void MoveTowardPlayer(DungeonMap map, Monster monster)
    {
        var player = map.Player!;

        // Already adjacent — stop (no combat in Phase 3)
        if (IsAdjacent(monster.X, monster.Y, player.X, player.Y))
            return;

        try
        {
            var pathFinder = new PathFinder(map, 1.41);
            var path = pathFinder.ShortestPath(
                map.GetCell(monster.X, monster.Y),
                map.GetCell(player.X, player.Y));

            if (path.Length > 1)
            {
                var nextStep = path.StepForward();
                // Skip the first cell (monster's current position)
                if (nextStep.X == monster.X && nextStep.Y == monster.Y)
                {
                    nextStep = path.StepForward();
                }

                // Don't step onto the player
                if (nextStep.X == player.X && nextStep.Y == player.Y)
                    return;

                map.TryMoveActor(monster, nextStep.X, nextStep.Y);
            }
        }
        catch (PathNotFoundException)
        {
            // No path available — stay in place
        }
    }

    private static bool IsAdjacent(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1;
    }
}
