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
		private Model.ChatClient _data { get; }

		public ChatClient(int maxResponseBytes = 1024, Action<Model.IMessage> onReceiveMessage = null, Action onCouldntConnect = null, Action onUnexpectedDisconnect = null, Action onCouldntSendResponse = null)
		{
			_data = new Model.ChatClient(maxResponseBytes, onReceiveMessage, onCouldntConnect, onUnexpectedDisconnect, onCouldntSendResponse);
			TcpClient = new TcpClient();
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
					_data.OnCouldntConnect?.Invoke();
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
			Model.Message message = new Model.Message(str);
			byte[] buffer = SerialiseMessage(message);

			if (buffer.Length > 0)
			{
				SendResponse(buffer);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Sends a response to the server.
		/// </summary>
		/// <param name="buffer">Byte array response to send</param>
		public void SendResponse(byte[] buffer)
		{
			if (buffer == null)
			{
				return;
			}

			if (buffer.Length > _data.MaxResponseBytes)
			{
				Array.Resize(ref buffer, _data.MaxResponseBytes);
			}

			if (Connected)
			{
				TcpClient.GetStream().Write(buffer, 0, buffer.Length);
			}
			else
			{
				_data.OnCouldntSendResponse?.Invoke();
			}
		}

		public int ReadResponse(out byte[] response)
		{
			response = new byte[_data.MaxResponseBytes];
			int byteCount = 0;

			try
			{
				byteCount = TcpClient.GetStream().Read(response, 0, response.Length);
			}
			catch (Exception e) when
			(e is System.IO.IOException
			|| e is System.InvalidOperationException)
			{
				_data.OnUnexpectedDisconnect?.Invoke();
			}

			return byteCount;
		}

		public void StartListeningForMessages()
		{
			if (Listening)
			{
				// Already listening
				return;
			}

			Listening = true;
			MessageListeningThread = new Thread(() =>
			{
				while (Listening)
				{
					if (ReadResponse(out byte[] buffer) > 0)
					{
						Model.IMessage message = DeserialiseMessage(buffer);
						App.Current.Dispatcher.Invoke(() => _data.OnReceiveMessage?.Invoke(message)); // Needs to be called from the UI thread
					}
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
		private byte[] SerialiseMessage(Model.IMessage message)
		{
			if (sizeof(byte) + sizeof(int) + sizeof(char) * message.Content.Length > _data.MaxResponseBytes)
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
						Model.ResponseType responseType = (Model.ResponseType)bReader.ReadByte();
						int contentSize = bReader.ReadInt32();
						byte[] content = bReader.ReadBytes(contentSize);

						return new Model.Message(content, responseType);
					}
					catch (System.IO.EndOfStreamException)
					{
						return new Model.Message("Message too large.");
					}
				}
			}
		}
		#endregion
	}
}
