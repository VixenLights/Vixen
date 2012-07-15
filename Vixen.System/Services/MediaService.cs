using Vixen.Module.Media;
using Vixen.Sys;

namespace Vixen.Services {
	public class MediaService {
		static private MediaService _instance;

		private MediaService() { }

		public static MediaService Instance {
			get { return _instance ?? (_instance = new MediaService()); }
		}

		public IMediaModuleInstance GetMedia(string filePath) {
			MediaModuleManagement manager = Modules.GetManager<IMediaModuleInstance, MediaModuleManagement>();
			IMediaModuleInstance module = manager.Get(filePath);

			if(module != null) {
				// Set the file in the instance.
				module.MediaFilePath = filePath;
			}

			return module;
		}
	}
}
