using System;

namespace Console_RPG;

/// <summary>
/// 装备与背包界面。
///
/// EquipmentManager 负责和玩家进行“装备相关的 UI 交互”，例如查看背包和选择装备。
/// 真正的装备状态仍由 Player 保存，因此这个类不会自己维护一份“当前武器”。
/// </summary>
public static class EquipmentManager
{
    /// <summary>
    /// 给新角色创建基础装备。
    /// 这些装备直接放入 Player.Inventory，并同时设置为当前装备。
    /// </summary>
    public static void InitializeStarterEquipment(Player player)
    {
        Equipment weapon = Create("木剑", "武器", 5, 0, 0, 0, "一把普通的木剑。", "普通");
        Equipment armor = Create("旅行皮甲", "防具", 0, 25, 0, 0.02, "适合新手旅行者的轻型皮甲。", "普通");
        player.EquipWeapon(weapon);
        player.EquipArmor(armor);
    }

    /// <summary>装备主菜单：查看背包、装备物品或返回主菜单。</summary>
    public static void ShowMenu(Player player)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== 装备与背包 ==========");
            Console.WriteLine($"当前武器：{player.Weapon?.Name ?? "无"}");
            Console.WriteLine($"当前防具：{player.Armor?.Name ?? "无"}");
            Console.WriteLine($"最终攻击：{player.FinalAttack:0.#}");
            Console.WriteLine($"最终生命：{player.FinalMaxHp:0.#}");
            Console.WriteLine("------------------------------");
            Console.WriteLine("1. 查看背包");
            Console.WriteLine("2. 装备物品");
            Console.WriteLine("3. 返回");
            Console.Write("请选择：");

            switch (Console.ReadLine())
            {
                case "1": ShowInventory(player); break;
                case "2": EquipItem(player); break;
                case "3": return;
                default:
                    Console.WriteLine("输入无效，请选择 1-3。");
                    Program.Loading();
                    break;
            }
        }
    }

    /// <summary>逐件展示库存，并标记当前装备。</summary>
    private static void ShowInventory(Player player)
    {
        Console.Clear();
        Console.WriteLine("========== 背包 ==========");

        if (player.Inventory.Count == 0)
        {
            Console.WriteLine("背包为空。");
        }
        else
        {
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Equipment item = player.Inventory[i];
                string equipped = item == player.Weapon || item == player.Armor ? " [已装备]" : "";
                Console.WriteLine($"{i + 1}. {item.Name} ({item.Type}){equipped}");
                Console.WriteLine($"   {item.GetAttributeText()}");
                Console.WriteLine($"   {item.Description}");
            }
        }

        Program.Loading();
    }

    /// <summary>
    /// 从背包选择装备。
    /// Player.EquipFromInventory 会检查索引和装备类型，因此 UI 不需要重复实现属性修改逻辑。
    /// </summary>
    private static void EquipItem(Player player)
    {
        Console.Clear();
        Console.WriteLine("========== 装备物品 ==========");
        if (player.Inventory.Count == 0)
        {
            Console.WriteLine("背包为空。");
            Program.Loading();
            return;
        }

        for (int i = 0; i < player.Inventory.Count; i++)
        {
            Equipment item = player.Inventory[i];
            Console.WriteLine($"{i + 1}. {item.Name} ({item.Type}) - {item.GetAttributeText()}");
        }

        Console.WriteLine("0. 返回");
        Console.Write("请选择：");
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > player.Inventory.Count)
        {
            Console.WriteLine("输入无效。");
            Program.Loading();
            return;
        }

        if (choice == 0) return;

        Equipment selected = player.Inventory[choice - 1];
        if (player.EquipFromInventory(choice - 1))
            Console.WriteLine($"已装备：{selected.Name}");
        else
            Console.WriteLine("该物品无法装备。");

        Program.Loading();
    }

    /// <summary>创建基础装备对象，避免初始化新手装备时重复写属性对象。</summary>
    private static Equipment Create(string name, string type, double attack, double hp, double critical, double evasion, string description, string rarity)
    {
        return new Equipment
        {
            Name = name,
            Type = type,
            AttackBonus = attack,
            HpBonus = hp,
            CriticalRateBonus = critical,
            EvasionRateBonus = evasion,
            Description = description,
            Rarity = rarity
        };
    }
}
