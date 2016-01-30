using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	[DataContract, Serializable]
	public class Pixel : PreviewPixel
	{
		public Pixel()
		{
		}
		public Pixel(int xPosition, int yPositoin, int zPosition, int pixelSize)
			: base(xPosition, yPositoin, zPosition, pixelSize)
		{
		}

		Guid _pixelId;
		[DataMember]
		public Guid PixelId
		{
			get
			{
				if (_pixelId == Guid.Empty) _pixelId = Guid.NewGuid();
				return _pixelId;
			}
			set { _pixelId = value; }
		}
		[DataMember]

		public new int X { get { return base.X; } set { base.X = value; } }

		[DataMember]
		public new int Y { get { return base.Y; } set { base.Y = value; } }
		[DataMember]
		public new int Z { get { return base.Z; } set { base.Z = value; } }

	}
}
