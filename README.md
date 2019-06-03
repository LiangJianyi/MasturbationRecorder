# MasturbationRecorder项目简介
该项目是一个可对事件的发生频率和发生时间进行可视化的UWP应用程序，其使用 GitHub 小绿点的形式表示事件的发生频率和发生时间。MasturbationRecorder 读取一个纯文本中的事件日期和事件频率，然后将其渲染成不同颜色的小方块。每个小方块对应一个日期，方块面板中的每一列对应一年中的一周，每一列中的每个方块对应一周中的某一天，最上面的代表周日，最下面的代表周六。小方块的颜色深浅代表事件的频率。颜色越深事件的频率越高，灰色代表当天没有事件发生。

# 开发环境
+ IDE： Visual Studio 2017
+ 语言：C# 7.3
+ 额外的依赖库：
  + [System.Numerics][1]
  + [Janyee.Utilty][2]
  
  [1]: https://docs.microsoft.com/en-us/dotnet/api/system.numerics?view=netframework-4.8
  [2]: https://github.com/LiangJianyi/SundryUtilty/tree/master/.NET%20Standard/BigIntegerExtension

# 开源协议
MasturbationRecorder 使用 MIT 协议。本项目的代码可由任何个人和组织随意使用。
