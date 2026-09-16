using System;

namespace Console_RPG;

// 经验和升级系统。
// 这里负责判断玩家什么时候升级，以及把升级奖励交给 Player 应用。
public static class UpLevel
{
    // 增加经验，并处理这一次奖励可能带来的全部升级。
    public static void GainExp(Player player, double amount)
    {
        if (amount <= 0)
            return;

        player.Exp += amount;
        Console.WriteLine($"获得经验：{amount:0.#}");

        // 一次奖励可能让玩家连升好几级，所以这里不能只判断一次。
        while (player.Exp >= player.ExpToNextLevel)
        {
            LevelUp(player);
        }
    }

    // 完成一次升级，并把超过升级门槛的经验保留下来。
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
        Console.WriteLine("技能点已补满");
        Console.WriteLine("HP 已完全恢复");
    }
}
