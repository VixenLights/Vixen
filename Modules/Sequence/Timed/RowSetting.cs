using System.Runtime.Serialization;

namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class RowSetting
	{
		public RowSetting(int rowHeight, bool expanded, bool visible)
		{
			RowHeight = rowHeight;
			Expanded = expanded;
			Visible = visible;
		}

		[DataMember]
		public bool Expanded { get; set; }

		[DataMember]
		public int RowHeight { get; set; }

		[DataMember]
		public bool Visible { get; set; }

	}
}
