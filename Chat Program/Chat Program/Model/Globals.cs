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

		private static string _projectRootFolder = null;
		public static string ProjectRootFolder
		{
			get
			{
				if (_projectRootFolder == null)
				{
					var path = System.Reflection.Assembly.GetEntryAssembly().Location;
					var directory = System.IO.Path.GetDirectoryName(path);
					_projectRootFolder = directory.Replace(@"\bin\Debug", string.Empty).Replace(@"\net5.0", string.Empty);
				}

				return _projectRootFolder;
			}
		}
	}
}
