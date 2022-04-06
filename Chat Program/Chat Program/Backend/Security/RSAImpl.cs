using System;
using System.Security.Cryptography;
using System.Text;

namespace Chat_Program.Backend
{
	class RsaImpl : IDisposable
	{
		public RSAParameters PrivateKey { get; private set; }
		public RSAParameters PublicKey { get; private set; }

		private RSA RSA { get; }
		private RNGCryptoServiceProvider RNG { get; }

		public RsaImpl(int keySize)
		{
			RSA = RSA.Create(keySize);
			PrivateKey = RSA.ExportParameters(true);
			PublicKey = RSA.ExportParameters(false);
			RNG = new RNGCryptoServiceProvider();
		}

		public string EncryptRsaToBase64(string plainText)
		{
			byte[] data = Encoding.UTF8.GetBytes(plainText);
			byte[] encryptedData = EncryptRsa(data);
			return Convert.ToBase64String(encryptedData);
		}

		public byte[] EncryptRsa(byte[] data)
		{
			RSA.ImportParameters(PublicKey);
			return RSA.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
		}

		public string DecryptRsaFromBase64(string cypherText)
		{
			var data = Convert.FromBase64String(cypherText);
			var decryptedData = DecryptRsa(data);
			return Encoding.UTF8.GetString(decryptedData);
		}

		public byte[] DecryptRsa(byte[] data)
		{
			RSA.ImportParameters(PrivateKey);
			return RSA.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
		}

		public byte[] ExportKeyPair()
		{
			return RSA.ExportRSAPrivateKey();
		}

		public byte[] ExportPublicKey()
		{
			return RSA.ExportRSAPublicKey();
		}

		public void SetKeyPair(byte[] keyPair)
		{
			RSA.ImportRSAPrivateKey(keyPair, out _);
			PrivateKey = RSA.ExportParameters(true);
			PublicKey = RSA.ExportParameters(false);
		}

		public byte[] ExportEncryptedKeyPair()
		{
			return null;
		}

		#region IDisposable
        public void Dispose()
        {
            RSA.Dispose();
			RNG.Dispose();
        }
		#endregion
    }
}