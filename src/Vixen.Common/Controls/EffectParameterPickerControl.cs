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

		private PropertyDescriptor _propertyInfo;

		private IEffectModuleDescriptor _effectPropertyInfo;

		public PropertyDescriptor PropertyInfo
		{
			get { return _propertyInfo; }
			set
			{
				_propertyInfo = value;
				if (_propertyInfo != null)
				{
					DisplayName = _propertyInfo.DisplayName;
				}
			}
		}
		public IEffectModuleDescriptor EffectPropertyInfo
		{
			get { return _effectPropertyInfo; }
			set
			{
				_effectPropertyInfo = value;
				if (_propertyInfo != null)
				{
					DisplayName = _effectPropertyInfo.EffectName;
				}
			}
		}

		public String DisplayName
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
