using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Chat_Program
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private IPAddress _hostIP = null;
		public IPAddress HostIP
		{
			get
			{
				if (_hostIP == null)
				{
					string hostName = Dns.GetHostName();
					IPAddress[] addresses = Dns.GetHostEntry(hostName).AddressList;
					_hostIP = addresses[1];
				}

				return _hostIP;
			}
		}

		private int _hostPort = 5000;
		public int HostPort
		{
			get => _hostPort;
			set
			{
				if (_hostPort != value)
				{
					_hostPort = value;
					OnPropertyChanged("HostPort");
				}
			}
		}

		private IPAddress _remoteIP = IPAddress.Parse("127.0.0.1");
		public IPAddress RemoteIP 
		{
			get => _remoteIP;
			set
			{
				if (!_remoteIP.Equals(value))
				{
					_remoteIP = value;
					OnPropertyChanged("RemoteIP");
				}
			}
		}

		public string RemoteIPStr
		{
			get => RemoteIP.ToString();
			set
			{
				if (IPAddress.TryParse(value, out IPAddress ipAddress))
				{
					RemoteIP = ipAddress;
				}
			}
		}

		private int _remotePort = 5000;
		public int RemotePort
		{
			get => _remotePort;
			set
			{
				if (_remotePort != value)
				{
					_remotePort = value;
					OnPropertyChanged("RemotePort");
				}
			}
		}

		private string _sendTextBoxText = string.Empty;
		public string SendTextBoxText
		{
			get => _sendTextBoxText;
			set
			{
				if (_sendTextBoxText != value)
				{
					_sendTextBoxText = value;
					OnPropertyChanged("SendTextBoxText");
				}
			}
		}


		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		
		private ChatClient ChatClient { get; }
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;

			ChatClient = new ChatClient(null, 1024);
		}

		private void SendMessage(string message)
		{
			ChatClient.SendString(message);
			SendTextBoxText = string.Empty;

			ConversationMessage conversationMessage = new ConversationMessage(message, "Sent", "Now", Visibility.Collapsed);
			Globals.ConversationMessages.Add(conversationMessage);
		}

		#region Event Handlers
		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			if (RemoteIP != null)
			{
				ChatClient.Connect(RemoteIP, RemotePort);
			}
		}

		private void SendTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				// Force updating value of SendTextBoxText
				textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

				bool minLengthAdded = false;

				// Checking if the changes could plausibly contain a newline
				foreach (var change in e.Changes)
				{
					if (change.AddedLength >= Environment.NewLine.Length)
					{
						minLengthAdded = true;
						break;
					}
				}

				// Only send message if enter was pressed
				if (minLengthAdded && SendTextBoxText.EndsWith(Environment.NewLine))
				{
					string message = SendTextBoxText.Substring(0, SendTextBoxText.Length - Environment.NewLine.Length);
					SendMessage(message);
				}
			}
		}

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(SendTextBoxText))
			{
				SendMessage(SendTextBoxText);
			}
		}
		#endregion
	}
}
