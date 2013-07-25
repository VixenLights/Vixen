using System.IO;
using System.Xml.Linq;

namespace Vixen.IO.Xml {
	internal class XElementFileReader : IFileReader<XElement> {
		public XElement ReadFile(string filePath) {
			if (!File.Exists(filePath)) return null;

			using (FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
				using (StreamReader reader = new StreamReader(fileStream)) {
					try {
						return XElement.Load(reader);
					}
					catch (System.Exception ex) {
						Vixen.Sys.VixenSystem.Logging.Error(string.Format("Error loading '{0}'", filePath), ex);
					}
				}
			}

			// if there was an error loading the file, back up the 'bad' one, so the user can manually recover it later if needed.
			// This may not be needed in all cases, since this class is quite generic, but it will do for now. Can be refactored later if needed.
			if (File.Exists(filePath)) {
				File.Copy(filePath, string.Format("{0}.{1}", filePath, System.DateTime.Now.ToFileTime()));
			}
			return null;
		}

		object IFileReader.ReadFile(string filePath) {
			return ReadFile(filePath);
		}
	}
}