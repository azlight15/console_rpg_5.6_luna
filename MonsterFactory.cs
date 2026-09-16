using System;

namespace Console_RPG;

// 怪物工厂。
// 所有“生成什么怪物、怪物多少级、怪物有多强”的规则都集中在这里。
// Battle 不需要知道这些细节，只需要调用 Create() 拿到一只已经准备好的怪物。
public static class MonsterFactory
{
    // 根据当前玩家等级随机生成一只怪物。
    public static MonsterStatistics Create(Player player)
    {
        int playerLevel = Math.Max(1, player.Level);

        // 怪物等级在玩家等级上下 2 级内随机变化，所以连续战斗不会完全一样。
        int minLevel = Math.Max(1, playerLevel - 2);
        int maxLevel = playerLevel + 2;
        int level = Random.Shared.Next(minLevel, maxLevel + 1);

        // 先随机选择怪物种类，再套用对应的基础属性模板。
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

        // 10% 的概率把普通怪物变成精英怪。
        // 精英怪仍然保留原本的种类和定位，只是等级、属性和奖励更高。
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

        // 等级差会转化成实际属性差。
        // 例如同样是史莱姆，Lv.10 的史莱姆会明显强于 Lv.1 的史莱姆，奖励也更高。
        double levelScale = 1 + (monster.Level - 1) * 0.18;
        monster.MaxHp *= levelScale;
        monster.Attack *= 1 + (monster.Level - 1) * 0.12;
        monster.ExpReward *= 1 + (monster.Level - 1) * 0.15;
        monster.GoldReward = (int)Math.Ceiling(monster.GoldReward * (1 + (monster.Level - 1) * 0.10));
        monster.Hp = monster.MaxHp;

        return monster;
    }

    // 创建一只怪物的基础模板。
    // 这里填的是“1级左右的基础数值”，真正等级成长在 Create() 里统一计算。
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
