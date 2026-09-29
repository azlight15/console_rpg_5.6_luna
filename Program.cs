using System;

namespace Console_RPG;

// 游戏入口。
// Program 只负责启动流程和菜单分发；具体游戏规则交给对应系统。
public static class Program
{
    private static bool _running = true;
    private static readonly Player Player = new();

    private static void Main()
    {
        EquipmentManager.InitializeStarterEquipment(Player);
        SkillSystem.InitializeStarterSkills(Player);

        StartMenu();
        GameConfirmed();

        while (_running)
            OptionsMenu();
    }

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
        Console.WriteLine($"战斗技能点：{Player.SkillPoints}/{Player.MaxSkillPoints}");
        Console.WriteLine($"武器：{Player.Weapon?.Name ?? "无"}");
        Console.WriteLine($"防具：{Player.Armor?.Name ?? "无"}");
        Console.WriteLine($"已学习技能：{Player.Skills.Count} 个");
        Console.WriteLine($"治疗资源：{Player.TreatmentCount}（每次恢复 {Player.Treatment:0.#} HP）");
        Console.WriteLine("================================");
        Console.WriteLine("按任意键开始游戏");
        Console.ReadKey(true);
    }

    // 主菜单把成长、战斗、装备、商店和存档分成清晰的区域。
    // 0 统一作为“返回/退出当前菜单”的按键，避免不同菜单使用不同返回键。
    private static void OptionsMenu()
    {
        Console.Clear();
        Console.WriteLine("==============================");
        Console.WriteLine("          Console RPG");
        Console.WriteLine("==============================");
        Console.WriteLine("【冒险】");
        Console.WriteLine("1. 开始战斗");
        Console.WriteLine("------------------------------");
        Console.WriteLine("【成长】");
        Console.WriteLine("2. 查看状态");
        Console.WriteLine("3. 装备管理");
        Console.WriteLine("4. 技能管理");
        Console.WriteLine("------------------------------");
        Console.WriteLine("【城镇】");
        Console.WriteLine("5. 治疗");
        Console.WriteLine("6. 城镇商店");
        Console.WriteLine("------------------------------");
        Console.WriteLine("【存档】");
        Console.WriteLine("7. 存档");
        Console.WriteLine("8. 读档");
        Console.WriteLine("9. 删除档案");
        Console.WriteLine("------------------------------");
        Console.WriteLine("0. 退出游戏");
        Console.WriteLine("==============================");
        Console.Write("请选择：");

        while (true)
        {
            if (!int.TryParse(Console.ReadLine(), out int option) || option is < 0 or > 9)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("输入无效，请输入 0-9：");
                Console.ResetColor();
                continue;
            }

            switch (option)
            {
                case 1:
                    Battle.StartBattle(Player);
                    return;
                case 2:
                    ShowStatus.Display(Player);
                    return;
                case 3:
                    EquipmentManager.ShowMenu(Player);
                    return;
                case 4:
                    SkillSystem.ShowMenu(Player);
                    return;
                case 5:
                    Heal.Use(Player);
                    return;
                case 6:
                    Shop.ShowMenu(Player);
                    return;
                case 7:
                    SaveManager.Save(Player);
                    return;
                case 8:
                    SaveManager.Load(Player);
                    return;
                case 9:
                    SaveManager.Delete();
                    return;
                case 0:
                    Console.Clear();
                    Console.WriteLine("感谢游玩 Console RPG！下次再见，勇者！");
                    _running = false;
                    return;
            }
        }
    }

    public static void Loading()
    {
        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey(true);
    }
}