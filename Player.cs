namespace Console_RPG;

/// <summary>
/// 表示游戏中的玩家角色及其当前状态。
/// 玩家数据通过实例传递给各个系统，避免依赖全局静态状态。
/// HP 不允许被外部系统直接赋值，统一通过角色行为修改，避免出现非法状态。
/// </summary>
public sealed class Player
{
    public string Name { get; set; } = "";
    public int Level { get; set; } = 1;
    public double Exp { get; set; }

    /// <summary>当前等级对应的升级经验需求。</summary>
    public double ExpToNextLevel => Level * 100;

    public double Hp { get; private set; } = 100;
    public double MaxHp { get; private set; } = 100;
    public double Attack { get; private set; } = 15;

    /// <summary>每次治疗恢复的生命值。</summary>
    public double Treatment { get; private set; } = 50;

    /// <summary>当前可用的治疗次数。</summary>
    public int TreatmentCount { get; private set; } = 3;

    /// <summary>让玩家承受伤害，并确保 HP 不会低于 0。</summary>
    public void TakeDamage(double amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Hp = System.Math.Max(0, Hp - amount);
    }

    /// <summary>恢复指定数量的 HP，并确保不会超过最大生命值。</summary>
    public double Heal(double amount)
    {
        if (amount <= 0 || Hp >= MaxHp)
        {
            return 0;
        }

        double oldHp = Hp;
        Hp = System.Math.Min(MaxHp, Hp + amount);
        return Hp - oldHp;
    }

    /// <summary>消耗一次治疗资源并恢复生命值。</summary>
    public double UseTreatment()
    {
        if (TreatmentCount <= 0 || Hp >= MaxHp)
        {
            return 0;
        }

        double recovered = Heal(Treatment);
        if (recovered > 0)
        {
            TreatmentCount--;
        }

        return recovered;
    }

    /// <summary>将玩家恢复到满 HP。</summary>
    public void RestoreFullHealth()
    {
        Hp = MaxHp;
    }

    /// <summary>升级时提升角色的基础属性。</summary>
    public void ApplyLevelUp()
    {
        MaxHp += 20;
        Attack += 5;
        TreatmentCount++;
        RestoreFullHealth();
    }

    /// <summary>由存档系统恢复角色属性，仅供内部存档流程使用。</summary>
    public void RestoreFromSave(double maxHp, double hp, double attack, double treatment, int treatmentCount)
    {
        if (maxHp <= 0 || attack <= 0 || treatment < 0 || treatmentCount < 0)
        {
            return;
        }

        MaxHp = maxHp;
        Hp = System.Math.Clamp(hp, 0, maxHp);
        Attack = attack;
        Treatment = treatment;
        TreatmentCount = treatmentCount;
    }
}
