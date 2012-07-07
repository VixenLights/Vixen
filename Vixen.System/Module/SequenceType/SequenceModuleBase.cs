using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Sequence {
	abstract public class SequenceModuleBase : ModuleBase, ISequenceModuleInstance, IEqualityComparer<ISequenceModuleInstance> {
		abstract public string FileExtension { get; }

		public bool Equals(ISequenceModuleInstance x, ISequenceModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ISequenceModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
