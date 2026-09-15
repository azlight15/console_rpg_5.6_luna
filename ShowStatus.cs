using System;

namespace Console_RPG;

/// <summary>负责展示玩家当前的角色数据。</summary>
public static class ShowStatus
{
    /// <summary>
    /// 显示等级、经验、最终属性、装备、技能和治疗资源。
    /// 最终属性使用装备加成后的结果，方便直接验证 v0.4.0 的属性计算。
    /// </summary>
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
        Console.WriteLine("------------------------------");
        Console.WriteLine($"武器：{player.Weapon?.Name ?? "无"} (+{player.Weapon?.AttackBonus ?? 0:0.#} 攻击)");
        Console.WriteLine($"防具：{player.Armor?.Name ?? "无"} (+{player.Armor?.HpBonus ?? 0:0.#} HP)");
        Console.WriteLine("技能：");

        if (player.Skills.Count == 0)
        {
            Console.WriteLine("  无");
        }
        else
        {
            foreach (Skill skill in player.Skills)
            {
                Console.WriteLine($"  - {skill.Name}：{skill.Description}");
            }
        }

        Console.WriteLine("------------------------------");
        Console.WriteLine($"单次治疗量：{player.Treatment:0.#}");
        Console.WriteLine($"剩余治疗资源：{player.TreatmentCount}");
        Console.WriteLine("==============================");

        Program.Loading();
    }
}
