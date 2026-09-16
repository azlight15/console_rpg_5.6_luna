using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Console_RPG;

/// <summary>
/// JSON 存档的数据传输模型（DTO）。
///
/// DTO 的作用是把 Player 的运行时状态转换成适合保存的普通数据。
/// 这样 SaveManager 不需要直接操作 Player 的私有属性，也更容易以后修改存档格式。
/// 新增字段都有默认值，所以旧版存档缺少金币、技能点时仍然可以读取。
/// </summary>
public sealed class SaveData
{
    public string Name { get; set; } = "";
    public int Level { get; set; }
    public double Exp { get; set; }
    public double Hp { get; set; }
    public double MaxHp { get; set; }
    public double Attack { get; set; }
    public double Treatment { get; set; }
    public int TreatmentCount { get; set; }
    public Equipment? Weapon { get; set; }
    public Equipment? Armor { get; set; }
    public List<Equipment> Inventory { get; set; } = new();
    public List<Skill> Skills { get; set; } = new();

    /// <summary>v1.0 经济系统：玩家当前金币。</summary>
    public int Gold { get; set; } = 100;

    /// <summary>v1.0 技能资源：玩家当前技能点。</summary>
    public int SkillPoints { get; set; } = 3;

    /// <summary>把运行时 Player 拆成可序列化的数据。</summary>
    public static SaveData FromPlayer(Player player)
    {
        return new SaveData
        {
            Name = player.Name,
            Level = player.Level,
            Exp = player.Exp,
            Hp = player.Hp,
            MaxHp = player.MaxHp,
            Attack = player.Attack,
            Treatment = player.Treatment,
            TreatmentCount = player.TreatmentCount,
            Weapon = player.Weapon,
            Armor = player.Armor,
            Inventory = new List<Equipment>(player.Inventory),
            Skills = new List<Skill>(player.Skills),
            Gold = player.Gold,
            SkillPoints = player.SkillPoints
        };
    }

    /// <summary>把存档数据交给 Player，由 Player 负责真正恢复状态。</summary>
    public void ApplyTo(Player player)
    {
        player.Name = Name.Trim();
        player.Level = Level;
        player.Exp = Exp;
        player.RestoreFromSave(
            MaxHp,
            Hp,
            Attack,
            Treatment,
            TreatmentCount,
            Weapon,
            Armor,
            Skills,
            Inventory,
            Gold,
            SkillPoints);
    }
}

/// <summary>
/// 本地档案管理器。
///
/// SaveManager 只负责文件层面的事情：列出档案、确认操作、读写 JSON、处理文件错误。
/// 它不负责战斗、升级或装备计算，这些规则仍然属于各自的游戏系统。
/// </summary>
public static class SaveManager
{
    private const string SaveDirectory = "saves";
    private const string SaveExtension = ".json";
    private const string LegacySaveFile = "save.json";

    private sealed record SaveProfile(string Name, string FilePath);

    public static bool HasAnySave() => GetProfiles().Count > 0;

    /// <summary>让玩家选择一个档案覆盖，或者创建新档案。</summary>
    public static void Save(Player player)
    {
        List<SaveProfile> profiles = GetProfiles();
        Console.Clear();
        Console.WriteLine("========== 存档 ==========");
        PrintProfiles(profiles);
        Console.WriteLine($"{profiles.Count + 1}. 新建档案");
        Console.WriteLine("0. 返回");
        Console.Write("请选择档案：");

        if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index > profiles.Count + 1)
        {
            Console.WriteLine("输入无效。");
            Program.Loading();
            return;
        }
        if (index == 0) return;

        string filePath;
        string profileName;
        if (index == profiles.Count + 1)
        {
            Console.Write($"请输入新档案名称（直接回车使用角色名“{player.Name}”）：");
            string? inputName = Console.ReadLine();
            profileName = string.IsNullOrWhiteSpace(inputName) ? player.Name.Trim() : inputName.Trim();
            if (string.IsNullOrWhiteSpace(profileName))
            {
                Console.WriteLine("档案名称不能为空。");
                Program.Loading();
                return;
            }

            filePath = GetProfilePath(profileName);
            if (File.Exists(filePath))
            {
                if (!Confirm($"档案“{profileName}”已存在，是否覆盖？")) return;
            }
            else if (!Confirm($"确定将当前进度保存为档案“{profileName}”吗？")) return;
        }
        else
        {
            SaveProfile profile = profiles[index - 1];
            profileName = profile.Name;
            filePath = profile.FilePath;
            if (!Confirm($"确定覆盖档案“{profileName}”吗？")) return;
        }

        try
        {
            Directory.CreateDirectory(SaveDirectory);
            string json = JsonSerializer.Serialize(
                SaveData.FromPlayer(player),
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            Console.WriteLine($"档案“{profileName}”保存成功！");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"保存失败：{ex.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("保存失败：当前目录没有写入权限。");
        }

        Program.Loading();
    }

    /// <summary>选择一个档案读取，并用读取的数据覆盖当前角色状态。</summary>
    public static bool Load(Player player)
    {
        List<SaveProfile> profiles = GetProfiles();
        if (profiles.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("目前没有可读取的档案。");
            Program.Loading();
            return false;
        }

        Console.Clear();
        Console.WriteLine("========== 读档 ==========");
        PrintProfiles(profiles);
        Console.WriteLine("0. 返回");
        Console.Write("请选择档案：");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index > profiles.Count)
        {
            Console.WriteLine("输入无效。");
            Program.Loading();
            return false;
        }
        if (index == 0) return false;

        SaveProfile selected = profiles[index - 1];
        if (!Confirm($"确定读取档案“{selected.Name}”吗？当前未保存的进度会被覆盖。")) return false;

        try
        {
            SaveData? data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(selected.FilePath));
            if (!IsValid(data))
            {
                Console.WriteLine("档案数据无效，未加载该档案。");
                Program.Loading();
                return false;
            }

            data!.ApplyTo(player);
            Console.WriteLine($"档案“{selected.Name}”读取成功！");
            Program.Loading();
            return true;
        }
        catch (JsonException)
        {
            Console.WriteLine("档案格式损坏，无法读取。");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"读取失败：{ex.Message}");
        }

        Program.Loading();
        return false;
    }

    /// <summary>列出档案并在二次确认后永久删除。</summary>
    public static void Delete()
    {
        List<SaveProfile> profiles = GetProfiles();
        if (profiles.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("目前没有可删除的档案。");
            Program.Loading();
            return;
        }

        Console.Clear();
        Console.WriteLine("========== 删除档案 ==========");
        PrintProfiles(profiles);
        Console.WriteLine("0. 返回");
        Console.Write("请选择要删除的档案：");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index > profiles.Count)
        {
            Console.WriteLine("输入无效。");
            Program.Loading();
            return;
        }
        if (index == 0) return;

        SaveProfile selected = profiles[index - 1];
        if (!Confirm($"确定永久删除档案“{selected.Name}”吗？此操作无法撤销。")) return;

        try
        {
            File.Delete(selected.FilePath);
            Console.WriteLine($"档案“{selected.Name}”已删除。");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"删除失败：{ex.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("删除失败：当前目录没有删除权限。");
        }

        Program.Loading();
    }

    /// <summary>扫描旧版单档 save.json 和 v0.4+ 的 saves/*.json。</summary>
    private static List<SaveProfile> GetProfiles()
    {
        List<SaveProfile> profiles = new();
        if (File.Exists(LegacySaveFile)) TryAddProfile(profiles, LegacySaveFile);

        if (Directory.Exists(SaveDirectory))
        {
            foreach (string filePath in Directory.GetFiles(SaveDirectory, $"*{SaveExtension}"))
                TryAddProfile(profiles, filePath);
        }

        return profiles.OrderBy(profile => profile.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>尝试读取一个档案；损坏档案直接跳过，不影响其他正常档案。</summary>
    private static void TryAddProfile(List<SaveProfile> profiles, string filePath)
    {
        try
        {
            SaveData? data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(filePath));
            if (IsValid(data)) profiles.Add(new SaveProfile(data!.Name.Trim(), filePath));
        }
        catch (JsonException) { }
        catch (IOException) { }
    }

    /// <summary>统一显示档案列表，避免存档、读档、删档各写一套相同代码。</summary>
    private static void PrintProfiles(List<SaveProfile> profiles)
    {
        if (profiles.Count == 0)
        {
            Console.WriteLine("暂无已有档案。");
            return;
        }

        for (int i = 0; i < profiles.Count; i++)
            Console.WriteLine($"{i + 1}. {profiles[i].Name}");
    }

    /// <summary>危险操作统一使用 Y/N 确认。</summary>
    private static bool Confirm(string message)
    {
        Console.Write($"{message} (Y/N)：");
        char choice = Console.ReadKey(true).KeyChar;
        Console.WriteLine(choice);
        return choice is 'Y' or 'y';
    }

    /// <summary>把档案名转换成当前操作系统允许的安全文件名。</summary>
    private static string GetProfilePath(string profileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string safeName = string.Concat(profileName.Select(character => invalidChars.Contains(character) ? '_' : character));
        if (string.IsNullOrWhiteSpace(safeName)) safeName = "Player";
        return Path.Combine(SaveDirectory, safeName + SaveExtension);
    }

    /// <summary>读取前检查核心字段，避免明显非法数据进入运行时 Player。</summary>
    private static bool IsValid(SaveData? data)
    {
        if (data is null || string.IsNullOrWhiteSpace(data.Name) || data.Level < 1 || data.Exp < 0
            || data.MaxHp <= 0 || data.Hp < 0 || data.Attack <= 0 || data.Treatment < 0
            || data.TreatmentCount < 0 || data.Gold < 0 || data.SkillPoints < 0)
            return false;

        double armorBonus = data.Armor?.HpBonus ?? 0;
        return data.Hp <= data.MaxHp + armorBonus;
    }
}
