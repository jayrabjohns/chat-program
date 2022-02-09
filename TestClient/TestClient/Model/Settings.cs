using System.Security.Cryptography;

namespace Chat_Program.Model
{
	class Settings
	{
		public static Security Security { get; }
	}

	class Security
	{
		public int Iterations { get; } = 2048;
		public int KeySize = 256;
		public HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA1;
		public CipherMode CipherMode = CipherMode.CBC;
	}
}
