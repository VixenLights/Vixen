using System;
using Vixen.Module.Effect;

namespace Vixen.Sys {
	/// <summary>
	/// Qualifies a Command with a start time and length.
	/// </summary>
	public class EffectNode : IEffectNode {
		public EffectNode()
			: this(null, TimeSpan.Zero) {
			// Default instance is empty.
		}

		public EffectNode(IEffectModuleInstance effect, TimeSpan startTime) {
			Effect = effect;
			StartTime = startTime;
		}

		public IEffectModuleInstance Effect { get; private set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan TimeSpan {
			get { return (Effect != null) ? Effect.TimeSpan : TimeSpan.Zero; }
		}

		public TimeSpan EndTime {
			get { return StartTime + TimeSpan; }
		}

		public bool IsEmpty {
			get { return Effect == null; }
		}

		static public readonly EffectNode Empty = new EffectNode();

		#region IComparable<IEffectNode>
		public int CompareTo(IEffectNode other) {
			return DataNode.Compare(this, other);
		}
		#endregion
	}

	public interface IEffectNode : IDataNode, IComparable<IEffectNode> {
		IEffectModuleInstance Effect { get; }
		bool IsEmpty { get; }
	}
}
