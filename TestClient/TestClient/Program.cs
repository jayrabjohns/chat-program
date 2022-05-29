using Chat_Program.Backend.Security;
using System;
using System.Text;

namespace Chat_Program.Backend
{
	class Program
	{
		static void Main(string[] args)
		{
			AesImpl aes = new AesImpl(Encoding.UTF8.GetBytes("Jay"));

			var Out = aes.Encrypt(Encoding.UTF8.GetBytes("test"));
			Console.WriteLine(Convert.ToBase64String(Out));
			Console.WriteLine(Encoding.UTF8.GetString(Out));

			var Out2 = aes.Decrypt(Out);
			Console.WriteLine(Encoding.UTF8.GetString(Out2));
		}
	}
}
