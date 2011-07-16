using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Module.Media {
	abstract public class MediaModuleInstanceBase : ModuleInstanceBase, IMediaModuleInstance, IEqualityComparer<IMediaModuleInstance> {
		abstract public string MediaFilePath { get; set; }

		abstract public void LoadMedia(long startTime);

		abstract public ITiming TimingSource { get; }

		abstract public void Setup();

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
	}
}
