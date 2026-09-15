namespace Console_RPG;

/// <summary>
/// 统一处理战斗伤害计算。
///
/// 将伤害逻辑从 Battle 中分离，避免战斗流程和数值计算混合。
/// 后续可继续扩展防御、属性克制等机制。
/// </summary>
public static class DamageCalculator
{
    private static readonly Random Random = new();

    public static double CalculateBasicDamage(double attack, double criticalRate, out bool critical)
    {
        critical = Random.NextDouble() < criticalRate;
        return critical ? attack * 1.5 : attack;
    }

    public static double CalculateSkillDamage(double attack, Skill skill, out bool critical)
    {
        double damage = attack * skill.DamageMultiplier;
        critical = Random.NextDouble() < 0.1;
        return critical ? damage * 1.5 : damage;
    }
}
