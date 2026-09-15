namespace Console_RPG;

/// <summary>
/// 管理技能释放逻辑与新角色的初始技能。
///
/// Battle 负责选择技能和推进回合，SkillSystem 负责把技能数据转换成实际伤害。
/// 这样未来加入治疗技能、状态技能或资源消耗时，可以继续扩展这里，而不必把逻辑塞回 Battle。
/// </summary>
public static class SkillSystem
{
    /// <summary>给新角色加入 v0.4.0 用于测试的基础攻击技能。</summary>
    public static void InitializeStarterSkills(Player player)
    {
        player.LearnSkill(new Skill
        {
            Name = "重击",
            Description = "以 1.5 倍攻击力造成伤害。",
            DamageMultiplier = 1.5,
            Cooldown = 0
        });

        player.LearnSkill(new Skill
        {
            Name = "火球",
            Description = "以 1.8 倍攻击力造成伤害。",
            DamageMultiplier = 1.8,
            Cooldown = 0
        });
    }

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
