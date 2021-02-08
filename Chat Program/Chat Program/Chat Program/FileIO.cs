using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Program
{
	class FileIO
	{
		private string FolderName { get; } = "ChatProgram";
		private string ConversationFileExtension { get; } = ".conv";
		private string ConversationsFolderPath { get; }

		public FileIO()
		{
			ConversationsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FolderName);
		}

		public void SaveConversation(Conversation conversation)
		{

		}

		public List<Conversation> GetAllConversations()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(ConversationsFolderPath);
			FileInfo[] fileInfos = directoryInfo.GetFiles($"*{ConversationFileExtension}");

			List<Conversation> conversations = new List<Conversation>(fileInfos.Length);

			for (int i = 0; i < fileInfos.Length; i++)
			{
				try
				{
					string contents = File.ReadAllText(fileInfos[i].FullName);
					Conversation conversation = JsonConvert.DeserializeObject<Conversation>(contents);
					conversations.Add(conversation);
				}
				catch (IOException)
				{
					// Nothing to do
				}
			}

			return conversations;
		}
	}
}
