using System.Collections.Generic;
using Console_RPG;
using Xunit;

namespace Console_RPG.Tests;

public class StatusEffectTests
{
    [Fact]
    public void Apply_ShouldRefreshSameEffectWithoutDuplicatingIt()
    {
        List<StatusEffect> effects = new();

        StatusEffectSystem.Apply(effects, new StatusEffect
        {
            Type = StatusEffectType.Burning,
            RemainingTurns = 2,
            DamagePerTurn = 4
        });

        StatusEffectSystem.Apply(effects, new StatusEffect
        {
            Type = StatusEffectType.Burning,
            RemainingTurns = 3,
            DamagePerTurn = 6
        });

        Assert.Single(effects);
        Assert.Equal(3, effects[0].RemainingTurns);
        Assert.Equal(6, effects[0].DamagePerTurn);
    }

    [Fact]
    public void ProcessTurnStart_ShouldDealDamageAndExpireEffect()
    {
        List<StatusEffect> effects = new()
        {
            new StatusEffect
            {
                Type = StatusEffectType.Poison,
                RemainingTurns = 2,
                DamagePerTurn = 5
            }
        };

        double damage = StatusEffectSystem.ProcessTurnStart(effects, out List<string> messages);

        Assert.Equal(5, damage);
        Assert.Single(messages);
        Assert.Equal(1, effects[0].RemainingTurns);

        damage = StatusEffectSystem.ProcessTurnStart(effects, out _);

        Assert.Equal(5, damage);
        Assert.Empty(effects);
    }

    [Fact]
    public void Stun_ShouldBeConsumedAfterOneSkippedTurn()
    {
        List<StatusEffect> effects = new()
        {
            new StatusEffect
            {
                Type = StatusEffectType.Stunned,
                RemainingTurns = 1
            }
        };

        Assert.True(StatusEffectSystem.HasStun(effects));

        StatusEffectSystem.ConsumeStun(effects);

        Assert.False(StatusEffectSystem.HasStun(effects));
        Assert.Empty(effects);
    }

    [Fact]
    public void PlayerRestoreFromSave_ShouldClearTemporaryStatusEffects()
    {
        Player player = new();
        player.ApplyStatus(new StatusEffect
        {
            Type = StatusEffectType.Poison,
            RemainingTurns = 3,
            DamagePerTurn = 4
        });

        player.RestoreFromSave(100, 100, 15, 50, 3, null, null, null);

        Assert.Empty(player.StatusEffects);
    }
}
