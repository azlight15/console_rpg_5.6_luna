using System;

namespace Console_RPG;

// 角色状态界面。
// 这个类只负责“把数据显示出来”，不负责修改玩家属性。
public static class ShowStatus
{
    // 显示玩家的成长、战斗属性、装备、技能和资源。
    public static void Display(Player player)
    {
        Console.Clear();
        Console.WriteLine("========== 角色状态 ==========");

        // 先看等级和经验，可以知道离下一次升级还有多远。
        Console.WriteLine($"名字：{player.Name}");
        Console.WriteLine($"等级：{player.Level}");
        Console.WriteLine($"经验：{player.Exp:0.#}/{player.ExpToNextLevel:0.#}");

        // 同时显示基础值和最终值，可以直观看出装备带来了多少提升。
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
