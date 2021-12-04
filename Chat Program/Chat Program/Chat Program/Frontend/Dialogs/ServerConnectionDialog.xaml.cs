using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public partial class ServerConnectionDialog : Window
	{
		public ServerConnectionDialog()
		{
			InitializeComponent();
			this.DataContext = this;
		}

		private void TryConnect()
		{

		}
	}
}
