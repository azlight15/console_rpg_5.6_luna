# ConsoleRPG

一个控制台回合制 RPG。

作者：AzLight15

Luna 重构版本当前为 **v1.5.0**。

***

## 当前玩法

- 10 种怪物，包含不同战斗定位。
- 怪物等级根据玩家等级生成。
- 第一场战斗保证为普通怪物，从第二场开始才启用精英怪随机机制。
- 回合制战斗：普通攻击、技能、治疗、撤退。
- 暴击、闪避、技能冷却和状态效果。
- 技能点与技能学习系统。
- 战斗胜利后获得 EXP、金币，并有概率获得装备。
- 普通、稀有、史诗三档装备稀有度。
- 装备背包、装备切换、出售和强化。
- 城镇商店：治疗资源、技能点、基础装备和装备强化。
- 多档案存档、读档和删除。
- 各级菜单统一使用数字 0 返回。

***

## 开始游戏

1. 安装 [.NET](https://dotnet.microsoft.com/zh-cn/download)。
2. 使用 **JetBrains Rider** 打开项目并运行 `Console_RPG.csproj`，或运行已经构建的程序。

项目目标框架：`.NET 10.0`。

***

## 推荐代码阅读顺序

1. `Program.cs` —— 游戏启动、主菜单和流程分发。
2. `Player.cs` —— 玩家状态和最终属性。
3. `Battle.cs` —— 一场战斗如何推进。
4. `MonsterFactory.cs` —— 怪物如何生成和成长。
5. `SkillSystem.cs` / `DamageCalculator.cs` —— 技能和伤害规则。
6. `Equipment.cs` / `EquipmentManager.cs` —— 装备和背包。
7. `Shop.cs` —— 商店与强化。
8. `SaveData.cs` —— 存档与恢复。
9. `StatusEffect.cs` / `StatusEffectSystem.cs` —— 战斗状态效果。

代码注释重点解释“为什么这么设计”，而不是逐行翻译语法。

***

## v1.5.0 测试状态

**核心功能与实际游玩测试全部通过。**

当前正在进行最终的注释、Markdown 和 UI 收尾整理。

***

祝你在这个游戏里玩的开心！
