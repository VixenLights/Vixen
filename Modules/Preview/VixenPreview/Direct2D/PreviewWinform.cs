using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Direct2D;

namespace VixenModules.Preview.VixenPreview.Direct2D
{
	public partial class PreviewWinform : UserControl
	{
		public PreviewWinform()
		{
			InitializeComponent();
		}
		public AnimatedScene Scene
		{
			get { return previewWPF1.Scene; }
			set
			{
				if (previewWPF1.Scene == null)
					previewWPF1.Scene = new DisplayScene(null);
				previewWPF1.Scene = value;
			}
		}
	}
}
