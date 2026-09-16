using System.Collections.Generic;

namespace Console_RPG;

/// <summary>
/// 玩家运行时状态。
/// v0.5.0 开始持有装备库存，并集中计算装备影响后的最终战斗属性。
/// </summary>
public sealed class Player
{
    public string Name { get; set; } = "";
    public int Level { get; set; } = 1;
    public double Exp { get; set; }

    public double ExpToNextLevel => 100 + (Level - 1) * 40;

    public double Hp { get; private set; } = 100;
    public double MaxHp { get; private set; } = 100;
    public double Attack { get; private set; } = 15;

    public Equipment? Weapon { get; private set; }
    public Equipment? Armor { get; private set; }
    public List<Equipment> Inventory { get; } = new();
    public List<Skill> Skills { get; } = new();

    public double FinalAttack => Attack + (Weapon?.AttackBonus ?? 0);
    public double FinalMaxHp => MaxHp + (Armor?.HpBonus ?? 0);
    public double FinalCriticalRate => 0.10 + (Weapon?.CriticalRateBonus ?? 0) + (Armor?.CriticalRateBonus ?? 0);
    public double FinalEvasionRate => (Weapon?.EvasionRateBonus ?? 0) + (Armor?.EvasionRateBonus ?? 0);

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

    public void AddEquipment(Equipment equipment) => Inventory.Add(equipment);

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

        if (Hp > FinalMaxHp)
            Hp = FinalMaxHp;
        return true;
    }

    public void EquipWeapon(Equipment equipment)
    {
        Weapon = equipment;
        if (!Inventory.Contains(equipment)) Inventory.Add(equipment);
    }

    public void EquipArmor(Equipment equipment)
    {
        Armor = equipment;
        if (!Inventory.Contains(equipment)) Inventory.Add(equipment);
    }

    public void LearnSkill(Skill skill)
    {
        if (!Skills.Exists(existing => existing.Name == skill.Name))
            Skills.Add(skill);
    }

    public void RestoreFromSave(
        double maxHp,
        double hp,
        double attack,
        double treatment,
        int treatmentCount,
        Equipment? weapon,
        Equipment? armor,
        IEnumerable<Skill>? skills,
        IEnumerable<Equipment>? inventory = null)
    {
        MaxHp = maxHp;
        Attack = attack;
        Treatment = treatment;
        TreatmentCount = treatmentCount;
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

        // 兼容 v0.4 存档：旧档没有库存时，把当前装备补进库存。
        if (Weapon is not null && !Inventory.Exists(item => item.Name == Weapon.Name))
            Inventory.Add(Weapon);
        if (Armor is not null && !Inventory.Exists(item => item.Name == Armor.Name))
            Inventory.Add(Armor);
    }

    public void RestoreFullHealth() => Hp = FinalMaxHp;

    public void ApplyLevelUp()
    {
        MaxHp += 20;
        Attack += 5;
        TreatmentCount++;
        RestoreFullHealth();
    }
}
