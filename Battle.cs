using System;

namespace Console_RPG;

/// <summary>
/// 回合制战斗系统。
///
/// v0.3.0 开始加入基础战斗变化：
/// - 普通攻击存在暴击与闪避概率。
/// - 怪物类型会影响战斗表现。
/// - 战斗计算集中管理，方便未来加入技能和状态效果。
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
            Console.Write("选择行动 [A]攻击 [D]治疗 [F]撤退：");

            char action = char.ToUpperInvariant(Console.ReadKey(true).KeyChar);
            Console.WriteLine(action);
            bool turnConsumed = true;

            switch (action)
            {
                case 'A':
                    Attack(player, monster);
                    break;

                case 'D':
                    if (player.TreatmentCount <= 0)
                    {
                        Console.WriteLine("你已经没有治疗资源了！");
                        turnConsumed = false;
                        Pause();
                        break;
                    }

                    HealInBattle(player);
                    break;

                case 'F':
                    Console.WriteLine("你撤退了。");
                    Pause();
                    return false;

                default:
                    Console.WriteLine("无效操作，请选择 A、D 或 F。");
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

    /// <summary>显示战斗双方当前状态。</summary>
    private static void PrintBattleStatus(Player player, MonsterStatistics monster)
    {
        Console.WriteLine("========== 战斗 ==========");
        Console.WriteLine($"{player.Name} Lv.{player.Level}");
        Console.WriteLine($"HP：{player.Hp:0.#}/{player.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{player.Attack:0.#}");
        Console.WriteLine($"治疗资源：{player.TreatmentCount}");
        Console.WriteLine("--------------------------");
        Console.WriteLine($"{monster.Name} Lv.{monster.Level}");
        Console.WriteLine($"类型：{monster.Type}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine("==========================");
    }

    /// <summary>
    /// 玩家攻击计算。
    /// 当前版本加入基础暴击和闪避，为后续技能系统预留扩展位置。
    /// </summary>
    private static void Attack(Player player, MonsterStatistics monster)
    {
        if (Random.Shared.Next(100) < monster.EvasionRate)
        {
            Console.WriteLine($"{monster.Name} 闪避了你的攻击！");
            return;
        }

        double damage = player.Attack;
        bool critical = Random.Shared.Next(100) < 10;

        if (critical)
        {
            damage *= 1.5;
            Console.WriteLine("暴击！");
        }

        monster.Hp -= damage;
        Console.WriteLine($"你攻击了 {monster.Name}，造成 {damage:0.#} 点伤害！");
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
