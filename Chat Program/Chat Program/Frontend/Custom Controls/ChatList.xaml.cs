using Chat_Program.Model;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Chat_Program.Frontend.Custom_Controls
{
	/// <summary>
	/// Interaction logic for ChatList.xaml
	/// </summary>
	public partial class ChatList : UserControl
	{
		public List<ChatListItem> ChatListItems
		{
			get
			{
				return new List<ChatListItem>
				{
					new ChatListItem(
						false,
						false,
						@"..\Assets\moss.jpg",
						"Jay Test",
						"10:30",
						"Offline",
						true,
						"Hey, this is testing some stuff, please ignore. TESTING\ntest",
						"1")
				};
			}
		}

		public ChatList()
		{
			InitializeComponent();
			DataContext = this;
		}
	}
}
