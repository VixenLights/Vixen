using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Module.Controller {
	abstract public class ControllerModuleInstanceBase : OutputModuleInstanceBase, IControllerModuleInstance, IEqualityComparer<IControllerModuleInstance>, IEquatable<IControllerModuleInstance>, IEqualityComparer<ControllerModuleInstanceBase>, IEquatable<ControllerModuleInstanceBase> {
		private IDataPolicyFactory _dataPolicyFactory;

		public event EventHandler DataPolicyFactoryChanged;

		public IDataPolicyFactory DataPolicyFactory {
			get { return _dataPolicyFactory; }
			protected set {
				if(_dataPolicyFactory != value) {
					_dataPolicyFactory = value;
					OnDataPolicyFactoryChanged();
				}
			}
		}

		virtual protected void OnDataPolicyFactoryChanged() {
			if(DataPolicyFactoryChanged != null) {
				DataPolicyFactoryChanged(this, EventArgs.Empty);
			}
		}

		abstract public void UpdateState(int chainIndex, ICommand[] outputStates);

		#region Equality
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
		#endregion
	}
}
