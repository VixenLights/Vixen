using System;

namespace Vixen.Sys
{
	public class ControllerLink : IEquatable<ControllerLink>
	{
		public ControllerLink(Guid controllerId)
		{
			ControllerId = controllerId;
			//PriorId = priorId;
			//NextId = nextId;
		}

		public readonly Guid ControllerId;
		public Guid? PriorId;
		public Guid? NextId;

		public bool Equals(ControllerLink other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.ControllerId.Equals(ControllerId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ControllerLink)) return false;
			return Equals((ControllerLink) obj);
		}

		public override int GetHashCode()
		{
			return ControllerId.GetHashCode();
		}
	}
}