using System.ComponentModel;
using Vixen.Module.Effect;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class EffectParameterPickerControl : UserControl
	{

		public EffectParameterPickerControl()
		{
			InitializeComponent();
		}

		private PropertyMetaData _propertyInfo;

		private IEffectModuleDescriptor _effectPropertyInfo;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PropertyMetaData PropertyInfo
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public String DisplayName
		{
			set { labelParameterName.Text = value; } 
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap ParameterImage
		{
			set { pictureParameterImage.Image = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Index { get; set; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string GroupName { get; set; } = string.Empty;

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
