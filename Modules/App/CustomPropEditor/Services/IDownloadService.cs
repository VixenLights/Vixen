using System;
using System.Threading.Tasks;

namespace VixenModules.App.CustomPropEditor.Services
{
	public interface IDownloadService
	{
		Task<bool> GetFileAsync(Uri url, string targetPath);
	}
}
