using System.Drawing;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	[Serializable]
	public class StateDefinition: BindableBase
	{
		private Color _defaultColor;
		private int _index;

		public Color DefaultColor
		{
			get => _defaultColor;
			set
			{
				if (value.Equals(_defaultColor)) return;
				_defaultColor = value;
				OnPropertyChanged(nameof(DefaultColor));
			}
		}

		public int Index
		{
			get => _index;
			set
			{
				if (value == _index) return;
				_index = value;
				OnPropertyChanged(nameof(Index));
			}
		}
	}
}
