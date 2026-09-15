using System;

namespace Console_RPG;

/// <summary>
/// 回合制战斗系统。
///
/// v0.4.0 将 Battle 的职责收敛为回合流程：
/// - 普通攻击交给 DamageCalculator 计算。
/// - 技能行动交给 SkillSystem 处理。
/// - 治疗继续使用 Player 的治疗资源。
/// - Battle 本身只负责输入、回合推进和战斗结果。
/// </summary>
public static class Battle
{
    /// <summary>进入连续战斗流程，直到玩家主动离开或战败。</summary>
    public static void StartBattle(Player player)
    {
        if (player.Hp <= 0)
        {
            Console.WriteLine("你目前无法战斗。");
            Program.Loading();
            return;
        }

        while (player.Hp > 0)
        {
            MonsterStatistics monster = MonsterFactory.Create(player);
            bool victory = Start(player, monster);

            if (!victory || player.Hp <= 0)
            {
                return;
            }

            Console.Write("继续寻找下一个敌人？(Y/N)：");
            char choice = Console.ReadKey(true).KeyChar;
            Console.WriteLine(choice);

            if (choice is not ('Y' or 'y'))
            {
                return;
            }
        }
    }

    /// <summary>执行一场完整战斗。</summary>
    private static bool Start(Player player, MonsterStatistics monster)
    {
        Console.Clear();
        Console.WriteLine($"你遇到了 {monster.Name}！");
        Console.WriteLine($"类型：{monster.Type}");
        Console.WriteLine($"等级：{monster.Level}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"击败奖励：{monster.ExpReward:0.#} EXP");
        Console.WriteLine("按任意键进入战斗。");
        Console.ReadKey(true);

        while (player.Hp > 0 && monster.Hp > 0)
        {
            Console.Clear();
            PrintBattleStatus(player, monster);
            Console.WriteLine("选择行动：");
            Console.WriteLine("1. 普通攻击");
            Console.WriteLine("2. 使用技能");
            Console.WriteLine("3. 治疗");
            Console.WriteLine("4. 撤退");
            Console.Write("请选择：");

            string? action = Console.ReadLine();
            bool turnConsumed = true;

            switch (action)
            {
                case "1":
                    Attack(player, monster);
                    break;

                case "2":
                    // 技能菜单返回 false 时表示没有真正使用技能，因此不消耗怪物回合。
                    turnConsumed = UseSkill(player, monster);
                    break;

                case "3":
                    if (player.TreatmentCount <= 0)
                    {
                        Console.WriteLine("你已经没有治疗资源了！");
                        turnConsumed = false;
                        Pause();
                        break;
                    }

                    HealInBattle(player);
                    break;

                case "4":
                    Console.WriteLine("你撤退了。");
                    Pause();
                    return false;

                default:
                    Console.WriteLine("无效操作，请选择 1-4。");
                    turnConsumed = false;
                    Pause();
                    break;
            }

            if (!turnConsumed)
            {
                continue;
            }

            if (monster.Hp <= 0)
            {
                monster.Hp = 0;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"你击败了 {monster.Name}！");
                Console.ResetColor();
                UpLevel.GainExp(player, monster.ExpReward);
                Pause();
                return true;
            }

            MonsterAttack(player, monster);

            if (player.Hp <= 0)
            {
                HandleDefeat(player);
                return false;
            }

            Pause();
        }

        return false;
    }

    /// <summary>显示战斗双方当前状态，包括装备后的最终攻击力。</summary>
    private static void PrintBattleStatus(Player player, MonsterStatistics monster)
    {
        Console.WriteLine("========== 战斗 ==========");
        Console.WriteLine($"{player.Name} Lv.{player.Level}");
        Console.WriteLine($"HP：{player.Hp:0.#}/{player.FinalMaxHp:0.#}");
        Console.WriteLine($"攻击力：{player.FinalAttack:0.#}");
        Console.WriteLine($"治疗资源：{player.TreatmentCount}");
        Console.WriteLine("--------------------------");
        Console.WriteLine($"{monster.Name} Lv.{monster.Level}");
        Console.WriteLine($"类型：{monster.Type}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine("==========================");
    }

    /// <summary>
    /// 普通攻击流程。
    /// 先处理怪物闪避，再由 DamageCalculator 处理基础伤害与暴击。
    /// </summary>
    private static void Attack(Player player, MonsterStatistics monster)
    {
        if (Random.Shared.Next(100) < monster.EvasionRate)
        {
            Console.WriteLine($"{monster.Name} 闪避了你的攻击！");
            return;
        }

        double damage = DamageCalculator.CalculateBasicDamage(player.FinalAttack, 0.10, out bool critical);

        if (critical)
        {
            Console.WriteLine("暴击！");
        }

        monster.Hp = Math.Max(0, monster.Hp - damage);
        Console.WriteLine($"你攻击了 {monster.Name}，造成 {damage:0.#} 点伤害！");
    }

    /// <summary>
    /// 显示玩家技能并执行一次技能行动。
    /// 没有技能或输入无效时返回 false，让玩家重新选择行动。
    /// </summary>
    private static bool UseSkill(Player player, MonsterStatistics monster)
    {
        if (player.Skills.Count == 0)
        {
            Console.WriteLine("你目前没有学会任何技能。");
            Pause();
            return false;
        }

        Console.WriteLine("========== 技能 ==========");
        for (int i = 0; i < player.Skills.Count; i++)
        {
            Skill currentSkill = player.Skills[i];
            Console.WriteLine($"{i + 1}. {currentSkill.Name} - {currentSkill.Description}");
        }
        Console.WriteLine("0. 返回");
        Console.Write("请选择技能：");

        if (!int.TryParse(Console.ReadLine(), out int index))
        {
            Console.WriteLine("输入无效。");
            Pause();
            return false;
        }

        if (index == 0)
        {
            return false;
        }

        if (index < 1 || index > player.Skills.Count)
        {
            Console.WriteLine("没有这个技能。");
            Pause();
            return false;
        }

        Skill selectedSkill = player.Skills[index - 1];

        if (Random.Shared.Next(100) < monster.EvasionRate)
        {
            Console.WriteLine($"{monster.Name} 闪避了你的 {selectedSkill.Name}！");
            return true;
        }

        double damage = SkillSystem.UseSkill(player, monster, selectedSkill, out bool critical);

        if (critical)
        {
            Console.WriteLine("技能暴击！");
        }

        Console.WriteLine($"你使用了 {selectedSkill.Name}，造成 {damage:0.#} 点伤害！");
        return true;
    }

    /// <summary>消耗治疗资源并恢复生命。</summary>
    private static void HealInBattle(Player player)
    {
        double recovered = player.UseTreatment();

        if (recovered <= 0)
        {
            Console.WriteLine("当前无法治疗。");
            return;
        }

        Console.WriteLine($"你恢复了 {recovered:0.#} HP，还剩 {player.TreatmentCount} 次治疗。");
    }

    /// <summary>执行怪物攻击。</summary>
    private static void MonsterAttack(Player player, MonsterStatistics monster)
    {
        double damage = monster.Attack;

        if (monster.Type == "黑暗法师")
        {
            damage *= 1.2;
            Console.WriteLine("黑暗法师释放魔法！");
        }

        damage = Math.Max(1, damage - player.Level * 0.5);
        player.TakeDamage(damage);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{monster.Name} 反击，造成 {damage:0.#} 点伤害！");
        Console.ResetColor();
    }

    /// <summary>处理玩家战败。</summary>
    private static void HandleDefeat(Player player)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\n你倒下了……");
        Console.ResetColor();

        player.Exp *= 0.9;
        player.RestoreFullHealth();
        Console.WriteLine("你被带回安全地点，HP 已恢复。当前经验损失 10%。");
        Pause();
    }

    /// <summary>暂停等待玩家阅读信息。</summary>
    private static void Pause()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
