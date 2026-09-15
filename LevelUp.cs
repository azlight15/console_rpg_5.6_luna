using System;

namespace Console_RPG;

/// <summary>负责经验值结算与角色升级。</summary>
public static class UpLevel
{
    /// <summary>
    /// 增加经验，并处理一次奖励带来的全部升级。
    /// </summary>
    public static void GainExp(double amount)
    {
        if (amount <= 0)
        {
            return;
        }

        PlayerStatistics.Exp += amount;
        Console.WriteLine($"获得经验：{amount:0.#}");

        while (PlayerStatistics.Exp >= PlayerStatistics.ExpToNextLevel)
        {
            LevelUp();
        }
    }

    /// <summary>完成一次升级，并把溢出的经验保留到下一级。</summary>
    private static void LevelUp()
    {
        double requiredExp = PlayerStatistics.ExpToNextLevel;
        PlayerStatistics.Exp -= requiredExp;
        PlayerStatistics.Level++;

        PlayerStatistics.MaxHp += 20;
        PlayerStatistics.Attack += 5;
        PlayerStatistics.Hp = PlayerStatistics.MaxHp;

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("🎉 升级！");
        Console.ResetColor();
        Console.WriteLine($"当前等级：{PlayerStatistics.Level}");
        Console.WriteLine("最大 HP +20");
        Console.WriteLine("攻击力 +5");
        Console.WriteLine("HP 已完全恢复");
    }
}
