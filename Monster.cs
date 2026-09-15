namespace Console_RPG;

/// <summary>
/// 战斗中敌人的数据模型。
/// 该类只保存状态，不负责生成规则或战斗流程。
/// </summary>
public class MonsterStatistics
{
    public string Name { get; set; } = "";
    public int Level { get; set; }
    public double Hp { get; set; }
    public double MaxHp { get; set; }
    public double Attack { get; set; }
    public double ExpReward { get; set; }
}
