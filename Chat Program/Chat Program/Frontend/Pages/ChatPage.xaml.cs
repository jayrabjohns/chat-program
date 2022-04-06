using Chat_Program.Backend;
using Chat_Program.Frontend.Dialogs;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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

		public ChatPage()
		{
			// OnReceiveMessage needs to be called from the UI thread
			ServerConnectionDialog serverConnectionDialog = new ServerConnectionDialog(
				(message) => Application.Current.Dispatcher.Invoke(() => OnReceiveMessage(message)),
				Model.Settings.Network.MaxConnectionAttempts, 
				Model.Settings.Network.ConnectionRetryDelayMs);

			serverConnectionDialog.Owner = Window.GetWindow(this);
			serverConnectionDialog.ShowDialog();

			InitializeComponent();
			this.DataContext = this;

			ChatClient = Model.Globals.CurrentConversation.ChatClient;
		}

		private void SendMessage(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			if (message.EndsWith("\r\n"))
			{
				message = message.Substring(0, message.Length - 2);
			}
			else if (message.EndsWith('\n'))
			{
				message = message.Substring(0, message.Length - 1);
			}

			if (ChatClient?.TrySendString(message) ?? false)
			{
				SendTextBoxText = string.Empty;

				Model.ConversationMessage conversationMessage = new Model.ConversationMessage(message, "Sent", "Now", Visibility.Collapsed);
				Model.Globals.CurrentConversation.ConversationMessages.Add(conversationMessage);
			}
		}

		private void OnReceiveMessage(Model.IMessage message)
		{
			Model.ConversationMessage conversationMessage;

			switch (message.ResponseType)
			{
				case Model.ResponseType.String:
					conversationMessage = new Model.ConversationMessage(Encoding.UTF8.GetString(message.Content), "Received", "Now", Visibility.Collapsed);
					break;

				case Model.ResponseType.Image:
				case Model.ResponseType.Audio:
				default:
					return;
			}

			Model.Globals.CurrentConversation.ConversationMessages.Add(conversationMessage);
		}

		#region Event Handlers
		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			SendMessage(SendTextBoxText);
		}
		private void messageSendBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter && !e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftShift))
			{
				SendMessage(SendTextBoxText);
			}
		}
		#endregion

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
