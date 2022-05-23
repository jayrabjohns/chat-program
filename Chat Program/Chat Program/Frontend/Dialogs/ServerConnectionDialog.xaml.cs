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
		private Backend.ChatClient ChatClient { get; }
		private Action<Exception> OnConnectionFailed { get; }
		private Action<ChatClient> OnConnectionSuccess { get; }

		public ServerConnectionDialog(Backend.ChatClient chatClient, int connectionAttempts, int connectionRetryDelay, Action<Exception> onConnectionFailed, Action<ChatClient> onConnectionSuccess)
		{
			ChatClient = chatClient ?? throw new ArgumentNullException(nameof(ChatClient));
			ConnectionAttempts = connectionAttempts;
			ConnectionRetryDelay = connectionRetryDelay;
			OnConnectionFailed = onConnectionFailed ?? Exceptions.DefaultAction;
			OnConnectionSuccess = onConnectionSuccess ?? throw new ArgumentNullException(nameof(onConnectionSuccess));

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
			bool success = false;

			for (int i = 0; i < ConnectionAttempts; i++)
			{
				if (int.TryParse(PortString, out int port) && ChatClient.TryConnect(address, port))
				{
					success = true;
					break;
				}

				await Task.Delay(ConnectionRetryDelay);
			}

			if (!success)
			{
				OnConnectionFailed.Invoke(new System.Net.Sockets.SocketException((int)System.Net.Sockets.SocketError.TimedOut));
				return;
			}

			OnConnectionSuccess.Invoke(ChatClient);
			this.Close();
		}

		private Task SetPort(string portString)
		{
			PortString = portString;
			return Task.CompletedTask;
		}
	}
}
