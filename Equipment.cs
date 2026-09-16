namespace Console_RPG;

/// <summary>
/// 玩家可以获得、收藏和装备的物品。
/// v0.5.0 开始加入稀有度与额外属性，装备不直接修改玩家基础属性。
/// 最终属性仍由 Player 在运行时统一计算。
/// </summary>
public class Equipment
{
    public string Name { get; set; } = "无装备";
    public string Type { get; set; } = "无";
    public string Rarity { get; set; } = "普通";
    public double AttackBonus { get; set; }
    public double HpBonus { get; set; }
    public double CriticalRateBonus { get; set; }
    public double EvasionRateBonus { get; set; }
    public string Description { get; set; } = "";

    public string GetAttributeText()
    {
        return $"攻击 +{AttackBonus:0.#} | HP +{HpBonus:0.#} | 暴击 +{CriticalRateBonus:P0} | 闪避 +{EvasionRateBonus:P0}";
    }
}
