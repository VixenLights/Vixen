using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace Vixen.Sys {
	static class Helper {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns>True if the directory exists after this call.</returns>
		static public bool EnsureDirectory(string path) {
			if(!Directory.Exists(path)) {
				try {
					Directory.CreateDirectory(path);
					return true;
				} catch {
					return false;
				}
			}
			return true;
		}

		static public XElement LoadXml(string filePath) {
			if(File.Exists(filePath)) {
				using(FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
					using(StreamReader reader = new StreamReader(fileStream)) {
						return XElement.Load(reader);
					}
				}
			}
			return null;
		}
	}
}
