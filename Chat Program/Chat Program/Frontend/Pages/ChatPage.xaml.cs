using System.Windows;
using System.Windows.Controls;

namespace Chat_Program.Frontend.Pages
{
	/// <summary>
	/// Interaction logic for ChatPage.xaml
	/// </summary>
	public partial class ChatPage : Page
	{
		public ChatPage()
		{
			InitializeComponent();
		}

		#region Event Handlers
		private void SendTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			//if (sender is TextBox textBox)
			//{
			//	// Force updating value of SendTextBoxText
			//	textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

			//	bool minLengthAdded = false;

			//	// Checking if the changes could plausibly contain a newline
			//	foreach (var change in e.Changes)
			//	{
			//		if (change.AddedLength >= Environment.NewLine.Length)
			//		{
			//			minLengthAdded = true;
			//			break;
			//		}
			//	}

			//	// Only send message if enter was pressed
			//	if (minLengthAdded && SendTextBoxText.EndsWith(Environment.NewLine))
			//	{
			//		SendTextBoxText = SendTextBoxText.Substring(0, SendTextBoxText.Length - Environment.NewLine.Length);
			//		SendMessage(SendTextBoxText);
			//	}
			//}
		}

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			//if (!string.IsNullOrWhiteSpace(SendTextBoxText))
			//{
			//	SendMessage(SendTextBoxText);
			//}
		}
		#endregion
	}
}
