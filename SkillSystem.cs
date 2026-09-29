using System;
using System.Collections.Generic;

namespace Console_RPG;

// 技能系统。
// 这里同时管理技能目录、学习入口和技能释放规则。
// “学习技能”和“战斗中释放技能”是两件不同的事：学习消耗金币，释放消耗技能点。
public static class SkillSystem
{
    // 新角色出生时只学会最基础的两个技能。
    // 其他技能必须进入“技能管理”手动学习，这样玩家才真正有技能成长路线。
    public static void InitializeStarterSkills(Player player)
    {
        player.LearnSkill(new Skill
        {
            Name = "重击",
            Description = "以 1.5 倍攻击力造成伤害，并使目标眩晕 1 回合。",
            DamageMultiplier = 1.5,
            Cooldown = 2,
            SkillPointCost = 1,
            StatusEffect = StatusEffectType.Stunned,
            StatusEffectDuration = 1,
            LearnLevel = 1,
            LearnCostGold = 0
        });

        player.LearnSkill(new Skill
        {
            Name = "火球",
            Description = "以 1.8 倍攻击力造成伤害，并使目标燃烧 3 回合。",
            DamageMultiplier = 1.8,
            Cooldown = 3,
            SkillPointCost = 2,
            StatusEffect = StatusEffectType.Burning,
            StatusEffectDuration = 3,
            StatusEffectPower = 5,
            LearnLevel = 1,
            LearnCostGold = 0
        });
    }

    // 所有可学习技能都集中放在这里。
    // 以后增加技能时只需要新增一个 Skill 数据，不需要改技能菜单流程。
    private static List<Skill> GetSkillCatalog() =>
        new()
        {
            new Skill
            {
                Name = "冰枪",
                Description = "以 2.0 倍攻击力造成伤害。",
                DamageMultiplier = 2.0,
                Cooldown = 3,
                SkillPointCost = 2,
                LearnLevel = 3,
                LearnCostGold = 80
            },
            new Skill
            {
                Name = "毒刃",
                Description = "以 1.6 倍攻击力造成伤害，并使目标中毒 3 回合。",
                DamageMultiplier = 1.6,
                Cooldown = 2,
                SkillPointCost = 1,
                StatusEffect = StatusEffectType.Poison,
                StatusEffectDuration = 3,
                StatusEffectPower = 6,
                LearnLevel = 5,
                LearnCostGold = 120
            },
            new Skill
            {
                Name = "旋风斩",
                Description = "以 2.4 倍攻击力造成伤害。",
                DamageMultiplier = 2.4,
                Cooldown = 4,
                SkillPointCost = 3,
                LearnLevel = 7,
                LearnCostGold = 180
            }
        };

    // 技能管理入口。
    // 玩家可以在这里查看已学技能，也可以花金币学习满足等级条件的新技能。
    public static void ShowMenu(Player player)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== 技能管理 ==========");
            Console.WriteLine($"等级：Lv.{player.Level}");
            Console.WriteLine($"金币：{player.Gold}");
            Console.WriteLine($"战斗技能点：{player.SkillPoints}/{player.MaxSkillPoints}");
            Console.WriteLine("------------------------------");

            Console.WriteLine("已学习技能：");
            if (player.Skills.Count == 0)
            {
                Console.WriteLine("  暂无");
            }
            else
            {
                foreach (Skill skill in player.Skills)
                    Console.WriteLine($"  • {skill.Name}：{skill.Description}");
            }

            Console.WriteLine("------------------------------");
            Console.WriteLine("可学习技能：");

            List<Skill> catalog = GetSkillCatalog();
            List<Skill> available = new();
            int number = 1;

            foreach (Skill skill in catalog)
            {
                if (player.HasSkill(skill.Name))
                    continue;

                available.Add(skill);
                string requirement = player.Level >= skill.LearnLevel
                    ? "可以学习"
                    : $"需要 Lv.{skill.LearnLevel}";
                Console.WriteLine($"{number}. {skill.Name} - {skill.Description}");
                Console.WriteLine($"   学习费用：{skill.LearnCostGold} 金币 | {requirement}");
                number++;
            }

            if (available.Count == 0)
                Console.WriteLine("  暂无可学习的新技能。");

            Console.WriteLine("------------------------------");
            Console.WriteLine("0. 返回");
            Console.Write("请选择要学习的技能：");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > available.Count)
            {
                Console.WriteLine("输入无效。");
                Program.Loading();
                continue;
            }

            if (choice == 0)
                return;

            Skill selected = available[choice - 1];

            if (player.Level < selected.LearnLevel)
            {
                Console.WriteLine($"等级不足。“{selected.Name}”需要达到 Lv.{selected.LearnLevel}。");
                Program.Loading();
                continue;
            }

            if (player.Gold < selected.LearnCostGold)
            {
                Console.WriteLine($"金币不足，学习“{selected.Name}”需要 {selected.LearnCostGold} 金币。");
                Program.Loading();
                continue;
            }

            Console.Write($"确定花费 {selected.LearnCostGold} 金币学习“{selected.Name}”吗？(Y/N)：");
            char confirm = Console.ReadKey(true).KeyChar;
            Console.WriteLine(confirm);

            if (confirm is not ('Y' or 'y'))
                continue;

            if (player.TryLearnSkill(selected))
                Console.WriteLine($"学习成功！你学会了“{selected.Name}”。");
            else
                Console.WriteLine("学习失败，金币或技能状态没有发生变化。");

            Program.Loading();
        }
    }

    // 执行一次技能。
    // 如果技能点不够，返回 -1，Battle 就知道这次操作没有消耗回合。
    public static double UseSkill(
        Player player,
        MonsterStatistics monster,
        Skill skill,
        out bool critical)
    {
        critical = false;

        if (!player.TryUseSkillPoint(skill.SkillPointCost))
            return -1;

        double damage = DamageCalculator.CalculateSkillDamage(
            player.FinalAttack,
            skill,
            out critical);

        monster.Hp = System.Math.Max(0, monster.Hp - damage);

        // 技能命中后再附加状态，避免技能资源不足时错误施加效果。
        if (skill.StatusEffect != StatusEffectType.None && skill.StatusEffectDuration > 0)
        {
            monster.ApplyStatus(new StatusEffect
            {
                Type = skill.StatusEffect,
                RemainingTurns = skill.StatusEffectDuration,
                DamagePerTurn = skill.StatusEffectPower
            });
        }

        return damage;
    }
}