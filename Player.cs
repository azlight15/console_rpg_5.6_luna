namespace Console_RPG;

/// <summary>
/// 保存当前玩家状态。
/// 目前项目仍使用简单的全局状态模型；后续如果系统继续扩张，可以再演进为 Player 实体。
/// </summary>
public static class PlayerStatistics
{
    public static string Name { get; set; } = "";
    public static int Level { get; set; } = 1;
    public static double Exp { get; set; }

    /// <summary>当前等级对应的升级经验需求。</summary>
    public static double ExpToNextLevel => Level * 100;

    public static double Hp { get; set; } = 100;
    public static double MaxHp { get; set; } = 100;
    public static double Attack { get; set; } = 15;
    public static double Treatment { get; set; } = 50;
}
