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

MasturbationRecorder的大致运行流程如下：

当用户点击文件选择器并选择指定的文本文件后（只能打开 .txt .mast 后缀的文件），提取文本内容，将文本的每一行切割成 string 对象装载为一个 IEnumerable<string>，然后将该文本序列传递给 MainPageViewModel.LinesConvertToStatistTotalByDateTimes 方法，MainPageViewModel.LinesConvertToStatistTotalByDateTimes 方法把每行 string 转换为一个 StatistTotalByDateTime 对象，最后装载成 LinkedList<StatistTotalByDateTime> 传递给 MainPageViewModel.GroupDateTimesByTotal 方法。MainPageViewModel.GroupDateTimesByTotal 方法对链表按 StatistTotalByDateTime.Total 分组，最后对分组结果进行分级，传递给 DrawRectangleColor 方法根据每个日期所在分组的级别对每个方块进行着色。
  
分组结果的结构如下：

![List<IGrouping<BigInteger, StatistTotalByDateTime>>[]](https://github.com/LiangJianyi/liangjianyi.github.io/raw/master/image/%E5%88%86%E7%BA%A7%E8%A1%A8%E7%BB%93%E6%9E%84.bmp)

最外层的灰色边框代表一个数组，类型为List<IGrouping<BigInteger, StatistTotalByDateTime>>[]，其第一列的数组是每个元素的索引，索引越大，记录级别越大，第二列为每个元素对应的List<IGrouping<BigInteger, StatistTotalByDateTime>>，用红框表示List的范围，索引越大的元素，其保存在List中的每个group的Key也越大。Key代表当前分组的事件频率，每个列表的元素按Key值升序排列。每个Key对应一张由StatistTotalByDateTime组成的表，同一个Key中，每个元素的StatistTotalByDateTime.Total相同。

DrawRectangleColor 方法的着色原理很简单，传递List<IGrouping<BigInteger, StatistTotalByDateTime>>[]的长度给ClassifyColorByLevelScore方法获得色阶字典，类型为IDictionary<int, SolidColorBrush>，然后根据每个条目所在的索引分配指定颜色。

  
注意：整个项目使用整数的地方几乎都采用 BigInteger，这是为了对付几亿数据量设计的，用来做极限测试。

# 开源协议  ![GitHub](https://img.shields.io/github/license/Liangjianyi/MasturbationRecorder.svg?style=popout)
MasturbationRecorder 使用 MIT 协议。本项目的代码可由任何个人和组织随意使用。
