using System.Security.Cryptography;

namespace Chat_Program.Model
{
	class Settings
	{
		public static Network Network { get; } = new Network();
		public static Aes Aes { get; set; } = new Aes();
		public static Rsa Rsa { get; } = new Rsa();
	}

	class Network
	{
		public int ResponseSizeBytes { get; } = 1024;
		public int MaxConnectionAttempts { get; } = 5;
		public int ConnectionRetryDelayMs { get; } = 1000;
		public int ReadMessageRetryDelayMs { get; } = 100;
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

	class Rsa
	{
		public int KeySize { get; } = 4096;
	}
}
