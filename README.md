# console_rpg_5.6_luna

一个控制台回合制 RPG 项目。

这是原 Console_RPG 项目的重构版本，重点整理了输入校验、战斗流程、升级、怪物生成、存档，以及玩家状态管理。

## 项目信息

- 游戏名：Console_RPG
- .NET 版本：10.0
- 作者：AzLight15
- 重构版本：GPT-5.6 Luna

## 当前结构

- `Program.cs`：程序入口与主菜单
- `Player.cs`：玩家角色实体
- `Monster.cs`：怪物数据模型
- `MonsterFactory.cs`：怪物生成
- `Battle.cs`：战斗系统
- `LevelUp.cs`：经验与升级
- `Heal.cs`：治疗
- `ShowStatus.cs`：状态显示
- `SaveData.cs`：JSON 存档

当前玩家状态由 `Player` 实例管理，各系统通过参数访问玩家数据，不再依赖全局静态 `PlayerStatistics`。

## 文档

详细的系统说明和后续计划见 `DevelopNotes.md`。
