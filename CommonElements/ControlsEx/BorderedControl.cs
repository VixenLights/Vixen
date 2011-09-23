using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ControlsEx
{
	/// <summary>
	/// Bordered control paintsa thin border width the specified pen
	/// </summary>
	[ToolboxItem(false)]
	public class BorderedControl:Control
	{
		#region variables
		private bool _border=true;
		#endregion
		#region ctor
		/// <summary>
		/// constructs a bordered scrollable control with control.dark border
		/// </summary>
		public BorderedControl(){}
		#endregion
		#region controller
		//override wndproc
		//protected override void WndProc(ref Message m)
		//{
		//    if(_border)
		//    {
		//        if (m.Msg==Win32.WM_NCPAINT)//draw nonclient
		//        {
		//            base.DefWndProc(ref m);
		//            //get dc for nc area
		//            IntPtr hdc=Win32.GetDCEx(m.HWnd,m.WParam,Win32.DCX_WINDOW|Win32.DCX_USESTYLE);
		//            if (hdc!=IntPtr.Zero)
		//            {
		//                using (Graphics gr=Graphics.FromHdc(hdc))
		//                {
		//                    Rectangle rct=Rectangle.Round(gr.VisibleClipBounds);
		//                    rct.Width--;rct.Height--;
		//                    //draw border
		//                    gr.DrawRectangle(SystemPens.ControlDark,rct);
		//                }
		//                Win32.ReleaseDC(m.HWnd,hdc);
		//                m.Result=IntPtr.Zero;
		//            }
		//        }
		//        else if (m.Msg == Win32.WM_NCCALCSIZE)//calculate nc area
		//        {
		//            base.DefWndProc(ref m);
		//            if (m.WParam == IntPtr.Zero)//make nc area 2x2 pixel smaller
		//            {
		//                Win32.RECT rct = (Win32.RECT)Marshal.PtrToStructure(m.LParam, typeof(Win32.RECT));
		//                rct.Inflate(-1, -1);
		//                Marshal.StructureToPtr(rct, m.LParam, true);
		//            }
		//            else//make nc area 2x2 pixel smaller
		//            {
		//                Win32.NCCALCSIZE_PARAMS calc =
		//                    (Win32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(Win32.NCCALCSIZE_PARAMS));
		//                calc.newbounds.Inflate(-1, -1);
		//                Marshal.StructureToPtr(calc, m.LParam, true);
		//            }
		//            m.Result = new IntPtr(Win32.WVR_REDRAW);
		//        }
		//    }
		//    base.WndProc (ref m);
		//}
		#endregion
		#region properties
		/// <summary>
		/// gets or sets whether the thin border is enabled
		/// </summary>
		[Description("gets or sets whether the thin border is enabled"),
		DefaultValue(true)]
		public bool Border
		{
			get{return _border;}
			set
			{
				if(value==_border) return;
				_border=value;
				this.RecreateHandle();
			}
		}
		#endregion
	}
}
