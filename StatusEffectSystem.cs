using System;
using System.Collections.Generic;
using System.Linq;

namespace Console_RPG;

// 状态效果规则集中处理在这里。
// 这样 Battle 不需要知道“中毒怎么结算、燃烧持续多久”等细节。
public static class StatusEffectSystem
{
    // 添加状态。如果目标已经有同类型状态，就刷新持续时间并保留较高的伤害值。
    public static void Apply(List<StatusEffect> effects, StatusEffect effect)
    {
        if (effect.Type == StatusEffectType.None || effect.RemainingTurns <= 0)
            return;

        StatusEffect? existing = effects.FirstOrDefault(x => x.Type == effect.Type);
        if (existing is null)
        {
            effects.Add(effect.Clone());
            return;
        }

        existing.RemainingTurns = Math.Max(existing.RemainingTurns, effect.RemainingTurns);
        existing.DamagePerTurn = Math.Max(existing.DamagePerTurn, effect.DamagePerTurn);
    }

    // 在一个行动方开始自己的回合时结算持续伤害，并减少状态持续时间。
    public static double ProcessTurnStart(List<StatusEffect> effects, out List<string> messages)
    {
        messages = new List<string>();
        double damage = 0;

        foreach (StatusEffect effect in effects.ToList())
        {
            if (effect.Type is StatusEffectType.Poison or StatusEffectType.Burning)
            {
                damage += Math.Max(0, effect.DamagePerTurn);
                messages.Add($"{effect.GetDisplayName()}造成 {effect.DamagePerTurn:0.#} 点伤害。");

                effect.RemainingTurns--;
                if (effect.RemainingTurns <= 0)
                    effects.Remove(effect);
            }
        }

        return damage;
    }

    public static bool HasStun(List<StatusEffect> effects) =>
        effects.Any(x => x.Type == StatusEffectType.Stunned);

    // 眩晕不是持续伤害，应该在被眩晕的一方完成一次“被跳过的回合”后移除。
    public static void ConsumeStun(List<StatusEffect> effects)
    {
        StatusEffect? stun = effects.FirstOrDefault(x => x.Type == StatusEffectType.Stunned);
        if (stun is null)
            return;

        stun.RemainingTurns--;
        if (stun.RemainingTurns <= 0)
            effects.Remove(stun);
    }

    public static void Clear(List<StatusEffect> effects) => effects.Clear();
}
