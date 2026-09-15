using System;

namespace Console_RPG;

/// <summary>
/// 回合制战斗系统。
/// 战斗状态通过 Player 和 MonsterStatistics 实例显式传递，不再依赖全局玩家数据。
/// </summary>
public static class Battle
{
    private const int MaxHealsPerBattle = 3;

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

    /// <summary>执行一场完整的单敌人战斗。</summary>
    private static bool Start(Player player, MonsterStatistics monster)
    {
        Console.Clear();
        Console.WriteLine($"你遇到了 {monster.Name}！");
        Console.WriteLine($"等级：{monster.Level}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"击败奖励：{monster.ExpReward:0.#} EXP");
        Console.WriteLine("按任意键进入战斗。");
        Console.ReadKey(true);

        int healsRemaining = MaxHealsPerBattle;

        while (player.Hp > 0 && monster.Hp > 0)
        {
            Console.Clear();
            PrintBattleStatus(player, monster, healsRemaining);
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
                    if (healsRemaining <= 0)
                    {
                        Console.WriteLine("本场战斗的治疗次数已经用完！");
                        turnConsumed = false;
                        Pause();
                        break;
                    }

                    HealInBattle(player, ref healsRemaining);
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

    /// <summary>显示本回合开始时双方的状态。</summary>
    private static void PrintBattleStatus(Player player, MonsterStatistics monster, int healsRemaining)
    {
        Console.WriteLine("========== 战斗 ==========");
        Console.WriteLine($"{player.Name} Lv.{player.Level}");
        Console.WriteLine($"HP：{player.Hp:0.#}/{player.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{player.Attack:0.#}");
        Console.WriteLine("--------------------------");
        Console.WriteLine($"{monster.Name} Lv.{monster.Level}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"本场剩余治疗：{healsRemaining}");
        Console.WriteLine("==========================");
    }

    private static void Attack(Player player, MonsterStatistics monster)
    {
        monster.Hp -= player.Attack;
        Console.WriteLine($"你攻击了 {monster.Name}，造成 {player.Attack:0.#} 点伤害！");
    }

    private static void HealInBattle(Player player, ref int healsRemaining)
    {
        double oldHp = player.Hp;
        player.Hp = Math.Min(player.MaxHp, player.Hp + player.Treatment);

        double recovered = player.Hp - oldHp;
        healsRemaining--;
        Console.WriteLine($"你恢复了 {recovered:0.#} HP，还可治疗 {healsRemaining} 次。");
    }

    private static void MonsterAttack(Player player, MonsterStatistics monster)
    {
        // 等级差只提供轻微减伤，避免高等级角色完全无视敌人攻击。
        double damage = Math.Max(1, monster.Attack - player.Level * 0.5);
        player.Hp = Math.Max(0, player.Hp - damage);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{monster.Name} 反击，造成 {damage:0.#} 点伤害！");
        Console.ResetColor();
    }

    private static void HandleDefeat(Player player)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\n你倒下了……");
        Console.ResetColor();

        // 战败保留角色进度，但扣除少量当前经验作为失败代价。
        player.Exp *= 0.9;
        player.Hp = player.MaxHp;
        Console.WriteLine("你被带回安全地点，HP 已恢复。当前经验损失 10%。");
        Pause();
    }

    private static void Pause()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
