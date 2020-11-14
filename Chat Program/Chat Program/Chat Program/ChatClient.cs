using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Program
{
	/// <summary>
	/// Handles sending / recieving of messages with a given server
	/// </summary>
	class ChatClient
	{
		public bool Connected { get => TcpClient.Connected; }
		public bool Listening { get; private set; }

		private TcpClient TcpClient { get; } = new TcpClient();
		private int MaxResponseBytes { get; }

		private Action OnCouldntConnect { get; }
		private Action OnUnexpectedDisconnect { get; }

		public ChatClient(int maxResponseBytes = 1024, Action onCouldntConnect = null, Action onUnexpectedDisconnect = null)
		{
			MaxResponseBytes = maxResponseBytes;
			OnCouldntConnect = onCouldntConnect;
			OnUnexpectedDisconnect = onUnexpectedDisconnect;
		}

		public void Connect(IPAddress ipAddress, int port)
		{
			if (!Connected)
			{
				try
				{
					TcpClient.Connect(ipAddress, port);
				}
				catch (System.Net.Sockets.SocketException)
				{
					OnCouldntConnect?.Invoke();
				}
			}
		}

		public void Disconnect()
		{
			if (Connected)
			{
				TcpClient.GetStream().Close();
				TcpClient.Close();
			}
		}

		public void SendMessage(string message)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(message);
			SendResponse(buffer);
		}

		/// <summary>
		/// Sends a message to the server.
		/// </summary>
		/// <param name="buffer">Message to send, encoded in UTF8.</param>
		public void SendResponse(byte[] buffer)
		{
			if (buffer.Length > MaxResponseBytes)
			{
				Array.Resize(ref buffer, MaxResponseBytes);
			}

			if (Connected)
			{	
				TcpClient.GetStream().Write(buffer, 0, buffer.Length);
			}
		}

		public int ReadResponse(out byte[] response)
		{
			response = new byte[MaxResponseBytes];
			int byteCount = 0;

			try
			{
				byteCount = TcpClient.GetStream().Read(response, 0, response.Length);
			}
			catch (System.IO.IOException)
			{
				OnUnexpectedDisconnect?.Invoke();
			}

			return byteCount;
		} 

		/*public async Task<string> WaitForMessage()
		{
			await TcpClient.GetStream().
		}*/

		/*public void StartListeningForMessages(AsyncCallback onMessageReceived)
		{
			if (TcpClient.GetStream().CanRead && Connected && !Listening)
			{
				byte[] buffer = new byte[MaxMessageBytes];

				try
				{
					TcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, onMessageReceived, TcpClient.GetStream());
				}
				catch (System.IO.IOException)
				{
					// Server shut unexpectedly
				}
			}
		}

		public void StopListeningForMessages()
		{
			if (Listening)
			{

			}
		}*/
	}
}
