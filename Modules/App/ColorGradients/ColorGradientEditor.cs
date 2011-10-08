using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.ColorGradients
{
	public partial class ColorGradientEditor : Form
	{
		public ColorGradientEditor(ColorGradient gradient)
		{
			InitializeComponent();
			gradientEditPanel.GradientChanged += GradientChangedHandler;
		}

		public bool Modified { get; internal set; }

		public ColorGradient Gradient
		{
			get
			{
				return gradientEditPanel.Gradient;
			}
			set
			{
				gradientEditPanel.Gradient = value;
				Modified = false;
			}
		}

		public void GradientChangedHandler(object sender, EventArgs e)
		{
			Modified = true;
		}
	}
}
