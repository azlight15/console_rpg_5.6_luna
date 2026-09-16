using System;

namespace Console_RPG;

/// <summary>
/// 城镇商店。
///
/// v1.0 先采用固定商品，不做复杂的商店刷新、随机价格和货币类型。
/// 目的不是把经济系统一次做满，而是先建立完整的“战斗 → 获得金币 → 消费 → 变强”循环。
/// </summary>
public static class Shop
{
    private const int TreatmentPrice = 20;
    private const int SkillPointPrice = 30;

    /// <summary>显示商店主菜单。</summary>
    public static void ShowMenu(Player player)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== 城镇商店 ==========");
            Console.WriteLine($"金币：{player.Gold}");
            Console.WriteLine("------------------------------");
            Console.WriteLine($"1. 治疗药剂 +1（{TreatmentPrice} 金币）");
            Console.WriteLine($"2. 技能点药剂 +1（{SkillPointPrice} 金币）");
            Console.WriteLine("3. 猎人短剑（80 金币）");
            Console.WriteLine("4. 铁甲（100 金币）");
            Console.WriteLine("5. 返回");
            Console.Write("请选择：");

            switch (Console.ReadLine())
            {
                case "1": BuyTreatment(player); break;
                case "2": BuySkillPoint(player); break;
                case "3": BuyWeapon(player); break;
                case "4": BuyArmor(player); break;
                case "5": return;
                default:
                    Console.WriteLine("输入无效，请选择 1-5。");
                    Program.Loading();
                    break;
            }
        }
    }

    /// <summary>购买一次治疗资源。</summary>
    private static void BuyTreatment(Player player)
    {
        if (!player.TrySpendGold(TreatmentPrice))
        {
            Console.WriteLine("金币不足。");
            Program.Loading();
            return;
        }

        player.AddTreatmentCount(1);
        Console.WriteLine($"购买成功！剩余金币：{player.Gold}");
        Program.Loading();
    }

    /// <summary>购买技能点并立即恢复 1 点，不能超过技能点上限。</summary>
    private static void BuySkillPoint(Player player)
    {
        if (player.SkillPoints >= player.MaxSkillPoints)
        {
            Console.WriteLine("技能点已经满了，不需要购买。");
            Program.Loading();
            return;
        }

        if (!player.TrySpendGold(SkillPointPrice))
        {
            Console.WriteLine("金币不足。");
            Program.Loading();
            return;
        }

        player.RecoverSkillPoint();
        Console.WriteLine($"购买成功！当前技能点：{player.SkillPoints}/{player.MaxSkillPoints}");
        Program.Loading();
    }

    /// <summary>购买固定武器并放入背包，不会自动替换当前装备。</summary>
    private static void BuyWeapon(Player player)
    {
        const int price = 80;
        if (!player.TrySpendGold(price))
        {
            Console.WriteLine("金币不足。");
            Program.Loading();
            return;
        }

        player.AddEquipment(new Equipment
        {
            Name = "[商店] 猎人短剑",
            Type = "武器",
            Rarity = "普通",
            AttackBonus = 7,
            CriticalRateBonus = 0.02,
            Description = "商店出售的轻型短剑。"
        });

        Console.WriteLine("购买成功！装备已放入背包。");
        Program.Loading();
    }

    /// <summary>购买固定防具并放入背包，不会自动替换当前装备。</summary>
    private static void BuyArmor(Player player)
    {
        const int price = 100;
        if (!player.TrySpendGold(price))
        {
            Console.WriteLine("金币不足。");
            Program.Loading();
            return;
        }

        player.AddEquipment(new Equipment
        {
            Name = "[商店] 铁甲",
            Type = "防具",
            Rarity = "普通",
            HpBonus = 45,
            Description = "商店出售的厚重铁甲。"
        });

        Console.WriteLine("购买成功！装备已放入背包。");
        Program.Loading();
    }
}
