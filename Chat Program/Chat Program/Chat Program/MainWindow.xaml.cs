using System;
using System.Collections.Generic;
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
					string ipString = Dns.GetHostEntry(hostName).AddressList[0].ToString();
					_hostIP = IPAddress.Parse(ipString);
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

		public string test
		{
			get => HostPort.ToString();
			set { }
		}

		private IPAddress _connecitonIP;
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

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
