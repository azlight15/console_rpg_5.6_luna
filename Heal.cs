using System;

namespace Console_RPG;

/// <summary>处理主菜单中的场外治疗。</summary>
public static class Heal
{
    /// <summary>恢复玩家生命值，并确保结果不会超过最大生命值。</summary>
    public static void Use(Player player)
    {
        Console.Clear();

        if (player.Hp >= player.MaxHp)
        {
            Console.WriteLine("你的 HP 已经是满的，不需要治疗。");
            Program.Loading();
            return;
        }

        double oldHp = player.Hp;
        player.Hp = Math.Min(player.MaxHp, player.Hp + player.Treatment);

        double recovered = player.Hp - oldHp;
        Console.WriteLine($"你恢复了 {recovered:0.#} HP。\n当前 HP：{player.Hp:0.#}/{player.MaxHp:0.#}");

        Program.Loading();
    }
}
