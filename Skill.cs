namespace Console_RPG;

/// <summary>
/// 技能的数据模型。
///
/// Skill 只描述“这个技能是什么”，不负责真正造成伤害。
/// 具体释放规则交给 SkillSystem，这样以后增加火焰、毒、控制等效果时，数据和逻辑不会混在一起。
/// </summary>
public class Skill
{
    /// <summary>技能名称。</summary>
    public string Name { get; set; } = "";

    /// <summary>显示给玩家看的技能说明。</summary>
    public string Description { get; set; } = "";

    /// <summary>技能伤害相对于玩家最终攻击力的倍率，例如 1.5 就是 150% 攻击力。</summary>
    public double DamageMultiplier { get; set; } = 1;

    /// <summary>
    /// 技能冷却回合数。
    /// v1.0 暂时保留这个字段作为后续扩展接口，目前主要限制来自技能点。
    /// </summary>
    public int Cooldown { get; set; }

    /// <summary>释放技能需要消耗的技能点。</summary>
    public int SkillPointCost { get; set; } = 1;
}
