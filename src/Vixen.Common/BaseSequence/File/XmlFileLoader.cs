﻿using System.Xml.Linq;

namespace BaseSequence.File
{
	internal class XmlFileLoader : IFileLoader
	{
		public object Load(string filePath)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
				using (StreamReader reader = new StreamReader(fileStream)) {
					return XElement.Load(reader);
				}
			}
		}
	}
}