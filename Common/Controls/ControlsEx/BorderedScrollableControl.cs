using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CommonElements.ControlsEx
{
	/// <summary>
	/// BorderedScrollableControl paints a thin border
	/// </summary>
	[ToolboxItem(false)]
	public class BorderedScrollableControl:ScrollableControl
	{
		//#region ctor
		///// <summary>
		///// constructs a bordered scrollable control with control.dark border
		///// </summary>
		//public BorderedScrollableControl(){}
		//#endregion
		//#region controller
		////get the rectangle where no scrollbar is, but belongs to
		////nc area
		//private Rectangle GetScrollBarCorner(Rectangle bounds)
		//{
		//    return Rectangle.FromLTRB(
		//        bounds.Right - SystemInformation.VerticalScrollBarWidth,
		//        bounds.Bottom - SystemInformation.HorizontalScrollBarHeight,
		//        bounds.Right - 1,
		//        bounds.Bottom - 1);
		//}
		////override wndproc
		//protected override void WndProc(ref Message m)
		//{
		//    if (m.Msg == Win32.WM_NCPAINT)//draw nonclient
		//    {
		//        base.DefWndProc(ref m);
		//        //get dc for nc area
		//        IntPtr hdc = Win32.GetDCEx(m.HWnd, m.WParam, Win32.DCX_INTERSECTRGN | Win32.DCX_WINDOW | Win32.DCX_USESTYLE);
		//        if (hdc == IntPtr.Zero) return;
		//        using (Graphics gr = Graphics.FromHdc(hdc))
		//        {
		//            Rectangle rct = Rectangle.Round(gr.VisibleClipBounds);
		//            rct.Width--; rct.Height--;
		//            //draw border
		//            gr.DrawRectangle(SystemPens.ControlDark, rct);
		//            //draw scrollbar corner
		//            if (this.VScroll && this.HScroll)
		//            {
		//                rct = GetScrollBarCorner(rct);
		//                gr.DrawLine(SystemPens.Control, rct.X, rct.Y, rct.Right, rct.Y);
		//                gr.DrawLine(SystemPens.Control, rct.X, rct.Y, rct.X, rct.Bottom);
		//            }
		//        }
		//        Win32.ReleaseDC(m.HWnd, hdc);
		//        m.Result = IntPtr.Zero;
		//    }
		//    else if (m.Msg == Win32.WM_NCCALCSIZE)//calculate nc area
		//    {
		//        base.DefWndProc(ref m);
		//        if (m.WParam == IntPtr.Zero)//make nc area 2x2 pixel smaller
		//        {
		//            Win32.RECT rct = (Win32.RECT)Marshal.PtrToStructure(m.LParam, typeof(Win32.RECT));
		//            rct.Inflate(-1, -1);
		//            Marshal.StructureToPtr(rct, m.LParam, true);
		//        }
		//        else//make nc area 2x2 pixel smaller
		//        {
		//            Win32.NCCALCSIZE_PARAMS calc =
		//                (Win32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(Win32.NCCALCSIZE_PARAMS));
		//            calc.newbounds.Inflate(-1, -1);
		//            Marshal.StructureToPtr(calc, m.LParam, true);
		//        }
		//        m.Result = new IntPtr(Win32.WVR_REDRAW);
		//    }
		//    base.WndProc(ref m);
		//}
		//#endregion
	}
}
