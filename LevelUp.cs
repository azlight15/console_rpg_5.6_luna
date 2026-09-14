using System;

namespace Console_RPG;

/*
    UpLevel 负责管理玩家经验获取与升级逻辑。
    当玩家获得经验后，会自动判断是否满足升级条件并执行升级。
*/
public static class UpLevel
{
    /*
        增加玩家经验值
        并检查是否需要连续升级
    */
    public static void GainExp(double getExp)
    {
        // 增加经验
        PlayerStatistics.Exp += getExp;
        Console.WriteLine($"获得经验 {getExp} 点");

        // 如果经验溢出则连续升级
        while (PlayerStatistics.Exp >= PlayerStatistics.ExpToNextLevel)
        {
            LevelUp();
        } // 这个是加了双重保险
    }

    /*
        执行一次等级提升
        提升等级并刷新玩家属性
    */
    private static void LevelUp()
    {
        while (PlayerStatistics.Exp >= PlayerStatistics.ExpToNextLevel)
        {
            // 扣除本级所需经验
            PlayerStatistics.Exp -= PlayerStatistics.ExpToNextLevel;

            // 等级提升
            PlayerStatistics.Level++;

            // 提升属性
            PlayerStatistics.MaxHp += 20;     // 最大生命增加
            PlayerStatistics.Attack += 5;     // 攻击力增加
            PlayerStatistics.Hp = PlayerStatistics.MaxHp; // 回满血量
        
            Console.WriteLine(" ");
            Console.WriteLine("🎉 升级了！");
            Console.WriteLine($"当前等级：{PlayerStatistics.Level}");
            Console.WriteLine("最大HP +20\n攻击值 +5\nHP已回满");
        
            Program.Loading();
        }
    }
}