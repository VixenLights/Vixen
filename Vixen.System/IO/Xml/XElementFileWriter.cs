using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Xml.Linq;

namespace Vixen.IO.Xml
{
	internal class XElementFileWriter : IFileWriter<XElement>
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public void WriteFile(string filePath, XElement content)
		{
		    while (IsFileLocked(filePath))
		    {
				Logging.Warn("Filepath {0} is locked! Sleeping for 250ms to wait for it to free up.", filePath);
		        System.Threading.Thread.Sleep(250);
		    }
			try
			{
				content.Save(filePath);
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occurred trying to save the file. Attempting to protect any backups");

				try
				{
					var backupFile = string.Format("{0}_{1}", filePath, "backup");
					if (File.Exists(backupFile))
					{
						File.Copy(backupFile, string.Format("{0}.protected.{1}", filePath, DateTime.Now.ToFileTime()));
					}
				}
				catch (Exception e2)
				{
					Logging.Error(e2, "Could not protect the backup file.");
				}

				throw;
			}
		}

		void IFileWriter.WriteFile(string filePath, object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content mst be an XElement.");
			
			var success = BackupFile(filePath);
			if (!success)
			{
				var result = MessageBox.Show("Config backups failed prior to save! Possible file system issue!", "Warning!",
					MessageBoxButton.OKCancel);
				if (result == MessageBoxResult.Cancel)
				{
					return;
				}
			}

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
				Logging.Error("IO Exception checking file lock.", e);
                return (Marshal.GetHRForException(e) & 0xFFFF) == 32;
            }

            return false;
        }

		private bool BackupFile(string filePath)
		{
			bool success = false;
			while (IsFileLocked(filePath))
			{
				Logging.Warn("Filepath {0} is locked! Sleeping for 250ms to wait for it to free up.", filePath);
				System.Threading.Thread.Sleep(250);
			}

			try
			{
				if (File.Exists(filePath))
				{
					File.Copy(filePath, string.Format("{0}_{1}", filePath, "backup"), true);
				}
				success = true;
			}
			catch (Exception e)
			{
				Logging.Error(e, "Error while backing up file!");
			}

			return success;

		}
    }
}