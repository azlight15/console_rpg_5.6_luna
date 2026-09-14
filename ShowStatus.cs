using System;

namespace Console_RPG;

public class ShowStatus
{
    /*
        显示玩家当前状态
    */
    public static void _ShowStatus()
    {
        Console.Clear();
        Console.WriteLine($"你的名字是：{PlayerStatistics.Name}");
        Console.WriteLine($"等级：{PlayerStatistics.Level}");
        Console.WriteLine($"Exp:{PlayerStatistics.Exp}/{PlayerStatistics.ExpToNextLevel}");
        Console.WriteLine($"HP：{PlayerStatistics.Hp}/{PlayerStatistics.MaxHp}");
        Console.WriteLine($"攻击值：{PlayerStatistics.Attack}");
                
        Program.Loading();
    }
}