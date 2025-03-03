using System.Drawing.Imaging;
using Vixen.Sys;
using System.Runtime.Serialization;
using System.Windows.Media;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Catel.Reflection;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	internal class PreviewTools
	{

        //AgentFire: Better approach (you can rename the struct if you need):
        public struct Vector2
        {
            public readonly double X;
            public readonly double Y;
            public Vector2(Point p)
                : this(p.X, p.Y)
            {
            }

            public Vector2(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
            public static Vector2 operator -(Vector2 a, Vector2 b)
            {
                return new Vector2(b.X - a.X, b.Y - a.Y);
            }
            public static Vector2 operator +(Vector2 a, Vector2 b)
            {
                return new Vector2(b.X + a.X, b.Y + a.Y);
            }
            public static Vector2 operator *(Vector2 a, double d)
            {
                return new Vector2(a.X * d, a.Y * d);
            }
            public static Vector2 operator /(Vector2 a, double d)
            {
                return new Vector2(a.X / d, a.Y / d);
            }

            public static implicit operator Point(Vector2 a)
            {
                return new Point((int)a.X, (int)a.Y);
            }

            public Vector2 UnitVector
            {
                get { return this / Length; }
            }

            public double Length
            {
                get
                {
                    double aSq = Math.Pow(X, 2);
                    double bSq = Math.Pow(Y, 2);
                    return Math.Sqrt(aSq + bSq);
                }
            }

            public override string ToString()
            {
                return string.Format("[{0}, {1}]", X, Y);
            }
        }

		public static System.Object renderLock = new System.Object();
		public static Color SelectedItemColor = Color.LimeGreen;
		public static Color HighlightedElementColor = Color.Pink;

		public static T ParseEnum<T>(string value)
		{
			return (T) Enum.Parse(typeof (T), value, true);
		}

		public static void ComboBoxSetSelectedText(ComboBox comboBox, string text)
		{
			int i = comboBox.FindString(text);
			if (i >= 0)
				comboBox.SelectedIndex = i;
		}

		private static double Perimeter(PreviewPoint p1, PreviewPoint p2)
		{
			double p;
			double a;
			double b;
			a = Math.Abs(p2.Y - p1.Y);
			b = Math.Abs(p2.X - p1.X);
			p = Math.PI*((3*(a + b)) - Math.Sqrt((3*a + b)*(a + 3*b)));
			return p;
		}

		public static List<Point> GetArcPoints(double Width, double Height, double NumPoints)
		{
			List<Point> points = new List<Point>();
			double degrees = 180;
			double C_x = Width/2;
			double C_y = Height;
			double totalRadians = ((degrees*2)*Math.PI)/180;
			double radianIncrement = Math.PI/(NumPoints - 1);

			double t = Math.PI;
			while (points.Count < NumPoints) {
				//Console.WriteLine("NumPoints: " + NumPoints + "; points.Count: " + points.Count);
				//for (double t = Math.PI; t <= (Math.PI*2)+1; t += radianIncrement)
				//{
				double X = C_x + (Width/2)*Math.Cos(t);
				double Y = C_y + (Height)*Math.Sin(t);
				points.Add(new Point((int) X, (int) Y));
				//}
				//radianIncrement += .001;
				t += radianIncrement;
			}
			return points;
		}

		public static List<Point> GetEllipsePoints(
			double leftOffset,
			double topOffset,
			double Width,
			double Height,
			double totalPoints,
			double degrees,
			double degreeOffset)
		{
			//const double C_x = 10, C_y = 20, w = 40, h = 50;
			//for (double t = 0; t <= 2 * pi; t += 0.01)
			//{
			//    double X = C_x + (w / 2) * cos(t);
			//    double Y = C_y + (h / 2) * sin(t);
			//    // Do what you want with X & Y here 
			//}
			List<Point> points = new List<Point>();


			//double degrees = 360;
			double totalRadians = (degrees*Math.PI)/180;
			double numPoints = (totalPoints/2);
			double C_x = Width/2;
			double C_y = Height/2;
			double radianIncrement;
			double radianOffset = (degreeOffset * Math.PI) / 180;
			double startRadian = radianOffset;
			double endRadian = totalRadians + radianOffset;

			if (degrees <= 180)
			{
				radianIncrement = Math.PI / (numPoints - 1);
			}
			else
			{
				radianIncrement = (Math.PI * 2) / totalPoints;
			}

			// watch out for rounding on the fp adds
			for (double t = startRadian; t < endRadian + radianIncrement / 10; t += radianIncrement)
			{
				double X = (C_x + (Width / 2) * Math.Cos(t)) + leftOffset;
				double Y = (C_y + (Height / 2) * Math.Sin(t)) + topOffset;
				points.Add(new Point((int)X, (int)Y));
			}
			return points;
		}

		/// <summary>
		/// Calculates the angle of the screenPoint point to the center point, relative to the Y-axis
		/// </summary>
		/// <param name="screenPoint">Specifies target point</param>
		/// <param name="center">Specifies the source point on the Y-axis</param>
		/// <returns>The angle of the point relative to the Y-axis</returns>
		public static int GetAngle(Point screenPoint, PreviewPoint center)
		{
			int dx = screenPoint.X - center.X;
			int dy = screenPoint.Y - center.Y;

			double inRads = Math.Atan2(dy, dx);

			// Convert from radians and adjust the angle from the 0 at 9:00 positon.
			return (int)(inRads * (180 / Math.PI) + 270) % 360;
		}

		public enum RotateTypes
		{
			Clockwise,
			Counterclockwise,
			Base0,
			Base180,
			None
		}

		/// <summary>
		/// Remap the Point, within the shape, based up the rotation and zoom levels
		/// </summary>
		/// <param name="shape">Target shape used as the basis for the remap</param>
		/// <param name="point">Specific Point coordinate to remap</param>
		/// <param name="zoomLevel">Amount of zooming. A negative zoom indicates a converse zoom impact</param>
		/// <param name="direction">Specifies if the rotation is clockwise or counterwise</param>
		/// <returns>Remapped Point</returns>
		/// <exception cref="ArgumentNullException">No rotation axis was defined</exception>
		public static PreviewPoint TransformPreviewPoint(PreviewBaseShape shape, PreviewPoint point, double zoomLevel = 1, RotateTypes direction = RotateTypes.Clockwise)
		{
			PreviewPoint returnPoint = null;
			double zoomToPhysical = 1;
			double zoomToRelative = 1;

			// If zoomLevel < 0, then we really want to do the converse of the current zoom level
			if (zoomLevel < 0)
				zoomToPhysical = -1 / zoomLevel;
			else
				zoomToRelative = zoomLevel;

			System.Windows.Point swPoint;
			int rotation;
			if (direction == RotateTypes.Clockwise)
				rotation = shape.RotationAngle;
			else if (direction == RotateTypes.Counterclockwise)
				rotation = -shape.RotationAngle;
			else if (direction == RotateTypes.Base0)
				rotation = 0;
			else if (direction == RotateTypes.Base180)
				rotation = 180;
			else
				rotation = 0;

			// Are we in the process of rotating the object
			if (shape?._selectedPoint?.PointType == PreviewPoint.PointTypes.RotateHandle)
			{
				PreviewPoint delta = new(0,0);

				// First we need to see if the object needs to be translated in the X-Y plane.
				// So, find where the new center would be
				shape._selectedPoint.PointType = PreviewPoint.PointTypes.None;
				var newCenter = PreviewTools.TransformPreviewPoint(shape, new PreviewPoint(shape.Center), 1);
				shape._selectedPoint.PointType = PreviewPoint.PointTypes.RotateHandle;

				// Check to see if any translation is needed.
				if (newCenter.X != shape.Center.X || newCenter.Y != shape.Center.Y)
				{
					// Calculate the X/Y translation
					PreviewPoint newTopLeft = new PreviewPoint(newCenter.X - (shape.Right - shape.Left) / 2, newCenter.Y - (shape.Bottom - shape.Top) / 2);
					delta.X = newTopLeft.X - shape.Left;
					delta.Y = newTopLeft.Y - shape.Top;

					// Adjust all points to match the new center
					foreach (var adjustPoint in shape._selectPoints)
					{
						adjustPoint.X += delta.X;
						adjustPoint.Y += delta.Y;
					}

					//Move the rotation axis to the current center
					shape.RotationAxis.X = shape.Center.X;
					shape.RotationAxis.Y = shape.Center.Y;
				}

				// Finally, calculate the new point, by rotating around the rotation axis, which is the new center
				var xyPlane = new RotateTransform(rotation, shape.RotationAxis.X, shape.RotationAxis.Y);
				swPoint = xyPlane.Transform(new System.Windows.Point(point.X, point.Y));
				if (zoomToPhysical == 1)
					returnPoint = new PreviewPoint(Math.Round(swPoint.X * zoomToRelative, 0).CastToInt32(), Math.Round(swPoint.Y * zoomToRelative, 0).CastToInt32());
				else
					returnPoint = new PreviewPoint(Math.Round(swPoint.X * zoomToPhysical, 0).CastToInt32(), Math.Round(swPoint.Y * zoomToPhysical, 0).CastToInt32());
			}

			// If there's no rotation, then just map the point soley based upon the zoom level
			else if (rotation == 0)
			{
				if (zoomToPhysical == 1)
					returnPoint = new PreviewPoint(Math.Round(point.X * zoomToRelative, 0).CastToInt32(), Math.Round(point.Y * zoomToRelative, 0).CastToInt32());
				else
					returnPoint = new PreviewPoint(Math.Round(point.X * zoomToPhysical, 0).CastToInt32(), Math.Round(point.Y * zoomToPhysical, 0).CastToInt32());
			}

			// Else map the point based upon the rotation and zoom level
			else if (shape?.RotationAxis != null)
			{
				// Rotate around the rotation axis
				var xyPlane = new RotateTransform(rotation, shape.RotationAxis.X, shape.RotationAxis.Y);

				// Convert the point's physical coordinates to it's relative coordinates within the rotated coordinate system
				swPoint = xyPlane.Transform(new System.Windows.Point(point.X * zoomToPhysical, point.Y * zoomToPhysical));
				returnPoint = new PreviewPoint(Math.Round(swPoint.X * zoomToRelative, 0).CastToInt32(), Math.Round(swPoint.Y * zoomToRelative, 0).CastToInt32());
			}
			else
			{
				throw new ArgumentNullException($"No rotation axis defined.  Name: {shape?.Name}  Shape: {shape?.TypeName}  Top: {shape?.Top}  Left: {shape?.Left}");
			}

			returnPoint.PointType = point.PointType;
			return returnPoint;
		}

		/// <summary>
		/// Reset the Rotation Axis
		/// </summary>
		/// <param name="shape">Target shape used as the basis for the rotation</param>
		public static void TransformPreviewPoint(PreviewBaseShape shape)
		{
			if (shape?.RotationAxis != null)
			{
				shape.RotationAxis.X = shape.Center.X;
				shape.RotationAxis.Y = shape.Center.Y;
			}
		}

		/// <summary>
		/// Calculate the amount of rotation of the shape
		/// </summary>
		/// <param name="shape">Specifies the shape to rotate</param>
		/// <param name="basePoint">Specifies the point, relative to the center of the shape, that defines the angle of rotation</param>
		/// <param name="zoomLevel">Amount of zooming. A negative zoom indicates a converse zoom impact</param>
		/// <param name="useDetents">Specifies if the rotation angle should be normalized to the nearest 45 degrees</param>
		/// <returns></returns>
		public static int CalculateRotation(PreviewBaseShape shape, Point basePoint, double zoomLevel, bool useDetents = false)
		{
			// Check to see if the shape is inverted (i.e. the Topleft is above the BottomRight).  If so, then set the rotational
			// origin to 180 degrees since all the calulations need to be rotated by 180 degrees.
			var topleft = shape._selectPoints.Find(x => x.PointType == PreviewPoint.PointTypes.SizeTopLeft);
			RotateTypes _rotateTargetAxis = RotateTypes.Base0;
			if ( topleft?.Y > shape.Center.Y)
				_rotateTargetAxis = RotateTypes.Base180;

			// Reset the rotational axis to the center of the object
			shape.RotationAxis.X = shape.Center.X;
			shape.RotationAxis.Y = shape.Center.Y;

			// Calculate the rotational geometry
			var point = PreviewTools.TransformPreviewPoint(shape, new PreviewPoint(basePoint.X, basePoint.Y), -zoomLevel, _rotateTargetAxis);
			double angle = PreviewTools.GetAngle(shape.RotationAxis.ToPoint(), point);

			// Use Detents of 0, 45, 90, 135, 180, 225, 270 and 315 when holding the Ctrl modifier key down.
			if (useDetents)
			{
				if (angle >= 22.5 && angle < 67.5) angle = 45;
				else if (angle >= 67.5 && angle < 112.5) angle = 90;
				else if (angle >= 112.5 && angle < 157.5) angle = 135;
				else if (angle >= 157.5 && angle < 202.5) angle = 180;
				else if (angle >= 202.5 && angle < 247.5) angle = 225;
				else if (angle >= 247.5 && angle < 292.5) angle = 270;
				else if (angle >= 292.5 && angle < 337.5) angle = 315;
				else if (angle >= 337.5 || angle < 22.5) angle = 0;
			}

			return (int)Math.Round(angle, MidpointRounding.AwayFromZero);
		}

		public static string SerializeToString(object obj)
		{
			var serializer = new DataContractSerializer(obj.GetType());
			using (var backing = new System.IO.StringWriter())
			using (var writer = new System.Xml.XmlTextWriter(backing)) {
				serializer.WriteObject(writer, obj);
				return backing.ToString();
			}
		}

		public static DisplayItem DeSerializeToDisplayItem(string st, Type type)
		{
			var serializer = new DataContractSerializer(type);
			DisplayItem item = null;
			using (var backing = new System.IO.StringReader(st)) {
				try {
					using (var reader = new System.Xml.XmlTextReader(backing)) {
						item = serializer.ReadObject(reader) as DisplayItem;
					}
				}
				catch {
				}
			}
			return item;
		}

        public static List<DisplayItem> DeSerializeToDisplayItemList(string st)
        {
            List<DisplayItem> result = new List<DisplayItem>();
            var serializer = new DataContractSerializer(result.GetType());
            using (var backing = new System.IO.StringReader(st))
            {
                try
                {
                    using (var reader = new System.Xml.XmlTextReader(backing))
                    {
                        result = serializer.ReadObject(reader) as List<DisplayItem>;
                    }
                }
                catch
                {  
                    // We're not going to do anything. If we get here, the result list should be empty, which is fine.
                }
            }
            return result;
        }

        public static Bitmap ResizeBitmap(Bitmap imgToResize, Size size)
        {
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }

                return b;
            }
            catch
            {
                throw;
            }
        }
 
		public static Bitmap Copy32BPPBitmapSafe(Bitmap srcBitmap)
		{
			Bitmap result = new Bitmap(srcBitmap.Width, srcBitmap.Height, PixelFormat.Format32bppArgb);

			Rectangle bmpBounds = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
			BitmapData srcData = srcBitmap.LockBits(bmpBounds, ImageLockMode.ReadOnly, srcBitmap.PixelFormat);
			BitmapData resData = result.LockBits(bmpBounds, ImageLockMode.WriteOnly, result.PixelFormat);

			Int64 srcScan0 = srcData.Scan0.ToInt64();
			Int64 resScan0 = resData.Scan0.ToInt64();
			int srcStride = srcData.Stride;
			int resStride = resData.Stride;
			int rowLength = Math.Abs(srcData.Stride);
			try {
				byte[] buffer = new byte[rowLength];
				for (int y = 0; y < srcData.Height; y++) {
					System.Runtime.InteropServices.Marshal.Copy(new IntPtr(srcScan0 + y*srcStride), buffer, 0, rowLength);
					System.Runtime.InteropServices.Marshal.Copy(buffer, 0, new IntPtr(resScan0 + y*resStride), rowLength);
				}
			}
			finally {
				srcBitmap.UnlockBits(srcData);
				result.UnlockBits(resData);
			}

			return result;
		}

		//
		//
		// Vixen does not allow unsafe compilation!
		//
		//
		//public unsafe static Bitmap Clone32BPPBitmap(Bitmap srcBitmap)
		//{
		//    Bitmap result = new Bitmap(srcBitmap.Width, srcBitmap.Height, PixelFormat.Format32bppArgb);

		//    Rectangle bmpBounds = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
		//    BitmapData srcData = srcBitmap.LockBits(bmpBounds, ImageLockMode.ReadOnly, srcBitmap.PixelFormat);
		//    BitmapData resData = result.LockBits(bmpBounds, ImageLockMode.WriteOnly, result.PixelFormat);

		//    int* srcScan0 = (int*)srcData.Scan0;
		//    int* resScan0 = (int*)resData.Scan0;
		//    int numPixels = srcData.Stride / 4 * srcData.Height;
		//    try
		//    {
		//        for (int p = 0; p < numPixels; p++)
		//        {
		//            resScan0[p] = srcScan0[p];
		//        }
		//    }
		//    finally
		//    {
		//        srcBitmap.UnlockBits(srcData);
		//        result.UnlockBits(resData);
		//    }

		//    return result;
		//}

        /// <summary>
        /// Returns the number of child nodes that do not have children
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
		public static List<ElementNode> GetLeafNodes(ElementNode node)
		{
			List<ElementNode> children = new List<ElementNode>();
			foreach (ElementNode child in node.Children) {
				if (child.IsLeaf)
					children.Add(child);
			}
			return children;
		}

        public static List<ElementNode> GetParentNodes(ElementNode node)
        {
            List<ElementNode> children = new List<ElementNode>();
            foreach (ElementNode child in node.Children)
            {
                if (!child.IsLeaf)
                    children.Add(child);
            }
            return children;
        }

        /// <summary>
        /// Retruns the number of strings and pixels in an ElementNode
        /// </summary>
        public static void CountPixelsAndStrings(ElementNode ParentNode, out int Pixels, out int Strings)
        {
            Pixels = 0;
            Strings = 0;
            foreach (ElementNode child in ParentNode.Children)
            {
                if (child.IsLeaf)
                {
                    Pixels++;
                }
                else
                {
                    Strings++;
                }
            }
        }

		public static string TemplateFolder
		{
			get
			{
				string destFolder = System.IO.Path.Combine(VixenPreviewDescriptor.ModulePath, "Templates");
				if (!System.IO.Directory.Exists(destFolder)) {
					System.IO.Directory.CreateDirectory(destFolder);
				}
				return destFolder;
			}
		}

		public static string TemplateWithFolder(string templateName)
		{
			return System.IO.Path.Combine(TemplateFolder, templateName);
		}

        public static Point CalculatePointOnLine(Vector2 a, Vector2 b, int distance)
        {
            Vector2 vectorAB = a - b;

            return a + vectorAB.UnitVector * distance;
        }

        public static double TriangleLeg(double hypotenuseLength, double leg1Length)
        {
            return Math.Sqrt(Math.Pow(hypotenuseLength, 2) - Math.Pow(leg1Length, 2));
        }

        public static double TriangleHypotenuse(double leg1Length, double leg2Length)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(leg1Length), 2) + Math.Pow(Math.Abs(leg2Length), 2));
        }
	}
}