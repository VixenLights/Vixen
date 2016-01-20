using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 


namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public class PropDisplayPanel: Panel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public Square[,] Squares{ get; set; }

		public PropDisplayPanel() {
			this.DoubleBuffered = true;
			this.ResizeRedraw = true;

			Squares = new Square[X, Y];
			for (int y = 0; y < Y; ++y)
			{
				for (int x = 0; x < X; ++x)
				{
					Squares[x, y] = new Square()
				}
			}
		}
	}
}
