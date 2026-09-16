using System;

namespace Console_RPG;

// 游戏入口。
// 这个类只负责三件事：启动游戏、显示菜单、把玩家的选择交给其他系统。
// 战斗、装备、商店、存档等具体工作都不放在这里，避免 Program 变成什么都管的“大杂烩”。
public static class Program
{
    private static bool _running = true;
    private static readonly Player Player = new();

    // 程序从这里开始。
    private static void Main()
    {
        // 新角色先获得默认装备和技能。
        // 如果玩家随后读取存档，存档里的内容会覆盖这些默认数据。
        EquipmentManager.InitializeStarterEquipment(Player);
        SkillSystem.InitializeStarterSkills(Player);

        StartMenu();
        GameConfirmed();

        // 游戏没有结束之前，一直显示主菜单。
        while (_running)
        {
            OptionsMenu();
        }
    }

    // 启动菜单：有存档就让玩家选择读档或创建新角色，没有存档就直接创建角色。
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
                        if (SaveManager.Load(Player)) return;
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

    // 创建新角色，并且不允许玩家把名字留空。
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
                Player.Name = name.Trim();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("名字不能为空，请重新输入：");
            Console.ResetColor();
        }
    }

    // 显示角色当前状态，让玩家确认自己加载/创建的是哪个角色。
    private static void GameConfirmed()
    {
        if (!_running) return;

        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine("角色准备完成！");
        Console.WriteLine($"名字：{Player.Name}");
        Console.WriteLine($"等级：{Player.Level}");
        Console.WriteLine($"HP：{Player.Hp:0.#}/{Player.FinalMaxHp:0.#}");
        Console.WriteLine($"攻击力：{Player.FinalAttack:0.#}");
        Console.WriteLine($"金币：{Player.Gold}");
        Console.WriteLine($"技能点：{Player.SkillPoints}/{Player.MaxSkillPoints}");
        Console.WriteLine($"武器：{Player.Weapon?.Name ?? "无"}");
        Console.WriteLine($"防具：{Player.Armor?.Name ?? "无"}");
        Console.WriteLine($"技能：{Player.Skills.Count} 个");
        Console.WriteLine($"治疗资源：{Player.TreatmentCount}（每次恢复 {Player.Treatment:0.#} HP）");
        Console.WriteLine("================================");
        Console.WriteLine("按任意键开始游戏");
        Console.ReadKey(true);
    }

    // 主菜单本身不处理游戏逻辑，只负责把选择交给对应的系统。
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
                    Battle.StartBattle(Player);
                    return;
                case 2:
                    Heal.Use(Player);
                    return;
                case 3:
                    ShowStatus.Display(Player);
                    return;
                case 4:
                    EquipmentManager.ShowMenu(Player);
                    return;
                case 5:
                    Shop.ShowMenu(Player);
                    return;
                case 6:
                    SaveManager.Save(Player);
                    return;
                case 7:
                    SaveManager.Load(Player);
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

    // 所有菜单都可以调用这个方法暂停画面，避免提示一闪而过。
    public static void Loading()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}
