using System;

namespace Console_RPG;

/// <summary>
/// 回合制战斗系统。
///
/// Battle 负责战斗流程本身：显示敌人、读取行动、推进回合、处理胜负。
/// 它不负责定义玩家属性，也不负责保存文件；这些职责分别属于 Player 和 SaveManager。
///
/// v1.0 新增：
/// - 技能点消耗；
/// - 战斗胜利获得金币；
/// - 战斗胜利恢复少量技能点；
/// - 更清晰的战斗状态显示。
/// </summary>
public static class Battle
{
    /// <summary>开始连续战斗。每场胜利后询问玩家是否继续挑战。</summary>
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
                return;

            Console.Write("继续寻找下一个敌人？(Y/N)：");
            char choice = Console.ReadKey(true).KeyChar;
            Console.WriteLine(choice);
            if (choice is not ('Y' or 'y')) return;
        }
    }

    /// <summary>
    /// 运行一场战斗。
    /// 每次玩家成功执行一个行动，怪物才会获得反击机会。
    /// 无效输入、返回技能菜单和资源不足都不会白白消耗回合。
    /// </summary>
    private static bool Start(Player player, MonsterStatistics monster)
    {
        Console.Clear();
        Console.WriteLine($"你遇到了 {monster.Name}！");
        Console.WriteLine($"类型：{monster.Type}");
        Console.WriteLine($"等级：{monster.Level}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"击败奖励：{monster.ExpReward:0.#} EXP + {monster.GoldReward} 金币");
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

            // false 表示这个操作没有真正执行，因此怪物不能趁机攻击。
            if (!turnConsumed) continue;

            if (monster.Hp <= 0)
            {
                monster.Hp = 0;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"你击败了 {monster.Name}！");
                Console.ResetColor();

                // 战斗奖励在这里统一结算，避免普通攻击、技能攻击各写一套奖励代码。
                UpLevel.GainExp(player, monster.ExpReward);
                player.AddGold(monster.GoldReward);
                player.RecoverSkillPoint();
                Console.WriteLine($"获得 {monster.ExpReward:0.#} EXP 和 {monster.GoldReward} 金币！");
                Console.WriteLine($"技能点恢复 1 点：{player.SkillPoints}/{player.MaxSkillPoints}");

                HandleDrop(player, monster);
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

    /// <summary>显示本回合行动所依赖的主要战斗数据。</summary>
    private static void PrintBattleStatus(Player player, MonsterStatistics monster)
    {
        Console.WriteLine("========== 战斗 ==========");
        Console.WriteLine($"{player.Name} Lv.{player.Level}");
        Console.WriteLine($"HP：{player.Hp:0.#}/{player.FinalMaxHp:0.#}");
        Console.WriteLine($"攻击力：{player.FinalAttack:0.#}");
        Console.WriteLine($"暴击率：{player.FinalCriticalRate:P0}");
        Console.WriteLine($"闪避率：{player.FinalEvasionRate:P0}");
        Console.WriteLine($"技能点：{player.SkillPoints}/{player.MaxSkillPoints}");
        Console.WriteLine($"治疗资源：{player.TreatmentCount}");
        Console.WriteLine($"金币：{player.Gold}");
        Console.WriteLine("--------------------------");
        Console.WriteLine($"{monster.Name} Lv.{monster.Level}");
        Console.WriteLine($"类型：{monster.Type}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"闪避率：{monster.EvasionRate:P0}");
        Console.WriteLine("==========================");
    }

    /// <summary>执行普通攻击：先判定怪物闪避，再计算伤害和暴击。</summary>
    private static void Attack(Player player, MonsterStatistics monster)
    {
        if (Random.Shared.NextDouble() < monster.EvasionRate)
        {
            Console.WriteLine($"{monster.Name} 闪避了你的攻击！");
            return;
        }

        double damage = DamageCalculator.CalculateBasicDamage(
            player.FinalAttack,
            player.FinalCriticalRate,
            out bool critical);

        if (critical) Console.WriteLine("暴击！");
        monster.Hp = Math.Max(0, monster.Hp - damage);
        Console.WriteLine($"你攻击了 {monster.Name}，造成 {damage:0.#} 点伤害！");
    }

    /// <summary>
    /// 打开技能选择菜单。
    /// 返回 false 表示没有真正使用技能，因此不会消耗玩家回合。
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
            Skill skill = player.Skills[i];
            Console.WriteLine($"{i + 1}. {skill.Name} - {skill.Description}（消耗 {skill.SkillPointCost} 点）");
        }
        Console.WriteLine($"当前技能点：{player.SkillPoints}/{player.MaxSkillPoints}");
        Console.WriteLine("0. 返回");
        Console.Write("请选择技能：");

        if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index > player.Skills.Count)
        {
            Console.WriteLine("输入无效。");
            Pause();
            return false;
        }
        if (index == 0) return false;

        Skill selectedSkill = player.Skills[index - 1];
        if (player.SkillPoints < selectedSkill.SkillPointCost)
        {
            Console.WriteLine("技能点不足，无法使用这个技能。");
            Pause();
            return false;
        }

        if (Random.Shared.NextDouble() < monster.EvasionRate)
        {
            // v1.0 维持与普通攻击一致：技能被怪物闪避时不消耗技能点。
            Console.WriteLine($"{monster.Name} 闪避了你的 {selectedSkill.Name}！");
            return true;
        }

        double damage = SkillSystem.UseSkill(player, monster, selectedSkill, out bool critical);
        if (damage < 0)
        {
            Console.WriteLine("技能点不足，无法使用这个技能。");
            Pause();
            return false;
        }

        if (critical) Console.WriteLine("技能暴击！");
        Console.WriteLine($"你使用了 {selectedSkill.Name}，造成 {damage:0.#} 点伤害！");
        Console.WriteLine($"消耗 {selectedSkill.SkillPointCost} 点技能点，剩余 {player.SkillPoints} 点。");
        return true;
    }

    /// <summary>战斗中使用一次治疗资源。</summary>
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

    /// <summary>怪物回合：处理特殊攻击、玩家闪避以及最终伤害。</summary>
    private static void MonsterAttack(Player player, MonsterStatistics monster)
    {
        double damage = monster.Attack;
        if (monster.Type.Contains("特殊", StringComparison.Ordinal))
        {
            damage *= 1.2;
            Console.WriteLine("黑暗法师释放魔法！");
        }

        // 随玩家等级减伤是早期版本留下的规则，先保留，之后可以独立抽成防御系统。
        damage = Math.Max(1, damage - player.Level * 0.5);
        if (Random.Shared.NextDouble() < player.FinalEvasionRate)
        {
            Console.WriteLine("你闪避了怪物的攻击！");
            return;
        }

        player.TakeDamage(damage);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{monster.Name} 反击，造成 {damage:0.#} 点伤害！");
        Console.ResetColor();
    }

    /// <summary>处理战斗后的装备掉落。</summary>
    private static void HandleDrop(Player player, MonsterStatistics monster)
    {
        // 精英怪掉落率更高，但普通怪也有机会掉装备，保证装备系统不是只在菜单里摆设。
        int dropRate = monster.Name.StartsWith("[精英]") ? 70 : 35;
        if (Random.Shared.Next(100) >= dropRate)
        {
            Console.WriteLine("这次没有发现装备掉落。");
            return;
        }

        Equipment equipment = EquipmentFactory.CreateRandomDrop(monster);
        player.AddEquipment(equipment);
        Console.WriteLine($"你获得装备：{equipment.Name}");
        Console.WriteLine(equipment.GetAttributeText());
    }

    /// <summary>处理玩家死亡：扣除部分当前经验并恢复到安全状态。</summary>
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

    /// <summary>统一的战斗暂停，避免每个分支都重复写 ReadKey。</summary>
    private static void Pause()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
