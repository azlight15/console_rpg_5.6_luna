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

    [Fact]
    public void EnhanceWeapon_ShouldIncreaseAttackAndSpendGold()
    {
        Player player = new();
        Equipment weapon = new() { Name = "测试剑", Type = "武器", AttackBonus = 10 };
        player.AddEquipment(weapon);

        int oldGold = player.Gold;
        int cost = weapon.GetEnhancementCost();

        Assert.True(player.EnhanceEquipment(0));
        Assert.Equal(1, weapon.EnhancementLevel);
        Assert.Equal(14, weapon.AttackBonus);
        Assert.Equal(oldGold - cost, player.Gold);
    }

    [Fact]
    public void EnhanceArmor_ShouldIncreaseHp()
    {
        Player player = new();
        Equipment armor = new() { Name = "测试甲", Type = "防具", HpBonus = 20 };
        player.AddEquipment(armor);

        Assert.True(player.EnhanceEquipment(0));
        Assert.Equal(1, armor.EnhancementLevel);
        Assert.Equal(35, armor.HpBonus);
    }

    [Fact]
    public void Enhance_ShouldStopAtPlusFive()
    {
        Player player = new();
        // +5 强化的总费用为 750 金币，玩家默认只有 100 金币。
        // 这里额外补充金币，测试重点是“能连续强化到 +5，并在 +5 后停止”，
        // 而不是测试金币不足的情况。
        player.AddGold(650);

        Equipment weapon = new() { Name = "测试剑", Type = "武器", AttackBonus = 10 };
        player.AddEquipment(weapon);

        for (int i = 0; i < Equipment.MaxEnhancementLevel; i++)
            Assert.True(player.EnhanceEquipment(0));

        Assert.False(player.EnhanceEquipment(0));
        Assert.Equal(5, weapon.EnhancementLevel);
        Assert.Equal(30, weapon.AttackBonus);
    }

    [Fact]
    public void Enhance_WithInsufficientGold_ShouldNotChangeEquipment()
    {
        Player player = new();
        Equipment weapon = new() { Name = "昂贵的剑", Type = "武器", AttackBonus = 10, Rarity = "史诗" };
        player.AddEquipment(weapon);

        int oldGold = player.Gold;

        Assert.False(player.EnhanceEquipment(0));
        Assert.Equal(oldGold, player.Gold);
        Assert.Equal(0, weapon.EnhancementLevel);
        Assert.Equal(10, weapon.AttackBonus);
    }

    [Fact]
    public void Enhance_EquippedWeapon_ShouldUpdateFinalAttack()
    {
        Player player = new();
        Equipment weapon = new() { Name = "强化剑", Type = "武器", AttackBonus = 10 };
        player.EquipWeapon(weapon);

        Assert.True(player.EnhanceEquipment(0));
        Assert.Equal(14, weapon.AttackBonus);
        Assert.Equal(29, player.FinalAttack);
    }

    [Fact]
    public void SellEquipment_ShouldGiveGoldAndRemoveUnequippedItem()
    {
        Player player = new();
        Equipment weapon = new() { Name = "待出售的剑", Type = "武器", AttackBonus = 10 };
        player.AddEquipment(weapon);

        int oldGold = player.Gold;
        int price = weapon.GetSellPrice();

        Assert.Equal(price, player.SellEquipment(0));
        Assert.Equal(oldGold + price, player.Gold);
        Assert.Empty(player.Inventory);
    }

    [Fact]
    public void SellEquipment_ShouldProtectEquippedItem()
    {
        Player player = new();
        Equipment weapon = new() { Name = "当前武器", Type = "武器", AttackBonus = 10 };
        player.EquipWeapon(weapon);

        int oldGold = player.Gold;
        Assert.Equal(0, player.SellEquipment(0));
        Assert.Equal(oldGold, player.Gold);
        Assert.Single(player.Inventory);
        Assert.Same(weapon, player.Weapon);
    }
}
