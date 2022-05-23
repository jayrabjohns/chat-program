using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Chat_Server.Data
{
	class ChatRoom
	{
		public int DefaultReceiveBufferSize { get; } = 1024;
	}

	class Settings
	{
		public static ChatRoom ChatRoom { get; } = new ChatRoom();
	}
}
