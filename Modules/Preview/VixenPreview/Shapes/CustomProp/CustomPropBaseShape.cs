using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	[DataContract]
	public class CustomPropBaseShape : PreviewCustom
	{

		public CustomPropBaseShape(PreviewPoint point, Element prop )
			: base(point, null)
		{

			Name = prop.Name;
			
			if (prop.Children.Any())
			{
				prop.Children.ForEach(c =>
				{
					if (!c.IsPixel)
					{
						if (Strings == null)
						{
							Strings = new List<PreviewBaseShape>();
						}
						Strings.Add(new CustomPropBaseShape(point, c));
					}
					else
					{
						c.Pixels.ForEach(p => AddCustomPixel(p));
					}
				});
			}
			else if (prop.Pixels != null && prop.Pixels.Any())
			{
				prop.Pixels.ForEach(p => AddCustomPixel(p));

			}
			IsPixel = prop.IsPixel;
			Layout();
		}

		public void AddCustomPixel(Pixel p)
		{
			var pixel = new CustomPreviewPixel(p.X, p.Y, 0, PixelSize);
			pixel.PixelColor = PixelColor;
			Pixels.Add(pixel);
		}


		[DataMember]
		public override int Top
		{
			get;
			set;
		}


		[DataMember]
		public override int Left
		{
			get;
			set;
		}

		public override int Right
		{
			get { return Pixels.Max(m => m.X); }

		}
		public bool IsPixel { get; set; }
		public override int Bottom
		{
			get { return Pixels.Min(x => x.Y); }

		}

		public override void Match(PreviewBaseShape matchShape)
		{

		}

		public override void Layout()
		{
			foreach (PreviewBaseShape shape in Strings)
			{
				shape.Layout();
			}

			SetPixelZoom();
		}

		public override void SelectDragPoints()
		{

		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{

		}

		public override bool PointInShape(PreviewPoint point)
		{
			foreach (PreviewPixel pixel in Pixels)
			{
				Rectangle r = new Rectangle(pixel.X - (SelectPointSize / 2), pixel.Y - (SelectPointSize / 2), SelectPointSize,
											SelectPointSize);
				if (point.X >= r.X && point.X <= r.X + r.Width && point.Y >= r.Y && point.Y <= r.Y + r.Height)
				{
					return true;
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point = null)
		{

		}

		public override void SelectDefaultSelectPoint()
		{

		}

		public override void MoveTo(int x, int y)
		{

		}

		public override void Resize(double aspect)
		{

		}

		public override void ResizeFromOriginal(double aspect)
		{

		}
	}
}
