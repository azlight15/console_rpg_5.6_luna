using System;

namespace Console_RPG;

// 城镇商店。
// v1 先做一个简单、固定的商店，让游戏形成“打怪赚金币 → 花钱变强”的循环。
// 暂时不做商店刷新、随机价格等复杂经济系统。
public static class Shop
{
    private const int TreatmentPrice = 20;
    private const int SkillPointPrice = 30;

    // 显示商店主菜单。
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

    // 买治疗资源。金币不足时什么都不会改变。
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

    // 买技能点。已经满点时不允许浪费金币购买。
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

    // 买一把固定属性的武器，只放进背包，不自动替换当前武器。
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

    // 买一件固定属性的防具，只放进背包，不自动替换当前防具。
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
