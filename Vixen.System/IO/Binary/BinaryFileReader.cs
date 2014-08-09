using System;
using System.IO;

namespace Vixen.IO.Binary
{
	class BinaryFileReader : IFileReader<byte[]>
	{
		public byte[] ReadFile(string filePath)
		{
			return File.ReadAllBytes(filePath);
		}

		object IFileReader.ReadFile(string filePath)
		{
			return ReadFile(filePath);
		}
	}
}
