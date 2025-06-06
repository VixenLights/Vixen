using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
    public class ArchPropWizardPageViewModel: WizardPageViewModelBase<ArchPropWizardPage>
    {
        public ArchPropWizardPageViewModel(ArchPropWizardPage wizardPage) : base(wizardPage)
        {
        }

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

		[ViewModelToModel]
		public int LightCount
        {
            get { return GetValue<int>(LightCountProperty); }
            set { SetValue(LightCountProperty, value); }
        }

        /// <summary>
        /// Name property data.
        /// </summary>
        public static readonly IPropertyData LightCountProperty = RegisterProperty<int>(nameof(LightCount));

		[ViewModelToModel]
		public StringTypes StringType
        {
            get { return GetValue<StringTypes>(StringTypeProperty); }
            set { SetValue(StringTypeProperty, value); }
        }

        /// <summary>
        /// Name property data.
        /// </summary>
        public static readonly IPropertyData StringTypeProperty = RegisterProperty<StringTypes>(nameof(StringType));

		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            base.ValidateFields(validationResults);

            if (string.IsNullOrWhiteSpace(Name))
            {
                validationResults.Add(FieldValidationResult.CreateError("Name", "Name is required"));
            }

            if (LightCount <= 0)
            {
                validationResults.Add(FieldValidationResult.CreateError("NodeCount", "Light Count must be greater than 0"));
            }
        }
	}
}