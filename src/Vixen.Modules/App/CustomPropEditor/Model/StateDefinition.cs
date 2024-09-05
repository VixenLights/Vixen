using System.Drawing;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	[Serializable]
	public class StateDefinition: BindableBase
	{
		private Color _defaultColor;
		private int _index;
		private string _name;
		private string _stateDefinitionName;

		/// <summary>
		/// State Definition defines a particular state of a group of nodes. They are grouped together by the StateDefinitionName property.
		/// Each state can have a color and index assigned to it. Thus, a prop can have several parts grouped together and each part can have its own color and index.
		/// </summary>
		public StateDefinition()
		{
			StateDefinitionName = "State Name 1";
			Name = "Item 1";
			DefaultColor = Color.White;
		}

		/// <summary>
		/// The overall StateDefinitionName Key that this definition is grouped with. 
		/// </summary>
		public string StateDefinitionName
		{
			get => _stateDefinitionName;
			set
			{
				if (value == _stateDefinitionName) return;
				_stateDefinitionName = value;
				OnPropertyChanged(nameof(StateDefinitionName));
			}
		}

		/// <summary>
		/// The individual state definition item
		/// </summary>
		public string Name
		{
			get => _name;
			set
			{
				if (value == _name) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		/// <summary>
		/// The defined color of this state item
		/// </summary>
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

		/// <summary>
		/// This is basically the row number when imported from an xModel. This is not used at this time.
		/// </summary>
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
