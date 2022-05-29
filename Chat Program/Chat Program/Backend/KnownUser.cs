using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Program.Backend
{
	class KnownUser
	{
		public RSAParameters PublicKey { get; }
		public string DisplayName { get; }

		public KnownUser(RSAParameters publicKey, string displayName)
		{
			PublicKey = publicKey;
			DisplayName = displayName;
		}
	}
}
