using System;
using System.Collections.Generic;
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
using Common.Controls.Direct2D;

namespace VixenModules.Preview.VixenPreview.Direct2D
{
	/// <summary>
	/// Interaction logic for PreviewWPF.xaml
	/// </summary>
	public partial class PreviewWPF : UserControl
	{
		public PreviewWPF()
		{
			InitializeComponent();
			 
		}

		public AnimatedScene Scene { get { return this.d2dControl.Scene; } set { this.d2dControl.Scene = value; } }

		System.Drawing.Image backgroundImage = null;


		public System.Drawing.Image BackgroundImage
		{
			get
			{
				return backgroundImage;
			}
			set
			{
				backgroundImage = value;
				Scene = new DisplayScene(backgroundImage);

				this.d2dControl.Scene = Scene;
				this.Scene.IsAnimating = true;
			}
		}

	}
}
