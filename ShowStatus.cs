using System;

namespace Console_RPG;

/// <summary>负责展示玩家当前的角色数据。</summary>
public static class ShowStatus
{
    /// <summary>显示等级、经验、生命值和攻击力。</summary>
    public static void Display()
    {
        Console.Clear();
        Console.WriteLine("========== 角色状态 ==========");
        Console.WriteLine($"名字：{PlayerStatistics.Name}");
        Console.WriteLine($"等级：{PlayerStatistics.Level}");
        Console.WriteLine($"经验：{PlayerStatistics.Exp:0.#}/{PlayerStatistics.ExpToNextLevel:0.#}");
        Console.WriteLine($"HP：{PlayerStatistics.Hp:0.#}/{PlayerStatistics.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{PlayerStatistics.Attack:0.#}");
        Console.WriteLine($"治疗量：{PlayerStatistics.Treatment:0.#}");
        Console.WriteLine("==============================");

        Program.Loading();
    }
}
