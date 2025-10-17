using Game.Stats;

public static class PlayerStatRouter
{
    public static Stat GetStat(this Player p, StatsType t) => t switch
    {
        StatsType.MaxHealth => p.MaxHealth,
        StatsType.Attack => p.Attack,
        StatsType.Defense => p.Defense,
        StatsType.Persuasion => p.Persuasion,
        _ => p.Attack
    };
}
