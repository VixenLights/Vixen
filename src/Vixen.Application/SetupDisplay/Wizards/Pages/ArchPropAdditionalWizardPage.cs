using Catel.Data;
using Orc.Wizard;
using Vixen.Extensions;
using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class ArchPropAdditionalWizardPage : WizardPageBase
	{
		public ArchPropAdditionalWizardPage()
		{
			Title = "Additional Props";
			Description = $"Create additional Props for {PropType.Arch.GetEnumDescription()}";

			// Set default value(s)
			LeftRight = false;
		}

		#region Optional Left / Right property
		/// <summary>
		/// Gets or sets if the wizard will additionally generate a left and right prop.
		/// </summary>
		public bool LeftRight
		{
			get { return GetValue<bool>(LeftRightProperty); }
			set { SetValue(LeftRightProperty, value); }
		}
		private static readonly IPropertyData LeftRightProperty = RegisterProperty<bool>(nameof(LeftRight));
		#endregion

		/// <summary>
		/// Get a summary of the wizard page settings.
		/// </summary>
		/// <returns>Summary object.</returns>
		public override ISummaryItem GetSummary()
		{
			return new SummaryItem
			{
				Title = this.Title,
				Summary = $"Left & Right Arches: {LeftRight}"
			};
		}
	}
}

