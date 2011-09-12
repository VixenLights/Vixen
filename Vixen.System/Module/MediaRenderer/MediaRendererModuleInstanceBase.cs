using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Module.Media;

namespace Vixen.Module.MediaRenderer {
	abstract public class MediaRendererModuleInstanceBase : ModuleInstanceBase, IMediaRendererModuleInstance, IEqualityComparer<IMediaRendererModuleInstance>, IEquatable<IMediaRendererModuleInstance>, IEqualityComparer<MediaRendererModuleInstanceBase>, IEquatable<MediaRendererModuleInstanceBase> {
		abstract public IMediaModuleInstance Media { get; set; }

		abstract public void Render(Graphics g, Rectangle invalidRect, TimeSpan startTime, TimeSpan timeSpan);

		abstract public void Setup();

		public bool Equals(IMediaRendererModuleInstance x, IMediaRendererModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IMediaRendererModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IMediaRendererModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(MediaRendererModuleInstanceBase x, MediaRendererModuleInstanceBase y) {
			return Equals(x as IMediaRendererModuleInstance, y as IMediaRendererModuleInstance);
		}

		public int GetHashCode(MediaRendererModuleInstanceBase obj) {
			return GetHashCode(obj as IMediaRendererModuleInstance);
		}

		public bool Equals(MediaRendererModuleInstanceBase other) {
			return Equals(other as IMediaRendererModuleInstance);
		}
}
}
