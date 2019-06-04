# MasturbationRecorder项目简介
该项目是一个可对事件的发生频率和发生时间进行可视化的UWP应用程序，其使用 GitHub 小绿点的形式表示事件的发生频率和发生时间。MasturbationRecorder 读取一个纯文本中的事件日期和事件频率，然后将其渲染成不同颜色的小方块。每个小方块对应一个日期，方块面板中的每一列对应一年中的一周，每一列中的每个方块对应一周中的某一天，最上面的代表周日，最下面的代表周六。小方块的颜色深浅代表事件的频率。颜色越深事件的频率越高，灰色代表当天没有事件发生。

# 开发环境  ![GitHub](https://img.shields.io/badge/csharp-7.3-blue.svg)
+ IDE： Visual Studio 2017
+ 语言：C# 7.3
+ 额外的依赖库：
  + [System.Numerics][1]
  + [Janyee.Utilty][2]
  
  [1]: https://docs.microsoft.com/en-us/dotnet/api/system.numerics?view=netframework-4.8
  [2]: https://github.com/LiangJianyi/SundryUtilty/tree/master/.NET%20Standard/BigIntegerExtension
  
# 项目结构
项目的解决方案文件名称为 MasturbationRecorder.sln，该解决方案下包含四个项目：
+ BigIntegerExtension
+ ManualTest
+ MasturbationRecorder(Universal Windows)
+ MasturbationRecorderTest(Universal Windows)

MasturbationRecorder(Universal Windows)是主体项目。BigIntegerExtension 来自 Janyee.Utilty，需要手动添加该项目到解决方案下，ManualTest 和 MasturbationRecorderTest(Universal Windows) 是主体项目的测试项目，前者利用控制台输出调试信息用于观测数据的准确性，后者是主体项目的单元测试。

# 开源协议  ![GitHub](https://img.shields.io/github/license/Liangjianyi/MasturbationRecorder.svg?style=popout)
MasturbationRecorder 使用 MIT 协议。本项目的代码可由任何个人和组织随意使用。
