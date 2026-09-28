using System.Collections.Generic;

namespace Console_RPG;

// 一只正在战斗中的怪物。
// 这里只保存怪物当前的数据，例如等级、HP、攻击和奖励。
// 怎么生成怪物由 MonsterFactory 负责，怎么打怪由 Battle 负责。
public class MonsterStatistics
{
    // 战斗中显示的名字。
    public string Name { get; set; } = "";

    // 怪物的定位，例如肉盾、高速、强攻。
    public string Type { get; set; } = "普通";

    // 这一次实际生成出来的等级。
    public int Level { get; set; }

    // 当前生命值。
    public double Hp { get; set; }

    // 最大生命值。
    public double MaxHp { get; set; }

    // 怪物的攻击力。
    public double Attack { get; set; }

    // 击败怪物后获得的经验。
    public double ExpReward { get; set; }

    // 击败怪物后获得的金币。
    public int GoldReward { get; set; }

    // 怪物闪避玩家攻击的概率。
    public double EvasionRate { get; set; }

    // 战斗中的临时状态效果，例如中毒、燃烧和眩晕。
    public List<StatusEffect> StatusEffects { get; } = new();

    // 判断怪物当前是否带有指定状态。
    public bool HasStatus(StatusEffectType type) =>
        StatusEffects.Exists(effect => effect.Type == type);

    // 添加或刷新一个状态效果。
    public void ApplyStatus(StatusEffect effect) =>
        StatusEffectSystem.Apply(StatusEffects, effect);

    // 结算怪物回合开始时的持续伤害。
    public double ProcessStatusDamage(out List<string> messages)
    {
        double damage = StatusEffectSystem.ProcessTurnStart(StatusEffects, out messages);
        if (damage > 0)
            Hp = System.Math.Max(0, Hp - damage);
        return damage;
    }

    // 怪物离开战斗后不会保留临时状态。
    public void ClearStatusEffects() => StatusEffectSystem.Clear(StatusEffects);

}
