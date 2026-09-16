using System;

namespace Console_RPG;

/// <summary>
/// 怪物工厂：负责根据玩家等级创建一只新的怪物。
///
/// 这里集中处理三个问题：
/// 1. 选择怪物种类；
/// 2. 决定本次怪物等级；
/// 3. 根据等级缩放 HP、攻击、经验和金币。
/// Battle 不需要知道“怪物到底怎么生成”，只拿到生成好的结果即可。
/// </summary>
public static class MonsterFactory
{
    public static MonsterStatistics Create(Player player)
    {
        int playerLevel = Math.Max(1, player.Level);

        // 怪物不会永远和玩家同级，而是在玩家等级附近随机浮动，避免每场战斗完全一样。
        int minLevel = Math.Max(1, playerLevel - 2);
        int maxLevel = playerLevel + 2;
        int level = Random.Shared.Next(minLevel, maxLevel + 1);

        MonsterStatistics monster = Random.Shared.Next(1, 11) switch
        {
            1 => Create("史莱姆", "肉盾", level, 30, 5, 30, 12, 0),
            2 => Create("哥布林", "均衡", level, 50, 8, 50, 18, 0.05),
            3 => Create("骷髅兵", "强攻", level + 1, 60, 12, 70, 25, 0),
            4 => Create("野狼", "高速", level, 35, 15, 65, 22, 0.15),
            5 => Create("巨魔", "高血量", level + 1, 120, 10, 100, 35, 0),
            6 => Create("黑暗法师", "特殊", level + 1, 70, 18, 120, 45, 0.1),
            7 => Create("蝙蝠", "高速", level, 28, 13, 55, 20, 0.2),
            8 => Create("兽人", "强攻", level, 75, 16, 85, 30, 0.03),
            9 => Create("冰霜巨兽", "肉盾", level + 2, 140, 14, 150, 50, 0.02),
            10 => Create("毒蛇", "高速", level, 40, 17, 90, 32, 0.12),
            _ => throw new InvalidOperationException("未知怪物类型。")
        };

        // 精英怪不是另一套怪物，而是在普通怪物的基础上强化。
        // 这样既保留普通怪物的职业定位，也能减少重复的怪物模板代码。
        if (Random.Shared.Next(100) < 10)
        {
            monster.Name = $"[精英] {monster.Name}";
            monster.Type = "精英 " + monster.Type;
            monster.Level += 2;
            monster.MaxHp *= 1.5;
            monster.Attack *= 1.5;
            monster.ExpReward *= 1.5;
            monster.GoldReward = (int)Math.Ceiling(monster.GoldReward * 1.5);
            monster.EvasionRate = Math.Min(monster.EvasionRate + 0.05, 0.3);
        }

        // 等级差异最终转换成属性差异。
        // 这样 Lv.10 怪物不会只是名字变了，而是真的比 Lv.1 怪物更强、奖励更多。
        double levelScale = 1 + (monster.Level - 1) * 0.18;
        monster.MaxHp *= levelScale;
        monster.Attack *= 1 + (monster.Level - 1) * 0.12;
        monster.ExpReward *= 1 + (monster.Level - 1) * 0.15;
        monster.GoldReward = (int)Math.Ceiling(monster.GoldReward * (1 + (monster.Level - 1) * 0.10));
        monster.Hp = monster.MaxHp;

        return monster;
    }

    /// <summary>
    /// 创建怪物的基础模板。
    /// baseGold 与 baseExp 类似，代表击败该种怪物的大致经济价值。
    /// </summary>
    private static MonsterStatistics Create(
        string name,
        string type,
        int level,
        double baseHp,
        double baseAttack,
        double baseExp,
        int baseGold,
        double evasion)
    {
        return new MonsterStatistics
        {
            Name = name,
            Type = type,
            Level = level,
            MaxHp = baseHp,
            Attack = baseAttack,
            ExpReward = baseExp,
            GoldReward = baseGold,
            EvasionRate = evasion
        };
    }
}
