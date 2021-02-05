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

		private Action OnReceiveMessage { get; }
		private Action OnCouldntConnect { get; }
		private Action OnUnexpectedDisconnect { get; }
		private Action OnCouldntSendResponse { get; }

		public ChatClient(Action onReceiveMessage = null, int maxResponseBytes = 1024, Action onCouldntConnect = null, Action onUnexpectedDisconnect = null, Action onCouldntSendResponse = null)
		{
			MaxResponseBytes = maxResponseBytes;

			OnReceiveMessage = onReceiveMessage;
			OnCouldntConnect = onCouldntConnect;
			OnUnexpectedDisconnect = onUnexpectedDisconnect;
			OnCouldntSendResponse = onCouldntSendResponse;
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

		public void SendString(string str)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(str);
			SendResponse(buffer);
		}

		/// <summary>
		/// Sends a response to the server.
		/// </summary>
		/// <param name="buffer">Byte array response to send</param>
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
			else
			{
				OnCouldntSendResponse?.Invoke();
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
		#region Serialising / Deserialising Messages
		private byte[] SerialiseMessage(Message message)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter bWriter = new BinaryWriter(memoryStream))
				{
					bWriter.Write((int)message.ResponseType);
					bWriter.Write(message.StringMessage);
					bWriter.Write(message.Image.Length);
					bWriter.Write(message.Image);
					bWriter.Write(message.Audio.Length);
					bWriter.Write(message.Audio);
				}

				return memoryStream.ToArray();
			}
		}

		public Message DeserialiseMessage(byte[] buffer)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (BinaryReader bReader = new BinaryReader(memoryStream))
				{
					ResponseType responseType = (ResponseType)bReader.ReadInt32();
					string stringMessage = bReader.ReadString();
					int imageLen = bReader.ReadInt32();
					byte[] image = bReader.ReadBytes(imageLen);
					int audioLen = bReader.ReadInt32();
					byte[] audio = bReader.ReadBytes(audioLen);

					return new Message(responseType, stringMessage, image, audio);
				}
			}
		}
		#endregion
	}
}
