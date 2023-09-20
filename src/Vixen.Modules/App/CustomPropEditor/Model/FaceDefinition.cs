using System.Drawing;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	[Serializable]
	public class FaceDefinition: BindableBase
	{
		private FaceComponent _faceComponent;
		private Color _defaultColor;

		public FaceDefinition()
		{
			FaceComponent = FaceComponent.None;
			DefaultColor = Color.White;
		}

		public FaceComponent FaceComponent
		{
			get => _faceComponent;
			set
			{
				if (value == _faceComponent) return;
				_faceComponent = value;
				OnPropertyChanged(nameof(FaceComponent));
			}
		}

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
	}
}
