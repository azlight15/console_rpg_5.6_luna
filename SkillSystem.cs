namespace Console_RPG;

// 技能系统。
// Battle 负责让玩家选择技能和推进回合，这里负责真正执行技能。
// 这样以后增加燃烧、冰冻、毒等效果时，不需要把大量技能逻辑塞进 Battle。
public static class SkillSystem
{
    // 新角色出生时学会两个基础技能，方便一开始就能体验技能玩法。
    // 重击便宜但伤害较低；火球更强，但一次要消耗 2 点技能点。
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

    // 执行一次技能。
    // 如果技能点不够，返回 -1，Battle 就知道这次操作没有消耗回合。
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
