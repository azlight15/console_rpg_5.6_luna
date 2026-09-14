// ==========================
// 怪物工厂
// 负责根据玩家等级随机生成战斗怪物
// ==========================

using System;

namespace Console_RPG;

/*
    MonsterFactory 用于创建战斗中使用的怪物对象。
    每次调用都会随机生成一种怪物，并根据玩家等级进行数值缩放。
    同时存在一定概率生成精英怪物。
*/
public static class MonsterFactory
{
    /*
        对外提供获取怪物的入口
        每次访问都会创建一个新的怪物实例
    */
    public static MonsterStatistics Monster => CreateMonster();

    /*
        创建怪物实例
        根据玩家等级动态计算怪物属性
    */
    private static MonsterStatistics CreateMonster()
    {
        // 随机怪物类型（1~3）
        int type = Random.Shared.Next(1, 4);

        // 是否生成精英怪（10%概率）
        bool isElite = Random.Shared.Next(100) < 10;

        // 获取玩家等级用于数值缩放
        int playerLevel = PlayerStatistics.Level;
        
        MonsterStatistics monster = new MonsterStatistics();

        // 根据不同类型初始化怪物基础属性
        switch (type)
        {
            case 1:
                monster.Name = "史莱姆";
                monster.Level = Math.Max(1, playerLevel - 1);
                monster.MaxHp = 30 + monster.Level * 4;
                monster.Attack = 5 + monster.Level * 1;
                monster.ExpReward = 30 + monster.Level * 10;
                break;

            case 2:
                monster.Name = "哥布林";
                monster.Level = playerLevel;
                monster.MaxHp = 50 + monster.Level * 5;
                monster.Attack = 8 + monster.Level * 2;
                monster.ExpReward = 50 + monster.Level * 15;
                break;

            case 3:
                monster.Name = "骷髅兵";
                monster.Level = playerLevel + 1;
                monster.MaxHp = 70 + monster.Level * 6;
                monster.Attack = 10 + monster.Level * 3;
                monster.ExpReward = 70 + monster.Level * 20;
                break;
        }

        // 如果是精英怪，则强化属性
        if (isElite)
        {
            monster.Name = "[精英]" + monster.Name;
            monster.Level = playerLevel + 3;
            monster.MaxHp *= 1.5;
            monster.Attack *= 1.5;
            monster.ExpReward *= 1.5;
        }

        // 初始化当前血量
        monster.Hp = monster.MaxHp;

        return monster;
    }
}
