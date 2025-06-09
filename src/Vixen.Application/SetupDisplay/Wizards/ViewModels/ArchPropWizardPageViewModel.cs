using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	public class ArchPropWizardPageViewModel : WizardPageViewModelBase<ArchPropWizardPage>
	{
		public ArchPropWizardPageViewModel(ArchPropWizardPage wizardPage) : base(wizardPage)
		{

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
			set { SetValue(NodeCountProperty, value); }
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