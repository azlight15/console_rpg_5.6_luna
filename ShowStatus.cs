using System;

namespace Console_RPG;

/// <summary>负责展示玩家当前的角色数据。</summary>
public static class ShowStatus
{
    /// <summary>显示基础属性、最终属性、装备、背包、技能和治疗资源。</summary>
    public static void Display(Player player)
    {
        Console.Clear();
        Console.WriteLine("========== 角色状态 ==========");
        Console.WriteLine($"名字：{player.Name}");
        Console.WriteLine($"等级：{player.Level}");
        Console.WriteLine($"经验：{player.Exp:0.#}/{player.ExpToNextLevel:0.#}");
        Console.WriteLine($"HP：{player.Hp:0.#}/{player.FinalMaxHp:0.#}");
        Console.WriteLine($"基础攻击力：{player.Attack:0.#}");
        Console.WriteLine($"最终攻击力：{player.FinalAttack:0.#}");
        Console.WriteLine($"最终暴击率：{player.FinalCriticalRate:P0}");
        Console.WriteLine($"最终闪避率：{player.FinalEvasionRate:P0}");
        Console.WriteLine("------------------------------");
        Console.WriteLine($"武器：{player.Weapon?.Name ?? "无"}");
        Console.WriteLine($"防具：{player.Armor?.Name ?? "无"}");
        Console.WriteLine($"背包装备：{player.Inventory.Count} 件");
        Console.WriteLine("技能：");

        if (player.Skills.Count == 0)
            Console.WriteLine("  无");
        else
            foreach (Skill skill in player.Skills)
                Console.WriteLine($"  - {skill.Name}：{skill.Description}");

        Console.WriteLine("------------------------------");
        Console.WriteLine($"单次治疗量：{player.Treatment:0.#}");
        Console.WriteLine($"剩余治疗资源：{player.TreatmentCount}");
        Console.WriteLine("==============================");
        Program.Loading();
    }
}
