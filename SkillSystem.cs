namespace Console_RPG;

/// <summary>
/// 技能系统。
///
/// Battle 负责“玩家选哪个技能”和“这一回合结束没有”，
/// SkillSystem 负责“这个技能能不能释放，以及释放后造成什么结果”。
/// 分开以后，Battle 不会因为技能越来越多而变成一大坨条件判断。
/// </summary>
public static class SkillSystem
{
    /// <summary>
    /// 给新角色加入基础技能。
    /// 重击便宜但倍率较低，火球更强但消耗更多技能点，让玩家需要考虑资源分配。
    /// </summary>
    public static void InitializeStarterSkills(Player player)
    {
        player.LearnSkill(new Skill
        {
            Name = "重击",
            Description = "以 1.5 倍攻击力造成伤害。",
            DamageMultiplier = 1.5,
            Cooldown = 0,
            SkillPointCost = 1
        });

        player.LearnSkill(new Skill
        {
            Name = "火球",
            Description = "以 1.8 倍攻击力造成伤害。",
            DamageMultiplier = 1.8,
            Cooldown = 0,
            SkillPointCost = 2
        });
    }

    /// <summary>
    /// 使用指定技能攻击目标。
    ///
    /// 返回值小于 0 表示资源不足，Battle 会据此判断本次操作没有消耗回合。
    /// 资源扣除必须发生在真正造成技能效果之前，避免“失败释放却扣资源”的问题。
    /// </summary>
    public static double UseSkill(
        Player player,
        MonsterStatistics monster,
        Skill skill,
        out bool critical)
    {
        critical = false;

        if (!player.TryUseSkillPoint(skill.SkillPointCost))
            return -1;

        double damage = DamageCalculator.CalculateSkillDamage(
            player.FinalAttack,
            skill,
            out critical);

        monster.Hp = System.Math.Max(0, monster.Hp - damage);
        return damage;
    }
}
