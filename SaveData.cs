using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Console_RPG;

// SaveData 是“存档用的数据盒子”。
// Player 是游戏运行时的角色，而 SaveData 只是把需要保存的东西装起来。
// 这样以后修改 Player 的内部实现时，不需要让文件读写代码到处跟着改。
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

    // v1 新增的资源也要跟着存档，否则退出游戏后金币和技能点会丢失。
    public int Gold { get; set; } = 100;
    public int SkillPoints { get; set; } = 3;

    // 把 Player 当前状态复制成一个适合 JSON 序列化的对象。
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

    // 把存档内容交回 Player。
    // 具体怎么恢复 HP、装备、技能等状态，由 Player 自己决定。
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

// 本地存档管理器。
// 它只负责文件：列出档案、保存、读取、删除和处理文件错误。
// 战斗、升级、装备属性等游戏规则不应该写在这里。
public static class SaveManager
{
    private const string SaveDirectory = "saves";
    private const string SaveExtension = ".json";
    private const string LegacySaveFile = "save.json";

    private sealed record SaveProfile(string Name, string FilePath);

    // 启动时用这个方法判断有没有可以读取的存档。
    public static bool HasAnySave() => GetProfiles().Count > 0;

    // 显示档案列表，让玩家决定覆盖哪个档案或创建新档案。
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
            // saves 文件夹不存在时自动创建。
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

    // 选择一个档案读取，并用档案内容覆盖当前角色。
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

    // 删除档案属于不可逆操作，所以一定要经过二次确认。
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

    // 同时检查旧版 save.json 和现在的 saves/*.json。
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

    // 尝试把一个 JSON 文件加入档案列表。
    // 如果文件损坏，就跳过它，不让一个坏档案拖垮整个游戏。
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

    // 统一显示档案编号，存档、读档和删档都使用这一套列表。
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

    // 覆盖和删除前都通过这个方法询问玩家。
    private static bool Confirm(string message)
    {
        Console.Write($"{message} (Y/N)：");
        char choice = Console.ReadKey(true).KeyChar;
        Console.WriteLine(choice);
        return choice is 'Y' or 'y';
    }

    // 档案名最终会变成文件名，所以要把系统不允许的字符替换掉。
    private static string GetProfilePath(string profileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string safeName = string.Concat(profileName.Select(character => invalidChars.Contains(character) ? '_' : character));
        if (string.IsNullOrWhiteSpace(safeName)) safeName = "Player";
        return Path.Combine(SaveDirectory, safeName + SaveExtension);
    }

    // 读取档案前先做最基本的数据检查，避免明显非法数据进入游戏。
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
