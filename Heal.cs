using System;

namespace Console_RPG;

/// <summary>处理主菜单中的场外治疗。</summary>
public static class Heal
{
    /// <summary>
    /// 消耗一次治疗资源恢复玩家生命值。
    /// 场外治疗与战斗治疗共用同一资源，避免无限免费回血。
    /// </summary>
    public static void Use(Player player)
    {
        Console.Clear();

        if (player.Hp >= player.MaxHp)
        {
            Console.WriteLine("你的 HP 已经是满的，不需要治疗。");
            Program.Loading();
            return;
        }

        if (player.TreatmentCount <= 0)
        {
            Console.WriteLine("你已经没有治疗资源了！");
            Program.Loading();
            return;
        }

        double recovered = player.UseTreatment();
        Console.WriteLine($"你恢复了 {recovered:0.#} HP。\n当前 HP：{player.Hp:0.#}/{player.MaxHp:0.#}");
        Console.WriteLine($"剩余治疗资源：{player.TreatmentCount}");

        Program.Loading();
    }
}
