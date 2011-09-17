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
    public partial class _595_Form : Form {
        private bool[] _values = new bool[0];
        private int _across = 1, _down = 1;
        private int _count;
        private int _boxWidth, _boxHeight;
        private SolidBrush _brush;

        public _595_Form() {
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
				if (value == 0)
					value = 1;
                _values = new bool[value];
                _across = (int)Math.Sqrt(value);
                _down = (int)Math.Round((double)value / _across, MidpointRounding.AwayFromZero);
                _boxWidth = (ClientRectangle.Width / _across) - 1;
                _boxHeight = (ClientRectangle.Height / _down) - 1;
            }
        }

        new public void Show() {
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

        private double _fps;
        public void UpdateState(double fps, Command[] outputStates) {
            _count++;
            _fps = fps;

            Command command;
            for(int i=0; i<outputStates.Length; i++) {
                command = outputStates[i];
                // If there is no command to update state from, command will be non-null,
                // but the members will be null because Command is a value type, a struct.
				if(!command.IsEmpty) {
					//*** need a better mechanism that accounts for multiple platforms and categories
					if(command.CommandIdentifier.Platform == CommandStandard.Standard.Lighting.Value &&
						command.CommandIdentifier.Category == CommandStandard.Standard.Lighting.Monochrome.Value &&
						command.CommandIdentifier.CommandIndex == CommandStandard.Standard.Lighting.Monochrome.SetLevel.Value) {
						if(command.ParameterValues.Length > 0) {
							//_values[i] = (command.ParameterValues[0] as DoubleParameterValue).Value > 0;
							_values[i] = (Level)command.ParameterValues[0] > 0;
						}
					}
				//} else if(command.IsValid) {
				} else {
					// Clear output
					_values[i] = false;
				}
				// Else leave the state as-is.
            }
            if(InvokeRequired) {
                IAsyncResult result = BeginInvoke(new MethodInvoker(Refresh));
            } else {
                Refresh();
            }
            //IAsyncResult result = BeginInvoke(new MethodInvoker(Refresh));
        }

        // For-fun implementation
        //protected override void OnPaint(PaintEventArgs e) {
        //    //Not taking any optimizing steps to see how it does.
        //    byte value;
        //    Color color;
        //    e.Graphics.Clear(Color.Black);
        //    for(int i = 0; i < _values.Length; i++) {
        //        if(_values[i]) {
        //            value = (byte)Math.Max(0, _count - i);
        //            color = Color.FromArgb(value, value, value);
        //        } else {
        //            color = Color.Black;
        //        }
        //        _brush.Color = color;
        //        e.Graphics.FillRectangle(_brush, (i % _across) * (_boxWidth + 1), (i / _across) * (_boxHeight + 1), _boxWidth, _boxHeight);
        //    }
        //    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        //    e.Graphics.DrawString(string.Format("{0:F2} fps", _fps), Font, Brushes.Red, 2, 2);
        //}
        protected override void OnPaint(PaintEventArgs e) {
            //Not taking any optimizing steps to see how it does.
			System.Drawing.Color color;
			e.Graphics.Clear(System.Drawing.Color.Black);
            for(int i = 0; i < _values.Length; i++) {
                if(_values[i]) {
					color = System.Drawing.Color.White;
                } else {
					color = System.Drawing.Color.Black;
                }
                _brush.Color = color;
                e.Graphics.FillRectangle(_brush, (i % _across) * (_boxWidth + 1), (i / _across) * (_boxHeight + 1), _boxWidth, _boxHeight);
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawString(string.Format("{0:F2} fps", _fps), Font, Brushes.Red, 2, 2);
        }

    }
}
