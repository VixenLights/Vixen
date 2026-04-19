using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Vixen.Sys.Props.Model;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	/// <summary>
	/// Maintains a base wizard page specific to a prop type.
	/// </summary>
	/// <typeparam name="TWizardPage">Type of wizard page</typeparam>
	/// <typeparam name="TPropModel">Type of prop model</typeparam>
	public class PropBaseWizardPageViewModel<TWizardPage, TPropModel> : GraphicsWizardPageViewModelBase<TWizardPage, TPropModel>
		where TWizardPage : class, IWizardPage
		where TPropModel : class, IPropModel, new()
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <typeparam name="TWizardPage">Type of wizard page</typeparam>
		public PropBaseWizardPageViewModel(TWizardPage wizardPage) : base(wizardPage)
		{
		}

		#endregion

		#region Name Property

		/// <summary>
		/// Gets or sets the prop name.
		/// </summary>
		[ViewModelToModel]
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		
		private static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

		#endregion

		#region Protected Methods

		/// <summary>
		/// Performs validation on the properties.
		/// </summary>
		/// <param name="validationResults"></param>
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			base.ValidateFields(validationResults);

			if (string.IsNullOrWhiteSpace(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError("Name", "Name is required"));
			}
		}

		#endregion
	}
}
