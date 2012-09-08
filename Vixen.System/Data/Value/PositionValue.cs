using System;
using System.Runtime.Serialization;

namespace Vixen.Data.Value {
	[DataContract]
	public struct PositionValue : IIntentDataType {
		public PositionValue(float percentage) {
			if(percentage < 0 || percentage > 1) throw new ArgumentOutOfRangeException("percentage");

			Position = percentage;
		}

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		[DataMember]
		public float Position;
	}
}
