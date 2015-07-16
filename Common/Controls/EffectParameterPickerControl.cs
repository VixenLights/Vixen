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

		private PropertyDescriptor _propertyInfo;

		public PropertyDescriptor PropertyInfo
		{
			get { return _propertyInfo; }
			set
			{
				_propertyInfo = value;
				if (_propertyInfo != null)
				{
					labelParameterName.Text = _propertyInfo.DisplayName;
				}
			}
		}

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
