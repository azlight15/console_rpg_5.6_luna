using Console_RPG;
using Xunit;

namespace Console_RPG.Tests;

public class SaveDataTests
{
    [Fact]
    public void FromPlayer_ShouldCopyCoreProgressAndResources()
    {
        Player player = new()
        {
            Name = "测试玩家",
            Level = 3,
            Exp = 42
        };

        player.AddGold(50);
        player.EquipWeapon(new Equipment
        {
            Name = "测试剑",
            Type = "武器",
            AttackBonus = 12
        });

        SaveData save = SaveData.FromPlayer(player);

        Assert.Equal("测试玩家", save.Name);
        Assert.Equal(3, save.Level);
        Assert.Equal(42, save.Exp);
        Assert.Equal(150, save.Gold);
        Assert.Equal("测试剑", save.Weapon?.Name);
    }

    [Fact]
    public void ApplyTo_ShouldRestoreSavedState()
    {
        SaveData save = new()
        {
            Name = "读取角色",
            Level = 4,
            Exp = 25,
            MaxHp = 160,
            Hp = 120,
            Attack = 30,
            Treatment = 50,
            TreatmentCount = 2,
            Gold = 275,
            SkillPoints = 4,
            Armor = new Equipment
            {
                Name = "测试甲",
                Type = "防具",
                HpBonus = 40
            },
            Skills =
            {
                new Skill
                {
                    Name = "测试技能",
                    DamageMultiplier = 2.0,
                    SkillPointCost = 1
                }
            }
        };

        Player player = new();
        save.ApplyTo(player);

        Assert.Equal("读取角色", player.Name);
        Assert.Equal(4, player.Level);
        Assert.Equal(25, player.Exp);
        Assert.Equal(120, player.Hp);
        Assert.Equal(30, player.Attack);
        Assert.Equal(200, player.FinalMaxHp);
        Assert.Equal(275, player.Gold);
        Assert.Equal(4, player.SkillPoints);
        Assert.Single(player.Skills);
        Assert.Equal("测试技能", player.Skills[0].Name);
        Assert.Equal("测试甲", player.Armor?.Name);
    }
}
