using System;
using Console_RPG;
using Xunit;

namespace Console_RPG.Tests;

public class MonsterFactoryTests
{
    [Fact]
    public void Create_ShouldGenerateMonsterNearPlayerLevel()
    {
        Player player = new() { Level = 10 };

        for (int i = 0; i < 200; i++)
        {
            MonsterStatistics monster = MonsterFactory.Create(player);

            // 普通怪物应该在玩家等级上下 2 级；
            // 精英怪额外提高 2 级，因此最多到玩家等级 + 4。
            int minimumLevel = Math.Max(1, player.Level - 2);
            int maximumLevel = monster.IsElite ? player.Level + 4 : player.Level + 2;

            Assert.InRange(monster.Level, minimumLevel, maximumLevel);
            Assert.True(monster.Hp > 0);
            Assert.True(monster.MaxHp > 0);
            Assert.True(monster.Attack > 0);
            Assert.True(monster.ExpReward > 0);
            Assert.True(monster.GoldReward > 0);

            if (monster.IsElite)
            {
                Assert.Equal(3, monster.SpecialAttackInterval);
                Assert.Equal(3, monster.SpecialAttackCooldown);
                Assert.Equal(1.6, monster.SpecialAttackMultiplier);
            }
        }
    }

    [Fact]
    public void Create_ForLevelOnePlayer_ShouldNeverCreateLevelBelowOne()
    {
        Player player = new() { Level = 1 };

        for (int i = 0; i < 100; i++)
        {
            MonsterStatistics monster = MonsterFactory.Create(player);

            Assert.True(monster.Level >= 1);
        }
    }

    [Fact]
    public void EliteSpecialAttack_ShouldWaitThreeAttacksThenBecomeAvailable()
    {
        MonsterStatistics monster = new()
        {
            IsElite = true,
            SpecialAttackInterval = 3,
            SpecialAttackCooldown = 3,
            SpecialAttackMultiplier = 1.6
        };

        Assert.False(monster.TryUseSpecialAttack());
        Assert.Equal(2, monster.SpecialAttackCooldown);

        Assert.False(monster.TryUseSpecialAttack());
        Assert.Equal(1, monster.SpecialAttackCooldown);

        Assert.False(monster.TryUseSpecialAttack());
        Assert.Equal(0, monster.SpecialAttackCooldown);

        Assert.True(monster.TryUseSpecialAttack());
        Assert.Equal(3, monster.SpecialAttackCooldown);
    }
}