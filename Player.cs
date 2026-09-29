using System.Collections.Generic;
using System.Linq;

namespace Console_RPG;

// 玩家的运行时状态。
// Player 是整个游戏的“角色本体”：基础属性、装备、背包、技能、金币和战斗资源都放在这里。
// 其他系统不要自己保存一份玩家属性，而应该通过 Player 读取或修改状态，避免数据不同步。
public sealed class Player
{
    // 角色名称。
    public string Name { get; set; } = "";

    // 当前等级，从 1 级开始。
    public int Level { get; set; } = 1;

    // 当前累计经验。升级后只扣除升级所需经验，剩余经验继续保留。
    public double Exp { get; set; }

    // 当前等级需要的升级经验，等级越高升级所需经验越多。
    public double ExpToNextLevel => 100 + (Level - 1) * 40;

    // 当前生命值。只能通过 Player 提供的方法修改。
    public double Hp { get; private set; } = 100;

    // 不计算装备加成的基础最大 HP。
    public double MaxHp { get; private set; } = 100;

    // 不计算武器加成的基础攻击力。
    public double Attack { get; private set; } = 15;

    // 当前装备的武器。
    public Equipment? Weapon { get; private set; }

    // 当前装备的防具。
    public Equipment? Armor { get; private set; }

    // 玩家拥有的全部装备。
    public List<Equipment> Inventory { get; } = new();

    // 玩家已经学会的技能。
    public List<Skill> Skills { get; } = new();

    // 金币，用于商店和技能学习。
    public int Gold { get; private set; } = 100;

    // 当前技能点。释放技能会消耗技能点，升级和战斗胜利可以恢复。
    public int SkillPoints { get; private set; } = 3;

    // 技能点上限随等级缓慢增加。
    public int MaxSkillPoints => 3 + (Level - 1) / 2;

    // 技能冷却属于当前战斗状态，不写进 SaveData。
    // 用技能名称记录剩余回合，避免修改 Skill 数据本身。
    private readonly Dictionary<string, int> _skillCooldowns = new();

    public int GetSkillCooldown(Skill skill) => _skillCooldowns.GetValueOrDefault(skill.Name);

    public bool IsSkillReady(Skill skill) => GetSkillCooldown(skill) <= 0;

    public void StartSkillCooldown(Skill skill)
    {
        if (skill.Cooldown > 0)
            _skillCooldowns[skill.Name] = skill.Cooldown;
    }

    // 一个完整回合结束后，所有正在冷却的技能减少 1 回合。
    public void TickSkillCooldowns()
    {
        foreach (string name in _skillCooldowns.Keys.ToList())
        {
            _skillCooldowns[name]--;
            if (_skillCooldowns[name] <= 0)
                _skillCooldowns.Remove(name);
        }
    }

    public void ResetSkillCooldowns() => _skillCooldowns.Clear();

    // 战斗中的临时状态效果。状态只存在于当前运行时，不会写入 SaveData。
    public List<StatusEffect> StatusEffects { get; } = new();

    public bool HasStatus(StatusEffectType type) =>
        StatusEffects.Exists(effect => effect.Type == type);

    public void ApplyStatus(StatusEffect effect) =>
        StatusEffectSystem.Apply(StatusEffects, effect);

    public double ProcessStatusDamage(out List<string> messages)
    {
        double damage = StatusEffectSystem.ProcessTurnStart(StatusEffects, out messages);
        if (damage > 0)
            TakeDamage(damage);
        return damage;
    }

    public void ClearStatusEffects() => StatusEffectSystem.Clear(StatusEffects);

    // 基础治疗量。
    public double Treatment { get; private set; } = 50;

    // 剩余治疗次数。
    public int TreatmentCount { get; private set; } = 3;

    // 最终攻击力 = 基础攻击力 + 当前武器攻击加成。
    public double FinalAttack => Attack + (Weapon?.AttackBonus ?? 0);

    // 最终最大 HP = 基础最大 HP + 当前防具 HP 加成。
    public double FinalMaxHp => MaxHp + (Armor?.HpBonus ?? 0);

    // 最终暴击率由基础暴击率和装备加成共同决定。
    public double FinalCriticalRate => 0.10 + (Weapon?.CriticalRateBonus ?? 0) + (Armor?.CriticalRateBonus ?? 0);

    // 最终闪避率由装备加成共同决定。
    public double FinalEvasionRate => (Weapon?.EvasionRateBonus ?? 0) + (Armor?.EvasionRateBonus ?? 0);

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

    public void AddTreatmentCount(int amount)
    {
        if (amount > 0) TreatmentCount += amount;
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

    // 直接把技能放入已学习列表。初始化和读档使用这个方法，不负责收费。
    public void LearnSkill(Skill skill)
    {
        if (!Skills.Exists(existing => existing.Name == skill.Name))
            Skills.Add(skill);
    }

    // 玩家从技能管理界面学习技能。
    // 学习消耗金币，不消耗战斗技能点；这样“学会技能”和“释放技能”不会混成一套资源。
    public bool TryLearnSkill(Skill skill)
    {
        if (skill is null || HasSkill(skill.Name))
            return false;

        if (Level < skill.LearnLevel || skill.LearnCostGold < 0)
            return false;

        if (!TrySpendGold(skill.LearnCostGold))
            return false;

        Skills.Add(skill);
        return true;
    }

    public bool HasSkill(string skillName) =>
        !string.IsNullOrWhiteSpace(skillName) &&
        Skills.Exists(skill => skill.Name == skillName);

    public bool TryUseSkillPoint(int cost)
    {
        if (cost <= 0) return true;
        if (SkillPoints < cost) return false;
        SkillPoints -= cost;
        return true;
    }

    public void RecoverSkillPoint()
    {
        SkillPoints = System.Math.Min(MaxSkillPoints, SkillPoints + 1);
    }

    private void RestoreSkillPoints()
    {
        SkillPoints = MaxSkillPoints;
    }

    public void AddGold(int amount)
    {
        if (amount > 0) Gold += amount;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount < 0 || Gold < amount) return false;
        Gold -= amount;
        return true;
    }

    public int SellEquipment(int index)
    {
        if (index < 0 || index >= Inventory.Count) return 0;
        Equipment equipment = Inventory[index];
        if (equipment == Weapon || equipment == Armor) return 0;

        int price = equipment.GetSellPrice();
        Inventory.RemoveAt(index);
        Gold += price;
        return price;
    }

    public bool EnhanceEquipment(int index)
    {
        if (index < 0 || index >= Inventory.Count) return false;

        Equipment equipment = Inventory[index];
        int cost = equipment.GetEnhancementCost();
        if (cost <= 0 || !TrySpendGold(cost)) return false;

        if (!equipment.Enhance())
        {
            Gold += cost;
            return false;
        }

        return true;
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

        if (Weapon is not null)
        {
            Equipment? savedWeapon = Inventory.FirstOrDefault(item =>
                item.Type == Weapon.Type && item.Name == Weapon.Name);
            if (savedWeapon is not null)
                Weapon = savedWeapon;
            else
                Inventory.Add(Weapon);
        }

        if (Armor is not null)
        {
            Equipment? savedArmor = Inventory.FirstOrDefault(item =>
                item.Type == Armor.Type && item.Name == Armor.Name);
            if (savedArmor is not null)
                Armor = savedArmor;
            else
                Inventory.Add(Armor);
        }

        SkillPoints = System.Math.Clamp(skillPoints, 0, MaxSkillPoints);
        ResetSkillCooldowns();
        ClearStatusEffects();
    }

    public void RestoreFullHealth() => Hp = FinalMaxHp;

    public void ApplyLevelUp()
    {
        MaxHp += 20;
        Attack += 5;
        TreatmentCount++;
        RestoreFullHealth();
        RestoreSkillPoints();
    }
}