using Chat_Program.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chat_Program.Backend
{
	/// <summary>
	/// Stores references to objects needed by multiple other classes.
	/// </summary>
	class Globals
	{
		// Maybe switch this out for a list of Conversations, each having their own messages? 
		public static ObservableCollection<ConversationMessage> ConversationMessages { get; } = new ObservableCollection<ConversationMessage>
		{
			new ConversationMessage(
				"I am on the left and a very long message, so long in fact that it is presisely *this* many characters! WOW it's wrapping, very nice. very very cool!",
				"Received",
				"Yesterday 14:26 PM",
				Visibility.Collapsed),

			new ConversationMessage(
				$"This is testing{Environment.NewLine}Newlines!{Environment.NewLine}Wowowow",
				"Sent",
				"Yesterday 14:38 PM",
				Visibility.Collapsed),

			new ConversationMessage(
				"01:24",
				"Received",
				"Yesterday 19:26 PM",
					Visibility.Visible),

			new ConversationMessage(
				"Amazing!",
				"Sent",
				"Today 06:18 AM",
					Visibility.Collapsed),
		};
	}
}
