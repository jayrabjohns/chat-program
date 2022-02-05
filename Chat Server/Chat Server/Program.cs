using System;
using System.Threading;

namespace Chat_Server
{
	class Program
	{
		static void Main(string[] args)
		{
			// e2ee with group chats
			// https://jameshfisher.com/2017/10/25/end-to-end-encryption-with-server-side-fanout/

			// probably use openSSL

			int port = 14000;

			// Preprocessing args
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-p" || args[i] == "--port" && i + 1 < args.Length)
				{
					if (int.TryParse(args[i + 1], out int val))
					{
						port = val;
					}
				}
			}

			ChatRoom chatRoom = new ChatRoom();
			chatRoom.ListenForConnections(port);
		}
	}
}
