namespace Console_RPG;

/// <summary>
/// 表示游戏中的玩家角色及其当前状态。
/// 玩家数据通过实例传递给各个系统，避免依赖全局静态状态。
/// </summary>
public sealed class Player
{
    public string Name { get; set; } = "";
    public int Level { get; set; } = 1;
    public double Exp { get; set; }

    /// <summary>当前等级对应的升级经验需求。</summary>
    public double ExpToNextLevel => Level * 100;

    public double Hp { get; set; } = 100;
    public double MaxHp { get; set; } = 100;
    public double Attack { get; set; } = 15;
    public double Treatment { get; set; } = 50;
}
