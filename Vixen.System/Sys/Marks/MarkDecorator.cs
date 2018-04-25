using System.Drawing;
using System.Runtime.Serialization;

namespace Vixen.Sys.Marks
{
	[DataContract]
	public class MarkDecorator
	{
		[DataMember]
		public bool IsBold { get; set; }

		[DataMember]
		public bool IsSolidLine { get; set; }

		[DataMember]
		public Color Color { get; set; }
	}
}
