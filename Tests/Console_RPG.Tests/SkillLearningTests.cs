using Xunit;

namespace Console_RPG.Tests;

public class SkillLearningTests
{
    [Fact]
    public void NewPlayerStartsWithStarterSkillsOnly()
    {
        Player player = new();
        SkillSystem.InitializeStarterSkills(player);

        Assert.Equal(2, player.Skills.Count);
        Assert.True(player.HasSkill("重击"));
        Assert.True(player.HasSkill("火球"));
        Assert.False(player.HasSkill("冰枪"));
    }

    [Fact]
    public void PlayerCanLearnSkillWhenLevelAndGoldAreEnough()
    {
        Player player = new();
        Skill skill = new()
        {
            Name = "测试技能",
            Description = "测试用技能。",
            LearnLevel = 3,
            LearnCostGold = 80
        };

        player.Level = 3;
        player.AddGold(80);

        bool learned = player.TryLearnSkill(skill);

        Assert.True(learned);
        Assert.True(player.HasSkill("测试技能"));
        Assert.Equal(100, player.Gold);
    }

    [Fact]
    public void PlayerCannotLearnSkillWhenLevelIsTooLow()
    {
        Player player = new();
        Skill skill = new()
        {
            Name = "高级测试技能",
            LearnLevel = 5,
            LearnCostGold = 80
        };

        player.Level = 1;
        player.AddGold(80);

        bool learned = player.TryLearnSkill(skill);

        Assert.False(learned);
        Assert.False(player.HasSkill("高级测试技能"));
        Assert.Equal(180, player.Gold);
    }

    [Fact]
    public void PlayerCannotLearnSkillWhenGoldIsInsufficient()
    {
        Player player = new();
        Skill skill = new()
        {
            Name = "昂贵测试技能",
            LearnLevel = 1,
            LearnCostGold = 500
        };

        bool learned = player.TryLearnSkill(skill);

        Assert.False(learned);
        Assert.False(player.HasSkill("昂贵测试技能"));
        Assert.Equal(100, player.Gold);
    }

    [Fact]
    public void SameSkillCannotBeLearnedTwice()
    {
        Player player = new();
        Skill skill = new()
        {
            Name = "重复测试技能",
            LearnLevel = 1,
            LearnCostGold = 20
        };

        Assert.True(player.TryLearnSkill(skill));
        int goldAfterFirstLearn = player.Gold;

        Assert.False(player.TryLearnSkill(skill));
        Assert.Equal(goldAfterFirstLearn, player.Gold);
        Assert.Single(player.Skills);
    }
}