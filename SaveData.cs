// ==========================
// 存档系统
// 负责将玩家当前状态保存为 JSON 文件，并在下次启动时读取恢复
// ==========================

using System;
using System.IO;
using System.Text.Json;

namespace Console_RPG;

/*
    SaveData 是用于序列化存档的数据结构。
    它从 PlayerStatistics 复制数据，用来写入 JSON 文件。
    读取存档时再把数据还原回 PlayerStatistics。
*/
public class SaveData
{
    public string Name { set; get; } = "";   // 玩家名字
    public int Level { get; set; }           // 等级
    public double Exp { get; set; }          // 当前经验值
    public double Hp { get; set; }           // 当前血量
    public double MaxHp { get; set; }        // 最大血量
    public double Attack { get; set; }       // 攻击力
    public double Treatment { get; set; }    // 治疗值
}

/*
    SaveManager 负责存档的保存与读取。
    将玩家数据序列化为 JSON 文件，或从 JSON 文件中恢复数据。
*/
public static class SaveManager
{
    // 存档文件名
    private const string SaveFile = "save.json";
    
    // 保存当前玩家数据到本地文件
    public static void Save()
    {
        // 从 PlayerStatistics 中拷贝当前玩家数据
        var data = new SaveData
        {
            Name = PlayerStatistics.Name,
            Level = PlayerStatistics.Level,
            Exp = PlayerStatistics.Exp,
            Hp = PlayerStatistics.Hp,
            MaxHp = PlayerStatistics.MaxHp,
            Attack = PlayerStatistics.Attack,
            Treatment = PlayerStatistics.Treatment
        };

        // 将数据序列化为 JSON 字符串
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true // 格式化输出，方便查看
        });

        // 写入到本地文件
        File.WriteAllText(SaveFile, json);

        Console.WriteLine("游戏已保存");
        Program.Loading();
    }
    
    // 从本地文件读取存档并恢复玩家数据
    public static void Load()
    {
        // 如果不存在存档文件则提示
        if (!File.Exists(SaveFile))
        {
            Console.WriteLine("没有找到存档文件");
            return;
        }

        // 读取 JSON 文本
        string json = File.ReadAllText(SaveFile);

        // 反序列化为 SaveData 对象
        var data = JsonSerializer.Deserialize<SaveData>(json);

        // 将存档数据还原回 PlayerStatistics
        PlayerStatistics.Name = data!.Name;
        PlayerStatistics.Level = data.Level;
        PlayerStatistics.Exp = data.Exp;
        PlayerStatistics.Hp = data.Hp;
        PlayerStatistics.MaxHp = data.MaxHp;
        PlayerStatistics.Attack = data.Attack;
        PlayerStatistics.Treatment = data.Treatment;

        Console.WriteLine("存档读取成功！");
        Program.Loading();
    }
}
