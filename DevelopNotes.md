# 开发笔记

***

## 关于Console_RPG

这是我个人开发的一款控制台游戏，可能功能不够全，在此致歉！

如果想玩的话，请在[GitHub](https://github.com/azlight15/console_rpg)页面下载文件

***

## 这个项目有哪些系统？

现在有以下系统：

### 1.`Program.cs`

这是主程序文件，里面有：
主函数——`Program.Main`、
开始菜单——`Program.StartMenu`、
确认菜单——`Program.GameConfirmed`、
选择页面——`Program.OptionsMenu`

架构是这样子：

```csharp
public static class Program
{
    private static bool _running = true;//游戏运行状态
    
    private static void Main()
    {
        StartMenu();
        GameConfirmed();
        while (_running)
        {
            OptionsMenu();
        }
    }
```

***

### 2.`Battle.cs`

这是对战系统

里面有两个方法：对战确认——`StartBattle`和对战系统——`Battle`

流程：

从`Program.OptionsMenu`（选择页面）选择**开始对战**后，会跳到`StartBattle`方法

这时会判断你的Hp值是不是大于等于0，判断通过后从怪物工厂——`MonsterFactory.cs`导入并显示怪物信息，之后转到`Battle`方法进行一换一对战

对战完成后，同时获得经验（Battle部分）并自动升级你的等级，然后回到`StartBattle`方法进行询问是否继续对战

填`y`（大小写都可以）继续，否则回到选择页面

***

### 3.`LevelUp.cs`

这是升级系统，里面有获得经验（LevelUp部分）和升级方法

不知道怎么描述，上个图吧（

![](photo/LevelUp.png)

~~总感觉像是上了双重保险~~

***

### 4.`Heal.cs`

这个是**治疗功能**，具体工作流程自己去看

***

### 5.`ShowStatus.cs`

这个是**显示玩家当前状态**的，懂得都懂，不赘述

***

### 6.`Player.cs`和`Monster.cs`

这两个是**玩家**和**怪物**的数值面板和基础信息，不多说

***

### 7.`MonsterFactory.cs`

这是“怪物工厂”，是怪物的预设

怪物的数值会跟玩家的等级的增长而增长

该文件搭配`Battle.cs`

***

### 8.`SaveData.cs`

这个是**存档系统**

工作流程：

* 预先建好`Savedata`类
* 把数据存到json文件（存档文件）
* 随时可以存储或读取存档

存档文件一般在`console_rpg-main/bin/Debug/net10.0/save.json`

***

## 之后要做什么？

想做其他项目就做其他项目