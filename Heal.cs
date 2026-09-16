using System;

namespace Console_RPG;

// 主菜单里的治疗功能。
// 治疗资源由 Player 统一管理，所以战斗内治疗和战斗外治疗不会各算一套资源。
public static class Heal
{
    // 使用一次治疗资源恢复 HP。
    public static void Use(Player player)
    {
        Console.Clear();

        if (player.Hp >= player.FinalMaxHp)
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
        Console.WriteLine($"你恢复了 {recovered:0.#} HP。\n当前 HP：{player.Hp:0.#}/{player.FinalMaxHp:0.#}");
        Console.WriteLine($"剩余治疗资源：{player.TreatmentCount}");

        Program.Loading();
    }
}
