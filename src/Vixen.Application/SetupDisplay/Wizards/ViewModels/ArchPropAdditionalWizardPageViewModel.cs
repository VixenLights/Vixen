using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
    public class ArchPropAdditionalWizardPageViewModel: WizardPageViewModelBase<ArchPropAdditionalWizardPage>
	{
		public ArchPropAdditionalWizardPageViewModel(ArchPropAdditionalWizardPage wizardPage) : base(wizardPage)
		{
		}


		#region Optional Left / Right property
		/// <summary>
		/// Gets or sets if the wizard will additional generate a left and right group.
		/// </summary>
		[ViewModelToModel]
		public bool LeftRight
		{
			get { return GetValue<bool>(leftRightProperty); }
			set { SetValue(leftRightProperty, value); }
		}
		private static readonly IPropertyData leftRightProperty = RegisterProperty<bool>(nameof(LeftRight));
		#endregion

		/// <summary>
		/// Performs validation on the properties.
		/// </summary>
		/// <param name="validationResults"></param>
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			base.ValidateFields(validationResults);

		}
	}
}
