using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Chat_Program.Model
{
	public interface IMessage
	{
		ResponseType ResponseType { get; }
		byte[] Content { get; }
	}

	public interface IMessageDisplayer
	{
		string Message { get; set; }
		string TimeStamp { get; set; }
	}

	public interface IChatListItem : IMessageDisplayer
	{
		bool IsChatSelected { get; set; }
		bool IsOnline { get; set; }
		string ContactProfilePic { get; set; }
		string ContactName { get; set; }
		string Availability { get; set; }
		bool IsRead { get; set; }
		string NewMsgCount { get; set; }
	}

	public interface IConversationMessage : IMessageDisplayer
	{
		string MessageStatus { get; set; }
		Visibility IsAudioTrack { get; set; }
	}

	public interface IMenuListItem
	{
		string PathData { get; set; }
		bool IsItemSelected { get; set; }
		int ListItemHeight { get; set; }
	}

	public interface IConversation
	{
		Backend.ChatClient ChatClient { get; }
		ObservableCollection<IConversationMessage> ConversationMessages { get; }
	}
}
