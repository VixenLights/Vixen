using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Module.OutputFilter {
	abstract public class OutputFilterModuleInstanceBase : ModuleInstanceBase, IAnyDataFlowDataHandler, IOutputFilterModuleInstance, IEqualityComparer<IOutputFilterModuleInstance>, IEquatable<IOutputFilterModuleInstance>, IEqualityComparer<OutputFilterModuleInstanceBase>, IEquatable<OutputFilterModuleInstanceBase> {
		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() { return false; }

		#region Equality
		public bool Equals(IOutputFilterModuleInstance x, IOutputFilterModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputFilterModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IOutputFilterModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(OutputFilterModuleInstanceBase x, OutputFilterModuleInstanceBase y) {
			return Equals(x as IOutputFilterModuleInstance, y as IOutputFilterModuleInstance);
		}

		public int GetHashCode(OutputFilterModuleInstanceBase obj) {
			return GetHashCode(obj as IOutputFilterModuleInstance);
		}

		public bool Equals(OutputFilterModuleInstanceBase other) {
			return Equals(other as IOutputFilterModuleInstance);
		}
		#endregion

		public void Update(IDataFlowData data) {
			data.Dispatch(this);
		}

		#region Data Flow data handlers
		virtual public void Handle(IDataFlowData<ICommand> obj) { }

		virtual public void Handle(IDataFlowData<IEnumerable<ICommand>> obj) { }

		virtual public void Handle(IDataFlowData<IEnumerable<IIntentState>> obj) { }
		#endregion

		#region IDataFlowComponent
		public Guid DataFlowComponentId {
			get { return InstanceId; }
		}

		abstract public DataFlowType InputDataType { get; }

		abstract public DataFlowType OutputDataType { get; }

		abstract public IDataFlowOutput[] Outputs { get; }

		public IDataFlowComponentReference Source { get; set; }
		#endregion
	}
}
