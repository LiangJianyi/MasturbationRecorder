using System;
using System.Security.Permissions;
using System.Reflection;

namespace ConsoleApp1 {
	class Program {
		static void Main(string[] args) {
			const string path = @"C:\Users\a124p\Documents\GitHub\MasturbationRecorder\MasturbationRecorder\bin\x64\Debug\MasturbationRecorder.exe";
			FileIOPermission permission = new FileIOPermission(
				FileIOPermissionAccess.Read,
				@"C:\Users\a124p\Documents\GitHub\MasturbationRecorder");
			permission.Demand();
			Assembly assembly = Assembly.LoadFrom(path);
			var methodInfo = assembly.GetType("MasturbationRecorder.DatetimeParser").GetMethod("StringToUInt16", BindingFlags.NonPublic | BindingFlags.Static);
			Console.WriteLine(methodInfo.Invoke(null, new object[] { "mon" }));
			Console.WriteLine(methodInfo.Invoke(null, new object[] { "feb" }));
			Console.WriteLine(methodInfo.Invoke(null, new object[] { "mar" }));
			Console.WriteLine(methodInfo.Invoke(null, new object[] { "apr" }));
			Console.WriteLine(methodInfo.Invoke(null, new object[] { "may" }));
			Console.ReadKey();
		}
	}
}
