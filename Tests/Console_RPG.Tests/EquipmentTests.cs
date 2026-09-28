using Console_RPG;
using Xunit;

namespace Console_RPG.Tests;

public class EquipmentTests
{
    [Fact]
    public void EquipFromInventory_ShouldEquipWeapon()
    {
        Player player = new();
        Equipment weapon = new()
        {
            Name = "测试剑",
            Type = "武器",
            AttackBonus = 30,
            CriticalRateBonus = 0.05
        };

        player.AddEquipment(weapon);

        Assert.True(player.EquipFromInventory(0));
        Assert.Equal(45, player.FinalAttack);
        Assert.Equal(0.15, player.FinalCriticalRate, 10);
    }

    [Fact]
    public void EquipFromInventory_ShouldEquipArmorAndAffectFinalStats()
    {
        Player player = new();
        Equipment armor = new()
        {
            Name = "测试甲",
            Type = "防具",
            HpBonus = 80,
            EvasionRateBonus = 0.10
        };

        player.AddEquipment(armor);

        Assert.True(player.EquipFromInventory(0));
        Assert.Equal(180, player.FinalMaxHp);
        Assert.Equal(0.10, player.FinalEvasionRate, 10);
    }

    [Fact]
    public void EquipFromInventory_WithInvalidIndex_ShouldReturnFalse()
    {
        Player player = new();

        Assert.False(player.EquipFromInventory(0));
    }

    [Fact]
    public void EquipFromInventory_WithUnknownEquipmentType_ShouldReturnFalse()
    {
        Player player = new();
        player.AddEquipment(new Equipment { Name = "奇怪的东西", Type = "饰品" });

        Assert.False(player.EquipFromInventory(0));
        Assert.Equal(15, player.FinalAttack);
        Assert.Equal(100, player.FinalMaxHp);
    }
}
