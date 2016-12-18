using System;
using Vixen.Module.Effect;

namespace Vixen.Sys
{
	/// <summary>
	/// Qualifies a Command with a start time and length.
	/// </summary>
	[Serializable]
	public class EffectNode : IEffectNode
	{
//, IEquatable<IEffectNode>, IEquatable<EffectNode> {
		public EffectNode()
			: this(null, TimeSpan.Zero)
		{
			// Default instance is empty.
		}

		public EffectNode(IEffectModuleInstance effect, TimeSpan startTime)
		{
			Effect = effect;
			StartTime = startTime;
		}

		public IEffectModuleInstance Effect { get; private set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan TimeSpan
		{
			get { return (Effect != null) ? Effect.TimeSpan : TimeSpan.Zero; }
		}

		public TimeSpan EndTime
		{
			get { return StartTime + TimeSpan; }
		}

		public bool IsEmpty
		{
			get { return Effect == null; }
		}

		#region IComparable<IEffectNode>

		public int CompareTo(IEffectNode other)
		{
			return DataNode.Compare(this, other);
		}

		#endregion

		#region IEquatable<IEffectNode>

		//public bool Equals(IEffectNode other) {
		//    throw new NotImplementedException();
		//}

		//public override int GetHashCode() {
		//    return base.GetHashCode();
		//}

		//public override bool Equals(object obj) {
		//    return base.Equals(obj);
		//}

		#endregion

		#region IEquatable<EffectNode>

		//public bool Equals(EffectNode other) {
		//    throw new NotImplementedException();
		//}

		#endregion
	}

	public interface IEffectNode : IDataNode, IComparable<IEffectNode>
	{
		IEffectModuleInstance Effect { get; }
		bool IsEmpty { get; }
	}
}