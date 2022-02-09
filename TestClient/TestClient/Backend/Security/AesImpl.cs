using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Chat_Program.Security
{
	class AesImpl
	{
		// Based on https://stackoverflow.com/questions/273452/using-aes-encryption-in-c-sharp

		private static RNGCryptoServiceProvider RNG { get; } = new RNGCryptoServiceProvider();

		public static EncryptedText Encrypt(byte[] textBytes, byte[] password, byte[] saltBytes)
		{
			return Encrypt<AesManaged>(textBytes, password, saltBytes);
		}

		public static EncryptedText Encrypt<T>(byte[] textBytes, byte[] password, byte[] saltBytes) where T : SymmetricAlgorithm, new()
		{
			var encryptedText = new EncryptedText();
			using (T cipher = new T())
			{
				byte[] vectorBytes = new byte[cipher.BlockSize / 16];
				RNG.GetBytes(vectorBytes);

				// Using PBKDF2 to derive key from password
				Rfc2898DeriveBytes passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, Model.Settings.Security.Iterations, Model.Settings.Security.HashAlgorithm);
				byte[] keyBytes = passwordBytes.GetBytes(Model.Settings.Security.KeySize / 8);

				cipher.Mode = Model.Settings.Security.CipherMode;

				using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						using (CryptoStream writer = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
						{
							writer.Write(textBytes, 0, textBytes.Length);
							writer.FlushFinalBlock();
							encryptedText.CipherTextBytes = ms.ToArray();
						}
					}
				}

				encryptedText.VectorBytes = vectorBytes;
				encryptedText.SaltBytes = saltBytes;

				cipher.Clear();
			}

			return encryptedText;
		}

		public static Tuple<byte[], int> Decrypt(byte[] textBytes, byte[] password, byte[] saltBytes, byte[] vectorBytes)
		{
			return Decrypt<AesManaged>(textBytes, password, saltBytes, vectorBytes);
		}

		public static Tuple<byte[], int> Decrypt<T>(byte[] textBytes, byte[] password, byte[] saltBytes, byte[] vectorBytes) where T : SymmetricAlgorithm, new()
		{
			byte[] decrypted;
			int decryptedByteCount = 0;

			using (T cipher = new T())
			{
				Rfc2898DeriveBytes passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, Model.Settings.Security.Iterations, Model.Settings.Security.HashAlgorithm);
				byte[] keyBytes = passwordBytes.GetBytes(Model.Settings.Security.KeySize / 8);

				cipher.Mode = Model.Settings.Security.CipherMode;

				//try
				//{
					using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
					{
						using (MemoryStream ms = new MemoryStream(textBytes))
						{
							using (CryptoStream reader = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
							{
								decrypted = new byte[textBytes.Length];
								decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
							}
						}
					}
				//}
				//catch (Exception ex)
				//{
				//	return string.Empty;
				//}
				cipher.Clear();
			}

			return new Tuple<byte[], int>(decrypted, decryptedByteCount);
		}

		public string ByteArrayToStringUTF8(byte[] bytes, int bytesRead)
		{
			return Encoding.UTF8.GetString(bytes, 0, bytesRead);
		}
	}
}
