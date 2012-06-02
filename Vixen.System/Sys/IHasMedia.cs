using System.Collections.Generic;
using Vixen.Module.Media;

namespace Vixen.Sys {
	public interface IHasMedia {
		void AddMedia(IMediaModuleInstance module);
		void AddMedia(IEnumerable<IMediaModuleInstance> module);
		bool RemoveMedia(IMediaModuleInstance module);
		IEnumerable<IMediaModuleInstance> GetAllMedia();
		void ClearMedia();
	}
}
