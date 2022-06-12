using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.utils
{
	public static class ConsoleLog
	{
		private static readonly object ConsoleWriterLock = new object();
		public static void Info(object type, object message)
		{
			lock (ConsoleWriterLock)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(string.Format("[{0}][{1}]{2}", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"), type, message));
			}
		}
		public static void Processing(object type, object message)
		{
			lock (ConsoleWriterLock)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("[{0}][", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")));
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("WARNINIG");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("][{0}]", type));
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(string.Format("{0}", message));
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		public static void Warning(object type, object message)
		{
			lock (ConsoleWriterLock)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("[{0}][", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")));
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("WARNINIG");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("][{0}]", type));
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(string.Format("{0}", message));
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		public static void Error(object type, object message)
		{
			lock (ConsoleWriterLock)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("[{0}][", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")));
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("ERROR");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("][{0}]", type));
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(message);
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		public static void Fatal(object type, object message)
		{
			lock (ConsoleWriterLock)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("[{0}][", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")));
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.Write("FATAL");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("][{0}]", type));
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(message);
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		public static void Debug(object type, object message)
		{
			lock (ConsoleWriterLock)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("[{0}][", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")));
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write("DEBUG");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(string.Format("][{0}]", type));
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine(message);
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		//public static void UnhandledExceptionLog(UnhandledExceptionEventArgs args)
		//{
		//	StringBuilder stringBuilder = new StringBuilder();
		//	stringBuilder.Append("检测到未处理的异常");
		//	if (args.IsTerminating)
		//	{
		//		stringBuilder.Append("进程将停止运行");
		//	}
		//	stringBuilder.Append("，错误信息:");
		//	stringBuilder.Append(Log.ErrorLogBuilder(args.ExceptionObject() as Exception));
		//	this.Fatal("System", stringBuilder);
		//	this.Warning("System", "将在5s后自动退出");
		//	Thread.Sleep(5000);
		//	Environment.Exit(-1);
		//}
	}
}
