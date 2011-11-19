using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	class ItemBlock : BlockLayoutEngine.IBlockLayoutAdapter {
		private Rectangle _rectangle;

		public ItemBlock(int top, int height) {
			_rectangle = new Rectangle();
			Top = top;
			Height = height;
		}

		public int Top {
			get { return _rectangle.Y; }
			set { _rectangle.Y = value; }
		}

		public int Left {
			get { return _rectangle.X; }
			set { _rectangle.X = value; }
		}

		public int Width {
			get { return _rectangle.Width; }
			set { _rectangle.Width = value; }
		}

		public int Height {
			get { return _rectangle.Height; }
			set { _rectangle.Height = value; }
		}

		public bool IntersectsWith(BlockLayoutEngine.IBlockLayoutAdapter block) {
			return _rectangle.IntersectsWith(new Rectangle(block.Left, block.Top, block.Width, block.Height));
		}
	}
}
