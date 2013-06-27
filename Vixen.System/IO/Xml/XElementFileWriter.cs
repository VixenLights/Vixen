using System;
using System.Xml.Linq;

namespace Vixen.IO.Xml
{
	internal class XElementFileWriter : IFileWriter<XElement>
	{
		public void WriteFile(string filePath, XElement content)
		{
			content.Save(filePath);
		}

		void IFileWriter.WriteFile(string filePath, object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content mst be an XElement.");
			WriteFile(filePath, (XElement) content);
		}
	}
}