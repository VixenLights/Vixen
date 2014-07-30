using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Resources;
using System.Reflection;


namespace VixenModules.Preview.VixenPreview.Shapes
{
	internal class PreviewTools
	{
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

		// 
		// Add the root nodes to the Display Element tree
		//
		public static void PopulateElementTree(TreeView tree)
		{
			foreach (ElementNode channel in VixenSystem.Nodes.GetRootNodes()) {
				AddNodeToElementTree(tree.Nodes, channel);
			}
		}

		// 
		// Add each child Display Element or Display Element Group to the tree
		// 
		private static void AddNodeToElementTree(TreeNodeCollection collection, ElementNode channelNode)
		{
			TreeNode addedNode = new TreeNode();
			addedNode.Name = channelNode.Id.ToString();
			addedNode.Text = channelNode.Name;
			addedNode.Tag = channelNode;

			collection.Add(addedNode);

			foreach (ElementNode childNode in channelNode.Children) {
				AddNodeToElementTree(addedNode.Nodes, childNode);
			}
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
			if (degrees <= 180) {
				radianIncrement = Math.PI/(numPoints - 1);
				// watch out for rounding on the fp adds
				for (double t = 0; t <= totalRadians+radianIncrement/10; t += radianIncrement) {
					double X = C_x + (Width/2)*Math.Cos(t) + leftOffset;
					double Y = C_y + (Height/2)*Math.Sin(t) + topOffset;
					points.Add(new Point((int) X, (int) Y));
				}
				return points;
			}
			else {
				//radianIncrement = (Math.PI * 2) / totalPoints;
				//for (double t = 0; t < totalRadians; t += radianIncrement)
				//{
				//    double X = (C_x + (Width / 2) * Math.Cos(t)) + leftOffset;
				//    double Y = (C_y + (Height / 2) * Math.Sin(t)) + topOffset;
				//    points.Add(new Point((int)X, (int)Y));
				//}
				double radianOffset = (degreeOffset*Math.PI)/180;
				double startRadian = radianOffset;
				double endRadian = totalRadians + radianOffset;
				radianIncrement = (Math.PI*2)/totalPoints;
				// watch out for rounding on the fp adds
				for (double t = startRadian; t < endRadian + radianIncrement / 10; t += radianIncrement)
				{
					double X = (C_x + (Width/2)*Math.Cos(t)) + leftOffset;
					double Y = (C_y + (Height/2)*Math.Sin(t)) + topOffset;
					points.Add(new Point((int) X, (int) Y));
				}
				return points;
			}
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

		public static DisplayItem DeSerializeToObject(string st, Type type)
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

		public static List<ElementNode> GetLeafNodes(ElementNode node)
		{
			List<ElementNode> children = new List<ElementNode>();
			foreach (ElementNode child in node.Children) {
				if (child.IsLeaf)
					children.Add(child);
			}
			return children;
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
	}
}