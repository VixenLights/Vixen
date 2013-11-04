using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Vixen.IO.Xml
{
	internal class XElementFileWriter : IFileWriter<XElement>
	{
		public void WriteFile(string filePath, XElement content)
		{
		    while (IsFileLocked(filePath))
		    {
		        System.Threading.Thread.Sleep(250);
		    }
			content.Save(filePath);
		}

		void IFileWriter.WriteFile(string filePath, object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content mst be an XElement.");
			WriteFile(filePath, (XElement) content);
        }
        bool IsFileLocked(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) { }
                }
            }
            catch (IOException e)
            {
                return (Marshal.GetHRForException(e) & 0xFFFF) == 32;
            }

            return false;
        }
    }
}