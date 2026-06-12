using System.Collections.ObjectModel;
using System.Drawing;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	/// <summary>
	/// Stores one State item authored in the Custom Prop Editor.
	/// </summary>
	[Serializable]
	public class StateItemModel : BindableBase
	{
		private Color _color;
		private string _name;

		/// <summary>
		/// The default State item name.
		/// </summary>
		public const string DefaultName = "Item Name 1";

		/// <summary>
		/// Initializes a new instance of the <see cref="StateItemModel"/> class.
		/// </summary>
		public StateItemModel()
		{
			Id = Guid.NewGuid();
			Name = DefaultName;
			Color = Color.White;
			ElementModelIds = new ObservableCollection<Guid>();
		}

		/// <summary>
		/// Gets or sets the stable identifier for this State item.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the user-visible State item name.
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
		/// Gets or sets the color assigned to this State item.
		/// </summary>
		public Color Color
		{
			get => _color;
			set
			{
				if (value.Equals(_color)) return;
				_color = value;
				OnPropertyChanged(nameof(Color));
			}
		}

		/// <summary>
		/// Gets or sets the custom prop element model identifiers assigned to this State item.
		/// </summary>
		public ObservableCollection<Guid> ElementModelIds { get; set; }

		internal void Normalize()
		{
			if (Id == Guid.Empty)
			{
				Id = Guid.NewGuid();
			}

			Name = string.IsNullOrWhiteSpace(Name) ? DefaultName : Name.Trim();
			ElementModelIds ??= new ObservableCollection<Guid>();
		}
	}
}
