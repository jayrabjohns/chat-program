using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat_Server
{
	public enum ChatRoomState
	{
		UnInitialised = 0,
		Initialising = 1,
		Running = 2,
		Stopping = 3,
		Stopped = 4,
	}

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
	/// Deals with receiving / sending responses for clients
	/// </summary>
	class ChatRoom
	{
		private List<Client> Clients { get; } = new List<Client>();
		private ulong TotalClientsConnected { get; set; }
		private object CurrentStateLock { get; } = new object();

		private ChatRoomState _currentChatRoomState = ChatRoomState.UnInitialised;
		private ChatRoomState CurrentState 
		{
			get 
			{ 
				lock (CurrentStateLock)
				{
					return _currentChatRoomState;
				}
			}
			set 
			{
				lock (CurrentStateLock)
				{
					_currentChatRoomState = value;
				}
			}
		}

		public ChatRoom()
		{

		}

		#region Start / Stop
		public void Start(int port)
		{
			CurrentState = ChatRoomState.Initialising;

			Thread connectionListeningThread = new Thread(() => ListenForConnections(port));
			connectionListeningThread.Start();

			CurrentState = ChatRoomState.Running;

			bool running = true;
			while (running)
			{
				if (CurrentState == ChatRoomState.Stopping)
				{
					running = false;
				}
				else
				{
					Thread.Sleep(Data.Settings.ChatRoom.HeartBeatFrequencyMs);
					CheckClientConnections();
				}
			}

			CurrentState = ChatRoomState.Stopped;
		}

		public void RequestStop()
		{
			CurrentState = ChatRoomState.Stopping;
		}
		#endregion


		#region Connections
		public void ListenForConnections(int port)
		{
			TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
			try
			{
				tcpListener.Start();
				ConsoleIO.Log($"Listening for new connections...{Environment.NewLine}");

				while (true)
				{
					if (TotalClientsConnected >= ulong.MaxValue)
					{
						ConsoleIO.Log("No longer accepting new connections, max number has been reach.");
						//TODO: Unsure what to do here?
						continue;
					}

					// Blocks thread until new connection found
					TcpClient newTcpClient = tcpListener.AcceptTcpClient();
					ConsoleIO.Log($"{newTcpClient.Client.RemoteEndPoint} is connecting...");

					Thread newClientThread = new Thread(tcpClient => OnNewConection((TcpClient)tcpClient));
					newClientThread.Start(newTcpClient);

					Thread.Sleep(Data.Settings.ChatRoom.NewConnectionsDelayMs);
				}
			}
			catch (SocketException e)
			{
				ConsoleIO.LogError(e.Message);
			}
			catch (Exception e)
			{
				ConsoleIO.LogError(e.Message);
			}
		}

		private void OnNewConection(TcpClient tcpClient)
		{
			Client client = LoginClient(tcpClient);

			lock (Clients)
			{
				Clients.Add(client);
			}

			ConsoleIO.Log($"{tcpClient.Client.RemoteEndPoint} connected as \"{client.DisplayName}.\"");

			ListenForMessages(client);
		}

		private void DisconnectClient(Client client)
		{
			lock (Clients)
			{
				int index = Clients.IndexOf(client);
				if (index != -1)
				{
					ConsoleIO.LogError($"Disconnecting {client.DisplayName}({client.TcpClient.Client.RemoteEndPoint})");

					Clients.RemoveAt(index);

					if (client.TcpClient.Connected)
					{
						client.TcpClient.Close();
					}
				}
			}
		}

		private void CheckClientConnections()
		{
			lock(Clients)
			{
				for (int i = Clients.Count - 1; i >= 0; i--)
				{
					if (!Clients[i].TcpClient.Connected)
					{
						DisconnectClient(Clients[i]);
					}
				}
			}
		}
		#endregion

		private Client LoginClient(TcpClient tcpClient)
		{
			Client client;
			string displayName = $"Client#{TotalClientsConnected++}";
			client = new Client(tcpClient, displayName, new System.Security.Cryptography.RSAParameters()); // TODO fix

			return client;
		}

		private void ListenForMessages(Client client)
		{
			// TODO: re-design to use TCS instead.

			NetworkStream stream = client.TcpClient.GetStream();
			byte[] receiveBuffer = new byte[Data.Settings.ChatRoom.DefaultReceiveBufferSize];

			while (true)
			{
				try
				{
					int numBytes = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
					if (numBytes > 0)
					{
						OnMessageReceived(receiveBuffer, numBytes, client);
						Array.Clear(receiveBuffer, 0, numBytes);
					}
				}
				catch (IOException)
				{
					ConsoleIO.LogError($"Failed sending response to '{client.DisplayName}'({client.TcpClient.Client.RemoteEndPoint})");
					DisconnectClient(client);
					break;
				}
			}
		}

		private void OnMessageReceived(byte[] buffer, int numBytes, Client sender)
		{
			ServerRequest request = (ServerRequest)buffer[0];
			const int sizeofRequest = 1;

			switch (request)
			{

				//case ServerRequest.GetChatroomMembers:
				//	byte[] clientsPublicInfo = SerialisePublicClientInfo((byte)ServerContext.ClientInfos);
				//	SendResponse(sender, clientsPublicInfo, 0, clientsPublicInfo.Length);
				//	break;
				case ServerRequest.RedistributeMessage:
				default:
					buffer[0] = (byte)ServerContext.Message;
					SendResponseToAllClients(buffer, 0, numBytes, sender, includeSender: true);
					break;
			}
		}

		private void SendString(Client client, string str)
		{
			byte[] response = Encoding.UTF8.GetBytes(str);
			SendResponse(client, response, 0, response.Length);
		}

		private void SendResponse(Client client, byte[] response, int offset, int size)
		{
			try
			{
				client.TcpClient.GetStream().Write(response, offset, size);
			}
            catch (System.InvalidOperationException)
			{
				ConsoleIO.LogError($"Failed sending response to '{client.DisplayName}'({client.TcpClient.Client.RemoteEndPoint})");
				DisconnectClient(client);
			}
		}

		private void SendResponseToAllClients(byte[] response, int offset, int size, Client sender = null, bool includeSender = false)
		{
			if (sender == null)
			{
				includeSender = false;
			}

			lock (Clients)
			{
				for (int i = Clients.Count - 1; i >= 0; i--)
				{
					if (includeSender || Clients[i] != sender)
					{
						SendResponse(Clients[i], response, offset, size);
					}
				}
			}
		}

		private byte[] SerialisePublicClientInfo(byte header)
		{
			byte[] jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(GetPublicClientInfo()));

			if (1 + sizeof(int) + jsonBytes.Length > Data.Settings.ChatRoom.DefaultMaxSendBufferSize)
			{
				return Array.Empty<byte>();
			}

			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter bWriter = new BinaryWriter(memoryStream))
				{
					bWriter.Write(header);
					bWriter.Write(jsonBytes.Length);
					bWriter.Write(jsonBytes);
				}

				return memoryStream.ToArray();
			}
		}

		private PublicClientInfo[] GetPublicClientInfo()
		{
			// TODO: could cache and check for differences

			PublicClientInfo[] publicClientInfos = new PublicClientInfo[Clients.Count];
			for (int i = 0; i < Clients.Count; i++)
			{
				publicClientInfos[i] = Clients[i].Info;
			}

			return publicClientInfos;
		}
	}
}
