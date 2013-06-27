using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Common.Controls.ControlsEx.ListControls
{
	public interface ILayout
	{
		/// <summary>
		/// gets the size of <pre>count</pre> elements
		/// within the bounds of <pre>clientsize</pre>
		/// </summary>
		Size GetTotalSize(Size size, int count);

		/// <summary>
		/// gets the selected index at the given postion
		/// </summary>
		int GetIndexAt(Point pt);

		/// <summary>
		/// gets the bounds at the selected index
		/// </summary>
		Rectangle GetBoundsAt(int index);

		/// <summary>
		/// gets the interval of elements that
		/// are in the given clip rectangle
		/// </summary>
		void GetDrawingInterval(Rectangle clip, out int start, out int stop);

		event EventHandler Changed;
	}
}