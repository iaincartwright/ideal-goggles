using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace iUtils
{
	public static class iCrypto
	{
		public static byte[] ComputeSHA1Hash(string a_str)
		{
			var data = Encoding.Unicode.GetBytes(a_str);

			return ComputeSHA1Hash(data);
		}

		public static byte[] ComputeSHA1Hash(byte[] a_data)
		{
			SHA1 shaM = new SHA1Managed();

			return shaM.ComputeHash(a_data);
		}

		private static readonly byte[] s_Salt = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

		public static byte[] EncryptAES(string a_str, string a_password)
		{
			return EncryptAES(Encoding.Unicode.GetBytes(a_str), a_password);
		}

		public static byte[] EncryptAES(byte[] a_plain, string a_password)
		{
			MemoryStream memoryStream;
			CryptoStream cryptoStream;
			using (var rijndael = Rijndael.Create())
			{
				var pdb = new Rfc2898DeriveBytes(a_password, s_Salt);

				rijndael.Key = pdb.GetBytes(32);
				rijndael.IV = pdb.GetBytes(16);

				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
			}

			cryptoStream.Write(a_plain, 0, a_plain.Length);
			cryptoStream.Close();

			return memoryStream.ToArray();
		}

		public static byte[] DecryptAes(byte[] a_cipher, string a_password)
		{
			MemoryStream memoryStream;
			CryptoStream cryptoStream;
			using (var rijndael = Rijndael.Create())
			{
				var pdb = new Rfc2898DeriveBytes(a_password, s_Salt);

				rijndael.Key = pdb.GetBytes(32);
				rijndael.IV = pdb.GetBytes(16);

				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
			}

			cryptoStream.Write(a_cipher, 0, a_cipher.Length);
			cryptoStream.Close();

			return memoryStream.ToArray();
		}
	}
}