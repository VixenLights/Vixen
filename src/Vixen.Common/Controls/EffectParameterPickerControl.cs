using System.ComponentModel;
using Vixen.Module.Effect;

namespace Common.Controls
{
	public partial class EffectParameterPickerControl : UserControl
	{

		public EffectParameterPickerControl()
		{
			InitializeComponent();
		}

		private PropertyDetail _propertyDetail;

		public PropertyDetail PropertyDetail
		{
			get { return _propertyDetail; }
			set
			{
				_propertyDetail = value;
				if (_propertyDetail != null)
				{
					DisplayName = _propertyDetail.Name;
				}
			}
		}

		public string DisplayName
		{
			set { labelParameterName.Text = value; } 
		}

		public Bitmap ParameterImage
		{
			set { pictureParameterImage.Image = value; }
		}

		public int Index { get; set; }

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
