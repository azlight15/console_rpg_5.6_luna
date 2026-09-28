using Console_RPG;
using Xunit;

namespace Console_RPG.Tests;

public class SkillSystemTests
{
    [Fact]
    public void InitializeStarterSkills_ShouldAddTwoUniqueSkills()
    {
        Player player = new();

        SkillSystem.InitializeStarterSkills(player);
        SkillSystem.InitializeStarterSkills(player);

        Assert.Equal(2, player.Skills.Count);
        Assert.Contains(player.Skills, skill => skill.Name == "重击");
        Assert.Contains(player.Skills, skill => skill.Name == "火球");
    }

    [Fact]
    public void UseSkill_ShouldConsumeRequiredSkillPoints()
    {
        Player player = new();
        SkillSystem.InitializeStarterSkills(player);
        Skill skill = player.Skills.Find(s => s.Name == "火球")!;
        MonsterStatistics monster = new() { Hp = 1000, MaxHp = 1000 };

        double damage = SkillSystem.UseSkill(player, monster, skill, out _);

        Assert.True(damage > 0);
        Assert.Equal(1, player.SkillPoints);
        Assert.Equal(1000 - damage, monster.Hp, 10);
    }

    [Fact]
    public void UseSkill_WhenSkillPointsAreInsufficient_ShouldNotDamageMonster()
    {
        Player player = new();
        SkillSystem.InitializeStarterSkills(player);
        Skill skill = player.Skills.Find(s => s.Name == "火球")!;
        MonsterStatistics monster = new() { Hp = 1000, MaxHp = 1000 };

        player.TryUseSkillPoint(3);
        double damage = SkillSystem.UseSkill(player, monster, skill, out _);

        Assert.Equal(-1, damage);
        Assert.Equal(0, player.SkillPoints);
        Assert.Equal(1000, monster.Hp);
    }
}
