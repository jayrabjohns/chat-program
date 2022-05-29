using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Program.Backend.Security
{
	class EncryptedBuffer
	{
		public byte[] CipherBytes { get; set; }
		public byte[] IV { get; set; }
		public int TotalBytesLength { get => CipherBytes.Length + IV.Length; }

		public EncryptedBuffer()
		{ }

		public EncryptedBuffer(byte[] asByteArray)
		{
			IV = asByteArray[0..Model.Settings.Aes.BlockSizeBytes];
			CipherBytes = asByteArray[IV.Length..];
		}

		public void ToByteArray(byte[] buffer, int offset, int length)
		{
			for (int i = 0; i < length - offset; i++)
			{
				buffer[i + offset] = (i < IV.Length ? IV[i] : CipherBytes[i - IV.Length]);
			}
		}
	}

	struct AesSetup
	{
		public byte[] Key;
		public Model.Aes Settings;
	}

	// https://tomrucki.com/posts/aes-encryption-in-csharp/

	class AesImpl : IDisposable
	{
		private byte[] Key { get; set; }
		private byte[] IV { get; set; }
		private Aes Aes { get; }
		private RNGCryptoServiceProvider RNG { get; } = new RNGCryptoServiceProvider();

		public AesImpl()
		{
			Key = new byte[Model.Settings.Aes.KeySizeBytes];
			RNG.GetBytes(Key);

			IV = new byte[Model.Settings.Aes.BlockSizeBytes];
			Aes = Aes.Create();
			Aes.Mode = Model.Settings.Aes.CipherMode;
		}

		public AesImpl(string password)
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

		public void UseSetup(AesSetup setup)
		{
			Model.Settings.Aes = setup.Settings;
			Key = setup.Key;

			IV = new byte[Model.Settings.Aes.BlockSizeBytes];
			Aes.Mode = Model.Settings.Aes.CipherMode;
		}

		public EncryptedBuffer Encrypt(byte[] plainBytes)
		{
			RNG.GetBytes(IV);
			EncryptedBuffer encryptedBuffer = new EncryptedBuffer() { IV = IV };

			using (ICryptoTransform encryptor = Aes.CreateEncryptor(Key, IV))
			{
				encryptedBuffer.CipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
			}

			return encryptedBuffer;
		}

		public byte[] Decrypt(EncryptedBuffer encryptedBuffer)
		{
			return Decrypt(encryptedBuffer.CipherBytes, encryptedBuffer.IV);
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

		public AesSetup ExtractKeyAndSettings()
		{
			return new AesSetup() { Key = Key, Settings = Model.Settings.Aes };
		}

		#region IDisposible
		public void Dispose()
		{
			Aes.Dispose();
			RNG.Dispose();
		}
		#endregion
	}
}
