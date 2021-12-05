using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace Chat_Program.Model
{
	public enum ResponseType : byte
	{
		String = 0,
		Image = 1,
		Audio = 2
	}

	public class MenuListItem : IMenuListItem
	{
		public string PathData { get; set; }
		public bool IsItemSelected { get; set; }
		public int ListItemHeight { get; set; }

		public MenuListItem(string pathData, bool isItemSelected = false, int listItemHeight = 0)
		{
			PathData = pathData;
			IsItemSelected = isItemSelected;
			ListItemHeight = listItemHeight;
		}
	}

	public class Message : IMessage
	{
		public ResponseType ResponseType { get; }
		public byte[] Content { get; }

		public Message(byte[] content, ResponseType responseType)
		{
			Content = content;
			ResponseType = responseType;
		}

		public Message(string content)
		{
			Content = Encoding.UTF8.GetBytes(content);
			ResponseType = ResponseType.String;
		}
	}

	public class ChatListItem : IChatListItem
	{
		public bool IsChatSelected { get; set; }
		public bool IsOnline { get; set; }
		public string ContactProfilePic { get; set; }
		public string ContactName { get; set; }
		public string TimeStamp { get; set; }
		public string Availability { get; set; }
		public bool IsRead { get; set; }
		public string Message { get; set; }
		public string NewMsgCount { get; set; }

		public ChatListItem(bool isChatSelected, bool isOnline, string contactProfilePic, string contactName, string timeStamp, string availability, bool isRead, string message, string newMsgCount)
		{
			IsChatSelected = isChatSelected;
			IsOnline = isOnline;
			ContactProfilePic = contactProfilePic;
			ContactName = contactName;
			TimeStamp = timeStamp;
			Availability = availability;
			IsRead = isRead;
			Message = message;
			NewMsgCount = newMsgCount;
		}

	}

	public class ConversationMessage : IConversationMessage
	{
		public string Message { get; set; }
		public string MessageStatus { get; set; }
		public string TimeStamp { get; set; }
		public Visibility IsAudioTrack { get; set; }

		public ConversationMessage(string message, string messageStatus, string timeStamp, Visibility isAudioTrack)
		{
			Message = message;
			MessageStatus = messageStatus;
			TimeStamp = timeStamp;
			IsAudioTrack = isAudioTrack;
		}
	}

	public class Conversation : IConversation
	{
		public Backend.ChatClient ChatClient { get; }
		public ObservableCollection<IConversationMessage> ConversationMessages { get; }

		public Conversation(Backend.ChatClient chatClient)
		{
			ChatClient = chatClient;

			//ConversationMessages = new ObservableCollection<IConversationMessage>();
			ConversationMessages = new ObservableCollection<IConversationMessage>()
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
						Visibility.Collapsed)
			};
		}
	}

	public class ChatClient : IChatClient
	{
		public int MaxResponseBytes { get; }
		public Action<IMessage> OnReceiveMessage { get; }
		public Action OnCouldntConnect { get; }
		public Action OnUnexpectedDisconnect { get; }
		public Action OnCouldntSendResponse { get; }

		public ChatClient(int maxResponseBytes = 1024, Action<IMessage> onReceiveMessage = null, Action onCouldntConnect = null, Action onUnexpectedDisconnect = null, Action onCouldntSendResponse = null)
		{
			MaxResponseBytes = maxResponseBytes;
			OnReceiveMessage = onReceiveMessage;
			OnCouldntConnect = onCouldntConnect;
			OnUnexpectedDisconnect = onUnexpectedDisconnect;
			OnCouldntSendResponse = onCouldntSendResponse;
		}
	}
}
