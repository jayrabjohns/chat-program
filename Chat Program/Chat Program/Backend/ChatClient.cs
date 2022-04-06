using Chat_Program.Model;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chat_Program.Backend
{
	/// <summary>
	/// Handles sending / recieving of messages with a given server
	/// </summary>
	public class ChatClient
	{
		public TcpClient TcpClient { get; }
		public bool Connected { get => TcpClient.Connected; }
		public bool Listening { get; private set; } = false;

		private Thread MessageListeningThread { get; set; }
		private RsaImpl RsaImpl { get; }
		private string KeyPairPath { get => @"..\Model\Keys\keyPair"; }
		private int MaxResponseBytes { get; }
		private Action<IMessage> OnReceiveMessage { get; }
		private Action OnCouldntConnect { get; }
		private Action OnUnexpectedDisconnect { get; }
		private Action OnCouldntSendResponse { get; }

		public ChatClient(int maxResponseBytes, Action<Model.IMessage> onReceiveMessage = null, Action onCouldntConnect = null, Action onUnexpectedDisconnect = null, Action onCouldntSendResponse = null)
		{
			MaxResponseBytes = maxResponseBytes;
			OnReceiveMessage = onReceiveMessage;
			OnCouldntConnect = onCouldntConnect;
			OnUnexpectedDisconnect = onUnexpectedDisconnect;
			OnCouldntSendResponse = onCouldntSendResponse;

			TcpClient = new TcpClient();
			RsaImpl = new RsaImpl(Model.Settings.Rsa.KeySize);
			//byte[] keyPair = File.ReadAllBytes(KeyPairPath);
			//RsaImpl.SetKeyPair(keyPair);
		}

		#region Connect / Disconnect
		public bool TryConnect(IPAddress ipAddress, int port)
		{
			if (!Connected)
			{
				try
				{
					TcpClient.Connect(ipAddress, port);
					return true;
				}
				catch (System.Net.Sockets.SocketException e)
				{
					OnCouldntConnect?.Invoke();
				}
			}

			return false;
		}

		public bool TryConnect(string ipString, int port)
		{
			if (string.Equals(ipString, "localhost", StringComparison.InvariantCultureIgnoreCase))
			{
				ipString = "127.0.0.1";
			}

			if (IPAddress.TryParse(ipString, out IPAddress address))
			{
				return TryConnect(address, port);
			}

			return false;
		}

		public void Disconnect()
		{
			if (Connected)
			{
				TcpClient.GetStream().Close();
				TcpClient.Close();
			}
		}
		#endregion

		#region Sending / Receiving Data
		public bool TrySendString(string str)
		{
			Message message = new Message(str);
			byte[] buffer = SerialiseMessage(message);

			if (buffer.Length > 0)
			{
				return TrySendResponse(buffer);
			}

			return false;
		}

		/// <summary>
		/// Sends a response to the server.
		/// </summary>
		/// <param name="buffer">Byte array response to send</param>
		public bool TrySendResponse(byte[] buffer)
		{
			if (buffer == null)
			{
				return false;
			}

			if (buffer.Length > MaxResponseBytes)
			{
				Array.Resize(ref buffer, MaxResponseBytes);
			}

			if (Connected)
			{
				byte[] encryptedBuf = RsaImpl.EncryptRsa(buffer);
				TcpClient.GetStream().Write(encryptedBuf, 0, encryptedBuf.Length);
				return true;
			}
			else
			{
				OnCouldntSendResponse?.Invoke();
			}

			return false;
		}

		public int ReadAndDecryptResponse(out byte[] response)
		{
			response = new byte[MaxResponseBytes];
			int byteCount = 0;

			try
			{
				byteCount = TcpClient.GetStream().Read(response, 0, response.Length);
			}
			catch (Exception e) when
			(e is System.IO.IOException
			|| e is System.InvalidOperationException)
			{
				OnUnexpectedDisconnect?.Invoke();
			}

			Array.Resize(ref response, byteCount);
			response = RsaImpl.DecryptRsa(response);

			return byteCount;
		}

		public void StartListeningForMessages()
		{
			if (Listening)
			{
				return;
			}

			Listening = true;
			MessageListeningThread = new Thread(() =>
			{
				while (Listening)
				{
					if (ReadAndDecryptResponse(out byte[] buffer) > 0)
					{
						IMessage message = DeserialiseMessage(buffer);
						OnReceiveMessage?.Invoke(message);
					}
					Thread.Sleep(100);
				}
			});
			MessageListeningThread.IsBackground = true;
			MessageListeningThread.Start();
		}

		public void StopListeningForMessages()
		{
			Listening = false;
		}
		#endregion

		#region Serialising / Deserialising Messages
		private byte[] SerialiseMessage(IMessage message)
		{
			if (sizeof(byte) + sizeof(int) + sizeof(char) * message.Content.Length > MaxResponseBytes)
			{
				// Message exceeds max response bytes
				return new byte[0];
			}

			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter bWriter = new BinaryWriter(memoryStream))
				{
					bWriter.Write((byte)message.ResponseType);
					bWriter.Write(message.Content.Length);
					bWriter.Write(message.Content);
				}

				return memoryStream.ToArray();
			}
		}

		public Model.Message DeserialiseMessage(byte[] buffer)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (BinaryReader bReader = new BinaryReader(memoryStream))
				{
					try
					{
						ResponseType responseType = (ResponseType)bReader.ReadByte();
						int contentSize = bReader.ReadInt32();
						byte[] content = bReader.ReadBytes(contentSize);

						return new Message(content, responseType);
					}
					catch (System.IO.EndOfStreamException)
					{
						return new Message("Message too large.");
					}
				}
			}
		}
		#endregion
	}
}
