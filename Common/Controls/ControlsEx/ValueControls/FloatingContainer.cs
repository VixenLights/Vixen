using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.ControlsEx.ValueControls
{
	/// <summary>
	/// encapsulates a popup form
	/// </summary>
	[ToolboxItem(false)]
	public class PopupForm : Form
	{
		#region types

		/// <summary>
		/// subclass for the mainform of an application,
		/// needed for proper functionality of the popup form
		/// </summary>
		private class ActivationFilter : NativeWindow
		{
			#region variables

			private PopupForm _owner;

			#endregion

			/// <summary>
			/// ctor
			/// </summary>
			public ActivationFilter(PopupForm owner)
			{
				if (owner == null)
					throw new ArgumentNullException("owner");
				this._owner = owner;
			}

			#region public members

			public void AssignHandle(Form mainform)
			{
				if (mainform == null)
					throw new ArgumentNullException("mainform");
				if (this.Handle != IntPtr.Zero)
					this.ReleaseHandle();
				if (mainform.IsHandleCreated)
					this.AssignHandle(mainform.Handle);
			}

			#endregion

			#region controller

			protected override void WndProc(ref Message m)
			{
				if (m.Msg == Win32.WM_NCACTIVATE) {
					if (m.WParam.ToInt32() == 0)
						m.WParam = new IntPtr(1);
				}
				else if (m.Msg == Win32.WM_ACTIVATEAPP) {
					if (m.WParam.ToInt32() == 0)
						_owner.Close();
				}
				base.WndProc(ref m);
			}

			#endregion
		}

		/// <summary>
		/// filter for interrupting events that
		/// cause the closing of the popup form
		/// </summary>
		private class MouseFilter : IMessageFilter
		{
			#region variables

			private PopupForm _owner;

			#endregion

			/// <summary>
			/// ctor
			/// </summary>
			public MouseFilter(PopupForm owner)
			{
				if (owner == null)
					throw new ArgumentNullException("owner");
				this._owner = owner;
			}

			#region controller

			bool IMessageFilter.PreFilterMessage(ref Message m)
			{
				switch (m.Msg) {
					case Win32.WM_LBUTTONDOWN:
					case Win32.WM_MBUTTONDOWN:
					case Win32.WM_RBUTTONDOWN:
					case Win32.WM_NCLBUTTONDOWN:
					case Win32.WM_NCMBUTTONDOWN:
					case Win32.WM_NCRBUTTONDOWN:
						ProcessMouseDown();
						break;
				}
				return false;
			}

			/// <summary>
			/// checks if the cursor is over the client area
			/// and closes the container, if not
			/// </summary>
			private void ProcessMouseDown()
			{
				if (!_owner.Bounds.Contains(Control.MousePosition))
					_owner.Close();
			}

			#endregion
		}

		#endregion

		#region variables

		private ActivationFilter _activationfilter;
		private MouseFilter _mousefilter;

		#endregion

		/// <summary>
		/// ctor
		/// </summary>
		public PopupForm()
		{
			this.ControlBox = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.StartPosition = FormStartPosition.Manual;
			this.ShowInTaskbar = false;
			//filters
			_mousefilter = new MouseFilter(this);
			_activationfilter = new ActivationFilter(this);
		}

		#region public members

		public void ShowByControl(Control ctl, Point screenpos)
		{
			if (ctl == null)
				throw new ArgumentNullException("ctl");
			this.Location = screenpos;
			//filters
			Application.AddMessageFilter(_mousefilter);
			Form mainfrm = ctl.FindForm();
			if (mainfrm != null)
				_activationfilter.AssignHandle(mainfrm);
			base.Show();
		}

		#endregion

		#region controller

		//don't allow closing, hide instead
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			base.OnClosing(e);
			this.Hide();
			//
			Application.RemoveMessageFilter(_mousefilter);
			_activationfilter.ReleaseHandle();
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ClassStyle |= 0x20000; //drop shadow
				cp.Style = 0x800000;
				cp.Style |= -2147483648; //popup window
				return cp;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == Win32.WM_NCPAINT) {
				base.DefWndProc(ref m);
				IntPtr hdc = Win32.GetWindowDC(m.HWnd);
				if (hdc == IntPtr.Zero) return;
				Graphics gr = Graphics.FromHdc(hdc);
				ControlPaint.DrawBorder3D(gr, 0, 0, this.Width, this.Height,
				                          Border3DStyle.RaisedInner, Border3DSide.All);
				gr.Dispose();
				Win32.ReleaseDC(m.HWnd, hdc);
				m.Result = IntPtr.Zero;
			}
			else
				base.WndProc(ref m);
		}

		#endregion
	}
}