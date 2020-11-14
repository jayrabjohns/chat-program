using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

		public ObservableCollection<string> ReceiveTextBoxText { get; } = new ObservableCollection<string>();

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

			ChatClient = new ChatClient(1024);
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			if (RemoteIP != null)
			{
				ChatClient.Connect(RemoteIP, RemotePort);
			}
		}
			}
		}
	}
}
