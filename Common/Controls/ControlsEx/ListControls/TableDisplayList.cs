using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.ControlsEx.ListControls
{
	/// <summary>
	/// abstract base class for VTableDisplayList and HTableList
	/// </summary>
	public abstract class TableDisplayList : DisplayList
	{
		#region variables

		protected int _lines = 1;
		protected Size _fieldsize = new Size(48, 48);
		//
		protected Rectangle _cachebounds;

		protected int _cacheindex = int.MinValue,
		              _cachecol;

		#endregion

		#region structs

		/// <summary>
		/// struct for holding a line and a column index
		/// </summary>
		protected struct Cell
		{
			public Cell(int row, int column)
			{
				this.Row = row;
				this.Column = column;
			}

			public int Row;
			public int Column;
		}

		#endregion

		#region inheritable

		/// <summary>
		/// override this member to implement calculating of
		/// line and column index from a specified position
		/// </summary>
		protected abstract Cell GetCellAtPosition(int x, int y);

		/// <summary>
		/// override this member to implement calculating of
		/// line and column index from a specified index
		/// </summary>
		protected abstract Cell GetCellAtIndex(int index);

		#endregion

		#region properties

		/// <summary>
		/// specifies the size of an element
		/// </summary>
		[Description("specifies the size of an element"),
		 DefaultValue(typeof (Size), "48;48")]
		public Size FieldSize
		{
			get { return _fieldsize; }
			set
			{
				//limit to maximal/minimal bounds
				value = new Size(
					Math.Max(10, Math.Min(1000, value.Width)),
					Math.Max(8, Math.Min(1000, value.Height)));
				if (value == _fieldsize) return;
				_fieldsize = value;
				//refresh the control
				Reload();
			}
		}

		#endregion
	}

	/// <summary>
	/// VTableList aligns DisplayElements as vertical table
	/// </summary>
	[ToolboxItem(true)]
	public class VTableDisplayList : TableDisplayList
	{
		#region helpers

		/// <summary>
		/// adjusts the scrollbars to content
		/// </summary>
		protected override Size GetTotalSize(Size clientsize, int count)
		{
			_cacheindex = int.MinValue;
			//calculate maximum number of lines which fit the control
			_lines = Math.Max(1, (clientsize.Width - 2)/(base._fieldsize.Width + 1));
			//calculate row count
			int rem, rows = Math.DivRem(count, _lines, out rem);
			if (rem != 0) rows++;
			//if hscroll is visible, recalculate
			if (rows*(base._fieldsize.Height + 1) > clientsize.Height) {
				_lines = Math.Max(1, (clientsize.Width - 2 - SystemInformation.VerticalScrollBarWidth)/
				                     (_fieldsize.Width + 1));
				//calculate row count
				rows = Math.DivRem(count, _lines, out rem);
				if (rem != 0) rows++;
			}
			//update scrollbars
			return new Size(0,
			                1 + rows*(base._fieldsize.Height + 1));
		}

		/// <summary>
		/// gets index of the item at the specified position.
		/// collection boundaries are checked
		/// </summary>
		protected override int GetIndexAt(int x, int y)
		{
			Cell c = this.GetCellAtPosition(x, y);
			return c.Row*_lines + c.Column;
		}

		/// <summary>
		/// gets the bounds of the item at the specified index.
		/// collection boundaries are not checked
		/// </summary>
		protected override Rectangle GetBoundsAt(int index)
		{
			//optimization for drawing
			if (index == _cacheindex + 1) {
				_cacheindex = index;
				_cachebounds.X += base._fieldsize.Width + 1;
				if (_cachecol >= _lines) {
					_cachecol = 0;
					_cachebounds.X = 1;
					_cachebounds.Y += base._fieldsize.Height + 1;
				}
			}
			_cacheindex = index;
			Cell c = this.GetCellAtIndex(index);
			_cachecol = c.Column;
			return _cachebounds = GetBoundsAtCell(c);
		}

		protected override void GetDrawingInterval(Rectangle clip, out int start, out int stop)
		{
			Cell startcell = GetCellAtPosition(clip.X, clip.Y);
			stop = GetIndexAt(clip.Right, clip.Bottom);
			start = startcell.Row*_lines + startcell.Column;
		}

		#region cell functions

		/// <summary>
		/// gets the bounds of the item at the specified
		/// line and column index.
		/// collection boundaries are not checked
		/// </summary>
		private Rectangle GetBoundsAtCell(Cell c)
		{
			return new Rectangle(
				1 + c.Column*(base._fieldsize.Width + 1),
				1 + c.Row*(base._fieldsize.Height + 1),
				base._fieldsize.Width,
				base._fieldsize.Height);
		}

		/// <summary>
		/// calculates a line and a column index from the
		/// specified position.
		/// collection boundaries are not checked
		/// </summary>
		protected override Cell GetCellAtPosition(int x, int y)
		{
			return new Cell(
				Math.Max(0, y/(base._fieldsize.Height + 1)), //row
				Math.Max(0, Math.Min(_lines - 1, x/(base._fieldsize.Width + 1)))); //column
		}

		/// <summary>
		/// calculates a line and a column index from the
		/// specified index.
		/// collection boundaries are not checked
		/// </summary>
		protected override Cell GetCellAtIndex(int index)
		{
			int col, row = Math.DivRem(index, _lines, out col);
			return new Cell(
				Math.Max(0, row),
				Math.Max(0, Math.Min(_lines - 1, col)));
		}

		#endregion

		#endregion
	}
}