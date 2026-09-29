using Console_RPG;
using Xunit;

namespace Console_RPG.Tests;

public class DamageCalculatorTests
{
    [Fact]
    public void BasicDamage_ShouldBeBaseAttackOrCriticalHit()
    {
        double attack = 20;

        double damage = DamageCalculator.CalculateBasicDamage(attack, 0, out bool critical);

        Assert.False(critical);
        Assert.Equal(20, damage);
    }

    [Fact]
    public void SkillDamage_ShouldRespectSkillMultiplier()
    {
        Skill skill = new()
        {
            Name = "测试技能",
            DamageMultiplier = 2.0
        };

        double damage = DamageCalculator.CalculateSkillDamage(20, skill, out bool critical);

        // 技能基础伤害是 40；如果同时暴击，则会变成 60。
        Assert.True(damage is 40 or 60);
        Assert.Equal(damage == 60, critical);
    }
}
