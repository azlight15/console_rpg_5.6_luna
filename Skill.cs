namespace Console_RPG;

/// <summary>
/// 表示玩家或怪物可以使用的技能。
///
/// v0.4.0 首先建立技能数据模型，具体释放逻辑将在战斗系统中接入。
/// </summary>
public class Skill
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public double DamageMultiplier { get; set; } = 1;
    public int Cooldown { get; set; }
}
