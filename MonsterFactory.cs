using System;

namespace Console_RPG;

/// <summary>
/// 根据玩家等级生成随机敌人。
/// 怪物的基础模板在这里定义，战斗系统只负责使用生成后的数据。
/// </summary>
public static class MonsterFactory
{
    /// <summary>创建一个新的随机怪物实例。</summary>
    public static MonsterStatistics Create()
    {
        int playerLevel = PlayerStatistics.Level;
        int type = Random.Shared.Next(1, 4);
        bool isElite = Random.Shared.Next(100) < 10;

        MonsterStatistics monster = type switch
        {
            1 => CreateSlime(playerLevel),
            2 => CreateGoblin(playerLevel),
            3 => CreateSkeleton(playerLevel),
            _ => throw new InvalidOperationException("未知的怪物类型。")
        };

        if (isElite)
        {
            monster.Name = $"[精英] {monster.Name}";
            monster.Level += 2;
            monster.MaxHp *= 1.5;
            monster.Attack *= 1.5;
            monster.ExpReward *= 1.5;
        }

        monster.Hp = monster.MaxHp;
        return monster;
    }

    private static MonsterStatistics CreateSlime(int playerLevel)
    {
        int level = Math.Max(1, playerLevel - 1);
        return new MonsterStatistics
        {
            Name = "史莱姆",
            Level = level,
            MaxHp = 30 + level * 4,
            Attack = 5 + level,
            ExpReward = 30 + level * 10
        };
    }

    private static MonsterStatistics CreateGoblin(int playerLevel)
    {
        int level = playerLevel;
        return new MonsterStatistics
        {
            Name = "哥布林",
            Level = level,
            MaxHp = 50 + level * 5,
            Attack = 8 + level * 2,
            ExpReward = 50 + level * 15
        };
    }

    private static MonsterStatistics CreateSkeleton(int playerLevel)
    {
        int level = playerLevel + 1;
        return new MonsterStatistics
        {
            Name = "骷髅兵",
            Level = level,
            MaxHp = 70 + level * 6,
            Attack = 10 + level * 3,
            ExpReward = 70 + level * 20
        };
    }
}
