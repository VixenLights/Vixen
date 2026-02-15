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
			LeftRight = false;
		}

		#region Optional Left / Right property
		/// <summary>
		/// Gets or sets if the wizard will additional generate a left and right group.
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

		///// <summary>
		///// Gets the prop based on the settings in the wizard.
		///// </summary>
		///// <returns>A copy of the prop.</returns>
		//public IProp GetProp()
		//{
		//	var arch = VixenSystem.Props.CreateProp<Arch>(Name);
		//	arch.NodeCount = NodeCount;
		//	arch.ArchWiringStart = ArchWiringStart;
		//	arch.StringType = StringType;
		//	arch.LightSize = LightSize;
		//	return arch;
		//}
	}
}

