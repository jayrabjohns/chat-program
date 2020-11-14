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

		private IPAddress _connecitonIP = IPAddress.Parse("127.0.0.1");
		public IPAddress ConnectionIP 
		{
			get => _connecitonIP;
			set
			{
				if (!_connecitonIP.Equals(value))
				{
					_connecitonIP = value;
					OnPropertyChanged("ConnectionIP");
				}
			}
		}

		public string ConnectionIPStr
		{
			get => ConnectionIP.ToString();
			set
			{
				if (IPAddress.TryParse(value, out IPAddress ipAddress))
				{
					ConnectionIP = ipAddress;
				}
			}
		}

		private int _connectionPort = 5000;
		public int ConnectionPort
		{
			get => _connectionPort;
			set
			{
				if (_connectionPort != value)
				{
					_connectionPort = value;
					OnPropertyChanged("ConnectionPort");
				}
			}
		}

		private string _sendBoxText = string.Empty;
		public string SendBoxText
		{
			get => _sendBoxText;
			set
			{
				if (_sendBoxText != value)
				{
					_sendBoxText = value;
					OnPropertyChanged("SendBoxText");
				}
			}
		}

		public ObservableCollection<string> ReceiveBoxText { get; } = new ObservableCollection<string>();

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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (ConnectionIP != null)
			{
				ChatClient.Connect(ConnectionIP, ConnectionPort);
			}
		}
	}
}
