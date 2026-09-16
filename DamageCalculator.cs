using System;

namespace Console_RPG;

/// <summary>
/// 统一处理战斗伤害计算。
///
/// Battle 决定“什么时候攻击”，DamageCalculator 决定“攻击造成多少伤害”。
/// 分开以后，未来想加入随机伤害区间、护甲减伤或不同暴击倍率时，修改数值逻辑不会破坏战斗流程。
/// </summary>
public static class DamageCalculator
{
    // 使用同一个 Random 实例生成随机判定，避免每次攻击都重新创建随机数生成器。
    private static readonly Random Random = new();

    /// <summary>
    /// 计算普通攻击伤害。
    /// criticalRate 使用 0-1 的概率表示，例如 0.10 就是 10%。
    /// 暴击时当前倍率为 1.5 倍。
    /// </summary>
    public static double CalculateBasicDamage(
        double attack,
        double criticalRate,
        out bool critical)
    {
        critical = Random.NextDouble() < criticalRate;
        return critical ? attack * 1.5 : attack;
    }

    /// <summary>
    /// 计算技能伤害。
    /// 先用技能倍率计算基础伤害，再独立进行一次技能暴击判定。
    /// 技能本身不直接修改怪物 HP，这件事由 SkillSystem 负责。
    /// </summary>
    public static double CalculateSkillDamage(
        double attack,
        Skill skill,
        out bool critical)
    {
        double damage = attack * skill.DamageMultiplier;
        critical = Random.NextDouble() < 0.10;
        return critical ? damage * 1.5 : damage;
    }
}
