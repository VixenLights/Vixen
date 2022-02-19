using System.Drawing;
using System.Runtime.Serialization;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	[DataContract]
	public class ColorBreakdownItem
	{
		private string _name;

		public ColorBreakdownItem()
		{
			Color = Color.White;
			Name = "Unnamed";
		}

		[DataMember]
		public Color Color { get; set; }

		[DataMember]
		public string Name
		{
			get => _name;
			set => _name = string.Intern(value);
		}
	}
}
