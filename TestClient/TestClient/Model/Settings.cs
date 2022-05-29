using System.Security.Cryptography;

namespace Chat_Program.Model
{
	class Settings
	{
		public static Security Security { get; }
		public static Aes Aes { get; } = new Aes();
	}

	class Aes
	{
		public int KeyIterations { get; } = 100_000;
		public int KeySize { get; } = 256;
		public int KeySizeBytes { get => KeySize / 8; }
		public int KeySaltSize { get; } = 128;
		public int KeySaltSizeBytes { get => KeySaltSize / 8; }
		public int BlockSizeBytes { get => KeySize / 16; }
		public HashAlgorithmName KeyPRF { get; } = HashAlgorithmName.SHA256;
		public CipherMode CipherMode { get; } = CipherMode.CBC;
	}

	class Security
	{
		public int Iterations { get; } = 2048;
		public int KeySize = 256;
		public HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA1;
		public CipherMode CipherMode = CipherMode.CBC;
	}
}
