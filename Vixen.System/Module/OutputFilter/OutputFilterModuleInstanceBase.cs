using System;
using System.Collections.Generic;
using Vixen.Data.Flow;
using Vixen.Sys.Dispatch;

namespace Vixen.Module.OutputFilter
{
	public abstract class OutputFilterModuleInstanceBase : ModuleInstanceBase, IAnyDataFlowDataHandler,
	                                                       IOutputFilterModuleInstance,
	                                                       IEqualityComparer<IOutputFilterModuleInstance>,
	                                                       IEquatable<IOutputFilterModuleInstance>,
	                                                       IEqualityComparer<OutputFilterModuleInstanceBase>,
	                                                       IEquatable<OutputFilterModuleInstanceBase>
	{
		public virtual bool HasSetup
		{
			get { return false; }
		}

		public virtual bool Setup()
		{
			return false;
		}

		public void Update(IDataFlowData data)
		{
			data.Dispatch(this);
		}

		#region Equality

		public bool Equals(IOutputFilterModuleInstance x, IOutputFilterModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputFilterModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(IOutputFilterModuleInstance other)
		{
			return base.Equals(other);
		}

		public bool Equals(OutputFilterModuleInstanceBase x, OutputFilterModuleInstanceBase y)
		{
			return Equals(x as IOutputFilterModuleInstance, y as IOutputFilterModuleInstance);
		}

		public int GetHashCode(OutputFilterModuleInstanceBase obj)
		{
			return GetHashCode(obj as IOutputFilterModuleInstance);
		}

		public bool Equals(OutputFilterModuleInstanceBase other)
		{
			return Equals(other as IOutputFilterModuleInstance);
		}

		#endregion

		#region Data Flow data handlers

		public virtual void Handle(CommandDataFlowData obj)
		{
		}

		public virtual void Handle(CommandsDataFlowData obj)
		{
		}

		public virtual void Handle(IntentsDataFlowData obj)
		{
		}

		public virtual void Handle(IntentDataFlowData obj)
		{
		}

		#endregion

		#region IDataFlowComponent

		public Guid DataFlowComponentId
		{
			get { return InstanceId; }
		}

		public abstract DataFlowType InputDataType { get; }

		public abstract DataFlowType OutputDataType { get; }

		public abstract IDataFlowOutput[] Outputs { get; }

		public IDataFlowComponentReference Source { get; set; }

		public virtual string Name
		{
			get { return Descriptor.TypeName; }
		}

		#endregion
	}
}