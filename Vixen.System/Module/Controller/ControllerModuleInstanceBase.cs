using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.Controller {
	abstract public class ControllerModuleInstanceBase : OutputModuleInstanceBase, IControllerModuleInstance, IEqualityComparer<IControllerModuleInstance>, IEquatable<IControllerModuleInstance>, IEqualityComparer<ControllerModuleInstanceBase>, IEquatable<ControllerModuleInstanceBase> {
		private int _outputCount;

		public int OutputCount {
			get { return _outputCount; }
			set {
				_outputCount = value;
				_SetOutputCount(value);
			}
		}

		abstract protected void _SetOutputCount(int outputCount);

		public virtual int ChainIndex { get; set; }

		abstract public IDataPolicy DataPolicy { get; }

		abstract public void UpdateState(ICommand[] outputStates);

		public bool Equals(IControllerModuleInstance x, IControllerModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IControllerModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IControllerModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(ControllerModuleInstanceBase x, ControllerModuleInstanceBase y) {
			return Equals(x as IControllerModuleInstance, y as IControllerModuleInstance);
		}

		public int GetHashCode(ControllerModuleInstanceBase obj) {
			return GetHashCode(obj as IControllerModuleInstance);
		}

		public bool Equals(ControllerModuleInstanceBase other) {
			return Equals(other as IControllerModuleInstance);
		}
	}
}
