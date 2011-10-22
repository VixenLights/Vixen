using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Module.Media {
	abstract public class MediaModuleInstanceBase : ModuleInstanceBase, IMediaModuleInstance, IEqualityComparer<IMediaModuleInstance>, IEquatable<IMediaModuleInstance>, IEqualityComparer<MediaModuleInstanceBase>, IEquatable<MediaModuleInstanceBase> {
		abstract public string MediaFilePath { get; set; }

		abstract public void LoadMedia(TimeSpan startTime);

		abstract public ITiming TimingSource { get; }

		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public void Setup() { }

		abstract public void Start();

		abstract public void Stop();

		abstract public void Pause();

		abstract public void Resume();

		public bool Equals(IMediaModuleInstance x, IMediaModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IMediaModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IMediaModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(MediaModuleInstanceBase x, MediaModuleInstanceBase y) {
			return Equals(x as IMediaModuleInstance, y as IMediaModuleInstance);
		}

		public int GetHashCode(MediaModuleInstanceBase obj) {
			return GetHashCode(obj as IMediaModuleInstance);
		}

		public bool Equals(MediaModuleInstanceBase other) {
			return Equals(other as IMediaModuleInstance);
		}
	}
}
