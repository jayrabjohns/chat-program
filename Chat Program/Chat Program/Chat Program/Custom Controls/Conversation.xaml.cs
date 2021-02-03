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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat_Program.Custom_Controls
{
	/// <summary>
	/// Interaction logic for Conversation.xaml
	/// </summary>
	public partial class Conversation : UserControl
	{
		public List<ConversationMessages> Messages { get { return Globals.ConversationMessages; } }

		public Conversation()
		{
			InitializeComponent();
			DataContext = this;
		}
	}
}
