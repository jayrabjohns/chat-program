using System;

namespace Chat_Program.Security
{
	class EncryptedText
	{
		public byte[] CipherTextBytes;
		public byte[] VectorBytes;
		public byte[] SaltBytes;

		private string _cipherTextBase64 = null;
		public string CipherTextBase64 
		{
			get
			{
				if (_cipherTextBase64 == null)
				{
					_cipherTextBase64 = Convert.ToBase64String(CipherTextBytes);
				}

				return _cipherTextBase64;
			}
		}
	}
}
