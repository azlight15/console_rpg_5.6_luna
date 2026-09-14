using System;

namespace Console_RPG;

/*
    Program 是游戏的入口类。
    负责初始化游戏、显示菜单，并控制整个游戏主循环。
*/
public static class Program
{
    // 控制游戏是否继续运行
    private static bool _running = true;
    
    /*
        游戏入口方法
        初始化菜单并进入主循环
    */
    private static void Main()
    {
        StartMenu();     // 输入玩家名字
        GameConfirmed(); // 显示初始状态确认

        // 游戏主循环
        while (_running)
        {
            OptionsMenu();
        }
    }
    
    /*
        开始菜单
        负责获取玩家名字并做基本校验
    */
    private static void StartMenu()
    {
        Console.Clear();
        Console.WriteLine("===== 欢迎来玩Console RPG游戏 =====");
        Console.WriteLine("此游戏是控制台游戏，没有ui");
        Console.WriteLine("那么接下来请好好享受游戏吧！");
        Console.WriteLine("=================================");
        Console.Write("请输入你的名字（取了名字后不能更改！）：");

        PlayerStatistics.Name = Console.ReadLine()!;

        // 防止输入空字符串或空格
        while (string.IsNullOrWhiteSpace(PlayerStatistics.Name))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\n不能只输入空格或直接回车！请重新输入你的名字：");
            Console.ResetColor();
            PlayerStatistics.Name = Console.ReadLine()!;
        }
    }

    /*
        游戏开始前的确认页面
        展示玩家初始属性
    */
    private static void GameConfirmed()
    {
        Console.Clear();
        Console.WriteLine("=================================");
        Console.WriteLine("正式游玩之前先看一下玩家状态");
        Console.WriteLine($"你的名字是：{PlayerStatistics.Name}");
        Console.WriteLine($"等级：{PlayerStatistics.Level}");
        Console.WriteLine($"HP：{PlayerStatistics.Hp}/{PlayerStatistics.MaxHp}");
        Console.WriteLine($"攻击值：{PlayerStatistics.Attack}");
        Console.WriteLine($"那么祝你玩的开心，{PlayerStatistics.Name}勇者！");
        Console.WriteLine("=================================");
        Console.WriteLine("按下任意键开始游戏");
        Console.ReadKey();
    }

    /*
        主菜单页面
        根据玩家输入分发到不同功能模块
    */
    private static void OptionsMenu()
    {
        Console.Clear();
        Console.WriteLine("=========================");
        Console.WriteLine("Console RPG");
        Console.WriteLine("请选择选项：");
        Console.WriteLine("1.开始对战");
        Console.WriteLine("2.升级");
        Console.WriteLine("3.治疗");
        Console.WriteLine("4.查看状态");
        Console.WriteLine("5.存档");
        Console.WriteLine("6.读档");
        Console.WriteLine("7.退出游戏");
        Console.WriteLine("==========================");
        Console.Write("请选择选项：");

        int options = Convert.ToInt32(Console.ReadLine());

        switch (options)
        {
            case 1:
                Battle.StartBattle();   // 进入战斗模块
                break;
            case 2:
                UpLevel.GainExp(100);    // 测试用升级
                break;
            case 3:
                Heal._Heal();              // 治疗玩家
                break;
            case 4:
                ShowStatus._ShowStatus();  // 显示玩家状态
                break;
            case 5:
                SaveManager.Save();     // 保存游戏
                break;
            case 6:
                SaveManager.Load();     // 读取存档
                break;
            case 7:
                Console.Clear();
                Console.WriteLine("欢迎再次玩Console RPG，谢谢");
                Console.WriteLine("那么下次再见，勇者！");
                _running = false;       // 结束主循环
                break;
            default:
                // 输入非法时重新输入
                while (options > 7 || options < 1)
                {
                    Console.Write("\n输入错误，请重新输入：");
                    options = Convert.ToInt32(Console.ReadLine());
                }
                break;
        }
    }

    /*
        等待用户输入后返回主菜单
    */
    public static void Loading()
    {
        Console.WriteLine("按下任意键回到选择页面");
        Console.ReadKey();
    }
}
