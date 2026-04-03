using Common.Controls;
using Common.Controls.Theme;
using System.ComponentModel;
using System.Windows.Media.Media3D;

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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Vector3D Offset { get; set; }
	}
}
