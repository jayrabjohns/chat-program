using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Chat_Server
{
	class Client
	{
		public TcpClient TcpClient { get; private set; }
		public string DisplayName { get; private set; }

		public Client(TcpClient tcpClient, string displayName)
		{
			TcpClient = tcpClient;
			DisplayName = displayName;
		}
	}
}
