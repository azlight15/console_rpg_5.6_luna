using System;

namespace Console_RPG;

/// <summary>
/// 经验与升级系统。
///
/// 这个类只处理“经验够不够”和“升级时调用什么”。
/// 真正的属性增长规则放在 Player.ApplyLevelUp()，避免升级规则散落在多个类里。
/// </summary>
public static class UpLevel
{
    /// <summary>
    /// 增加经验，并处理本次奖励可能触发的全部升级。
    ///
    /// 使用 while 而不是 if，是因为一次大额经验奖励可能连续升很多级。
    /// 例如当前只差 20 EXP，但一次获得 300 EXP，就应该连续处理多次升级。
    /// </summary>
    public static void GainExp(Player player, double amount)
    {
        if (amount <= 0)
            return;

        player.Exp += amount;
        Console.WriteLine($"获得经验：{amount:0.#}");

        // 每次循环只升一级，并保留溢出的经验给下一级继续计算。
        while (player.Exp >= player.ExpToNextLevel)
        {
            LevelUp(player);
        }
    }

    /// <summary>
    /// 完成一次升级。
    /// 这里先记录旧等级所需经验，再扣除经验并提升等级。
    /// 这样升级所需经验来自“升级前等级”，不会因为 Level++ 导致计算错位。
    /// </summary>
    private static void LevelUp(Player player)
    {
        double requiredExp = player.ExpToNextLevel;
        player.Exp -= requiredExp;
        player.Level++;

        // Player 统一处理 HP、攻击、治疗资源和技能点的成长。
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
