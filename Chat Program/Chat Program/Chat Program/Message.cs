using System.ComponentModel;

namespace Chat_Program
{
	public enum ResponseType
	{
		StringMessage = 0,
		Image = 1,
		Audio = 2
	}

	public class Message
	{
		public ResponseType ResponseType { get; }
		public string StringMessage { get; }
		public byte[] Image { get; }
		public byte[] Audio { get; }

		public Message(ResponseType responseType, string stringMessage, byte[] image, byte[] audio)
		{
			ResponseType = responseType;
			StringMessage = stringMessage;
			Image = image ?? new byte[0];
			Audio = audio ?? new byte[0];
		}
	}
}
