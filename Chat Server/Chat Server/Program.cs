using System;
using System.Threading;

namespace Chat_Server
{
	class Program
	{
		static int Main(string[] args)
		{
			int port = 14000;

			// Preprocessing args
			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "--port":
					case "-p":
						if (i + 1 < args.Length && int.TryParse(args[i + 1], out int val))
						{
							port = val;
						}
						else
						{
							Console.WriteLine($"Provide a valid int after '{args[i]}'");
							return 1;
						}
						break;
					default:
						ConsoleIO.LogError($"Unrecognised parameter '{args[i]}'.");
						return 1;
				}
			}

			ChatRoom chatRoom = new ChatRoom();
			chatRoom.ListenForConnections(port);
			return 0;
		}
	}
}
