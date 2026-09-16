using System;

namespace Console_RPG;

/// <summary>
/// 游戏入口与主菜单。
///
/// Program 的职责很简单：启动游戏、创建/读取玩家，以及把菜单选择交给对应系统。
/// 它不应该自己计算伤害、升级或装备属性，否则主入口会逐渐变成“万能类”。
/// </summary>
public static class Program
{
    private static bool _running = true;
    private static readonly Player _player = new();

    /// <summary>程序入口：初始化新角色默认内容，然后进入启动菜单和主循环。</summary>
    private static void Main()
    {
        // 新角色需要有最基本的装备和技能；如果随后读取旧档，这些内容会被存档状态覆盖。
        EquipmentManager.InitializeStarterEquipment(_player);
        SkillSystem.InitializeStarterSkills(_player);

        StartMenu();
        GameConfirmed();

        // 主循环只负责不断显示菜单，具体功能全部交给独立系统。
        while (_running)
        {
            OptionsMenu();
        }
    }

    /// <summary>
    /// 游戏启动菜单。
    /// 有存档时先让玩家选择读取还是创建新角色；没有存档则直接创建角色。
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
                        if (SaveManager.Load(_player)) return;
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

    /// <summary>创建新角色，并验证玩家姓名不能是空白。</summary>
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

    /// <summary>显示角色初始化/读档后的状态，让玩家知道当前角色是什么样子。</summary>
    private static void GameConfirmed()
    {
        if (!_running) return;

        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine("角色准备完成！");
        Console.WriteLine($"名字：{_player.Name}");
        Console.WriteLine($"等级：{_player.Level}");
        Console.WriteLine($"HP：{_player.Hp:0.#}/{_player.FinalMaxHp:0.#}");
        Console.WriteLine($"攻击力：{_player.FinalAttack:0.#}");
        Console.WriteLine($"金币：{_player.Gold}");
        Console.WriteLine($"技能点：{_player.SkillPoints}/{_player.MaxSkillPoints}");
        Console.WriteLine($"武器：{_player.Weapon?.Name ?? "无"}");
        Console.WriteLine($"防具：{_player.Armor?.Name ?? "无"}");
        Console.WriteLine($"技能：{_player.Skills.Count} 个");
        Console.WriteLine($"治疗资源：{_player.TreatmentCount}（每次恢复 {_player.Treatment:0.#} HP）");
        Console.WriteLine("================================");
        Console.WriteLine("按任意键开始游戏");
        Console.ReadKey(true);
    }

    /// <summary>
    /// 主菜单。
    /// 每个选项只负责调用一个系统，然后把控制权交回主循环。
    /// </summary>
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
        Console.WriteLine("5. 城镇商店");
        Console.WriteLine("6. 存档");
        Console.WriteLine("7. 读档");
        Console.WriteLine("8. 删除档案");
        Console.WriteLine("9. 退出游戏");
        Console.WriteLine("=========================");
        Console.Write("请选择：");

        while (true)
        {
            if (!int.TryParse(Console.ReadLine(), out int option) || option is < 1 or > 9)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("输入无效，请输入 1-9：");
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
                    Shop.ShowMenu(_player);
                    return;
                case 6:
                    SaveManager.Save(_player);
                    return;
                case 7:
                    SaveManager.Load(_player);
                    return;
                case 8:
                    SaveManager.Delete();
                    return;
                case 9:
                    Console.Clear();
                    Console.WriteLine("感谢游玩 Console RPG！下次再见，勇者！");
                    _running = false;
                    return;
            }
        }
    }

    /// <summary>统一的菜单暂停。</summary>
    public static void Loading()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
