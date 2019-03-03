using System.Windows.Media;
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

		public Brush LightColor
		{
			get => _data.LightColor;
			set
			{
				_data.LightColor = value;
				OnPropertyChanged(nameof(LightColor));
			} 
		}

		public Brush SelectedLightColor
		{
			get => _data.SelectedLightColor;
			set
			{
				_data.SelectedLightColor = value;
				OnPropertyChanged(nameof(SelectedLightColor));
			}
		}

	}
}
