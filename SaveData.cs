using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Console_RPG;

/// <summary>
/// JSON 存档的数据传输模型。
/// 只保存需要持久化的玩家状态，不直接承担运行时逻辑。
/// v0.4.0 开始同时保存当前装备和已学习技能。
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
    public List<Skill> Skills { get; set; } = new();

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
            Skills = new List<Skill>(player.Skills)
        };
    }

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
            Skills);
    }
}

/// <summary>
/// 负责本地档案的创建、保存、读取和删除。
///
/// v0.4.0 使用 saves 文件夹保存多个档案，并兼容旧版 save.json。
/// 存档/读档/删档都会先显示档案列表，再进行确认。
/// </summary>
public static class SaveManager
{
    private const string SaveDirectory = "saves";
    private const string SaveExtension = ".json";
    private const string LegacySaveFile = "save.json";

    private sealed record SaveProfile(string Name, string FilePath);

    public static bool HasAnySave()
    {
        return GetProfiles().Count > 0;
    }

    public static void Save(Player player)
    {
        List<SaveProfile> profiles = GetProfiles();

        Console.Clear();
        Console.WriteLine("========== 存档 ==========");
        PrintProfiles(profiles);
        Console.WriteLine($"{profiles.Count + 1}. 新建档案");
        Console.WriteLine("0. 返回");
        Console.Write("请选择档案：");

        if (!int.TryParse(Console.ReadLine(), out int index)
            || index < 0
            || index > profiles.Count + 1)
        {
            Console.WriteLine("输入无效。");
            Program.Loading();
            return;
        }

        if (index == 0)
        {
            return;
        }

        string filePath;
        string profileName;

        if (index == profiles.Count + 1)
        {
            Console.Write($"请输入新档案名称（直接回车使用角色名“{player.Name}”）：");
            string? inputName = Console.ReadLine();
            profileName = string.IsNullOrWhiteSpace(inputName)
                ? player.Name.Trim()
                : inputName.Trim();

            if (string.IsNullOrWhiteSpace(profileName))
            {
                Console.WriteLine("档案名称不能为空。");
                Program.Loading();
                return;
            }

            filePath = GetProfilePath(profileName);

            if (File.Exists(filePath))
            {
                if (!Confirm($"档案“{profileName}”已存在，是否覆盖？"))
                {
                    return;
                }
            }
            else if (!Confirm($"确定将当前进度保存为档案“{profileName}”吗？"))
            {
                return;
            }
        }
        else
        {
            SaveProfile profile = profiles[index - 1];
            profileName = profile.Name;
            filePath = profile.FilePath;

            if (!Confirm($"确定覆盖档案“{profileName}”吗？"))
            {
                return;
            }
        }

        try
        {
            Directory.CreateDirectory(SaveDirectory);
            SaveData data = SaveData.FromPlayer(player);
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

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

        if (!int.TryParse(Console.ReadLine(), out int index)
            || index < 0
            || index > profiles.Count)
        {
            Console.WriteLine("输入无效。");
            Program.Loading();
            return false;
        }

        if (index == 0)
        {
            return false;
        }

        SaveProfile selected = profiles[index - 1];
        if (!Confirm($"确定读取档案“{selected.Name}”吗？当前未保存的进度会被覆盖。"))
        {
            return false;
        }

        try
        {
            string json = File.ReadAllText(selected.FilePath);
            SaveData? data = JsonSerializer.Deserialize<SaveData>(json);

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

    /// <summary>
    /// 显示档案列表并删除选中的档案。
    /// 删除前必须二次确认，避免误删角色进度。
    /// </summary>
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

        if (!int.TryParse(Console.ReadLine(), out int index)
            || index < 0
            || index > profiles.Count)
        {
            Console.WriteLine("输入无效。");
            Program.Loading();
            return;
        }

        if (index == 0)
        {
            return;
        }

        SaveProfile selected = profiles[index - 1];
        if (!Confirm($"确定永久删除档案“{selected.Name}”吗？此操作无法撤销。"))
        {
            return;
        }

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

    private static List<SaveProfile> GetProfiles()
    {
        List<SaveProfile> profiles = new();

        if (File.Exists(LegacySaveFile))
        {
            TryAddProfile(profiles, LegacySaveFile);
        }

        if (Directory.Exists(SaveDirectory))
        {
            foreach (string filePath in Directory.GetFiles(SaveDirectory, $"*{SaveExtension}"))
            {
                TryAddProfile(profiles, filePath);
            }
        }

        return profiles
            .OrderBy(profile => profile.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void TryAddProfile(List<SaveProfile> profiles, string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            SaveData? data = JsonSerializer.Deserialize<SaveData>(json);

            if (IsValid(data))
            {
                profiles.Add(new SaveProfile(data!.Name.Trim(), filePath));
            }
        }
        catch (JsonException)
        {
            // 损坏的档案不进入正常列表。
        }
        catch (IOException)
        {
            // 单个文件读取失败时跳过它，继续扫描其他档案。
        }
    }

    private static void PrintProfiles(List<SaveProfile> profiles)
    {
        if (profiles.Count == 0)
        {
            Console.WriteLine("暂无已有档案。");
            return;
        }

        for (int i = 0; i < profiles.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {profiles[i].Name}");
        }
    }

    private static bool Confirm(string message)
    {
        Console.Write($"{message} (Y/N)：");
        char choice = Console.ReadKey(true).KeyChar;
        Console.WriteLine(choice);
        return choice is 'Y' or 'y';
    }

    private static string GetProfilePath(string profileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string safeName = string.Concat(profileName.Select(character =>
            invalidChars.Contains(character) ? '_' : character));

        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = "Player";
        }

        return Path.Combine(SaveDirectory, safeName + SaveExtension);
    }

    private static bool IsValid(SaveData? data)
    {
        if (data is null
            || string.IsNullOrWhiteSpace(data.Name)
            || data.Level < 1
            || data.Exp < 0
            || data.MaxHp <= 0
            || data.Hp < 0
            || data.Attack <= 0
            || data.Treatment < 0
            || data.TreatmentCount < 0)
        {
            return false;
        }

        double armorBonus = data.Armor?.HpBonus ?? 0;
        return data.Hp <= data.MaxHp + armorBonus;
    }
}
