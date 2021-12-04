using Chat_Program.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
		private Action<Model.IMessage> OnReceiveMessage { get; }

		public ServerConnectionDialog(int connectionAttempts = 5, Action<Model.IMessage> onReceiveMessage = null)
		{
			ConnectionAttempts = connectionAttempts;
			OnReceiveMessage = onReceiveMessage;

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
			ChatClient chatClient = new ChatClient(1024, OnReceiveMessage);
			bool connected = false;

			for (int i = 0; i < ConnectionAttempts; i++)
			{
				if (chatClient.TryConnect("localhost", 5000))
				{
					connected = true;
					break;
				}

				await Task.Delay(1000);
			}

			if (connected)
			{
				Model.Globals.Conversations.Add(new Model.Conversation(chatClient));
				chatClient.StartListeningForMessages();
				this.Close();
			}
		}
	}
}
