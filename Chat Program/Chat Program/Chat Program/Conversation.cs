using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Program
{
	class Conversation
	{
		public ObservableCollection<ConversationMessage> ConversationMessages { get; } = new ObservableCollection<ConversationMessage>();
		public ChatListItem ChatListItem { get; }
		public string Id { get; }

		public Conversation(ObservableCollection<ConversationMessage> conversationMessages, ChatListItem chatListItem, string id)
		{
			ConversationMessages = conversationMessages;
			ChatListItem = chatListItem;
			Id = id;
		}
	}
}
