using Chat_Program.Backend.Security;
using Chat_Program.Model;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chat_Program.Backend
{
	public enum ServerRequest : byte
	{
		RedistributeMessage = 0,
	}

	public enum ServerContext : byte
	{
		Message = 0,
		KeyRequest = 1,
	}

	/// <summary>
	/// Handles sending / recieving of messages with a given server
	/// </summary>
	public class ChatClient
	{
		public TcpClient TcpClient { get; }
		public bool IsConnected { get => TcpClient.Connected; }
		public bool IsListening { get; private set; } = false;

		private Thread MessageListeningThread { get; set; }
		private RsaImpl RsaImpl { get; }
		private AesImpl AesImpl { get; }
		private string KeyPairPath { get => @"..\Model\Keys\keyPair"; }
		private int MaxResponseBytes { get; }

		private Action<IMessage> OnReceiveMessage { get; }
		private Action<Exception> OnCouldntConnect { get; }
		private Action<Exception> OnUnexpectedDisconnect { get; }
		private Action<Exception> OnCouldntSendResponse { get; }

		public ChatClient(int maxResponseBytes, Action<Model.IMessage> onReceiveMessage, Action<Exception> onCouldntConnect, Action<Exception> onUnexpectedDisconnect, Action<Exception> onCouldntSendResponse)
		{
			MaxResponseBytes = maxResponseBytes;
			OnReceiveMessage = onReceiveMessage ?? throw new ArgumentNullException(nameof(onReceiveMessage));
			OnCouldntConnect = onCouldntConnect ?? Exceptions.DefaultAction;
			OnUnexpectedDisconnect = onUnexpectedDisconnect ?? Exceptions.DefaultAction;
			OnCouldntSendResponse = onCouldntSendResponse ?? Exceptions.DefaultAction;

			TcpClient = new TcpClient();

			AesImpl = new AesImpl();

			//RsaImpl = new RsaImpl(Model.Settings.Rsa.KeySize);
			//byte[] keyPair = File.ReadAllBytes(KeyPairPath);
			//RsaImpl.SetKeyPair(keyPair);
		}

		#region Connect / Disconnect
		public bool TryConnect(IPAddress ipAddress, int port)
		{
			if (!IsConnected)
			{
				try
				{
					TcpClient.Connect(ipAddress, port);
					OnConnectionSuccessful();
					return true;
				}
				catch (System.Net.Sockets.SocketException e)
				{
					OnCouldntConnect.Invoke(e);
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

		private void OnConnectionSuccessful()
		{
			StartListeningForMessages();
		}

		public void Disconnect()
		{
			if (IsConnected)
			{
				TcpClient.GetStream().Close();
				TcpClient.Close();
			}
		}
		#endregion

		#region Sending / Receiving Data
		private void StartListeningForMessages()
		{
			if (IsListening)
			{
				return;
			}

			IsListening = true;
			MessageListeningThread = new Thread(() =>
			{
				while (IsListening)
				{
					if (ReadAndDecryptResponse(out byte[] buffer, out ServerContext context) > 0)
					{
						ParseServerResponse(buffer, context);
					}

					Thread.Sleep(Model.Settings.Network.ReadMessageRetryDelayMs);
				}
			});
			MessageListeningThread.IsBackground = true;
			MessageListeningThread.Start();
		}

		public bool TrySendString(string str)
		{
			Message message = new Message(str);
			byte[] buffer = SerialiseMessage(message);

			if (buffer.Length > 0)
			{
				return TrySendEncryptedResponse(ServerRequest.RedistributeMessage, buffer);
			}

			return false;
		}

		/// <summary>
		/// Sends a response to the server.
		/// </summary>
		/// <param name="buffer">Byte array response to send</param>
		public bool TrySendEncryptedResponse(ServerRequest request, byte[] buffer)
		{
			if (buffer == null)
			{
				return false;
			}

			if (buffer.Length > MaxResponseBytes)
			{
				Array.Resize(ref buffer, MaxResponseBytes);
			}

			if (IsConnected)
			{
				var encrypted = AesImpl.Encrypt(buffer);
				byte[] encryptedBuf = new byte[sizeof(ServerRequest) + encrypted.TotalBytesLength];
				encryptedBuf[0] = (byte)request;
				encrypted.ToByteArray(encryptedBuf, 1, encryptedBuf.Length);
				
				try
				{
					TcpClient.GetStream().Write(encryptedBuf, 0, encryptedBuf.Length);
				}
				catch (System.IO.IOException e)
				{
					OnCouldntSendResponse.Invoke(e);
				}
				return true;
			}
			else
			{
				OnCouldntSendResponse.Invoke(new NotConnectedException(TcpClient));
			}

			return false;
		}

		public int ReadAndDecryptResponse(out byte[] response, out ServerContext context)
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
				OnUnexpectedDisconnect.Invoke(e);
			}

			Array.Resize(ref response, byteCount);
			context = (ServerContext)response[0];
			response = AesImpl.Decrypt(new EncryptedBuffer(response[1..]));//RsaImpl.DecryptRsa(response);

			return byteCount;
		}
		#endregion

		#region Serialising / Deserialising Messages
		private byte[] SerialiseMessage(IMessage message)
		{
			if (sizeof(ResponseType) + sizeof(int) + message.Content.Length > MaxResponseBytes)
			{
				// Message exceeds max response bytes
				return Array.Empty<byte>();
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

		private void ParseServerResponse(byte[] buffer, ServerContext context)
		{
			switch (context)
			{
				case ServerContext.Message:
					IMessage message = DeserialiseMessage(buffer);
					OnReceiveMessage.Invoke(message);
					break;
				case ServerContext.KeyRequest:
					break;
				default:
					break;
			}
		}
		#endregion
	}
}
