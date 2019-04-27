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
            const string MasturbationRecorder_path = @"C:\Users\a124p\Documents\GitHub\MasturbationRecorder";

            FileIOPermission permission = new FileIOPermission(
                FileIOPermissionAccess.Read,
                MasturbationRecorder_path
            );
            permission.Demand();
            Assembly assembly = Assembly.LoadFrom(MasturbationRecorder_exe_path);
            MasturbationRecorder_DatetimeParser_StringToUInt16_Test(assembly);
            MasturbationRecorder_MainPageViewModel_GroupDateTimesDiff(assembly);

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

        private static void MasturbationRecorder_MainPageViewModel_GroupDateTimesDiff(Assembly assembly) {
            var methodInfo = assembly.GetType("MasturbationRecorder.MainPageViewModel").GetMethod("GroupDateTimesDiff", BindingFlags.Public | BindingFlags.Static);
            Type statistTotalByDateTime = assembly.GetType("MasturbationRecorder.StatistTotalByDateTime");
            Type[] genericArgs = { statistTotalByDateTime };
            Type likType = typeof(LinkedList<>);
            object o = Activator.CreateInstance(likType.MakeGenericType(genericArgs));
            Console.WriteLine(o.GetType().InvokeMember(
                name: "First",
                invokeAttr: BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
                target: o,
                binder: null,
                args: null
            ));
            //LinkedList<StatistTotalByDateTime> dateTimes = null;
        }
    }
}
