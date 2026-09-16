using Console_RPG;
using Xunit;

namespace Console_RPG.Tests;

public class LevelUpTests
{
    [Fact]
    public void GainExp_ShouldLevelUpAndKeepOverflowExp()
    {
        Player player = new();

        UpLevel.GainExp(player, 100);

        Assert.Equal(2, player.Level);
        Assert.Equal(0, player.Exp);
        Assert.Equal(120, player.MaxHp);
        Assert.Equal(20, player.Attack);
        Assert.Equal(4, player.TreatmentCount);
    }

    [Fact]
    public void GainExp_ShouldHandleMultipleLevelUps()
    {
        Player player = new();

        UpLevel.GainExp(player, 250);

        Assert.Equal(3, player.Level);
        Assert.Equal(10, player.Exp);
        Assert.Equal(140, player.MaxHp);
        Assert.Equal(25, player.Attack);
    }

    [Fact]
    public void GainExp_ShouldIgnoreNonPositiveExp()
    {
        Player player = new();

        UpLevel.GainExp(player, 0);
        UpLevel.GainExp(player, -50);

        Assert.Equal(1, player.Level);
        Assert.Equal(0, player.Exp);
    }
}
