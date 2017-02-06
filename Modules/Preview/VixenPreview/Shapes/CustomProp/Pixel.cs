using System;
using System.Runtime.Serialization;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	[DataContract, Serializable]
	public class Pixel 
	{
		public Pixel()
		{
		}
		public Pixel(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		[DataMember]
		public int X { get; set; }

		[DataMember]
		public int Y { get; set; }

		[DataMember]
		public int Z { get; set; }

	}
}
