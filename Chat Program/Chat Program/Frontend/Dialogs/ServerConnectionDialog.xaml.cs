using Chat_Program.Backend;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace Chat_Program.Frontend.Dialogs
{
	/// <summary>
	/// Interaction logic for ServerConnectionDialog.xaml
	/// </summary>
	public partial class ServerConnectionDialog : Window, INotifyPropertyChanged
	{
		private string _portString = string.Empty;
		public string PortString
		{
			get => _portString;
			set
			{
				if (_portString != value)
				{
					_portString = value;
					OnPropertyChanged(nameof(PortString));
				}
			}
		}

		private int ConnectionAttempts { get; }
		private int ConnectionRetryDelay { get; }

		private Action<Model.IMessage> OnReceiveMessage { get; }

		public ServerConnectionDialog(Action<Model.IMessage> onReceiveMessage, int connectionAttempts, int connectionRetryDelay)
		{
			OnReceiveMessage = onReceiveMessage;
			ConnectionAttempts = connectionAttempts;
			ConnectionRetryDelay = connectionRetryDelay;

			InitializeComponent();
			this.DataContext = this;
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		private async Task TryConnectAsync(string address)
		{
			ChatClient chatClient = new ChatClient(Model.Settings.Network.ResponseSizeBytes, OnReceiveMessage);
			bool connected = false;

			for (int i = 0; i < ConnectionAttempts; i++)
			{
				if (int.TryParse(PortString, out int port) && chatClient.TryConnect(address, port))
				{
					connected = true;
					break;
				}

				await Task.Delay(ConnectionRetryDelay);
			}

			if (connected)
			{
				Model.Globals.Conversations.Add(new Model.Conversation(chatClient));
				chatClient.StartListeningForMessages();
				this.Close();
			}
		}

		private Task SetPort(string portString)
		{
			PortString = portString;
			return Task.CompletedTask;
		}
	}
}
