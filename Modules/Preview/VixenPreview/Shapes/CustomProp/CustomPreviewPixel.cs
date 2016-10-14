using System;
using System.Runtime.Serialization;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	[DataContract, Serializable]
	public class CustomPreviewPixel: PreviewPixel
	{
		public CustomPreviewPixel(int xPosition, int yPositoin, int zPosition, int pixelSize):base(xPosition, yPositoin, zPosition, pixelSize)
		{
			OriginX = xPosition;
			OriginY = yPositoin;
		}

		[DataMember]
		public int OriginX { get; set; }

		[DataMember]
		public int OriginY { get; set; }
	}
}
