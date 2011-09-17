using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;

namespace TestOutput {
	public partial class RenardForm : Form {
		private byte[] _values = new byte[0];
		private int _across = 1, _down = 1;
		private int _count;
		private int _boxWidth, _boxHeight;
		private SolidBrush _brush;

		public RenardForm() {
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
			_brush = new SolidBrush(System.Drawing.Color.Black);
			// If the form has been created but not shown, the handle may not yet be
			// created.  If that's the case, then calls to InvokeRequired may return false
			// when it should really return true.
			IntPtr handle = this.Handle;
		}

		public int OutputCount {
			set {
				_values = new byte[value];
				if(value > 0) {
					_across = (int)Math.Sqrt(value);
					_down = (int)Math.Round((double)value / _across, MidpointRounding.AwayFromZero);
					_boxWidth = (ClientRectangle.Width / _across) - 1;
					_boxHeight = (ClientRectangle.Height / _down) - 1;
				} else {
					_across = 0;
					_down = 0;
					_boxWidth = 0;
					_boxHeight = 0;
				}
			}
		}

		private List<byte> _levels = new List<byte>();
		private List<Level> _rawlevels = new List<Level>();
		private List<double> _doublelevels = new List<double>();
		new public void Show() {
			//ms.Clear();
			_levels.Clear();
			_rawlevels.Clear();
			_doublelevels.Clear();
			// Reset state.
			_count = 0;
			Array.Clear(_values, 0, _values.Length);
			if(InvokeRequired) {
				BeginInvoke(new MethodInvoker(base.Show));
			} else {
				base.Show();
			}
		}

		new public void Hide() {
			if(InvokeRequired) {
				BeginInvoke(new MethodInvoker(base.Hide));
			} else {
				base.Hide();
			}
		}

		//List<int> ms = new List<int>();
		private double _fps;
		public void UpdateState(double fps, Command[] outputStates) {
			_count++;
			_fps = fps;

			Command command;
			for(int i = 0; i < outputStates.Length; i++) {
				command = outputStates[i];
				// If there is no command to update state from, command will be non-null,
				// but the members will be null because Command is a value type, a struct.
				if(!command.IsEmpty) {
					//*** need a better mechanism that accounts for multiple platforms and categories
					if(command.CommandIdentifier.Platform == CommandStandard.Standard.Lighting.Value &&
						command.CommandIdentifier.Category == CommandStandard.Standard.Lighting.Monochrome.Value &&
						command.CommandIdentifier.CommandIndex == CommandStandard.Standard.Lighting.Monochrome.SetLevel.Value) {
						if(command.ParameterValues.Length > 0) {
							// Value will be 0-100% (0-100)
							_rawlevels.Add((Level)command.ParameterValues[0]);
							_doublelevels.Add((Level)command.ParameterValues[0] * byte.MaxValue / 100);
							_values[i] = (byte)((Level)command.ParameterValues[0] * byte.MaxValue / 100);
							//if(_values[i] != 0) ms.Add(DateTime.Now.Millisecond);
							_levels.Add(_values[i]);
						}
					}
				//} else if(command.IsValid) {
				} else {
					// Clear the output.
					_values[i] = 0;
				}
				// Else leave state as-is.
			}
			if(InvokeRequired) {
				IAsyncResult result = BeginInvoke(new MethodInvoker(Refresh));
			} else {
				Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs e) {
			//Not taking any optimizing steps to see how it does.
			System.Drawing.Color color;
			e.Graphics.Clear(System.Drawing.Color.Black);
			for(int i = 0; i < _values.Length; i++) {
				color = System.Drawing.Color.FromArgb(_values[i], System.Drawing.Color.White);
				_brush.Color = color;
				e.Graphics.FillRectangle(_brush, (i % _across) * (_boxWidth + 1), (i / _across) * (_boxHeight + 1), _boxWidth, _boxHeight);
			}
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			e.Graphics.DrawString(string.Format("{0:F2} fps", _fps), Font, Brushes.Red, 2, 2);
		}

	}
}
