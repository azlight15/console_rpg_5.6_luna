using System;

namespace Console_RPG;

/// <summary>
/// 根据玩家等级生成怪物。
/// v0.5.0 不再让所有怪物固定等于玩家等级，而是使用等级区间与怪物定位共同计算属性。
/// </summary>
public static class MonsterFactory
{
    public static MonsterStatistics Create(Player player)
    {
        int playerLevel = Math.Max(1, player.Level);
        int minLevel = Math.Max(1, playerLevel - 2);
        int maxLevel = playerLevel + 2;
        int level = Random.Shared.Next(minLevel, maxLevel + 1);

        MonsterStatistics monster = Random.Shared.Next(1, 11) switch
        {
            1 => Create("史莱姆", "肉盾", level, 30, 5, 30, 0),
            2 => Create("哥布林", "均衡", level, 50, 8, 50, 0.05),
            3 => Create("骷髅兵", "强攻", level + 1, 60, 12, 70, 0),
            4 => Create("野狼", "高速", level, 35, 15, 65, 0.15),
            5 => Create("巨魔", "高血量", level + 1, 120, 10, 100, 0),
            6 => Create("黑暗法师", "特殊", level + 1, 70, 18, 120, 0.1),
            7 => Create("蝙蝠", "高速", level, 28, 13, 55, 0.2),
            8 => Create("兽人", "强攻", level, 75, 16, 85, 0.03),
            9 => Create("冰霜巨兽", "肉盾", level + 2, 140, 14, 150, 0.02),
            10 => Create("毒蛇", "高速", level, 40, 17, 90, 0.12),
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

        // 最终属性按实际生成等级缩放，保证 Lv.10 的怪物不会仍然只有 Lv.1 的基础数值。
        double levelScale = 1 + (monster.Level - 1) * 0.18;
        monster.MaxHp *= levelScale;
        monster.Attack *= 1 + (monster.Level - 1) * 0.12;
        monster.ExpReward *= 1 + (monster.Level - 1) * 0.15;
        monster.Hp = monster.MaxHp;
        return monster;
    }

    private static MonsterStatistics Create(string name, string type, int level, double baseHp, double baseAttack, double baseExp, double evasion)
    {
        return new MonsterStatistics
        {
            Name = name,
            Type = type,
            Level = level,
            MaxHp = baseHp,
            Attack = baseAttack,
            ExpReward = baseExp,
            EvasionRate = evasion
        };
    }
}
