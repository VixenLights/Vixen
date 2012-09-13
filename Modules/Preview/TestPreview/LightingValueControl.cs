using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.Preview.TestPreview {
	public partial class LightingValueControl : UserControl {
		private float _intensity;
		private Rectangle _intensityRect;

		public LightingValueControl() {
			InitializeComponent();
			_intensityRect = new Rectangle(25, 7, 67, 9);
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
		}

		public IIntentState<LightingValue> IntentState {
			set {
				LightingValue lightingValue = value.GetValue();
				pictureBoxColor.BackColor = lightingValue.Color;
				_intensity = lightingValue.Intensity;
				Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs e) {
			float separationPoint = _intensityRect.Width * _intensity;
			e.Graphics.FillRectangle(Brushes.White, _intensityRect.X, _intensityRect.Y, separationPoint, _intensityRect.Height);
			e.Graphics.FillRectangle(Brushes.Black, _intensityRect.X + separationPoint, _intensityRect.Y, _intensityRect.Width - separationPoint, _intensityRect.Height);
		}
	}
}
