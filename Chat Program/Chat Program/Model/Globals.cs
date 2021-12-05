using System.Collections.Generic;

namespace Chat_Program.Model
{
	/// <summary>
	/// Stores references to objects needed by multiple other classes.
	/// </summary>
	class Globals
	{
		public static IConversation CurrentConversation { get => (Conversations.Count > 0 ? Conversations[0] : new Conversation(null)); }
		public static List<IConversation> Conversations { get; } = new List<IConversation>();
	}
}
