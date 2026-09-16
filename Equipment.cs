namespace Console_RPG;

/// <summary>
/// 一件装备的数据模型。
///
/// Equipment 只描述装备本身的属性，不直接修改 Player。
/// 玩家装备它以后，Player 的 FinalAttack、FinalMaxHp 等计算属性才会把这些加成算进去。
/// 这样装备被换下时不会留下“残余属性”。
/// </summary>
public class Equipment
{
    /// <summary>装备名称。</summary>
    public string Name { get; set; } = "无装备";

    /// <summary>装备类型，目前主要分为“武器”和“防具”。</summary>
    public string Type { get; set; } = "无";

    /// <summary>稀有度，例如普通、稀有、史诗。</summary>
    public string Rarity { get; set; } = "普通";

    /// <summary>提供的攻击力加成。</summary>
    public double AttackBonus { get; set; }

    /// <summary>提供的最大 HP 加成。</summary>
    public double HpBonus { get; set; }

    /// <summary>提供的暴击率加成，使用 0-1 的小数表示。</summary>
    public double CriticalRateBonus { get; set; }

    /// <summary>提供的闪避率加成，使用 0-1 的小数表示。</summary>
    public double EvasionRateBonus { get; set; }

    /// <summary>给玩家看的装备描述。</summary>
    public string Description { get; set; } = "";

    /// <summary>把装备主要属性格式化成一行文字，供背包和掉落提示复用。</summary>
    public string GetAttributeText()
    {
        return $"攻击 +{AttackBonus:0.#} | HP +{HpBonus:0.#} | 暴击 +{CriticalRateBonus:P0} | 闪避 +{EvasionRateBonus:P0}";
    }
}
