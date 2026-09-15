using System;

namespace Console_RPG;

/// <summary>
/// 回合制战斗系统。
/// 负责战斗输入、伤害结算、胜负判断以及战斗后的经验奖励。
/// </summary>
public static class Battle
{
    private const int MaxHealsPerBattle = 3;

    /// <summary>进入连续战斗流程，直到玩家主动离开或战败。</summary>
    public static void StartBattle()
    {
        if (PlayerStatistics.Hp <= 0)
        {
            Console.WriteLine("你目前无法战斗。");
            Program.Loading();
            return;
        }

        while (PlayerStatistics.Hp > 0)
        {
            MonsterStatistics monster = MonsterFactory.Create();
            bool victory = Start(monster);

            if (!victory || PlayerStatistics.Hp <= 0)
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
    private static bool Start(MonsterStatistics monster)
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

        while (PlayerStatistics.Hp > 0 && monster.Hp > 0)
        {
            Console.Clear();
            PrintBattleStatus(monster, healsRemaining);
            Console.Write("选择行动 [A]攻击 [D]治疗 [F]撤退：");

            char action = char.ToUpperInvariant(Console.ReadKey(true).KeyChar);
            Console.WriteLine(action);

            bool turnConsumed = true;

            switch (action)
            {
                case 'A':
                    Attack(monster);
                    break;

                case 'D':
                    if (healsRemaining <= 0)
                    {
                        Console.WriteLine("本场战斗的治疗次数已经用完！");
                        turnConsumed = false;
                        Pause();
                        break;
                    }

                    HealInBattle(ref healsRemaining);
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
                UpLevel.GainExp(monster.ExpReward);
                Pause();
                return true;
            }

            MonsterAttack(monster);

            if (PlayerStatistics.Hp <= 0)
            {
                HandleDefeat();
                return false;
            }

            Pause();
        }

        return false;
    }

    /// <summary>显示本回合开始时双方的状态。</summary>
    private static void PrintBattleStatus(MonsterStatistics monster, int healsRemaining)
    {
        Console.WriteLine("========== 战斗 ==========");
        Console.WriteLine($"{PlayerStatistics.Name} Lv.{PlayerStatistics.Level}");
        Console.WriteLine($"HP：{PlayerStatistics.Hp:0.#}/{PlayerStatistics.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{PlayerStatistics.Attack:0.#}");
        Console.WriteLine("--------------------------");
        Console.WriteLine($"{monster.Name} Lv.{monster.Level}");
        Console.WriteLine($"HP：{monster.Hp:0.#}/{monster.MaxHp:0.#}");
        Console.WriteLine($"攻击力：{monster.Attack:0.#}");
        Console.WriteLine($"本场剩余治疗：{healsRemaining}");
        Console.WriteLine("==========================");
    }

    private static void Attack(MonsterStatistics monster)
    {
        monster.Hp -= PlayerStatistics.Attack;
        Console.WriteLine($"你攻击了 {monster.Name}，造成 {PlayerStatistics.Attack:0.#} 点伤害！");
    }

    private static void HealInBattle(ref int healsRemaining)
    {
        double oldHp = PlayerStatistics.Hp;
        PlayerStatistics.Hp = Math.Min(
            PlayerStatistics.MaxHp,
            PlayerStatistics.Hp + PlayerStatistics.Treatment);

        double recovered = PlayerStatistics.Hp - oldHp;
        healsRemaining--;
        Console.WriteLine($"你恢复了 {recovered:0.#} HP，还可治疗 {healsRemaining} 次。");
    }

    private static void MonsterAttack(MonsterStatistics monster)
    {
        // 等级差只提供轻微减伤，避免高等级角色完全无视敌人攻击。
        double damage = Math.Max(1, monster.Attack - PlayerStatistics.Level * 0.5);
        PlayerStatistics.Hp = Math.Max(0, PlayerStatistics.Hp - damage);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{monster.Name} 反击，造成 {damage:0.#} 点伤害！");
        Console.ResetColor();
    }

    private static void HandleDefeat()
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\n你倒下了……");
        Console.ResetColor();

        // 战败保留角色进度，但扣除少量当前经验作为失败代价。
        PlayerStatistics.Exp *= 0.9;
        PlayerStatistics.Hp = PlayerStatistics.MaxHp;
        Console.WriteLine("你被带回安全地点，HP 已恢复。当前经验损失 10%。");
        Pause();
    }

    private static void Pause()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
