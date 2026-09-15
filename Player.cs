using System.Collections.Generic;

namespace Console_RPG;

/// <summary>
/// 表示游戏中的玩家角色及其当前状态。
/// 玩家数据通过实例传递给各个系统，避免依赖全局静态状态。
/// v0.4.0 开始支持装备和技能扩展。
/// </summary>
public sealed class Player
{
    public string Name { get; set; } = "";
    public int Level { get; set; } = 1;
    public double Exp { get; set; }

    public double ExpToNextLevel => Level * 100;

    public double Hp { get; private set; } = 100;
    public double MaxHp { get; private set; } = 100;
    public double Attack { get; private set; } = 15;

    /// <summary>当前装备的武器。</summary>
    public Equipment? Weapon { get; private set; }

    /// <summary>当前装备的防具。</summary>
    public Equipment? Armor { get; private set; }

    /// <summary>玩家已学习的技能列表。</summary>
    public List<Skill> Skills { get; } = new();

    /// <summary>计算装备后的最终攻击力。</summary>
    public double FinalAttack => Attack + (Weapon?.AttackBonus ?? 0);

    /// <summary>计算装备后的最终最大生命值。</summary>
    public double FinalMaxHp => MaxHp + (Armor?.HpBonus ?? 0);

    public double Treatment { get; private set; } = 50;
    public int TreatmentCount { get; private set; } = 3;

    /// <summary>减少玩家生命值，并确保生命值不会低于 0。</summary>
    public void TakeDamage(double amount)
    {
        if (amount <= 0) return;
        Hp = System.Math.Max(0, Hp - amount);
    }

    /// <summary>恢复生命值，并把恢复量限制在装备后的最终生命上限以内。</summary>
    public double Heal(double amount)
    {
        if (amount <= 0 || Hp >= FinalMaxHp) return 0;

        double oldHp = Hp;
        Hp = System.Math.Min(FinalMaxHp, Hp + amount);
        return Hp - oldHp;
    }

    /// <summary>消耗一次治疗资源，并恢复玩家生命值。</summary>
    public double UseTreatment()
    {
        if (TreatmentCount <= 0 || Hp >= FinalMaxHp) return 0;

        double recovered = Heal(Treatment);
        if (recovered > 0) TreatmentCount--;
        return recovered;
    }

    /// <summary>装备指定武器；装备加成不会直接写入基础攻击力。</summary>
    public void EquipWeapon(Equipment equipment)
    {
        Weapon = equipment;
    }

    /// <summary>装备指定防具；防具生命加成通过 FinalMaxHp 计算。</summary>
    public void EquipArmor(Equipment equipment)
    {
        Armor = equipment;
    }

    /// <summary>学习技能；相同技能对象不会重复加入列表。</summary>
    public void LearnSkill(Skill skill)
    {
        if (!Skills.Contains(skill))
        {
            Skills.Add(skill);
        }
    }

    /// <summary>
    /// 从存档恢复完整玩家状态。
    /// 装备和技能必须在恢复生命值之前设置，以便正确计算最终生命上限。
    /// </summary>
    public void RestoreFromSave(
        double maxHp,
        double hp,
        double attack,
        double treatment,
        int treatmentCount,
        Equipment? weapon,
        Equipment? armor,
        IEnumerable<Skill>? skills)
    {
        MaxHp = maxHp;
        Attack = attack;
        Treatment = treatment;
        TreatmentCount = treatmentCount;
        Weapon = weapon;
        Armor = armor;
        Hp = System.Math.Clamp(hp, 0, FinalMaxHp);

        Skills.Clear();
        if (skills is null) return;

        foreach (Skill skill in skills)
        {
            LearnSkill(skill);
        }
    }

    /// <summary>把当前生命值恢复到装备后的最终生命上限。</summary>
    public void RestoreFullHealth()
    {
        Hp = FinalMaxHp;
    }

    /// <summary>应用一次升级带来的基础属性成长，并重新计算最终生命值。</summary>
    public void ApplyLevelUp()
    {
        MaxHp += 20;
        Attack += 5;
        TreatmentCount++;
        RestoreFullHealth();
    }
}
