using System;

namespace Console_RPG;

/// <summary>处理主菜单中的场外治疗。</summary>
public static class Heal
{
    /// <summary>
    /// 恢复固定数值的 HP，并确保结果不会超过最大生命值。
    /// </summary>
    public static void Use()
    {
        Console.Clear();

        if (PlayerStatistics.Hp >= PlayerStatistics.MaxHp)
        {
            Console.WriteLine("你的 HP 已经是满的，不需要治疗。");
            Program.Loading();
            return;
        }

        double oldHp = PlayerStatistics.Hp;
        PlayerStatistics.Hp = Math.Min(
            PlayerStatistics.MaxHp,
            PlayerStatistics.Hp + PlayerStatistics.Treatment);

        double recovered = PlayerStatistics.Hp - oldHp;
        Console.WriteLine($"你恢复了 {recovered:0.#} HP。\n当前 HP：{PlayerStatistics.Hp:0.#}/{PlayerStatistics.MaxHp:0.#}");

        Program.Loading();
    }
}
