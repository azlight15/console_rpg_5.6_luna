// ==========================
// 对战系统
// 负责处理玩家与怪物之间的完整战斗流程
// ==========================

using System;

namespace Console_RPG;

/*
    Battle 模块负责控制战斗入口、战斗流程与战斗结果结算。
    包括刷怪循环、战斗操作、胜负判断以及经验奖励。
*/
public static class Battle
{
    /*
        开始战斗入口
        控制是否继续刷怪的整体流程
    */
    public static void StartBattle()
    {
        // 玩家血量不足时无法进入战斗
        if (PlayerStatistics.Hp <= 0)
        {
            Console.WriteLine("你的HP不足，帮你回到选择页面");
            Program.Loading();
            return;
        }
        
        // 玩家存活时可以连续刷怪
        while (PlayerStatistics.Hp > 0)
        {
            // 从怪物工厂随机生成怪物
            var monster = MonsterFactory.Monster;
            
            // 进入单次战斗流程
            Start(monster);

            Console.WriteLine("是否继续刷怪？（选择Y/y则继续，否则返回菜单）");
            char choice = Console.ReadKey().KeyChar;

            // 非 Y 则退出刷怪循环
            if (choice != 'Y' && choice != 'y')
            {
                Program.Loading();
                break;
            }
        }
    }
    
    /*
        单次战斗流程
        玩家与指定怪物进行回合制战斗
    */
    private static void Start(MonsterStatistics monster)
    {
        // 显示怪物信息面板
        Console.Clear();
        Console.WriteLine($"你遇到了 {monster.Name}");
        Console.WriteLine($"等级：{monster.Level}");
        Console.WriteLine($"血量 {monster.Hp}/{monster.MaxHp}");
        Console.WriteLine($"攻击力 {monster.Attack}");
        Console.WriteLine($"预计获得经验值：{monster.ExpReward}");
        Console.WriteLine("按任意键进入战斗！");
        Console.ReadKey();

        bool monsterCanBattle = true;

        // 战斗主循环：双方存活并且没有撤退
        while ((PlayerStatistics.Hp > 0 && monster.Hp > 0) && monsterCanBattle)
        {
            Console.Clear();

            // 显示双方状态
            Console.WriteLine($"{PlayerStatistics.Name}");
            Console.WriteLine($"{PlayerStatistics.Level}");
            Console.WriteLine($"{PlayerStatistics.Hp}/{PlayerStatistics.MaxHp}");
            Console.WriteLine("=======================");
            Console.WriteLine($"{monster.Name}");
            Console.WriteLine($"{monster.Level}");
            Console.WriteLine($"{monster.Hp}/{monster.MaxHp}");
            Console.WriteLine("=======================");

            Console.WriteLine("注：只能选对应按键，否则直接退出战斗！");
            Console.WriteLine("普通攻击（a/A）| 治疗（d/D）| 退出（f/F）");

            char battleOption = Console.ReadKey().KeyChar;

            // 玩家回合
            switch (battleOption)
            {
                case 'A':
                case 'a':
                    // 玩家普通攻击
                    monster.Hp -= PlayerStatistics.Attack;
                    Console.WriteLine($"你攻击了 {monster.Name}，造成 {PlayerStatistics.Attack} 点伤害！");
                    break;

                case 'D':
                case 'd':
                    // 玩家治疗
                    PlayerStatistics.Hp += PlayerStatistics.Treatment;
                    if (PlayerStatistics.Hp > PlayerStatistics.MaxHp)
                    {
                        PlayerStatistics.Hp = PlayerStatistics.MaxHp;
                    }
                    Console.WriteLine($"你治疗了自己，恢复 {PlayerStatistics.Treatment} HP");
                    break;

                case 'F':
                case 'f':
                    // 玩家撤退
                    Console.WriteLine("你选择了撤退");
                    monsterCanBattle = false;
                    break;

                default:
                    // 非法操作直接结束战斗
                    Console.WriteLine("无效操作！帮你回到选择页面");
                    monsterCanBattle = false;
                    break;
            }

            // 胜利判定
            if (monster.Hp <= 0)
            {
                monster.Hp = 0;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"你击败了 {monster.Name}！");
                Console.ResetColor();

                // 给予经验并自动升级
                UpLevel.GainExp(monster.ExpReward);
                break;
            }
            
            // 怪物回合
            if (monsterCanBattle && monster.Hp > 0)
            {
                // 隐式防御：玩家等级作为减伤
                double damage = monster.Attack - PlayerStatistics.Level;

                if (damage < 1)
                {
                    damage = 1;
                }

                PlayerStatistics.Hp -= damage;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{monster.Name} 反击你，造成 {damage} 点伤害");
                Console.ResetColor();
            }

            // 失败判定
            if (PlayerStatistics.Hp <= 0)
            {
                PlayerStatistics.Hp = 0;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n你倒下了……");
                Console.ResetColor();

                // 死亡惩罚
                PlayerStatistics.Hp = PlayerStatistics.MaxHp;
                PlayerStatistics.Exp -= PlayerStatistics.Exp * 0.1;
                break;
            }
            
            Console.WriteLine("\n按任意键进入下一回合");
            Console.ReadKey();
        }
    }
}
