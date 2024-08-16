using System.Drawing;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	public class Configuration : BindableBase
	{
		private readonly CustomPropEditorData _data;
		
		public Configuration(CustomPropEditorData data)
		{
			_data = data;
		}

		public Color LightColor
		{
			get => _data.LightColor;
			set
			{
				_data.LightColor = value;
				OnPropertyChanged(nameof(LightColor));
			} 
		}

		public Color SelectedLightColor
		{
			get => _data.SelectedLightColor;
			set
			{
				_data.SelectedLightColor = value;
				OnPropertyChanged(nameof(SelectedLightColor));
			}
		}

		public uint DefaultLightSize
		{
			get => _data.DefaultLightSize; 
			set 
			{
				if(value < 1) value = ElementModel.DefaultLightSize;
				_data.DefaultLightSize = value;
				OnPropertyChanged(nameof(DefaultLightSize));
			}
		}

	}
}
