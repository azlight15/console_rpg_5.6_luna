using System;

namespace Console_RPG;

/// <summary>
/// 回合制战斗系统。
/// v0.5.0 加入等级缩放怪物、装备掉落，以及装备带来的暴击/闪避属性。
/// </summary>
public static class Battle
{
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

            if (!turnConsumed) continue;

            if (monster.Hp <= 0)
            {
                monster.Hp = 0;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"你击败了 {monster.Name}！");
                Console.ResetColor();
                UpLevel.GainExp(player, monster.ExpReward);
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

    private static void PrintBattleStatus(Player player, MonsterStatistics monster)
    {
        Console.WriteLine("========== 战斗 ==========");
        Console.WriteLine($"{player.Name} Lv.{player.Level}");
        Console.WriteLine($"HP：{player.Hp:0.#}/{player.FinalMaxHp:0.#}");
        Console.WriteLine($"攻击力：{player.FinalAttack:0.#}");
        Console.WriteLine($"暴击率：{player.FinalCriticalRate:P0}");
        Console.WriteLine($"闪避率：{player.FinalEvasionRate:P0}");
        Console.WriteLine($"治疗资源：{player.TreatmentCount}");
        Console.WriteLine("--------------------------");
        Console.WriteLine($"{monster.Name} Lv.{monster.Level}");
        Console.WriteLine($"类型：{monster.Type}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"闪避率：{monster.EvasionRate:P0}");
        Console.WriteLine("==========================");
    }

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
            Console.WriteLine($"{i + 1}. {skill.Name} - {skill.Description}");
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
            Console.WriteLine($"{monster.Name} 闪避了你的 {selectedSkill.Name}！");
            return true;
        }

        double damage = SkillSystem.UseSkill(player, monster, selectedSkill, out bool critical);
        if (critical) Console.WriteLine("技能暴击！");
        Console.WriteLine($"你使用了 {selectedSkill.Name}，造成 {damage:0.#} 点伤害！");
        return true;
    }

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

    private static void MonsterAttack(Player player, MonsterStatistics monster)
    {
        double damage = monster.Attack;
        if (monster.Type.Contains("特殊", StringComparison.Ordinal))
        {
            damage *= 1.2;
            Console.WriteLine("黑暗法师释放魔法！");
        }

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

    private static void HandleDrop(Player player, MonsterStatistics monster)
    {
        // 普通怪物 35% 掉落，精英怪物 70% 掉落；让装备获得成为稳定但不泛滥的成长来源。
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

    private static void Pause()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
