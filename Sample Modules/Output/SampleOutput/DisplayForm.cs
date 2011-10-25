using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace SampleOutput {
	public partial class DisplayForm : Form {
		private byte[] _values = new byte[0];
		private int _across = 1, _down = 1;
		private int _count;
		private int _boxWidth, _boxHeight;
		private SolidBrush _brush;
		private int _outputCount;

		public DisplayForm() {
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
			_brush = new SolidBrush(System.Drawing.Color.Black);
			IntPtr handle = this.Handle;
		}

		public int OutputCount {
			get { return _outputCount; }
			set {
				_outputCount = value;
				_values = new byte[_outputCount];

				if(_outputCount > 0) {
					_across = (int)Math.Sqrt(_outputCount);
					_down = (int)Math.Round((double)_outputCount / _across, MidpointRounding.AwayFromZero);
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

		new public void Show() {
			//if(_values != null)
			//    Array.Clear(_values, 0, _values.Length);

			BeginInvoke(new MethodInvoker(base.Show));
		}

		new public void Hide() {
			BeginInvoke(new MethodInvoker(base.Hide));
		}

		public void UpdateState(Command[] outputStates) {
			Command command;
			for(int i = 0; i < outputStates.Length; i++) {
				command = outputStates[i];
				if(command != null) {
					if(command is Lighting.Monochrome.SetLevel) {
						Level level = (command as Lighting.Monochrome.SetLevel).Level;
						// Value will be 0-100% (0-100)
						_values[i] = (byte)(level * byte.MaxValue / 100);
					}
				} else {
					// Clear the output.
					_values[i] = 0;
				}
			}

			BeginInvoke(new MethodInvoker(Refresh));
		}

		protected override void OnPaint(PaintEventArgs e) {
			e.Graphics.Clear(Color.Black);

			for(int i = 0; i < _values.Length; i++) {
				Color color = Color.FromArgb(_values[i], Color.White);
				_brush.Color = color;
				e.Graphics.FillRectangle(_brush, (i % _across) * (_boxWidth + 1), (i / _across) * (_boxHeight + 1), _boxWidth, _boxHeight);
			}
		}

	}
}
