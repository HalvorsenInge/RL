namespace EldritchDungeon.Data.Gods;

public class GodSummonWave
{
    public int AngerThreshold { get; init; }
    /// <summary>Probability per turn that the summon fires when threshold is met.</summary>
    public double TriggerChance { get; init; } = 0.04;
    public string Message { get; init; } = string.Empty;
    /// <summary>List of (monster name, count) pairs to spawn.</summary>
    public List<(string Name, int Count)> Monsters { get; init; } = new();
}
