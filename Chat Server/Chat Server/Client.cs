using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Chat_Server
{
	class Client
	{
		public TcpClient TcpClient { get; }
		public string DisplayName { get; }

		public Client(TcpClient tcpClient, string displayName)
		{
			TcpClient = tcpClient;
			DisplayName = displayName;
		}

		public override string ToString()
		{
			return DisplayName;
		}
	}
}
