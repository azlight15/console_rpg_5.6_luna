using System;

namespace Console_RPG;

public class Heal
{
    /*
        治疗功能
        按照 Treatment 数值恢复玩家血量
    */
    public static void _Heal()
    {
        PlayerStatistics.Hp += PlayerStatistics.Treatment;

        // 防止血量超过最大值
        if (PlayerStatistics.Hp > PlayerStatistics.MaxHp)
        {
            PlayerStatistics.Hp = PlayerStatistics.MaxHp;
        }

        Console.Clear();
        Console.WriteLine($"你治疗了自己，恢复 {PlayerStatistics.Treatment} HP");

        Program.Loading();
    }
}