using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat_Server
{
	/// <summary>
	/// Deals with receiving / sending responses for clients
	/// </summary>
	class Server
	{
		public void ListenForConnections(int port)
		{
			TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
			tcpListener.Start();

			Console.WriteLine($"Listening for new connections...{Environment.NewLine}");
			try
			{
				while (true)
				{
					// Blocks thread until new connection found
					TcpClient newConnection = tcpListener.AcceptTcpClient();
					Console.WriteLine("Someone is connecting...");

					
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}
