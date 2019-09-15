using System;
using System.Threading.Tasks;

namespace VixenModules.App.CustomPropEditor.Services
{
	public interface IDownloadService
	{
		Task<bool> GetFileAsync(Uri url, string targetPath);

		Task<bool> GetFileAsync(Uri url, string targetPath, bool isNewer);

		Task<string> GetFileAsStringAsync(Uri url);
	}
}
