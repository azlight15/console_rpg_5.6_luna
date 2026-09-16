namespace Console_RPG;

// 技能的数据。
// 这里负责描述技能“是什么”，例如名字、伤害倍率和消耗。
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
    // v1 还没有真正实现冷却，但先把数据保留下来，方便以后扩展。
    public int Cooldown { get; set; }

    // 使用一次技能需要消耗多少技能点。
    public int SkillPointCost { get; set; } = 1;
}
