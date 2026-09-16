using System.Collections.Generic;

namespace Console_RPG;

/// <summary>
/// 玩家的运行时状态。
///
/// Player 是整个游戏的“角色本体”：基础属性、装备、背包、技能、金币和战斗资源都放在这里。
/// 其他系统不要自己保存一份玩家属性，而应该通过 Player 读取或修改状态，避免数据不同步。
/// </summary>
public sealed class Player
{
    /// <summary>角色名称。</summary>
    public string Name { get; set; } = "";

    /// <summary>当前等级，从 1 级开始。</summary>
    public int Level { get; set; } = 1;

    /// <summary>当前累计经验。升级后只扣除升级所需经验，剩余经验继续保留。</summary>
    public double Exp { get; set; }

    /// <summary>
    /// 当前等级需要的升级经验。
    /// v1.0 使用逐级增加的曲线，而不是每一级都固定需要 100 EXP。
    /// </summary>
    public double ExpToNextLevel => 100 + (Level - 1) * 40;

    /// <summary>当前生命值。只能通过 Player 提供的方法修改，避免外部随意写入非法 HP。</summary>
    public double Hp { get; private set; } = 100;

    /// <summary>不计算装备加成的基础最大 HP。</summary>
    public double MaxHp { get; private set; } = 100;

    /// <summary>不计算武器加成的基础攻击力。</summary>
    public double Attack { get; private set; } = 15;

    /// <summary>当前装备的武器。</summary>
    public Equipment? Weapon { get; private set; }

    /// <summary>当前装备的防具。</summary>
    public Equipment? Armor { get; private set; }

    /// <summary>玩家拥有的全部装备。装备和背包是两个概念：物品仍然留在背包中，只改变当前装备槽。</summary>
    public List<Equipment> Inventory { get; } = new();

    /// <summary>玩家已经学会的技能。</summary>
    public List<Skill> Skills { get; } = new();

    /// <summary>金币，用于 v1.0 商店系统。</summary>
    public int Gold { get; private set; } = 100;

    /// <summary>当前技能点。释放技能会消耗技能点。</summary>
    public int SkillPoints { get; private set; } = 3;

    /// <summary>
    /// 技能点上限随等级缓慢增加。
    /// 这样升级不仅提高攻击和 HP，也会逐渐提高技能使用能力。
    /// </summary>
    public int MaxSkillPoints => 3 + (Level - 1) / 2;

    /// <summary>基础治疗量。</summary>
    public double Treatment { get; private set; } = 50;

    /// <summary>剩余治疗次数。</summary>
    public int TreatmentCount { get; private set; } = 3;

    /// <summary>最终攻击力 = 基础攻击力 + 当前武器的攻击加成。</summary>
    public double FinalAttack => Attack + (Weapon?.AttackBonus ?? 0);

    /// <summary>最终最大 HP = 基础最大 HP + 当前防具的 HP 加成。</summary>
    public double FinalMaxHp => MaxHp + (Armor?.HpBonus ?? 0);

    /// <summary>最终暴击率由基础暴击率和装备加成共同决定。</summary>
    public double FinalCriticalRate => 0.10 + (Weapon?.CriticalRateBonus ?? 0) + (Armor?.CriticalRateBonus ?? 0);

    /// <summary>最终闪避率主要来自装备。</summary>
    public double FinalEvasionRate => (Weapon?.EvasionRateBonus ?? 0) + (Armor?.EvasionRateBonus ?? 0);

    /// <summary>承受伤害，并把 HP 限制在 0 以上。</summary>
    public void TakeDamage(double amount)
    {
        if (amount <= 0) return;
        Hp = System.Math.Max(0, Hp - amount);
    }

    /// <summary>
    /// 恢复 HP，并把结果限制在最终最大 HP 以内。
    /// 返回实际恢复量，让 UI 可以显示“实际恢复了多少”。
    /// </summary>
    public double Heal(double amount)
    {
        if (amount <= 0 || Hp >= FinalMaxHp) return 0;

        double oldHp = Hp;
        Hp = System.Math.Min(FinalMaxHp, Hp + amount);
        return Hp - oldHp;
    }

    /// <summary>消耗一次治疗资源进行治疗。</summary>
    public double UseTreatment()
    {
        if (TreatmentCount <= 0 || Hp >= FinalMaxHp) return 0;

        double recovered = Heal(Treatment);
        if (recovered > 0) TreatmentCount--;
        return recovered;
    }

    /// <summary>获得一件装备并放入背包。</summary>
    public void AddEquipment(Equipment equipment) => Inventory.Add(equipment);

    /// <summary>
    /// 从背包选择装备。
    /// 装备不会叠加到基础属性上，而是替换对应装备槽，因此反复装备不会产生“属性越穿越高”的 bug。
    /// </summary>
    public bool EquipFromInventory(int index)
    {
        if (index < 0 || index >= Inventory.Count) return false;

        Equipment equipment = Inventory[index];
        if (equipment.Type == "武器")
            Weapon = equipment;
        else if (equipment.Type == "防具")
            Armor = equipment;
        else
            return false;

        // 如果换下了高 HP 防具，当前 HP 不能超过新的最大 HP。
        if (Hp > FinalMaxHp)
            Hp = FinalMaxHp;

        return true;
    }

    /// <summary>直接设置初始武器，并确保它也存在于背包中。</summary>
    public void EquipWeapon(Equipment equipment)
    {
        Weapon = equipment;
        if (!Inventory.Contains(equipment)) Inventory.Add(equipment);
    }

    /// <summary>直接设置初始防具，并确保它也存在于背包中。</summary>
    public void EquipArmor(Equipment equipment)
    {
        Armor = equipment;
        if (!Inventory.Contains(equipment)) Inventory.Add(equipment);
    }

    /// <summary>学习技能。同名技能不会重复添加。</summary>
    public void LearnSkill(Skill skill)
    {
        if (!Skills.Exists(existing => existing.Name == skill.Name))
            Skills.Add(skill);
    }

    /// <summary>
    /// 尝试消耗技能点。
    /// 资源扣除放在 Player 中，而不是 Battle 中，这样以后其他技能系统也能复用同一套规则。
    /// </summary>
    public bool TryUseSkillPoint(int cost)
    {
        if (cost <= 0) return true;
        if (SkillPoints < cost) return false;

        SkillPoints -= cost;
        return true;
    }

    /// <summary>战斗胜利后恢复 1 点技能点，但不会超过上限。</summary>
    public void RecoverSkillPoint()
    {
        SkillPoints = System.Math.Min(MaxSkillPoints, SkillPoints + 1);
    }

    /// <summary>升级后将技能点补满，让升级具有即时的战斗收益。</summary>
    private void RestoreSkillPoints()
    {
        SkillPoints = MaxSkillPoints;
    }

    /// <summary>获得金币，统一通过方法修改，避免其他系统直接操作金币数。</summary>
    public void AddGold(int amount)
    {
        if (amount > 0) Gold += amount;
    }

    /// <summary>
    /// 尝试消费金币。
    /// 商店等系统只需要关心“够不够钱”，不需要自己处理扣款逻辑。
    /// </summary>
    public bool TrySpendGold(int amount)
    {
        if (amount < 0 || Gold < amount) return false;
        Gold -= amount;
        return true;
    }

    /// <summary>
    /// 从存档恢复玩家状态。
    /// 存档系统只负责读 JSON；真正把数据安全地写回 Player 的工作集中在这里。
    /// </summary>
    public void RestoreFromSave(
        double maxHp,
        double hp,
        double attack,
        double treatment,
        int treatmentCount,
        Equipment? weapon,
        Equipment? armor,
        IEnumerable<Skill>? skills,
        IEnumerable<Equipment>? inventory = null,
        int gold = 100,
        int skillPoints = 3)
    {
        MaxHp = System.Math.Max(1, maxHp);
        Attack = System.Math.Max(1, attack);
        Treatment = System.Math.Max(0, treatment);
        TreatmentCount = System.Math.Max(0, treatmentCount);
        Gold = System.Math.Max(0, gold);
        Weapon = weapon;
        Armor = armor;
        Hp = System.Math.Clamp(hp, 0, FinalMaxHp);

        Skills.Clear();
        if (skills is not null)
        {
            foreach (Skill skill in skills)
                LearnSkill(skill);
        }

        Inventory.Clear();
        if (inventory is not null)
        {
            foreach (Equipment equipment in inventory)
                Inventory.Add(equipment);
        }

        // 兼容旧档：v0.4 没有 Inventory 时，把当前装备补回背包。
        if (Weapon is not null && !Inventory.Exists(item => item.Name == Weapon.Name))
            Inventory.Add(Weapon);
        if (Armor is not null && !Inventory.Exists(item => item.Name == Armor.Name))
            Inventory.Add(Armor);

        SkillPoints = System.Math.Clamp(skillPoints, 0, MaxSkillPoints);
    }

    /// <summary>恢复满血。</summary>
    public void RestoreFullHealth() => Hp = FinalMaxHp;

    /// <summary>
    /// 应用一次升级。
    /// 基础属性在这里增长，其他系统不应该自己修改 MaxHp/Attack，否则升级规则会散落到多个文件。
    /// </summary>
    public void ApplyLevelUp()
    {
        MaxHp += 20;
        Attack += 5;
        TreatmentCount++;
        RestoreFullHealth();
        RestoreSkillPoints();
    }
}
