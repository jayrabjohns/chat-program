using System;
using System.Threading;

namespace Chat_Server
{
	class Program
	{
		#region Default Arg Values
		static int Port = 14000;
		#endregion

		static int Main(string[] args)
		{
			if (!TryProcessArgs(args))
			{
				return -1;
			}

			ChatRoom chatRoom = new ChatRoom();
			chatRoom.ListenForConnections(Port);

			return 0;
		}

		#region Argument Processing
		static void PrintUsage()
		{
			Console.WriteLine("Program.cs <args>");
			Console.WriteLine("Args:");
			Console.WriteLine("-h    --help    Prints this help message");
			Console.WriteLine("-p    --port    Sets port to listen for connections");
		}

		static bool TryProcessArgs(string[] args)
		{
			bool? ret = null;
			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-p":
					case "--port":
						if (i + 1 < args.Length && int.TryParse(args[i + 1], out int port))
						{
							Port = port;
						}
						else
						{
							Console.WriteLine($"Provide a valid int after '{args[i]}'");
							ret = false;
						}
						break;

					case "-h":
					case "--help":
						PrintUsage();
						ret = true;
						break;

					default:
						ConsoleIO.LogError($"Unrecognised parameter '{args[i]}'.");
						PrintUsage();
						ret = false;
						break;
				}

				if (ret.HasValue)
				{
					return ret.GetValueOrDefault();
				}
			}

			return true;
		}
		#endregion
	}
}
