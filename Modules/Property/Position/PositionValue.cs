using System.Runtime.Serialization;

namespace VixenModules.Property.Position {
	[DataContract]
	public class PositionValue {
		public PositionValue(PositionValue value)
			: this(value.Start, value.Width) {
		}

		public PositionValue(float start, float width) {
			Start = start;
			Width = width;
		}

		[DataMember]
		public float Start { get; private set; }
		[DataMember]
		public float Width { get; private set; }
	}
}
