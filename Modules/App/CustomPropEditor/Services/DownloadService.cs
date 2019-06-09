using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
		public async Task<bool> GetFileAsync(Uri url, string targetPath)
		{
			if (string.IsNullOrEmpty(targetPath))
			{
				throw new ArgumentNullException(nameof(targetPath));
			}

			try
			{
				using (HttpClient wc = new HttpClient())
				{
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
				Logging.Error(e, $"An error occured downloading the file {url.AbsoluteUri}");
				return false;
			}

			return true;
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
