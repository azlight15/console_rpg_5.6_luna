namespace Console_RPG;

/// <summary>
/// 战斗中敌人的数据模型。
///
/// MonsterStatistics 只保存“这只怪物现在是什么状态”，例如等级、HP、攻击和奖励。
/// 具体如何生成怪物由 MonsterFactory 负责，具体如何战斗由 Battle 负责。
/// 这种职责划分可以让后续增加 Boss、特殊技能时不用把所有代码塞进一个文件。
/// </summary>
public class MonsterStatistics
{
    /// <summary>战斗中显示的怪物名称。</summary>
    public string Name { get; set; } = "";

    /// <summary>怪物的战斗定位，例如肉盾、高速、强攻。</summary>
    public string Type { get; set; } = "普通";

    /// <summary>本次生成出来的实际等级。</summary>
    public int Level { get; set; }

    /// <summary>当前 HP。</summary>
    public double Hp { get; set; }

    /// <summary>最大 HP。</summary>
    public double MaxHp { get; set; }

    /// <summary>基础攻击能力。</summary>
    public double Attack { get; set; }

    /// <summary>击败后给予的经验。</summary>
    public double ExpReward { get; set; }

    /// <summary>击败后给予的金币。</summary>
    public int GoldReward { get; set; }

    /// <summary>怪物闪避概率。</summary>
    public double EvasionRate { get; set; }
}
