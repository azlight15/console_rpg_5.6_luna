namespace Console_RPG;

/// <summary>
/// 管理技能释放逻辑。
///
/// Battle 负责选择技能和推进回合，SkillSystem 负责把技能数据转换成实际伤害。
/// 这样未来加入治疗技能、状态技能或资源消耗时，可以继续扩展这里，而不必把逻辑塞回 Battle。
/// </summary>
public static class SkillSystem
{
    /// <summary>
    /// 使用指定技能攻击目标。
    /// 暴击判定和技能倍率由 DamageCalculator 统一处理。
    /// </summary>
    public static double UseSkill(
        Player player,
        MonsterStatistics monster,
        Skill skill,
        out bool critical)
    {
        double damage = DamageCalculator.CalculateSkillDamage(
            player.FinalAttack,
            skill,
            out critical);

        monster.Hp = System.Math.Max(0, monster.Hp - damage);
        return damage;
    }
}
