namespace Console_RPG;

/*
    PlayerStatistics 用于保存玩家的全局状态数据。
    包含等级、经验、血量、攻击力等核心属性。
    所有战斗、升级、治疗、存档模块都会直接读取或修改这里的数据。
*/
public static class PlayerStatistics
{
    public static string Name = null!;   // 玩家名字
    public static int Level = 1;         // 玩家等级
    public static double Exp = 0;        // 当前经验值

    // 当前等级升级所需经验
    public static double ExpToNextLevel => Level * 100;

    public static double Hp = 100;       // 当前血量
    public static double MaxHp = 100;    // 最大血量
    public static double Attack = 15;    // 攻击力
    public static double Treatment = 50; // 每次治疗恢复的血量
}