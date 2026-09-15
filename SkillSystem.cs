namespace Console_RPG;

/// <summary>
/// 管理技能释放逻辑。
///
/// Battle 只负责回合流程，技能效果由此类处理。
/// </summary>
public static class SkillSystem
{
    /// <summary>
    /// 使用技能攻击目标。
    /// </summary>
    public static double UseSkill(Player player, MonsterStatistics monster, Skill skill)
    {
        double damage = DamageCalculator.CalculateSkillDamage(
            player.FinalAttack,
            skill,
            out bool critical);

        monster.Hp = System.Math.Max(0, monster.Hp - damage);
        return damage;
    }
}
