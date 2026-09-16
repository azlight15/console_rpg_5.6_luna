using System;

namespace Console_RPG;

/// <summary>
/// v0.5.0 装备与背包界面。
/// 玩家可以查看库存、装备已有物品，并直接看到最终属性变化。
/// </summary>
public static class EquipmentManager
{
    public static void InitializeStarterEquipment(Player player)
    {
        Equipment weapon = Create("木剑", "武器", 5, 0, 0, 0, "一把普通的木剑。", "普通");
        Equipment armor = Create("旅行皮甲", "防具", 0, 25, 0, 0.02, "适合新手旅行者的轻型皮甲。", "普通");
        player.EquipWeapon(weapon);
        player.EquipArmor(armor);
    }

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
                case "1":
                    ShowInventory(player);
                    break;
                case "2":
                    EquipItem(player);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("输入无效，请选择 1-3。");
                    Program.Loading();
                    break;
            }
        }
    }

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
            }
        }

        Program.Loading();
    }

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
