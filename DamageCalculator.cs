using System;

namespace Console_RPG;

/// <summary>
/// 统一处理战斗伤害计算。
///
/// 将伤害逻辑从 Battle 中分离，避免战斗流程和数值计算混合。
/// v0.4.0 开始同时支持普通攻击和技能伤害。
/// </summary>
public static class DamageCalculator
{
    private static readonly Random Random = new();

    /// <summary>
    /// 计算普通攻击伤害。
    /// criticalRate 使用 0-1 的概率表示，例如 0.10 代表 10% 暴击率。
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
    /// 根据技能倍率计算技能伤害，并统一处理技能暴击。
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
