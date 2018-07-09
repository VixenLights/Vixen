using System;

namespace Vixen.Data.Flow
{
	public class DataFlowComponentReference : IDataFlowComponentReference, IEquatable<DataFlowComponentReference>
	{
		public DataFlowComponentReference(IDataFlowComponent component, int outputIndex)
		{
			if (component == null) throw new ArgumentNullException("component");

			Component = component;
			OutputIndex = outputIndex;
		}

		public IDataFlowComponent Component { get; private set; }

		public int OutputIndex { get; private set; }

		public IDataFlowData GetOutputState()
		{
			return Component.Outputs[OutputIndex].Data;
		}

		#region Equality

		public bool Equals(DataFlowComponentReference other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Component, Component) && other.OutputIndex == OutputIndex;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (DataFlowComponentReference)) return false;
			return Equals((DataFlowComponentReference) obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (Component.GetHashCode()*397) ^ OutputIndex;
			}
		}

		#endregion
	}
}