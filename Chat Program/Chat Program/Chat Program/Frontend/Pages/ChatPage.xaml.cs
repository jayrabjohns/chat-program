using Chat_Program.Backend;
using Chat_Program.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat_Program.Frontend.Pages
{
	/// <summary>
	/// Interaction logic for ChatPage.xaml
	/// </summary>
	public partial class ChatPage : Page, INotifyPropertyChanged
	{
		private string _sendTextBoxText = string.Empty;
		public string SendTextBoxText
		{
			get => _sendTextBoxText;
			set
			{
				if (_sendTextBoxText != value)
				{
					_sendTextBoxText = value;
					OnPropertyChanged(nameof(SendTextBoxText));
				}
			}
		}

		private ChatClient ChatClient { get; }

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		public ChatPage()
		{
			InitializeComponent();
			this.DataContext = this;

			ChatClient = new ChatClient(1024, OnReceiveMessage);

			//while (!ChatClient.Connect(IPAddress.Parse("127.0.0.1"), 5000))
			//{
			//	Thread.Sleep(1000);
			//}

			//ChatClient.StartListeningForMessages();
		}

		private void SendMessage(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			if (ChatClient.TrySendString(message))
			{
				SendTextBoxText = string.Empty;

				ConversationMessage conversationMessage = new ConversationMessage(message, "Sent", "Now", Visibility.Collapsed);
				Globals.ConversationMessages.Add(conversationMessage);
			}
		}

		#region Event Handlers
		private void OnReceiveMessage(Message message)
		{
			ConversationMessage conversationMessage;

			switch (message.ResponseType)
			{
				case ResponseType.StringMessage:
					conversationMessage = new ConversationMessage(message.StringMessage, "Received", "Now", Visibility.Collapsed);
					break;

				case ResponseType.Image:
				case ResponseType.Audio:
				default:
					return;
			}

			Globals.ConversationMessages.Add(conversationMessage);
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
					SendTextBoxText = SendTextBoxText.Substring(0, SendTextBoxText.Length - Environment.NewLine.Length);
					SendMessage(SendTextBoxText);
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
