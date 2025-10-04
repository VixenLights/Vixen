using Catel.Data;
using Catel.MVVM;

using Vixen.Sys.Props;

using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	/// <summary>
	/// Maintains a tree prop wizard page view model.
	/// </summary>
	public class TreePropWizardPageViewModel : GraphicsWizardPageViewModelBase<TreePropWizardPage, VixenModules.App.Props.Models.Tree.TreeModel>, IPropWizardPageViewModel
	{
		public TreePropWizardPageViewModel(TreePropWizardPage wizardPage) : base(wizardPage)
		{			
		}
		
		/// <summary>
		/// Refreshes the 3-D OpenGL graphics.
		/// </summary>
		private void RefreshGraphics()
		{
			// Pass the properties needed to draw the graphics to the temporary light prop model
			LightPropModel.Strings = Strings;
			LightPropModel.NodesPerString = NodesPerString;

			// Update the prop nodes
			LightPropModel.UpdatePropNodes();
		}

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel]
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

		#endregion

		#region Strings property

		/// <summary>
		/// Gets or sets the Strings value.
		/// </summary>
		[ViewModelToModel]
		public int Strings
		{
			get { return GetValue<int>(StringsProperty); }
			set
			{
				SetValue(StringsProperty, value);
				RefreshGraphics();
			}
		}

		/// <summary>
		/// NodeCount property data.
		/// </summary>
		public static readonly IPropertyData StringsProperty = RegisterProperty<int>(nameof(Strings));

		#endregion

		#region NodesPerString property

		/// <summary>
		/// Gets or sets the NodesPerString value.
		/// </summary>
		[ViewModelToModel]
		public int NodesPerString
		{
			get { return GetValue<int>(NodesPerStringProperty); }
			set
			{
				SetValue(NodesPerStringProperty, value);
				RefreshGraphics();
			}
		}

		/// <summary>
		/// NodesPerString property data.
		/// </summary>
		public static readonly IPropertyData NodesPerStringProperty = RegisterProperty<int>(nameof(NodesPerString));

		#endregion

		#region StringType property

		/// <summary>
		/// Gets or sets the StringType value.
		/// </summary>
		[ViewModelToModel]
		public StringTypes StringType
		{
			get { return GetValue<StringTypes>(StringTypeProperty); }
			set { SetValue(StringTypeProperty, value); }
		}

		/// <summary>
		/// StringType property data.
		/// </summary>
		public static readonly IPropertyData StringTypeProperty = RegisterProperty<StringTypes>(nameof(StringType));

		#endregion

		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			base.ValidateFields(validationResults);

			if (string.IsNullOrWhiteSpace(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError(nameof(Name), "Name is required"));
			}

			if (Strings <= 0)
			{
				validationResults.Add(
					FieldValidationResult.CreateError(nameof(Strings), "String Count must be greater than 0"));
			}

			if (NodesPerString <= 0)
			{
				validationResults.Add(FieldValidationResult.CreateError(nameof(NodesPerString),
					"Nodes per string must be greater than 0"));

			}
		}
	}
}