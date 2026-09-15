using System;

namespace Console_RPG;

/// <summary>
/// 游戏入口与主菜单。
/// Program 持有当前玩家实例，并将它显式传给需要操作角色状态的系统。
/// v0.4.0 开始在启动时检测本地档案，并在存档/读档时使用档案列表。
/// </summary>
public static class Program
{
    private static bool _running = true;
    private static readonly Player _player = new();

    private static void Main()
    {
        EquipmentManager.InitializeStarterEquipment(_player);
        SkillSystem.InitializeStarterSkills(_player);

        StartMenu();
        GameConfirmed();

        while (_running)
        {
            OptionsMenu();
        }
    }

    /// <summary>
    /// 启动时检测本地档案。
    /// 检测到档案时，先让玩家选择读取已有档案或创建新角色，避免读档前被强制要求注册名字。
    /// </summary>
    private static void StartMenu()
    {
        if (SaveManager.HasAnySave())
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== 欢迎来到 Console RPG =====");
                Console.WriteLine("检测到本地档案。");
                Console.WriteLine("1. 读取已有档案");
                Console.WriteLine("2. 创建新角色");
                Console.WriteLine("0. 退出游戏");
                Console.Write("请选择：");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (SaveManager.Load(_player))
                        {
                            return;
                        }
                        break;

                    case "2":
                        RegisterNewPlayer();
                        return;

                    case "0":
                        _running = false;
                        return;

                    default:
                        Console.WriteLine("输入无效，请选择 0-2。");
                        Loading();
                        break;
                }
            }
        }

        RegisterNewPlayer();
    }

    /// <summary>创建新游戏时读取玩家名称。</summary>
    private static void RegisterNewPlayer()
    {
        Console.Clear();
        Console.WriteLine("===== 创建新角色 =====");
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

    /// <summary>展示当前角色状态，包括装备和技能。</summary>
    private static void GameConfirmed()
    {
        if (!_running)
        {
            return;
        }

        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine("角色准备完成！");
        Console.WriteLine($"名字：{_player.Name}");
        Console.WriteLine($"等级：{_player.Level}");
        Console.WriteLine($"HP：{_player.Hp:0.#}/{_player.FinalMaxHp:0.#}");
        Console.WriteLine($"攻击力：{_player.FinalAttack:0.#}");
        Console.WriteLine($"武器：{_player.Weapon?.Name ?? "无"}");
        Console.WriteLine($"防具：{_player.Armor?.Name ?? "无"}");
        Console.WriteLine($"技能：{_player.Skills.Count} 个");
        Console.WriteLine($"治疗资源：{_player.TreatmentCount}（每次恢复 {_player.Treatment:0.#} HP）");
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
        Console.WriteLine("4. 装备管理");
        Console.WriteLine("5. 存档");
        Console.WriteLine("6. 读档");
        Console.WriteLine("7. 退出游戏");
        Console.WriteLine("=========================");
        Console.Write("请选择：");

        while (true)
        {
            if (!int.TryParse(Console.ReadLine(), out int option) || option is < 1 or > 7)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("输入无效，请输入 1-7：");
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
                    EquipmentManager.ShowMenu(_player);
                    return;
                case 5:
                    SaveManager.Save(_player);
                    return;
                case 6:
                    SaveManager.Load(_player);
                    return;
                case 7:
                    Console.Clear();
                    Console.WriteLine("感谢游玩 Console RPG！下次再见，勇者！");
                    _running = false;
                    return;
            }
        }
    }

    /// <summary>暂停当前界面，等待玩家返回上一级菜单。</summary>
    public static void Loading()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
