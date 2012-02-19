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

		static public T Load<T>(string filePath, IFileLoader<T> loader)
			where T : class {
			return File.Exists(filePath) ? loader.Load(filePath) : null;
		}

		//static public XElement LoadXml(string filePath) {
		//    if(File.Exists(filePath)) {
		//        using(FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
		//            using(StreamReader reader = new StreamReader(fileStream)) {
		//                return XElement.Load(reader);
		//            }
		//        }
		//    }
		//    return null;
		//}

		//In case we ever change how times are stored again, we only have to change it in one place.
		static public TimeSpan? GetXmlTimeValue(XElement element, string attributeName) {
			return XmlHelper.GetTimeSpanAttribute(element, attributeName);
		}
	}
}
