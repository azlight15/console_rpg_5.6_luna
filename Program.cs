using System;

namespace Console_RPG;

/// <summary>
/// 游戏入口与主菜单。
/// Program 持有当前玩家实例，并将它显式传给需要操作角色状态的系统。
/// </summary>
public static class Program
{
    private static bool _running = true;
    private static readonly Player _player = new();

    private static void Main()
    {
        StartMenu();
        GameConfirmed();

        while (_running)
        {
            OptionsMenu();
        }
    }

    /// <summary>创建新游戏时读取玩家名称。</summary>
    private static void StartMenu()
    {
        Console.Clear();
        Console.WriteLine("===== 欢迎来到 Console RPG =====");
        Console.WriteLine("这是一个纯控制台回合制 RPG。");
        Console.WriteLine("================================");
        Console.Write("请输入你的名字：");

        while (true)
        {
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                _player.Name = name.Trim();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("名字不能为空，请重新输入：");
            Console.ResetColor();
        }
    }

    /// <summary>展示新游戏的初始状态。</summary>
    private static void GameConfirmed()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine("角色创建完成！");
        Console.WriteLine($"名字：{_player.Name}");
        Console.WriteLine($"等级：{_player.Level}");
        Console.WriteLine($"HP：{_player.Hp}/{_player.MaxHp}");
        Console.WriteLine($"攻击力：{_player.Attack}");
        Console.WriteLine("================================");
        Console.WriteLine("按任意键开始游戏");
        Console.ReadKey(true);
    }

    /// <summary>显示主菜单，并把当前玩家交给对应系统处理。</summary>
    private static void OptionsMenu()
    {
        Console.Clear();
        Console.WriteLine("=========================");
        Console.WriteLine("        Console RPG");
        Console.WriteLine("=========================");
        Console.WriteLine("1. 开始战斗");
        Console.WriteLine("2. 治疗");
        Console.WriteLine("3. 查看状态");
        Console.WriteLine("4. 存档");
        Console.WriteLine("5. 读档");
        Console.WriteLine("6. 退出游戏");
        Console.WriteLine("=========================");
        Console.Write("请选择：");

        while (true)
        {
            if (!int.TryParse(Console.ReadLine(), out int option) || option is < 1 or > 6)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("输入无效，请输入 1-6：");
                Console.ResetColor();
                continue;
            }

            switch (option)
            {
                case 1:
                    Battle.StartBattle(_player);
                    return;
                case 2:
                    Heal.Use(_player);
                    return;
                case 3:
                    ShowStatus.Display(_player);
                    return;
                case 4:
                    SaveManager.Save(_player);
                    return;
                case 5:
                    SaveManager.Load(_player);
                    return;
                case 6:
                    Console.Clear();
                    Console.WriteLine("感谢游玩 Console RPG！下次再见，勇者！");
                    _running = false;
                    return;
            }
        }
    }

    /// <summary>暂停当前界面，等待玩家返回主菜单。</summary>
    public static void Loading()
    {
        Console.WriteLine("\n按任意键返回主菜单...");
        Console.ReadKey(true);
    }
}
