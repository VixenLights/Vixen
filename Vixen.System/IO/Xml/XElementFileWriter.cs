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
		private const int BackupsToKeep = 3;

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
				ProtectBackups(filePath);
				throw;
			}
		}

		void IFileWriter.WriteFile(string filePath, object content)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content mst be an XElement.");
			
			var success = BackupFile(filePath);
			if (!success)
			{
				var result = MessageBox.Show("Backups failed prior to save! Possible file system issue!", "Warning!",
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
					var backupFile = $"{filePath}_backup.{DateTime.Now:dd-MM-yyyy_h_m_s}";
					if (File.Exists(backupFile))
					{
						//This should never happen being as we are using the date time as part of the file name
						File.Delete(backupFile);
					}
					//Under the covers move does a rename which should be safer and faster than a copy
					File.Move(filePath, backupFile);
					success = PurgeOldBackups(filePath);
				}
				success = true;
			}
			catch (Exception e)
			{
				Logging.Error(e, "Error while backing up file!");
			}

			return success;

		}

		private bool PurgeOldBackups(string filePath)
		{
			var folderPath = Path.GetDirectoryName(filePath);
			var fileName = Path.GetFileName(filePath);
			if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(folderPath))
			{
				var folderContent = new DirectoryInfo(folderPath).GetFileSystemInfos();
				var backups = folderContent.Where(x => x.Name.StartsWith($"{fileName}_backup"));
				if (backups.Count() > BackupsToKeep)
				{
					var orderedBackups = backups.OrderBy(x => x.LastWriteTime);
					int numToDelete = backups.Count() - BackupsToKeep;

					try
					{
						foreach (var fileSystemInfo in orderedBackups)
						{
							fileSystemInfo.Delete();
							if (--numToDelete <= 0) break;
						}
					}
					catch (Exception e)
					{
						Logging.Error(e, "Error while pruning the backup files.");
						return false;
					}
				}
			}

			return true;
		}

		private bool ProtectBackups(string filePath)
		{
			var folderPath = Path.GetDirectoryName(filePath);
			var fileName = Path.GetFileName(filePath);
			if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(folderPath))
			{
				var folderContent = new DirectoryInfo(folderPath).GetFileSystemInfos();
				var backups = folderContent.Where(x => x.Name.StartsWith($"{fileName}_backup"));
				if (backups.Any())
				{
					try
					{
						foreach (var fileSystemInfo in backups)
						{
							File.Move(fileSystemInfo.FullName, fileSystemInfo.FullName.Replace("backup", "protected"));
						}
					}
					catch (Exception e)
					{
						Logging.Error(e, "Error while protecting the backup files.");
						return false;
					}
				}
			}
			
			return true;
		}

    }
}