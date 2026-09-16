using System;

namespace Console_RPG;

/// <summary>
/// 负责生成装备。
/// v0.5.0 使用少量固定装备模板配合稀有度倍率，避免装备系统一开始就过度复杂。
/// </summary>
public static class EquipmentFactory
{
    private static readonly string[] Rarities = { "普通", "稀有", "史诗" };

    public static Equipment CreateRandomDrop(MonsterStatistics monster)
    {
        int template = Random.Shared.Next(1, 7);
        string rarity = RollRarity();
        double multiplier = rarity switch
        {
            "稀有" => 1.35,
            "史诗" => 1.8,
            _ => 1.0
        };

        Equipment equipment = template switch
        {
            1 => Create("猎人短剑", "武器", 7, 0, 0.02, 0, "轻巧的短剑。"),
            2 => Create("精钢剑", "武器", 12, 0, 0.01, 0, "打磨精良的钢剑。"),
            3 => Create("狼牙刃", "武器", 9, 0, 0.05, 0.02, "以野兽材料制作的利刃。"),
            4 => Create("旅行皮甲", "防具", 0, 25, 0, 0.03, "轻便的旅行护甲。"),
            5 => Create("铁甲", "防具", 0, 45, 0, 0, "厚重的铁制护甲。"),
            6 => Create("法师长袍", "防具", 0, 30, 0.03, 0.05, "适合施法者的轻型长袍。"),
            _ => throw new InvalidOperationException("未知装备模板。")
        };

        equipment.Rarity = rarity;
        equipment.AttackBonus *= multiplier;
        equipment.HpBonus *= multiplier;
        equipment.CriticalRateBonus *= multiplier;
        equipment.EvasionRateBonus *= multiplier;
        equipment.Name = $"[{rarity}] {equipment.Name}";

        return equipment;
    }

    private static string RollRarity()
    {
        int roll = Random.Shared.Next(100);
        return roll switch
        {
            < 70 => Rarities[0],
            < 95 => Rarities[1],
            _ => Rarities[2]
        };
    }

    private static Equipment Create(
        string name,
        string type,
        double attack,
        double hp,
        double critical,
        double evasion,
        string description)
    {
        return new Equipment
        {
            Name = name,
            Type = type,
            AttackBonus = attack,
            HpBonus = hp,
            CriticalRateBonus = critical,
            EvasionRateBonus = evasion,
            Description = description
        };
    }
}
