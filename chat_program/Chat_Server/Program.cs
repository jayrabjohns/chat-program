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

			Server server = new Server();
			Thread newConnectionsThread = new Thread(port => server.ListenForConnections((int)port));
			newConnectionsThread.Start(5000);
		}
	}
}
