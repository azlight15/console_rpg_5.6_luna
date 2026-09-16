using System;

namespace Console_RPG;

/// <summary>
/// 角色状态界面。
///
/// ShowStatus 只负责“展示”，不修改玩家数据。
/// 这样以后调整属性计算时，只需要改 Player，状态界面自然会显示新的最终结果。
/// </summary>
public static class ShowStatus
{
    /// <summary>显示玩家的成长、战斗属性、装备、技能和资源。</summary>
    public static void Display(Player player)
    {
        Console.Clear();
        Console.WriteLine("========== 角色状态 ==========");

        // 先显示成长数据，方便判断当前角色距离下一次升级还有多少经验。
        Console.WriteLine($"名字：{player.Name}");
        Console.WriteLine($"等级：{player.Level}");
        Console.WriteLine($"经验：{player.Exp:0.#}/{player.ExpToNextLevel:0.#}");

        // 基础属性和最终属性同时显示，可以直接看出装备到底提供了多少收益。
        Console.WriteLine($"HP：{player.Hp:0.#}/{player.FinalMaxHp:0.#}");
        Console.WriteLine($"基础攻击力：{player.Attack:0.#}");
        Console.WriteLine($"最终攻击力：{player.FinalAttack:0.#}");
        Console.WriteLine($"最终暴击率：{player.FinalCriticalRate:P0}");
        Console.WriteLine($"最终闪避率：{player.FinalEvasionRate:P0}");
        Console.WriteLine($"金币：{player.Gold}");
        Console.WriteLine($"技能点：{player.SkillPoints}/{player.MaxSkillPoints}");

        Console.WriteLine("------------------------------");
        Console.WriteLine($"武器：{player.Weapon?.Name ?? "无"}");
        Console.WriteLine($"防具：{player.Armor?.Name ?? "无"}");
        Console.WriteLine($"背包装备：{player.Inventory.Count} 件");

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
                Console.WriteLine($"    消耗技能点：{skill.SkillPointCost}");
            }
        }

        Console.WriteLine("------------------------------");
        Console.WriteLine($"单次治疗量：{player.Treatment:0.#}");
        Console.WriteLine($"剩余治疗资源：{player.TreatmentCount}");
        Console.WriteLine("==============================");
        Program.Loading();
    }
}
