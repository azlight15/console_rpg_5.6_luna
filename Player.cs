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

    /// <summary>玩家已学习的技能。</summary>
    public List<Skill> Skills { get; } = new();

    /// <summary>计算装备后的最终攻击力。</summary>
    public double FinalAttack => Attack + (Weapon?.AttackBonus ?? 0);

    /// <summary>计算装备后的最终最大生命值。</summary>
    public double FinalMaxHp => MaxHp + (Armor?.HpBonus ?? 0);

    public double Treatment { get; private set; } = 50;
    public int TreatmentCount { get; private set; } = 3;

    public void TakeDamage(double amount)
    {
        if (amount <= 0) return;
        Hp = System.Math.Max(0, Hp - amount);
    }

    public double Heal(double amount)
    {
        if (amount <= 0 || Hp >= FinalMaxHp) return 0;

        double oldHp = Hp;
        Hp = System.Math.Min(FinalMaxHp, Hp + amount);
        return Hp - oldHp;
    }

    public double UseTreatment()
    {
        if (TreatmentCount <= 0 || Hp >= FinalMaxHp) return 0;

        double recovered = Heal(Treatment);
        if (recovered > 0) TreatmentCount--;
        return recovered;
    }

    public void EquipWeapon(Equipment equipment)
    {
        Weapon = equipment;
    }

    public void EquipArmor(Equipment equipment)
    {
        Armor = equipment;
    }

    public void LearnSkill(Skill skill)
    {
        if (!Skills.Contains(skill))
        {
            Skills.Add(skill);
        }
    }

    public void RestoreFullHealth()
    {
        Hp = FinalMaxHp;
    }

    public void ApplyLevelUp()
    {
        MaxHp += 20;
        Attack += 5;
        TreatmentCount++;
        RestoreFullHealth();
    }
}
