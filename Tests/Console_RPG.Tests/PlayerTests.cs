using Console_RPG;
using Xunit;

namespace Console_RPG.Tests;

public class PlayerTests
{
    [Fact]
    public void TakeDamage_ShouldNotReduceHpBelowZero()
    {
        Player player = new();

        player.TakeDamage(999);

        Assert.Equal(0, player.Hp);
    }

    [Fact]
    public void Heal_ShouldNotExceedFinalMaxHp()
    {
        Player player = new();
        player.EquipArmor(new Equipment { Name = "测试护甲", Type = "防具", HpBonus = 50 });
        player.TakeDamage(10);

        double recovered = player.Heal(999);

        Assert.Equal(150, player.Hp);
        Assert.Equal(60, recovered);
        Assert.Equal(150, player.FinalMaxHp);
    }

    [Fact]
    public void Weapon_ShouldChangeFinalAttackWithoutChangingBaseAttack()
    {
        Player player = new();
        double baseAttack = player.Attack;

        player.EquipWeapon(new Equipment { Name = "测试剑", Type = "武器", AttackBonus = 25 });

        Assert.Equal(baseAttack, player.Attack);
        Assert.Equal(baseAttack + 25, player.FinalAttack);
    }

    [Fact]
    public void Armor_ShouldChangeFinalMaxHpWithoutChangingBaseMaxHp()
    {
        Player player = new();
        double baseMaxHp = player.MaxHp;

        player.EquipArmor(new Equipment { Name = "测试甲", Type = "防具", HpBonus = 40 });

        Assert.Equal(baseMaxHp, player.MaxHp);
        Assert.Equal(baseMaxHp + 40, player.FinalMaxHp);
    }

    [Fact]
    public void Treatment_ShouldRecoverHpAndConsumeOneUse()
    {
        Player player = new();
        player.TakeDamage(30);

        double recovered = player.UseTreatment();

        Assert.Equal(50, recovered);
        Assert.Equal(100, player.Hp);
        Assert.Equal(2, player.TreatmentCount);
    }

    [Fact]
    public void Treatment_WhenHpIsFull_ShouldNotConsumeUse()
    {
        Player player = new();

        double recovered = player.UseTreatment();

        Assert.Equal(0, recovered);
        Assert.Equal(3, player.TreatmentCount);
    }

    [Fact]
    public void AddTreatmentCount_ShouldIncreaseAvailableUses()
    {
        Player player = new();

        player.AddTreatmentCount(5);

        Assert.Equal(8, player.TreatmentCount);
    }

    [Fact]
    public void Gold_ShouldBeAddedAndSpentSafely()
    {
        Player player = new();

        player.AddGold(50);

        Assert.Equal(150, player.Gold);
        Assert.True(player.TrySpendGold(40));
        Assert.Equal(110, player.Gold);
        Assert.False(player.TrySpendGold(999));
        Assert.Equal(110, player.Gold);
        Assert.False(player.TrySpendGold(-1));
        Assert.Equal(110, player.Gold);
    }

    [Fact]
    public void SkillPoints_ShouldRecoverButNotExceedLevelCap()
    {
        Player player = new() { Level = 3 };

        Assert.Equal(4, player.MaxSkillPoints);
        Assert.True(player.TryUseSkillPoint(3));
        Assert.Equal(1, player.SkillPoints);

        player.RecoverSkillPoint();
        player.RecoverSkillPoint();
        player.RecoverSkillPoint();
        player.RecoverSkillPoint();

        Assert.Equal(4, player.SkillPoints);
    }

}
