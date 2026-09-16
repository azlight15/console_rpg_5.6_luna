using System;

namespace Console_RPG;

// 专门负责计算伤害。
// Battle 只负责决定“谁打谁”，具体伤害数字在这里计算，这样以后调数值会更方便。
public static class DamageCalculator
{
    private static readonly Random Random = new();

    // 计算普通攻击。
    // criticalRate 是 0 到 1 之间的概率，例如 0.10 就是 10% 暴击率。
    public static double CalculateBasicDamage(
        double attack,
        double criticalRate,
        out bool critical)
    {
        critical = Random.NextDouble() < criticalRate;
        return critical ? attack * 1.5 : attack;
    }

    // 计算技能攻击。
    // 先用技能倍率放大攻击力，再判断这次攻击是否暴击。
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
