using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Vixen.IO.Binary
{
	internal class BinaryFileWriter : IFileWriter<byte[]>
	{
		public void WriteFile(string filePath, byte[] content)
		{
			while (IsFileLocked(filePath))
			{
				System.Threading.Thread.Sleep(250);
			}
			File.WriteAllBytes(filePath,content);
		}

		public void WriteFile(string filePath, object content)
		{
			if (!(content is byte[])) throw new InvalidOperationException("Content mst be an XElement.");
			WriteFile(filePath, (byte[])content);
		}

		//Probably need to move this up to abstract class
		internal bool IsFileLocked(string path)
		{
			try
			{
				if (File.Exists(path))
				{
					using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) { }
				}
			} catch (IOException e)
			{
				return (Marshal.GetHRForException(e) & 0xFFFF) == 32;
			}

			return false;
		}
	}
}
