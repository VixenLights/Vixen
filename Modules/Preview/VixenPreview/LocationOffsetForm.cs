using System;
using System.Windows.Media.Media3D;
using Common.Controls;
using Common.Controls.Theme;

namespace VixenModules.Preview.VixenPreview
{
	public partial class LocationOffsetForm : BaseForm
	{
		public LocationOffsetForm(Vector3D offset)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			txtX.Text = offset.X.ToString();
			txtY.Text = offset.Y.ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Offset = new Vector3D(txtX.IntValue, txtY.IntValue, 0);
		}

		public Vector3D Offset { get; set; }
	}
}
