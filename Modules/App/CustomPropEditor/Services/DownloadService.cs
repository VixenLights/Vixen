using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Catel.IoC;

namespace VixenModules.App.CustomPropEditor.Services
{
	[ServiceLocatorRegistration(typeof(IDownloadService))]
	public class DownloadService:IDownloadService
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		
		#region Implementation of IDownloadService

		/// <inheritdoc />
		public Task<bool> GetFileAsync(Uri url, string targetPath)
		{
			return GetFileAsync(url, targetPath, false);
		}

		/// <inheritdoc />
		public async Task<bool> GetFileAsync(Uri url, string targetPath, bool isNewer)
		{
			if (string.IsNullOrEmpty(targetPath))
			{
				throw new ArgumentNullException(nameof(targetPath));
			}

			var modifiedTime = DateTime.MinValue;
			if (isNewer && File.Exists(targetPath))
			{
				modifiedTime = File.GetLastWriteTime(targetPath).ToUniversalTime();
			}

			try
			{
				using (HttpClient wc = new HttpClient())
				{
					wc.DefaultRequestHeaders.IfModifiedSince = modifiedTime;
					wc.Timeout = TimeSpan.FromMilliseconds(5000);
					//Get Latest inventory from the url.
					var content = await wc.GetStreamAsync(url);
					var fileStream = File.Create(targetPath);
					fileStream.Seek(0, SeekOrigin.Begin);
					await CopyStream(content, fileStream);
					fileStream.Close();
					content.Close();
				}
			}
			catch(Exception e)
			{
				Logging.Error(e, $"An error occurred downloading the file {url.AbsoluteUri}");
				return false;
			}

			return true;
		}

		/// <inheritdoc />
		public async Task<string> GetFileAsStringAsync(Uri url)
		{
			using (HttpClient wc = new HttpClient())
			{
				wc.Timeout = TimeSpan.FromMilliseconds(5000);
				return await wc.GetStringAsync(url);
			}
		}

		#endregion

		/// <summary>
		/// Copies the contents of input to output. Doesn't close either stream.
		/// </summary>
		public static async Task CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[8 * 1024];
			int len;
			while ((len = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				await output.WriteAsync(buffer, 0, len);
			}
		}
	}
}
