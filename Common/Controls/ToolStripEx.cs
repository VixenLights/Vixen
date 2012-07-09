using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls
{
	/// <summary>
	/// This class adds on to the functionality provided in System.Windows.Forms.ToolStrip.
	/// </summary>
	public class ToolStripEx : ToolStrip
	{

		private bool clickThrough = true;

		/// <summary>
		/// Gets or sets whether the ToolStripEx honors item clicks when its containing form does
		/// not have input focus.
		/// </summary>
		public bool ClickThrough
		{
			get { return this.clickThrough; }
			set { this.clickThrough = value; }
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (this.clickThrough &&
				m.Msg == NativeConstants.WM_MOUSEACTIVATE &&
				m.Result == (IntPtr)NativeConstants.MA_ACTIVATEANDEAT) {
				m.Result = (IntPtr)NativeConstants.MA_ACTIVATE;
			}
		}
	}

	internal sealed class NativeConstants
	{
		private NativeConstants()
		{
		}

		internal const uint WM_MOUSEACTIVATE = 0x21;
		internal const uint MA_ACTIVATE = 1;
		internal const uint MA_ACTIVATEANDEAT = 2;
		internal const uint MA_NOACTIVATE = 3;
		internal const uint MA_NOACTIVATEANDEAT = 4;
	}
}








