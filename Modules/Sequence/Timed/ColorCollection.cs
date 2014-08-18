using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;


namespace VixenModules.Sequence.Timed
{
	[DataContract]
	public class ColorCollection: IEquatable<ColorCollection>
	{
		public ColorCollection()
		{
			Name = "Default";
			Color = new List<Color>();
		}

		public ColorCollection(ColorCollection original)
		{
			Name = original.Name;
			Color = new List<Color>(original.Color);
		}

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public List<Color> Color { get; set; }

		public override string ToString()
		{
			return Name;
		}

		public bool Equals(ColorCollection other)
		{
			return this.Name.ToLower() == other.Name.ToLower();
		}
	}
}
