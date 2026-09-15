using System;

namespace Console_RPG;

/// <summary>
/// 根据玩家等级生成敌人。
///
/// v0.3 开始让不同怪物拥有不同战斗定位，而不是只有名字和数值变化。
/// 后续可以继续在此基础上加入技能和特殊行为。
/// </summary>
public static class MonsterFactory
{
    public static MonsterStatistics Create(Player player)
    {
        int level = player.Level;
        int type = Random.Shared.Next(1, 7);
        bool isElite = Random.Shared.Next(100) < 10;

        MonsterStatistics monster = type switch
        {
            1 => CreateSlime(level),
            2 => CreateGoblin(level),
            3 => CreateSkeleton(level),
            4 => CreateWolf(level),
            5 => CreateTroll(level),
            6 => CreateMage(level),
            _ => throw new InvalidOperationException("未知怪物类型。")
        };

        if (isElite)
        {
            monster.Name = $"[精英] {monster.Name}";
            monster.Type = "精英 " + monster.Type;
            monster.Level += 2;
            monster.MaxHp *= 1.5;
            monster.Attack *= 1.5;
            monster.ExpReward *= 1.5;
        }

        monster.Hp = monster.MaxHp;
        return monster;
    }

    private static MonsterStatistics CreateSlime(int level) => Create("史莱姆", "肉盾", level, 30 + level * 5, 5 + level, 30 + level * 10);
    private static MonsterStatistics CreateGoblin(int level) => Create("哥布林", "均衡", level, 50 + level * 5, 8 + level * 2, 50 + level * 15);
    private static MonsterStatistics CreateSkeleton(int level) => Create("骷髅兵", "强攻", level + 1, 60 + level * 6, 12 + level * 3, 70 + level * 20);
    private static MonsterStatistics CreateWolf(int level) => Create("野狼", "高速", level, 35 + level * 4, 15 + level * 3, 65 + level * 18);
    private static MonsterStatistics CreateTroll(int level) => Create("巨魔", "高血量", level + 1, 120 + level * 10, 10 + level * 2, 100 + level * 25);
    private static MonsterStatistics CreateMage(int level) => Create("黑暗法师", "特殊", level + 1, 70 + level * 5, 18 + level * 3, 120 + level * 30);

    private static MonsterStatistics Create(string name, string type, int level, double hp, double attack, double exp)
    {
        return new MonsterStatistics
        {
            Name = name,
            Type = type,
            Level = level,
            MaxHp = hp,
            Attack = attack,
            ExpReward = exp
        };
    }
}
