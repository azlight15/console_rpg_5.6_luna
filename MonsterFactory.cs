using System;

namespace Console_RPG;

/// <summary>
/// 根据玩家等级生成敌人。
///
/// Luna v0.3.0 中不同怪物开始拥有不同战斗定位，
/// 为未来技能、特殊行为和 Boss 机制提供基础。
/// </summary>
public static class MonsterFactory
{
    public static MonsterStatistics Create(Player player)
    {
        int level = player.Level;
        MonsterStatistics monster = Random.Shared.Next(1, 7) switch
        {
            1 => Create("史莱姆", "肉盾", level, 30 + level * 5, 5 + level, 30 + level * 10, 0),
            2 => Create("哥布林", "均衡", level, 50 + level * 5, 8 + level * 2, 50 + level * 15, 0.05),
            3 => Create("骷髅兵", "强攻", level + 1, 60 + level * 6, 12 + level * 3, 70 + level * 20, 0),
            4 => Create("野狼", "高速", level, 35 + level * 4, 15 + level * 3, 65 + level * 18, 0.15),
            5 => Create("巨魔", "高血量", level + 1, 120 + level * 10, 10 + level * 2, 100 + level * 25, 0),
            6 => Create("黑暗法师", "特殊", level + 1, 70 + level * 5, 18 + level * 3, 120 + level * 30, 0.1),
            _ => throw new InvalidOperationException("未知怪物类型。")
        };

        if (Random.Shared.Next(100) < 10)
        {
            monster.Name = $"[精英] {monster.Name}";
            monster.Type = "精英 " + monster.Type;
            monster.Level += 2;
            monster.MaxHp *= 1.5;
            monster.Attack *= 1.5;
            monster.ExpReward *= 1.5;
            monster.EvasionRate = Math.Min(monster.EvasionRate + 0.05, 0.3);
        }

        monster.Hp = monster.MaxHp;
        return monster;
    }

    /// <summary>
    /// 创建怪物基础数据。
    /// 所有怪物统一通过该方法初始化，避免重复代码。
    /// </summary>
    private static MonsterStatistics Create(string name, string type, int level, double hp, double attack, double exp, double evasion)
    {
        return new MonsterStatistics
        {
            Name = name,
            Type = type,
            Level = level,
            MaxHp = hp,
            Attack = attack,
            ExpReward = exp,
            EvasionRate = evasion
        };
    }
}
