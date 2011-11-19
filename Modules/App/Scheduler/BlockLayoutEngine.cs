using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	class BlockLayoutEngine {
		public void Layout(IEnumerable<IBlockLayoutAdapter> blocks, Rectangle container) {
			List<IBlockLayoutAdapter> placedBlocks = new List<IBlockLayoutAdapter>();
			List<IBlockLayoutAdapter> intersectingBlocks = new List<IBlockLayoutAdapter>();

			int minX;
			int x;
			int width;
			foreach(IBlockLayoutAdapter block in blocks) {
				// Initially set block to the full display width.
				block.Left = container.Left;
				block.Width = container.Width;

				//control.Top = (int)control.Item.RunStartTime.TotalMinutes / 30 * parent.HalfHourHeight;
				//control.Height = (int)(control.Item.RunEndTime - control.Item.RunStartTime).TotalMinutes / 30 * parent.HalfHourHeight;
				////(testing)
				//control.Height = Math.Max(control.Height, 10);

				if(placedBlocks.Count > 0) {
					minX = container.Right;
					intersectingBlocks.Clear();

					// Iterate placed blocks, find all blocks that intersect
					foreach(IBlockLayoutAdapter placedBlock in placedBlocks) {
						if(placedBlock.IntersectsWith(block)) {
							intersectingBlocks.Add(placedBlock);
							minX = Math.Min(placedBlock.Left, minX);
						}
					}
					// If the min of all intersecting blocks = rect.X, recalc all intersected
					// blocks and set the new block size, location appropriately.
					if(minX == container.X) {
						x = container.X;
						width = container.Width / (intersectingBlocks.Count + 1);
						foreach(IBlockLayoutAdapter intersectingBlock in intersectingBlocks) {
							intersectingBlock.Left = x;
							intersectingBlock.Width = width;
							x += width;
						}
						block.Left = x;
						block.Width = width;
					} else {
						block.Width = minX - container.X;
					}
				}

				placedBlocks.Add(block);
			}
		}
		//class BlockLayoutAdapter<T>
		//    where T : class {
		//    public BlockLayoutAdapter(T item) {
		//        Item = item;
		//        Rectangle = new Rectangle();
		//    }

		//    public T Item { get; private set; }
		//    public Rectangle Rectangle;
		//}
		public interface IBlockLayoutAdapter {
			int Top { get; set; }
			int Left { get; set; }
			int Width { get; set; }
			int Height { get; set; }
			bool IntersectsWith(IBlockLayoutAdapter block);
		}
	}
}
