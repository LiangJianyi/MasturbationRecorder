using System;
using System.Security.Permissions;
using System.Reflection;
using System.Collections.Generic;

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
            var methodInfo = assembly.GetType("MasturbationRecorder.DatetimeParser").GetMethod("StringToUInt16", BindingFlags.NonPublic | BindingFlags.Static);
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "mon" }));
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "feb" }));
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "mar" }));
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "apr" }));
            Console.WriteLine(methodInfo.Invoke(null, new object[] { "may" }));
        }

        private static void MasturbationRecorder_MainPageViewModel_GroupDateTimesDiff_Test(Assembly mastAssembly, Assembly mastTestAssembly) {
            var groupDateTimesByDiffInfo = mastAssembly.GetType("MasturbationRecorder.MainPageViewModel").GetMethod("GroupDateTimesByDiff", BindingFlags.Public | BindingFlags.Static);
            var linesConvertToStatistTotalByDateTimesInfo = mastAssembly.GetType("MasturbationRecorder.MainPageViewModel").GetMethod("LinesConvertToStatistTotalByDateTimes", BindingFlags.Public | BindingFlags.Static);
            var testTextLineInfo = mastTestAssembly.GetType("MasturbationRecorderTest.CommonTestResource").GetProperty("TestTextLine");
            var testTextLine = testTextLineInfo.GetValue(null);
            var dateTimes = linesConvertToStatistTotalByDateTimesInfo.Invoke(
                obj: null,
                invokeAttr: BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
                binder: null,
                culture: null,
                parameters: new object[] { testTextLine }
            );
            for (var node = dateTimes.GetType().InvokeMember("First", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, dateTimes, null);
                node != null;
                node = node.GetType().InvokeMember("Next", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, node, null)
                ) {
                Console.WriteLine(node.GetType().InvokeMember("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, node, null));

                Type statistTotalByDateTime = mastAssembly.GetType("MasturbationRecorder.StatistTotalByDateTime");
                Type[] genericArgs = { statistTotalByDateTime };
                Type likType = typeof(LinkedList<>);
                object statistTotalByDateTimeOfLik = Activator.CreateInstance(likType.MakeGenericType(genericArgs));

                Type likNodeType = typeof(LinkedListNode<>);
                object statistTotalByDateTimeOfLikNode = Activator.CreateInstance(
                    type: likNodeType.MakeGenericType(genericArgs),
                    args: node.GetType().InvokeMember("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, node, null)
                );

                var statistTotalByDateTimeOfLikInfo = statistTotalByDateTimeOfLik.GetType();
                statistTotalByDateTimeOfLikInfo.InvokeMember("AddLast", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, statistTotalByDateTimeOfLik, new object[] { statistTotalByDateTimeOfLikNode });
                var statistTotalByDateTimeOfLik_Count = statistTotalByDateTimeOfLikInfo.InvokeMember("Count", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance, null, statistTotalByDateTimeOfLik, null);
                var statistTotalByDateTimeOfLik_First = statistTotalByDateTimeOfLikInfo.InvokeMember("First", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance, null, statistTotalByDateTimeOfLik, null);
                var statistToTalByDateTimeOfLik_First_Value = statistTotalByDateTimeOfLik_First.GetType().InvokeMember("Value", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance, null, statistTotalByDateTimeOfLik_First, null);
                Console.WriteLine($"statistTotalByDateTimeOfLik Length: {statistTotalByDateTimeOfLik_Count}, First: {statistToTalByDateTimeOfLik_First_Value}");
            }
            Console.WriteLine();
            Console.WriteLine();

            //var res = groupDateTimesByDiffInfo.Invoke(
            //    obj: null,
            //    invokeAttr: BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
            //    binder: null,
            //    culture: null,
            //    parameters: new object[] { dateTimes }
            //);
            //Console.WriteLine(res);
        }

        void fuck() {
            //Type statistTotalByDateTime = mastAssembly.GetType("MasturbationRecorder.StatistTotalByDateTime");
            //Type[] genericArgs = { statistTotalByDateTime };
            //Type likType = typeof(LinkedList<>);
            //object o = Activator.CreateInstance(likType.MakeGenericType(genericArgs));
            //o = new LinkedList<int>();
            //o.GetType().InvokeMember(
            //    name: "AddLast",
            //    invokeAttr: BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
            //    target: o,
            //    binder: null,
            //    args: new object[] { 11223344 }
            //);
            //o = o.GetType().InvokeMember(
            //    name: "First",
            //    invokeAttr: BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
            //    target: o,
            //    binder: null,
            //    args: null
            //);
            //Console.WriteLine(o.GetType().InvokeMember(
            //    name: "Value",
            //    invokeAttr: BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
            //    target: o,
            //    binder: null,
            //    args: null
            //));
        }
    }
}
