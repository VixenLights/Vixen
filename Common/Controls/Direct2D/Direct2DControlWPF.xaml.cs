using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Common.Controls.Direct2D {
	/// <summary>
	/// Interaction logic for Direct2DControlWPF.xaml
	/// </summary>
	/// <summary>
	/// Interaction logic for Direct2DControlWPF.xaml
	/// </summary>
	public partial class Direct2DControlWPF : UserControl {
		public DisplayScene Scene { get; set; }
		public Direct2DControlWPF() {

			InitializeComponent();

			Scene = new DisplayScene(BackgroundImage);
			this.d2dControl.Scene = Scene;
			this.Scene.IsAnimating = true;

		}

		System.Drawing.Image backgroundImage = null;

		[Bindable(true)]
		public System.Drawing.Image BackgroundImage {
			get {
				return backgroundImage;
			}
			set {
				backgroundImage = value;
				Scene = new DisplayScene(backgroundImage);

				this.d2dControl.Scene = Scene;
				this.Scene.IsAnimating = true;
			}
		}

		[Bindable(true)]
		public bool Paused { get { return !this.Scene.IsAnimating; } set { this.Scene.IsAnimating = !value; } }


		public void WritePoints(List<DisplayScene.DisplayPoint> points) {
			Scene.WritePoints(points);
		}

	}
}
