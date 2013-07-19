using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Concurrent;
using Vixen.Sys;
using System.Threading;

namespace Common.Controls.Direct2D {
	public partial class Direct2DControlWinForm : UserControl {
		Image backgroundImage = null;
		public new Image BackgroundImage {
			get {
				return backgroundImage;

			}
			set {
				backgroundImage = value;
				direct2DControlWPF1.BackgroundImage = backgroundImage;

			}
		}
		public List<DisplayScene.DisplayPoint> Points { get; set; }

		public bool Paused { get { return direct2DControlWPF1.Paused; } set { direct2DControlWPF1.Paused = value; } }


		public void WritePoints(List<DisplayScene.DisplayPoint> Points) {
		 
			 
			direct2DControlWPF1.WritePoints(Points);

		}


		public Direct2DControlWinForm() {
			Points = new List<DisplayScene.DisplayPoint>();
			InitializeComponent();
		}
	}
}
