using System.ComponentModel;

namespace Common.Controls.ControlsEx.ListControls
{
	/// <summary>
	/// abstract base class for VDisplayList, HDisplayList
	/// </summary>
	public abstract class SingleLineDisplayList : DisplayList
	{
		#region variables

		protected int _aspect = 80;
		protected Size _fieldsize = new Size(10, 8);
		//
		protected Rectangle _cachebounds;
		protected int _cacheindex = int.MinValue;

		#endregion

		#region properties

		/// <summary>
		/// specifies the aspect rate every item has, except if scrollbars are visible
		/// </summary>
		[Description("specifies the aspect rate every item has, except if scrollbars are visible"),
		 DefaultValue(80)]
		public int Aspect
		{
			get { return _aspect; }
			set
			{
				value = Math.Max(5, Math.Min(2000, value));
				if (value == _aspect) return;
				_aspect = value;
				AutoScrollMinSize = GetTotalSize(
					ClientSize, Items.Count);
				if (Items.Count > 0)
					Refresh();
			}
		}

		#endregion
	}

	/// <summary>
	/// HDisplayList displays DisplayElement in a horizontal row
	/// </summary>
	[ToolboxItem(true)]
	public class HDisplayList : SingleLineDisplayList
	{
		#region overrides

		/// <summary>
		/// adjusts the scrollbars to content
		/// </summary>
		protected override Size GetTotalSize(Size clientsize, int count)
		{
			_cacheindex = int.MinValue;
			//item covers full clientheight (without border, scrollbar)
			_fieldsize.Height = Math.Max(10, clientsize.Height - 2);
			//width is calculated by aspect rate
			_fieldsize.Width = Math.Max(10, ((clientsize.Height - 4)*100)/_aspect);
			//width
			int w = 1 + count*(_fieldsize.Width + 1);
			if (w > clientsize.Width)
				_fieldsize.Height = Math.Max(10,
				                             _fieldsize.Height - SystemInformation.HorizontalScrollBarHeight);
			//
			return new Size(w, 0);
		}

		/// <summary>
		/// returns the index of the item at the specified position.
		/// collection boundaries are checked
		/// </summary>
		protected override int GetIndexAt(int x, int y)
		{
			return x/(_fieldsize.Width + 1);
		}

		/// <summary>
		/// returns the bounds of the item at the specified position.
		/// collection boundaries are not checked.
		/// </summary>
		protected override Rectangle GetBoundsAt(int index)
		{
			//optimization for drawing
			if (index == _cacheindex + 1) {
				_cacheindex = index;
				_cachebounds.X += _fieldsize.Width + 1;
				return _cachebounds;
			}
			_cacheindex = index;
			return _cachebounds = new Rectangle(
			                      	(index*(_fieldsize.Width + 1)) + 1, 1,
			                      	_fieldsize.Width, _fieldsize.Height);
		}

		protected override void GetDrawingInterval(Rectangle clip, out int start, out int stop)
		{
			start = GetIndexAt(clip.X, 0);
			stop = GetIndexAt(clip.Right, 0);
		}

		#endregion
	}

	/// <summary>
	/// VDisplayList displays DisplayElement in a vertical row
	/// </summary>
	[ToolboxItem(true)]
	public class VDisplayList : SingleLineDisplayList
	{
		#region overrides

		/// <summary>
		/// adjusts the scrollbars to content
		/// </summary>
		protected override Size GetTotalSize(Size clientsize, int count)
		{
			_cacheindex = int.MinValue;
			//item covers full clientwidth (without border, scrollbar)
			_fieldsize.Width = Math.Max(10, clientsize.Width - 2);
			//calculate height by aspect rate
			_fieldsize.Height = Math.Max(10, ((clientsize.Width - 4)*_aspect)/100);
			//height
			int h = 1 + count*(_fieldsize.Height + 1);
			if (h > clientsize.Height)
				_fieldsize.Width = Math.Max(10,
				                            _fieldsize.Width - SystemInformation.VerticalScrollBarWidth);
			//
			return new Size(0, h);
		}

		/// <summary>
		/// gets the index of the item at the specified position.
		/// collection boundaries are checked.
		/// </summary>
		protected override int GetIndexAt(int x, int y)
		{
			return y/(_fieldsize.Height + 1);
		}

		/// <summary>
		/// returns the bounds of the item at the specified position.
		/// collection boundaries are not checked.
		/// </summary>
		protected override Rectangle GetBoundsAt(int index)
		{
			//optimization for drawing
			if (index == _cacheindex + 1) {
				_cacheindex = index;
				_cachebounds.Y += _fieldsize.Height + 1;
				return _cachebounds;
			}
			_cacheindex = index;
			return _cachebounds = new Rectangle(
			                      	1, (index*(_fieldsize.Height + 1)) + 1,
			                      	_fieldsize.Width, _fieldsize.Height);
		}

		protected override void GetDrawingInterval(Rectangle clip, out int start, out int stop)
		{
			start = GetIndexAt(0, clip.Y);
			stop = GetIndexAt(0, clip.Bottom);
		}

		#endregion
	}
}