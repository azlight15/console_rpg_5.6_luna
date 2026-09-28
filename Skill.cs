namespace Console_RPG;

// 技能的数据。
// 这里负责描述技能“是什么”，例如名字、伤害倍率、消耗和冷却。
// 真正释放技能的过程放在 SkillSystem，避免数据和战斗逻辑混在一起。
public class Skill
{
    // 技能名称。
    public string Name { get; set; } = "";

    // 显示给玩家看的技能说明。
    public string Description { get; set; } = "";

    // 技能伤害倍率。
    // 例如 1.5 就表示造成玩家最终攻击力的 1.5 倍伤害。
    public double DamageMultiplier { get; set; } = 1;

    // 技能冷却回合数。
    // 战斗中释放成功后由 Player 记录剩余冷却，回合结束时逐步减少。
    // 冷却属于当前战斗状态，不会写入存档。
    public int Cooldown { get; set; }

    // 技能命中后附加的状态类型；None 表示不附加状态。
    public StatusEffectType StatusEffect { get; set; } = StatusEffectType.None;

    // 状态持续的回合数。
    public int StatusEffectDuration { get; set; }

    // 中毒、燃烧等持续伤害状态每回合造成的伤害。
    // 眩晕不使用这个数值。
    public double StatusEffectPower { get; set; }

    // 使用一次技能需要消耗多少技能点。
    public int SkillPointCost { get; set; } = 1;
}
