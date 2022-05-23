using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Program.Backend
{

	public class NotConnectedException : Exception
	{
		public NotConnectedException(System.Net.Sockets.TcpClient tcpClient)
			: base($"Failed to perform an action involving remote end point `{tcpClient.Client.RemoteEndPoint}`, because there is no existing connection to it.") { }
	}

	class Exceptions
	{
		public static Action<Exception> DefaultAction { get; } = (Exception exception) => throw exception;
	}
}
