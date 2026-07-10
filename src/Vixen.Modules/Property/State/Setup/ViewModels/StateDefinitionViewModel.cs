using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using VixenModules.Property.State.Setup.Models;

namespace VixenModules.Property.State.Setup.ViewModels
{
	/// <summary>
	/// Provides editable values for one State definition.
	/// </summary>
	public sealed class StateDefinitionViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateDefinitionViewModel"/> class.
		/// </summary>
		/// <param name="definition">The State definition draft to edit.</param>
		/// <param name="elementTree">The element hierarchy available for assignment.</param>
		public StateDefinitionViewModel(StateDefinitionData definition, StateElementNodeSnapshot elementTree)
		{
			ArgumentNullException.ThrowIfNull(definition);
			ArgumentNullException.ThrowIfNull(elementTree);

			DeferValidationUntilFirstSaveCall = false;
			Definition = definition;
			Items = new ObservableCollection<StateItemViewModel>(
				definition.Items.Select(item => new StateItemViewModel(item, elementTree)));
			Validate(true);
		}

		[Model]
		internal StateDefinitionData Definition
		{
			get => GetValue<StateDefinitionData>(DefinitionProperty);
			private set => SetValue(DefinitionProperty, value);
		}

		private static readonly IPropertyData DefinitionProperty = RegisterProperty<StateDefinitionData>(nameof(Definition));

		/// <summary>
		/// Gets or sets the user-visible State definition name.
		/// </summary>
		/// <value>The user-visible State definition name.</value>
		[ViewModelToModel(nameof(Definition))]
		public string Name
		{
			get => GetValue<string>(NameProperty);
			set
			{
				var normalizedName = value?.Trim() ?? string.Empty;
				SetValue(NameProperty, normalizedName);
			}
		}

		/// <summary>
		/// Identifies the <see cref="Name"/> property.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name), string.Empty);

		/// <summary>
		/// Gets or sets the user-provided State definition description.
		/// </summary>
		/// <value>The user-provided State definition description.</value>
		[ViewModelToModel(nameof(Definition))]
		public string Description
		{
			get => GetValue<string>(DescriptionProperty);
			set => SetValue(DescriptionProperty, value);
		}

		/// <summary>
		/// Identifies the <see cref="Description"/> property.
		/// </summary>
		public static readonly IPropertyData DescriptionProperty = RegisterProperty<string>(nameof(Description), string.Empty);

		/// <summary>
		/// Gets the editable State item rows.
		/// </summary>
		/// <value>The editable State item rows.</value>
		public ObservableCollection<StateItemViewModel> Items { get; }

		internal StateDefinitionData ToData()
		{
			return new StateDefinitionData
			{
				Id = Definition.Id,
				Name = Name,
				Description = Description,
				Items = Items.Select(item => item.Item.Clone()).ToList()
			};
		}

		/// <inheritdoc />
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError(NameProperty, "State definition name is required."));
			}
			else if (Name.Length < 3)
			{
				validationResults.Add(FieldValidationResult.CreateWarning(NameProperty, "State definition names shorter than three characters may be unclear."));
			}
		}
	}
}
