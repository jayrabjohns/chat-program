using System.Security.Cryptography;

namespace Chat_Program.Model
{
	class Settings
	{
		public static Network Network { get; } = new Network();
		public static Aes Aes { get; } = new Aes();
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
		public int Iterations { get; } = 2048;
		public int KeySize { get; } = 256;
		public HashAlgorithmName HashAlgorithm { get; } = HashAlgorithmName.SHA1;
		public CipherMode CipherMode { get; } = CipherMode.CBC;
	}

	class Rsa
	{
		public int KeySize { get; } = 4096;
	}
}
