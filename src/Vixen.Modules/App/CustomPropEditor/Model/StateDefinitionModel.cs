using System.Collections.ObjectModel;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	/// <summary>
	/// Stores one State definition authored in the Custom Prop Editor.
	/// </summary>
	[Serializable]
	public class StateDefinitionModel : BindableBase
	{
		private string _description;
		private string _name;

		/// <summary>
		/// The default State definition name.
		/// </summary>
		public const string DefaultName = "State - 1";

		/// <summary>
		/// Initializes a new instance of the <see cref="StateDefinitionModel"/> class.
		/// </summary>
		public StateDefinitionModel()
		{
			Id = Guid.NewGuid();
			Name = DefaultName;
			Description = string.Empty;
			Items = new ObservableCollection<StateItemModel>();
		}

		/// <summary>
		/// Creates a default State definition.
		/// </summary>
		/// <param name="name">The user-visible State definition name.</param>
		/// <returns>A State definition with one default State item.</returns>
		public static StateDefinitionModel CreateDefault(string name)
		{
			return new StateDefinitionModel
			{
				Name = string.IsNullOrWhiteSpace(name) ? DefaultName : name.Trim(),
				Items = new ObservableCollection<StateItemModel> { new StateItemModel() }
			};
		}

		/// <summary>
		/// Gets or sets the stable identifier for this State definition.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the user-visible State definition name.
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
		/// Gets or sets the user-visible State definition description.
		/// </summary>
		public string Description
		{
			get => _description;
			set
			{
				if (value == _description) return;
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}

		/// <summary>
		/// Gets or sets the ordered State items in this definition.
		/// </summary>
		public ObservableCollection<StateItemModel> Items { get; set; }

		internal void Normalize()
		{
			if (Id == Guid.Empty)
			{
				Id = Guid.NewGuid();
			}

			Name = string.IsNullOrWhiteSpace(Name) ? DefaultName : Name.Trim();
			Description ??= string.Empty;
			Items ??= new ObservableCollection<StateItemModel>();

			foreach (var item in Items)
			{
				item.Normalize();
			}
		}
	}
}
