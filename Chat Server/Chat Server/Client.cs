using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Chat_Server
{
	class PublicClientInfo
	{
		public string DisplayName { get; }
		public RSAParameters PubKey { get; }

		public PublicClientInfo(string displayName, RSAParameters pubKey)
		{
			DisplayName = displayName;
			PubKey = pubKey;
		}
	}

	class Client
	{
		public TcpClient TcpClient { get; }
		public PublicClientInfo Info { get; }
		public string DisplayName { get => Info.DisplayName; }
		public RSAParameters PubKey { get => Info.PubKey; }

		public Client(TcpClient tcpClient, string displayName, RSAParameters pubKey)
		{
			TcpClient = tcpClient;
			Info = new PublicClientInfo(displayName, pubKey);
		}

		public override string ToString()
		{
			return DisplayName;
		}
	}
}
