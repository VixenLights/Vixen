using System.IO;
using Vixen.Module.Media;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace Vixen.Services
{
	public class MediaService
	{
		[DataPath]
		public static readonly string MediaDirectory = Path.Combine(Paths.DataRootPath, "Media");
		private static MediaService _instance;

		private MediaService()
		{
		}

		public static MediaService Instance
		{
			get { return _instance ?? (_instance = new MediaService()); }
		}

		public IMediaModuleInstance ImportMedia(string filePath)
		{
			string fileName = Path.GetFileName(filePath);
			string newPath = Path.Combine(MediaDirectory, fileName);
			File.Copy(filePath, newPath);	
			
			return GetMedia(Path.Combine(MediaDirectory, newPath));

		}

		public IMediaModuleInstance GetMedia(string fileName)
		{
			string filePath = Path.Combine(MediaDirectory, fileName);
			MediaModuleManagement manager = Modules.GetManager<IMediaModuleInstance, MediaModuleManagement>();
			IMediaModuleInstance module = manager.Get(filePath);

			if (module != null) {
				// Set the full file path in the instance.
				module.MediaFilePath = Path.Combine(MediaDirectory, filePath);
			}

			return module;
		}
	}
}