using Catel.Data;
using Catel.MVVM;

using Vixen.Sys.Props;

using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	public class ArchPropWizardPageViewModel : GraphicsWizardPageViewModelBase<ArchPropWizardPage, VixenModules.App.Props.Models.Arch.ArchModel>, IPropWizardPageViewModel
	{
		public ArchPropWizardPageViewModel(ArchPropWizardPage wizardPage) : base(wizardPage)
		{					
		}

		/// <summary>
		/// Refreshes the 3-D OpenGL graphics.
		/// </summary>
		private void RefreshGraphics()
		{
			// Pass the properties needed to draw the graphics to the temporary light prop model
			LightPropModel.NodeCount = NodeCount;

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

		#region NodeCount property

		/// <summary>
		/// Gets or sets the NodeCount value.
		/// </summary>
		[ViewModelToModel]
		public int NodeCount
		{
			get { return GetValue<int>(NodeCountProperty); }
			set 
			{ 
				SetValue(NodeCountProperty, value);
				RefreshGraphics();
			}
		}

		/// <summary>
		/// NodeCount property data.
		/// </summary>
		public static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(NodeCount));

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
				validationResults.Add(FieldValidationResult.CreateError("Name", "Name is required"));
			}

			if (NodeCount <= 0)
			{
				validationResults.Add(FieldValidationResult.CreateError("NodeCount", "Node Count must be greater than 0"));
			}
		}
	}
}