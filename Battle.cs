using System;

namespace Console_RPG;

// 回合制战斗主流程。
// Battle 负责“这一回合发生什么”，具体伤害、技能和怪物生成交给其他类。
public static class Battle
{
    // 开始连续战斗。每打赢一只怪物，可以选择继续寻找下一只。
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

    // 处理一整场战斗，直到怪物死亡、玩家死亡或玩家主动撤退。
    private static bool Start(Player player, MonsterStatistics monster)
    {
        Console.Clear();
        Console.WriteLine($"你遇到了 {monster.Name}！");
        Console.WriteLine($"类型：{monster.Type}");
        Console.WriteLine($"等级：{monster.Level}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"击败奖励：{monster.ExpReward:0.#} EXP");
        Console.WriteLine($"金币奖励：{monster.GoldReward}");
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
                    // 输入错误不会消耗回合。
                    Console.WriteLine("无效操作，请选择 1-4。");
                    turnConsumed = false;
                    Pause();
                    break;
            }

            if (!turnConsumed) continue;

            // 玩家把怪物打倒后，直接结算奖励，不让已经死亡的怪物反击。
            if (monster.Hp <= 0)
            {
                monster.Hp = 0;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"你击败了 {monster.Name}！");
                Console.ResetColor();

                UpLevel.GainExp(player, monster.ExpReward);
                player.AddGold(monster.GoldReward);
                Console.WriteLine($"获得金币：{monster.GoldReward}，当前金币：{player.Gold}");
                HandleDrop(player, monster);
                Pause();
                return true;
            }

            // 玩家行动结束后怪物还活着，所以怪物反击。
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

    // 每回合显示双方的关键数据，让玩家知道自己的实际战斗能力。
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
        Console.WriteLine("--------------------------");
        Console.WriteLine($"{monster.Name} Lv.{monster.Level}");
        Console.WriteLine($"类型：{monster.Type}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"闪避率：{monster.EvasionRate:P0}");
        Console.WriteLine("==========================");
    }

    // 普通攻击先判断怪物是否闪避，再交给 DamageCalculator 计算伤害和暴击。
    private static void Attack(Player player, MonsterStatistics monster)
    {
        if (Random.Shared.NextDouble() < monster.EvasionRate)
        {
            Console.WriteLine($"{monster.Name} 闪避了你的攻击！");
            return;
        }

        double damage = DamageCalculator.CalculateBasicDamage(player.FinalAttack, player.FinalCriticalRate, out bool critical);
        if (critical) Console.WriteLine("暴击！");
        monster.Hp = Math.Max(0, monster.Hp - damage);
        Console.WriteLine($"你攻击了 {monster.Name}，造成 {damage:0.#} 点伤害！");
    }

    // 打开技能列表，让玩家选择要释放的技能。
    // 技能点扣除由 SkillSystem 统一处理，避免重复扣除。
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
            Console.WriteLine($"{i + 1}. {skill.Name} - {skill.Description}（消耗 {skill.SkillPointCost} 点，冷却 {skill.Cooldown} 回合）");
        }
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
        if (Random.Shared.NextDouble() < monster.EvasionRate)
        {
            // 技能已经被选择并消耗本回合，但这里暂时不扣技能点。
            // 是否闪避属于战斗结果，而技能资源由真正释放成功的 SkillSystem 处理。
            Console.WriteLine($"{monster.Name} 闪避了你的 {selectedSkill.Name}！");
            return true;
        }

        double damage = SkillSystem.UseSkill(player, monster, selectedSkill, out bool critical);
        if (damage < 0)
        {
            Console.WriteLine("技能点不足！");
            Pause();
            return false;
        }

        if (critical) Console.WriteLine("技能暴击！");
        Console.WriteLine($"你使用了 {selectedSkill.Name}，造成 {damage:0.#} 点伤害！");
        return true;
    }

    // 战斗内治疗和主菜单治疗共用同一套治疗资源。
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

    // 怪物反击。特殊类型的怪物可以在这里加入额外攻击效果。
    private static void MonsterAttack(Player player, MonsterStatistics monster)
    {
        double damage = monster.Attack;
        if (monster.Type.Contains("特殊", StringComparison.Ordinal))
        {
            damage *= 1.2;
            Console.WriteLine("黑暗法师释放魔法！");
        }

        // 玩家等级会稍微降低实际受到的伤害，避免高等级玩家仍被低级怪物轻易击杀。
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

    // 战斗胜利后，按概率给玩家一件随机装备。
    private static void HandleDrop(Player player, MonsterStatistics monster)
    {
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

    // 玩家死亡时损失当前经验的 10%，但等级、装备、技能和金币都保留。
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

    // 暂停画面，让玩家有时间看清结果。
    private static void Pause()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
