namespace Console_RPG;

// 装备数据。
// 装备只保存“自己有什么属性”，不会直接修改 Player 的基础属性。
// Player 会根据当前装备计算最终攻击、最终生命、暴击和闪避。
public class Equipment
{
    // 装备名称，例如“精钢剑”或“[史诗] 狼牙刃”。
    public string Name { get; set; } = "无装备";

    // 装备类型。目前只有武器和防具两种。
    public string Type { get; set; } = "无";

    // 稀有度会影响装备属性。当前有普通、稀有、史诗三档。
    public string Rarity { get; set; } = "普通";

    // 装备提供的攻击力加成。
    public double AttackBonus { get; set; }

    // 装备提供的最大 HP 加成。
    public double HpBonus { get; set; }

    // 装备提供的额外暴击率。0.05 就代表增加 5% 暴击率。
    public double CriticalRateBonus { get; set; }

    // 装备提供的额外闪避率。0.05 就代表增加 5% 闪避率。
    public double EvasionRateBonus { get; set; }

    // 给玩家看的文字说明。
    public string Description { get; set; } = "";

    // 把装备的主要数值整理成一行，方便背包和掉落提示直接使用。
    public string GetAttributeText()
    {
        return $"[{Rarity}] 攻击 +{AttackBonus:0.#} | HP +{HpBonus:0.#} | 暴击 +{CriticalRateBonus:P0} | 闪避 +{EvasionRateBonus:P0}";
    }
}
