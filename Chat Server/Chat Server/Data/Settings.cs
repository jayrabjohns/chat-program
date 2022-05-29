using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Chat_Server.Data
{
	class ChatRoom
	{
		public int NewConnectionsDelayMs { get; } = 200;
		public int HeartBeatFrequencyMs { get; } = 5000; // TODO: Change this to vary based on chatroom usage
		public int DefaultReceiveBufferSize { get; } = 2048;
		public int DefaultMaxSendBufferSize { get; } = 2048;
	}

	class Settings
	{
		public static ChatRoom ChatRoom { get; } = new ChatRoom();
	}
}
