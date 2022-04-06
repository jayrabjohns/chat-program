using System;

namespace Chat_Server
{
	public class ConsoleIO
	{
		public static void Log(string message)
		{
			Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}]:  {message}");
		}

		public static void LogError(string message)
		{
			ConsoleColor prevColour = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Log(message);
			Console.ForegroundColor = prevColour;
		}
	}
}
