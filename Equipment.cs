namespace Console_RPG;

/// <summary>
/// 表示玩家可以装备的物品。
///
/// 装备不会直接修改玩家基础属性，而是在计算最终属性时提供额外加成。
/// 这样可以避免更换装备时产生属性混乱。
/// </summary>
public class Equipment
{
    public string Name { get; set; } = "无装备";
    public string Type { get; set; } = "无";
    public double AttackBonus { get; set; }
    public double HpBonus { get; set; }
    public string Description { get; set; } = "";
}
