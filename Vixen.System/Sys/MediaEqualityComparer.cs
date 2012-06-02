using System.Collections.Generic;
using Vixen.Module.Media;

namespace Vixen.Sys {
	class MediaEqualityComparer : IEqualityComparer<IMediaModuleInstance> {
		public bool Equals(IMediaModuleInstance x, IMediaModuleInstance y) {
			return x.MediaFilePath == y.MediaFilePath;
		}

		public int GetHashCode(IMediaModuleInstance obj) {
			return obj.MediaFilePath.GetHashCode();
		}
	}
}
