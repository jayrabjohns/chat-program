using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Chat_Server.Data
{
	class Security
	{
		public int Iterations { get; } = 2048;
		public int KeySize = 256;
		public HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA1;
		public CipherMode CipherMode = CipherMode.CBC;
	}

	class Settings
	{
		public static Security Security { get; }
	}
}
