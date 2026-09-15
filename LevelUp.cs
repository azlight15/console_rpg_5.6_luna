using System;

namespace Console_RPG;

/// <summary>负责经验值结算与角色升级。</summary>
public static class UpLevel
{
    /// <summary>增加经验，并处理本次奖励带来的全部升级。</summary>
    public static void GainExp(Player player, double amount)
    {
        if (amount <= 0)
        {
            return;
        }

        player.Exp += amount;
        Console.WriteLine($"获得经验：{amount:0.#}");

        while (player.Exp >= player.ExpToNextLevel)
        {
            LevelUp(player);
        }
    }

    /// <summary>完成一次升级，并把溢出的经验保留到下一级。</summary>
    private static void LevelUp(Player player)
    {
        double requiredExp = player.ExpToNextLevel;
        player.Exp -= requiredExp;
        player.Level++;
        player.ApplyLevelUp();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("🎉 升级！");
        Console.ResetColor();
        Console.WriteLine($"当前等级：{player.Level}");
        Console.WriteLine("最大 HP +20");
        Console.WriteLine("攻击力 +5");
        Console.WriteLine("获得 1 次额外治疗资源");
        Console.WriteLine("HP 已完全恢复");
    }
}
