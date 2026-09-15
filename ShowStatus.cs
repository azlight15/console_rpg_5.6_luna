using System;

namespace Console_RPG;

/// <summary>负责展示玩家当前的角色数据。</summary>
public static class ShowStatus
{
    /// <summary>显示等级、经验、生命值、攻击力和治疗资源。</summary>
    public static void Display(Player player)
    {
        Console.Clear();
        Console.WriteLine("========== 角色状态 ==========");
        Console.WriteLine($"名字：{player.Name}");
        Console.WriteLine($"等级：{player.Level}");
        Console.WriteLine($"经验：{player.Exp:0.#}/{player.ExpToNextLevel:0.#}");
        Console.WriteLine($"HP：{player.Hp:0.#}/{player.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{player.Attack:0.#}");
        Console.WriteLine($"单次治疗量：{player.Treatment:0.#}");
        Console.WriteLine($"剩余治疗资源：{player.TreatmentCount}");
        Console.WriteLine("==============================");

        Program.Loading();
    }
}
