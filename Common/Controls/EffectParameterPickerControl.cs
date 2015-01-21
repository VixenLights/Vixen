using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls
{
	public partial class EffectParameterPickerControl : UserControl
	{

		public EffectParameterPickerControl()
		{
			InitializeComponent();
		}

		public string ParameterName
		{
			get { return labelParameterName.Text; }
			set { labelParameterName.Text = value; }
		}

		public int ParameterIndex { get; set; }

		
		/// <summary>
		/// The index of the value when the target parameter is a List
		/// </summary>
		public int ParameterListIndex { get; set; }

		public Bitmap ParameterImage
		{
			set { pictureParameterImage.Image = value; }
		}

		private void pictureParameterImage_Click(object sender, EventArgs e)
		{
			OnClick(e);
		}

		private void labelParameterName_Click(object sender, EventArgs e)
		{
			OnClick(e);
		}
	}
}
