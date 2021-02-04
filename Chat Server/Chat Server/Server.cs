using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat_Server
{
	/// <summary>
	/// Deals with receiving / sending responses for clients
	/// </summary>
	class Server
	{
		private static object Lock { get; } = new object();
		private List<Client> Clients { get; } = new List<Client>();

		public void ListenForConnections(int port)
		{
			TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
			tcpListener.Start();

			ConsoleIO.Log($"Listening for new connections...{Environment.NewLine}");
			try
			{
				while (true)
				{
					// Blocks thread until new connection found
					TcpClient newTcpClient = tcpListener.AcceptTcpClient();
					ConsoleIO.Log($"{newTcpClient.Client.RemoteEndPoint} is connecting...");

					Thread newClientThread = new Thread(tcpClient => OnNewConection((TcpClient)tcpClient));
					newClientThread.Start(newTcpClient);
				}
			}
			catch (Exception e)
			{
				ConsoleIO.LogError(e.Message);
			}
		}

		private void OnNewConection(TcpClient tcpClient)
		{
			Client client = LoginClient(tcpClient);
			Clients.Add(client);
			ConsoleIO.Log($"{tcpClient.Client.RemoteEndPoint} conencted as \"{client.DisplayName}.\"");

			ListenForMessages(client);
		}

		private void DisconnectClient(Client client)
		{
			int index = Clients.IndexOf(client);

			if (index != -1)
			{
				ConsoleIO.LogError($"Disconnecting {client.DisplayName}({client.TcpClient.Client.RemoteEndPoint})");

				if (client.TcpClient.Connected)
				{
					client.TcpClient.Close();
				}

				Clients.RemoveAt(index);
			}
		}

		private Client LoginClient(TcpClient tcpClient)
		{
			Client client;
			string displayName = $"Client#{Clients.Count}";

			lock (Lock)
			{
				client = new Client(tcpClient, displayName);
			}

			return client;
		}

		private void ListenForMessages(Client client)
		{
			// TODO: re design to use Async/await instead. It's dangerous to be able to call this fucntion and effectively be stuck for ever.

			NetworkStream stream = client.TcpClient.GetStream();

			while (true)
			{
				byte[] receiveBuffer = new byte[1024];

				try
				{
					int numBytes = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
					SendResponseToAllClients(receiveBuffer, numBytes, client, false);
				}
				catch (IOException)
				{
					ConsoleIO.LogError($"Failed sending response to '{client.DisplayName}'({client.TcpClient.Client.RemoteEndPoint})");
					DisconnectClient(client);
					break;
				}
			}
		}

		private void SendString(Client client, string str)
		{
			byte[] response = Encoding.UTF8.GetBytes(str);
			SendResponse(client, response, response.Length);
		}

		private void SendResponse(Client client, byte[] response, int size)
		{
			try
			{
				client.TcpClient.GetStream().Write(response, 0, size);
			}
            catch (System.InvalidOperationException)
			{
				ConsoleIO.LogError($"Failed sending response to '{client.DisplayName}'({client.TcpClient.Client.RemoteEndPoint})");
				DisconnectClient(client);
			}
		}

		private void SendResponseToAllClients(byte[] response, int size, Client sender = null, bool includeSender = false)
		{
			if (sender == null)
			{
				includeSender = false;
			}

			lock (Lock)
			{
				foreach (Client client in Clients)
				{
					if (includeSender || client != sender)
					{
						SendResponse(client, response, size);
					}
				}
			}
		}
	}
}
