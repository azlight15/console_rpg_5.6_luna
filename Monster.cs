namespace Console_RPG;

/// <summary>
/// 战斗中敌人的数据模型。
///
/// v0.3 开始为怪物加入类型信息，方便未来扩展特殊能力、技能和不同战斗行为。
/// 当前阶段仍主要保存基础战斗数据。
/// </summary>
public class MonsterStatistics
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "普通";
    public int Level { get; set; }
    public double Hp { get; set; }
    public double MaxHp { get; set; }
    public double Attack { get; set; }
    public double ExpReward { get; set; }
}
