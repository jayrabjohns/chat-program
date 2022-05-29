using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Program.Backend.Security
{
	struct EncryptedMessage
	{
		byte[] CipherText;
		byte[] IV;
	}

	// https://tomrucki.com/posts/aes-encryption-in-csharp/

	class AesImpl : IDisposable
	{
		private byte[] Key { get; }
		private byte[] IV { get; }
		private Aes Aes { get; }
		private RNGCryptoServiceProvider RNG { get; } = new RNGCryptoServiceProvider();

		public AesImpl(byte[] password)
		{
			byte[] salt = new byte[Model.Settings.Aes.KeySaltSizeBytes];
			RNG.GetBytes(salt);

			// Using PBKDF2 to derive key from password
			Rfc2898DeriveBytes keyBytes = new Rfc2898DeriveBytes(password, salt, Model.Settings.Aes.KeyIterations, Model.Settings.Aes.KeyPRF);
			Key = keyBytes.GetBytes(Model.Settings.Aes.KeySizeBytes);

			IV = new byte[Model.Settings.Aes.BlockSizeBytes];

			Aes = Aes.Create();
			Aes.Mode = Model.Settings.Aes.CipherMode;
		}

		public byte[] Encrypt(byte[] plainBytes)
		{
			RNG.GetBytes(IV);

			byte[] cipherBytes;
			using (ICryptoTransform encryptor = Aes.CreateEncryptor(Key, IV))
			{
				 cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
			}

			byte[] result = new byte[IV.Length + cipherBytes.Length];
			for (int i = 0; i < IV.Length; i++)
			{
				result[i] = IV[i];
			}
			for (int i = IV.Length; i < result.Length; i++)
			{
				result[i] = cipherBytes[i - IV.Length];
			}

			return result;
		}

		public byte[] Decrypt(byte[] combinedCipherBytes)
		{
			byte[] IV = combinedCipherBytes[0..Model.Settings.Aes.BlockSizeBytes];
			byte[] cipherBytes = combinedCipherBytes[Model.Settings.Aes.BlockSizeBytes..];
			return Decrypt(cipherBytes, IV);
		}

		public byte[] Decrypt(byte[] cipherBytes, byte[] IV)
		{
			byte[] plainBytes;
			using (ICryptoTransform decryptor = Aes.CreateDecryptor(Key, IV))
			{
				plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
			}

			return plainBytes;
		}

		public void Dispose()
		{
			Aes.Dispose();
			RNG.Dispose();
		}
	}
}
