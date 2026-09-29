namespace Console_RPG;

public enum StatusEffectType
{
    None,
    Poison,
    Burning,
    Stunned
}

// 战斗中的临时状态效果。
// StatusEffect 只描述“中了什么、还剩几回合、每回合造成多少伤害”。
// 真正处理状态效果的规则由 StatusEffectSystem 负责。
public class StatusEffect
{
    public StatusEffectType Type { get; set; }
    public int RemainingTurns { get; set; }
    public double DamagePerTurn { get; set; }

    public StatusEffect Clone() => new()
    {
        Type = Type,
        RemainingTurns = RemainingTurns,
        DamagePerTurn = DamagePerTurn
    };

    public string GetDisplayName() => Type switch
    {
        StatusEffectType.Poison => "中毒",
        StatusEffectType.Burning => "燃烧",
        StatusEffectType.Stunned => "眩晕",
        _ => "未知状态"
    };
}
