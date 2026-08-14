using System.Runtime.InteropServices;

namespace VixenApplication
{
	internal static class KnownFolders
	{
		private static readonly Guid DownloadsFolderId = new("374DE290-123F-4565-9164-39C4925E467B");

		internal static string GetDownloadsPath()
		{
			var fallbackPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
			var downloadsFolderId = DownloadsFolderId;
			var result = SHGetKnownFolderPath(ref downloadsFolderId, 0, IntPtr.Zero, out var path);
			try
			{
				return result >= 0 && path != IntPtr.Zero
					? Marshal.PtrToStringUni(path) ?? fallbackPath
					: fallbackPath;
			}
			finally
			{
				if (path != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(path);
				}
			}
		}

		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		private static extern int SHGetKnownFolderPath(
			ref Guid rfid,
			uint flags,
			IntPtr token,
			out IntPtr path);
	}
}
