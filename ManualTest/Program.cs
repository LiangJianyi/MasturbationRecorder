using System;
using System.Security.Permissions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 该项目只要用于输出log等测试数据，为 MasturbationRecorder 解决方案下的项目
/// 进行人工数据比对。
/// </summary>
namespace ManualTest {
    class Program {
        static void Main(string[] args) {
            const string MasturbationRecorder_exe_path = @"C:\Users\a124p\Documents\GitHub\MasturbationRecorder\MasturbationRecorder\bin\x64\Debug\MasturbationRecorder.exe";
            const string MasturbationRecorderTest_path = @"C:\Users\a124p\Documents\GitHub\MasturbationRecorder\MasturbationRecorderTest\bin\x64\Debug\MasturbationRecorderTest.exe";
            const string MasturbationRecorder_path = @"C:\Users\a124p\Documents\GitHub\MasturbationRecorder";

            FileIOPermission permission = new FileIOPermission(
                FileIOPermissionAccess.Read,
                MasturbationRecorder_path
            );
            permission.Demand();

            Assembly MasturbationRecorderAssembly = Assembly.LoadFrom(MasturbationRecorder_exe_path);
            Assembly MasturbationRecorderTestAssembly = Assembly.LoadFrom(MasturbationRecorderTest_path);
            MasturbationRecorder_DatetimeParser_StringToUInt16_Test(MasturbationRecorderAssembly);
            MasturbationRecorder_MainPageViewModel_GroupDateTimesDiff_Test(MasturbationRecorderAssembly, MasturbationRecorderTestAssembly);

            Console.ReadKey();
        }

        private static void MasturbationRecorder_DatetimeParser_StringToUInt16_Test(Assembly assembly) {
            Console.WriteLine("Executing MasturbationRecorder_DatetimeParser_StringToUInt16_Test:");

            var methodInfo = assembly.GetType("MasturbationRecorder.DatetimeParser")
                                     .GetMethod("StringToUInt16", BindingFlags.NonPublic | BindingFlags.Static);
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "mon" }));
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "feb" }));
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "mar" }));
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "apr" }));
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "may" }));
        }

        /*
         * 该方法使用的反射技术参见： https://docs.microsoft.com/zh-cn/dotnet/framework/reflection-and-codedom/how-to-examine-and-instantiate-generic-types-with-reflection
         */
        private static void MasturbationRecorder_MainPageViewModel_GroupDateTimesDiff_Test(Assembly mastAssembly, Assembly mastTestAssembly) {
            Console.WriteLine("Executing MasturbationRecorder_MainPageViewModel_GroupDateTimesDiff_Test:");

            var groupDateTimesByDiffInfo = mastAssembly.GetType("MasturbationRecorder.MainPageViewModel")
                                                       .GetMethod("GroupDateTimesByDiff", BindingFlags.Public | BindingFlags.Static);
            object testData_linkedlist_dateTimes = MakeTestDataObject(mastAssembly, mastTestAssembly);
            /*
             * 下面的循环目的是为了遍历所有测试数据 testData_linkedlist_dateTimes，
             * 每次遍历都把当前元素临时封装成链表然后传送给 MainPageViewModel.GroupDateTimesByDiff，
             * 然后再把转换出的结果打印一遍，然后人工校对打印结果，这样做的好处是如果
             * MainPageViewModel.GroupDateTimesByDiff 存在运行时异常，我们可以知道
             * 它对 DateTimes 分组并发生异常之前执行到哪一行数据。
             * 这里的 for 语句等价于 
             * for(var node = testData_linkedlist_dateTimes.First; 
             *     node != null; 
             *     node = node.Next;) {
             * }
             */
            for (var node = testData_linkedlist_dateTimes.GetType().InvokeMember(
                                                            name: "First",
                                                            invokeAttr: BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                                                            binder: null,
                                                            target: testData_linkedlist_dateTimes,
                                                            args: null);
                node != null;
                node = node.GetType().InvokeMember("Next", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, node, null)
                ) {

                // 把 StatistTotalByDateTime 作为 LinkedList 的泛型参数
                Type statistTotalByDateTime = mastAssembly.GetType("MasturbationRecorder.StatistTotalByDateTime");
                Type[] genericArgsOfDateTimes = { statistTotalByDateTime };

                // 实例化 LinkedList<StatistTotalByDateTime> tempLikDatetimes
                Type likType = typeof(LinkedList<>);
                object tempLikDateTimes = Activator.CreateInstance(likType.MakeGenericType(genericArgsOfDateTimes));

                // 实例化 LinkedListNode<StatistTotalByDateTime> 
                // 并初始化为当前 node 然后打印
                Type likNodeType = typeof(LinkedListNode<>);
                object tempLikNode = Activator.CreateInstance(
                    type: likNodeType.MakeGenericType(genericArgsOfDateTimes),
                    args: node.GetType().InvokeMember("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, node, null)
                );
                Console.WriteLine(tempLikNode.GetType().InvokeMember("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, tempLikNode, null));

                // 把上面的 tempLikNode 添加进临时封装的单元素链表 tempLikDateTimes，
                // 然后作为参数传递给 GroupDateTimesByDiff
                tempLikDateTimes.GetType()
                                .InvokeMember(
                                    name: "AddLast",
                                    invokeAttr: BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
                                    binder: null,
                                    target: tempLikDateTimes,
                                    args: new object[] { tempLikNode }
                                );

                // 将 tempLikDateTimes 转换为单元素 List<(BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList)>
                // 之后调用 List.First<(BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList)>() 
                // 用以提取内部封装的元素（即当前循环的 node 值，类型为元组）
                var listOfTuple = groupDateTimesByDiffInfo.Invoke(null, new object[] { tempLikDateTimes });
                InvokeFirstMethodOfListOfTuple(listOfTuple);
            }
        }

        /// <summary>
        /// 调用 List.First<(BigInteger Ordinal, BigInteger Diff, SortedList<BigInteger, StatistTotalByDateTime> StaticsList)>()
        /// </summary>
        /// <param name="listOfTuple"></param>
        private static void InvokeFirstMethodOfListOfTuple(object listOfTuple) {
            IEnumerable<MethodInfo> firstInfo = typeof(Enumerable)
                                                    .GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static)
                                                    .Where(m => m.IsGenericMethod &&
                                                           m.Name == "First" &&
                                                           m.GetParameters().Length == 1 &&
                                                           m.GetParameters()[0].ParameterType.GetGenericTypeDefinition().IsEquivalentTo(typeof(IEnumerable<>)));
            var listOfTuple_First = firstInfo.First().MakeGenericMethod(new Type[] { listOfTuple.GetType().GetGenericArguments().First() });
            Console.WriteLine($"Current Tuple: {listOfTuple_First.Invoke(null, new object[] { listOfTuple })}");
        }

        /// <summary>
        /// 从 MasturbationRecorder.MainPageViewModel 的 LinesConvertToStatistTotalByDateTimes
        /// 方法中提取测试数据并生成 StatistTotalByDateTimes 实例用于测试。
        /// </summary>
        /// <param name="mastAssembly">主体项目</param>
        /// <param name="mastTestAssembly">当前测试项目</param>
        /// <returns>返回一个LinkedList<StatistTotalByDateTime>实例</returns>
        private static object MakeTestDataObject(Assembly mastAssembly, Assembly mastTestAssembly) {
            var linesConvertToStatistTotalByDateTimesInfo = mastAssembly.GetType("MasturbationRecorder.MainPageViewModel").GetMethod("LinesConvertToStatistTotalByDateTimes", BindingFlags.Public | BindingFlags.Static);
            var testTextLineInfo = mastTestAssembly.GetType("MasturbationRecorderTest.CommonTestResource").GetProperty("TestTextLine");
            var testTextLine = testTextLineInfo.GetValue(null);
            var linkedlist_dateTimes = linesConvertToStatistTotalByDateTimesInfo.Invoke(
                obj: null,
                parameters: new object[] { testTextLine }
            );
            return linkedlist_dateTimes;
        }
    }
}
