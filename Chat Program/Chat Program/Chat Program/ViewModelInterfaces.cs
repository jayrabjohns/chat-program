using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chat_Program
{
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
}
