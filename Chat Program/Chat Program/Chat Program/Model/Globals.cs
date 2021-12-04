using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chat_Program.Model
{
	/// <summary>
	/// Stores references to objects needed by multiple other classes.
	/// </summary>
	class Globals
	{
		public static IConversation CurrentConversation { get => (Conversations.Count > 0 ? Conversations[0] : null); }
		public static List<IConversation> Conversations { get; } = new List<IConversation>();
	}
}
