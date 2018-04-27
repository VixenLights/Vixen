using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Vixen.Sys.Marks
{
	[DataContract]
	public class MarkDecorator:ICloneable
	{
		public MarkDecorator()
		{
			Color = Color.Black;
			IsBold = false;
			IsSolidLine = false;
		}
		[DataMember]
		public bool IsBold { get; set; }

		[DataMember]
		public bool IsSolidLine { get; set; }

		[DataMember]
		public Color Color { get; set; }

		[DataMember]
		public Mode Mode { get; set; }

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return new MarkDecorator()
			{
				Color = Color,
				IsBold = IsBold,
				IsSolidLine = IsSolidLine,
				Mode = Mode
			};
		}

		#endregion
	}
}
