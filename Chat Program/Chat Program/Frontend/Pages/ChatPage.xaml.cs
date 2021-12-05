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

			ServerConnectionDialog serverConnectionDialog = new ServerConnectionDialog(OnReceiveMessage);
			serverConnectionDialog.Owner = Window.GetWindow(this);
			serverConnectionDialog.ShowDialog();
		}

		private void SendMessage(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
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
