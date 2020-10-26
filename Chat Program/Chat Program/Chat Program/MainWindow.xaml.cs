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
		private IPAddress _ipAddress;
		public IPAddress IPAddress 
		{
			get => _ipAddress;
			set
			{
				if (!_ipAddress.Equals(value))
				{
					_ipAddress = value;
					RaisePropertyChanged("IPAddress");
				}
			}
		}

		private int _port;
		public int Port
		{
			get => _port;
			set
			{
				if (_port != value)
				{
					_port = value;
					RaisePropertyChanged("Port");
				}
			}
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
