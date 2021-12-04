using Chat_Program.Backend;
using Chat_Program.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chat_Program.Frontend.Custom_Controls
{
	/// <summary>
	/// Interaction logic for Conversation.xaml
	/// </summary>
	public partial class Conversation : UserControl
	{
		public ObservableCollection<ConversationMessage> Messages { get { return Globals.ConversationMessages; } }
		private ListBox ConversationListBox = null;

		public Conversation()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void MessagesListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (VisualTreeHelper.GetChildrenCount(ConversationListBox) > 0)
			{
				Border border = (Border)VisualTreeHelper.GetChild(ConversationListBox, 0);
				ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			}

			//if (e.Action == NotifyCollectionChangedAction.Add)
			//{
			//	ConversationListBox.SelectedItem = e.NewItems[0];
			//}
		}

		private void MessagesListBox_Loaded(object sender, RoutedEventArgs e)
		{
			if (sender is ListBox listBox 
				&& listBox.ItemsSource is INotifyCollectionChanged itemsSource)
			{
				itemsSource.CollectionChanged += new NotifyCollectionChangedEventHandler(MessagesListBox_CollectionChanged);
				ConversationListBox = listBox;
			}
		}

		private void MessageListBox_Unloaded(object sender, RoutedEventArgs e)
		{
			if (sender is ListBox listBox 
				&& listBox.ItemsSource is INotifyCollectionChanged itemsSource)
			{
				itemsSource.CollectionChanged -= new NotifyCollectionChangedEventHandler(MessagesListBox_CollectionChanged);
				ConversationListBox = null;
			}
		}
	}
}
