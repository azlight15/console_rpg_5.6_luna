using System;
using System.Collections.Generic;

namespace Console_RPG;

/// <summary>
/// 管理当前版本的装备目录与装备界面。
///
/// v0.4.0 暂不引入完整背包系统，因此玩家可以从固定目录中直接选择并装备物品。
/// 后续版本可以在此基础上加入掉落、背包和装备稀有度。
/// </summary>
public static class EquipmentManager
{
    /// <summary>初始化新角色的默认装备，确保新游戏一开始就能看到装备效果。</summary>
    public static void InitializeStarterEquipment(Player player)
    {
        player.EquipWeapon(new Equipment
        {
            Name = "木剑",
            Type = "武器",
            AttackBonus = 5,
            Description = "一把普通的木剑。"
        });

        player.EquipArmor(new Equipment
        {
            Name = "旅行皮甲",
            Type = "防具",
            HpBonus = 25,
            Description = "适合新手旅行者的轻型皮甲。"
        });
    }

    /// <summary>
    /// 显示装备目录，并允许玩家立即更换当前装备。
    /// 这是 v0.4.0 的简化装备入口，暂不处理物品持有数量。
    /// </summary>
    public static void ShowMenu(Player player)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== 装备管理 ==========");
            Console.WriteLine($"当前武器：{player.Weapon?.Name ?? "无"}");
            Console.WriteLine($"当前防具：{player.Armor?.Name ?? "无"}");
            Console.WriteLine("------------------------------");
            Console.WriteLine("1. 木剑        攻击 +5");
            Console.WriteLine("2. 铁剑        攻击 +15");
            Console.WriteLine("3. 旅行皮甲    HP +25");
            Console.WriteLine("4. 铁甲        HP +60");
            Console.WriteLine("5. 返回");
            Console.Write("请选择：");

            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    player.EquipWeapon(CreateWeapon("木剑", 5, "一把普通的木剑。"));
                    Console.WriteLine("已装备木剑。");
                    break;
                case "2":
                    player.EquipWeapon(CreateWeapon("铁剑", 15, "可靠的铁制武器。"));
                    Console.WriteLine("已装备铁剑。");
                    break;
                case "3":
                    player.EquipArmor(CreateArmor("旅行皮甲", 25, "适合新手旅行者的轻型皮甲。"));
                    Console.WriteLine("已装备旅行皮甲。");
                    break;
                case "4":
                    player.EquipArmor(CreateArmor("铁甲", 60, "沉重但可靠的铁制护甲。"));
                    Console.WriteLine("已装备铁甲。");
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("输入无效，请选择 1-5。");
                    break;
            }

            Program.Loading();
        }
    }

    private static Equipment CreateWeapon(string name, double attackBonus, string description)
    {
        return new Equipment
        {
            Name = name,
            Type = "武器",
            AttackBonus = attackBonus,
            Description = description
        };
    }

    private static Equipment CreateArmor(string name, double hpBonus, string description)
    {
        return new Equipment
        {
            Name = name,
            Type = "防具",
            HpBonus = hpBonus,
            Description = description
        };
    }
}
