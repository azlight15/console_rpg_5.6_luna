using System;
using System.IO;
using System.Text.Json;

namespace Console_RPG;

/// <summary>
/// JSON 存档的数据传输模型。
/// 只保存需要持久化的玩家状态，不直接暴露运行时逻辑。
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
}

/// <summary>负责把玩家状态写入本地 JSON，并在读取时进行基本校验。</summary>
public static class SaveManager
{
    private const string SaveFile = "save.json";

    /// <summary>保存当前角色状态。</summary>
    public static void Save()
    {
        try
        {
            SaveData data = new()
            {
                Name = PlayerStatistics.Name,
                Level = PlayerStatistics.Level,
                Exp = PlayerStatistics.Exp,
                Hp = PlayerStatistics.Hp,
                MaxHp = PlayerStatistics.MaxHp,
                Attack = PlayerStatistics.Attack,
                Treatment = PlayerStatistics.Treatment
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(SaveFile, json);
            Console.WriteLine("游戏已保存。");
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

    /// <summary>读取存档，并在数据明显异常时拒绝加载。</summary>
    public static void Load()
    {
        if (!File.Exists(SaveFile))
        {
            Console.WriteLine("没有找到存档文件。");
            Program.Loading();
            return;
        }

        try
        {
            string json = File.ReadAllText(SaveFile);
            SaveData? data = JsonSerializer.Deserialize<SaveData>(json);

            if (!IsValid(data))
            {
                Console.WriteLine("存档数据无效，未加载该存档。");
                Program.Loading();
                return;
            }

            PlayerStatistics.Name = data!.Name.Trim();
            PlayerStatistics.Level = data.Level;
            PlayerStatistics.Exp = data.Exp;
            PlayerStatistics.MaxHp = data.MaxHp;
            PlayerStatistics.Hp = Math.Clamp(data.Hp, 0, data.MaxHp);
            PlayerStatistics.Attack = data.Attack;
            PlayerStatistics.Treatment = data.Treatment;

            Console.WriteLine("存档读取成功！");
        }
        catch (JsonException)
        {
            Console.WriteLine("存档格式损坏，无法读取。");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"读取失败：{ex.Message}");
        }

        Program.Loading();
    }

    private static bool IsValid(SaveData? data)
    {
        return data is not null
            && !string.IsNullOrWhiteSpace(data.Name)
            && data.Level >= 1
            && data.Exp >= 0
            && data.MaxHp > 0
            && data.Hp >= 0
            && data.Hp <= data.MaxHp
            && data.Attack > 0
            && data.Treatment >= 0;
    }
}
