namespace Console_RPG;

/// <summary>
/// 战斗中敌人的数据模型。
///
/// Luna v0.3.0 开始加入怪物战斗定位数据。
/// 怪物不再只是名称和数值，而会逐渐拥有自己的战斗特点。
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

    /// <summary>
    /// 怪物闪避概率。
    /// 用于表现不同怪物的战斗风格，例如野狼更灵活。
    /// </summary>
    public double EvasionRate { get; set; }
}
