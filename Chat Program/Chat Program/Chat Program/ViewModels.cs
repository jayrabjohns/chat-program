using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chat_Program
{
	public class MenuItem
	{
		public string PathData { get; set; }
		public bool IsItemSelected { get; set; }
		public int ListItemHeight { get; set; }

		public MenuItem(string pathData, bool isItemSelected = false, int listItemHeight = 0)
		{
			PathData = pathData;
			IsItemSelected = isItemSelected;
			ListItemHeight = listItemHeight;
		}
	}

	public class ChatListItem
	{
		public bool IsChatSelected { get; set; }
		public bool IsOnline { get; set; }
		public string ContactProfilePic { get; set; }
		public string ContactName { get; set; }
		public string LastMessageTime { get; set; }
		public string Availability { get; set; }
		public bool IsRead { get; set; }
		public string Message { get; set; }
		public string NewMsgCount { get; set; }

		public ChatListItem(bool isChatSelected, bool isOnline, string contactProfilePic, string contactName, string lastMessageTime, string availability, bool isRead, string message, string newMsgCount)
		{
			IsChatSelected = isChatSelected;
			IsOnline = isOnline;
			ContactProfilePic = contactProfilePic;
			ContactName = contactName;
			LastMessageTime = lastMessageTime;
			Availability = availability;
			IsRead = isRead;
			Message = message;
			NewMsgCount = newMsgCount;
		}

	}

	public class ConversationMessage
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
}
