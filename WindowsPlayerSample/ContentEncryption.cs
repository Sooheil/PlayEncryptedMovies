using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPlayerSample
{
	public static class ContentEncryption
	{
		/// <summary>
		/// This method did not use in this project, but exists if you nead encrypt some movie files.
		/// </summary>
		/// <param name="sourceFile">source of file for encrypting</param>
		/// <param name="destFile">destination of encrypted file</param>
		public static void EncryptBinaryFile(String sourceFile, String destFile)
		{
			FileInfo info = new FileInfo(sourceFile);
			FileStream input = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
			byte[] dta = new byte[info.Length];
			input.Read(dta, 0, (int)info.Length);
			input.Close();

			FileStream output = new FileStream(destFile, FileMode.Create);
			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
			cryptic.Key = ASCIIEncoding.ASCII.GetBytes("YOUR8KEY");
			cryptic.IV = ASCIIEncoding.ASCII.GetBytes("YOURIVS8");
			CryptoStream crStream = new CryptoStream(output, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
			crStream.Write(dta, 0, dta.Length);
			crStream.Close();
			output.Close();
		}
		/// <summary>
		/// This method get source of encrypted file and decrypt it and return it with MemoryStream
		/// </summary>
		/// <param name="sourceFile">source of encrypted file</param>
		/// <returns>read all contents of encrypted file and return it by MemoryStream</returns>
		public static MemoryStream DecryptBinaryFile(String sourceFile)
		{
			if (sourceFile.Length == 0)
				return null;

			FileInfo info = new FileInfo(sourceFile);
			FileStream input = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
			cryptic.Key = ASCIIEncoding.ASCII.GetBytes("YOUR8KEY");
			cryptic.IV = ASCIIEncoding.ASCII.GetBytes("YOURIVS8");
			CryptoStream crStream = new CryptoStream(input, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
			BinaryReader rdr = new BinaryReader(crStream);
			byte[] dta = new byte[info.Length];
			rdr.Read(dta, 0, (int)info.Length);
			rdr.Close();
			crStream.Close();
			input.Close();
			return new MemoryStream(dta);
		}
	}
}
