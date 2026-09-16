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

        Assert.Equal(110, player.Hp);
        Assert.Equal(110, recovered + 10);
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
}
