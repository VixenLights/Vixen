using System.IO;
using System.Xml.Linq;

namespace Vixen.IO.Xml
{
	internal class XElementFileReader : IFileReader<XElement>
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public XElement ReadFile(string filePath)
		{
			if (!File.Exists(filePath)) return null;

			using (FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
				using (StreamReader reader = new StreamReader(fileStream)) {
					try {
						return XElement.Load(reader);
					}
					catch (System.Exception ex) {
						Logging.Error(ex, "Error loading '" + filePath + "'.");
					}
				}
			}

			// if there was an error loading the file, back up the 'bad' one, so the user can manually recover it later if needed.
			// This may not be needed in all cases, since this class is quite generic, but it will do for now. Can be refactored later if needed.
			if (File.Exists(filePath)) {
				File.Copy(filePath, string.Format("{0}.corrupt.{1}", filePath, System.DateTime.Now.ToFileTime()));
			}
			
			var backupFile = string.Format("{0}_{1}", filePath, "backup");
			if (File.Exists(backupFile))
			{
				File.Copy(backupFile, string.Format("{0}.protected.{1}", filePath, System.DateTime.Now.ToFileTime()));
			}
			return null;
		}

		object IFileReader.ReadFile(string filePath) {
			return ReadFile(filePath);
		}
	}
}