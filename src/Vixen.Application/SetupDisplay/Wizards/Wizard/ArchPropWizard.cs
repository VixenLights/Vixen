using Catel.Data;
using Catel.IoC;
using Catel.Services;
using Orc.Wizard;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.Editor.PropWizard;
using Vixen.Sys.Props;
using Vixen.Extensions;

namespace VixenApplication.SetupDisplay.Wizards.Wizard
{
	public class ArchPropWizard : PropWizardBase
	{
		private readonly IMessageService _messageService;

		public ArchPropWizard(ITypeFactory typeFactory, IMessageService messageService) : base(typeFactory)
		{
			ArgumentNullException.ThrowIfNull(messageService);

			_messageService = messageService;

			Title = $"{PropType.Arch.GetEnumDescription()} Prop";
			ShowInTaskbar = true;

			this.AddPage<ArchPropWizardPage>();
			this.AddPage<ArchPropAdditionalWizardPage>();
			DimmingWizardPage dimmingPage = this.AddPage<DimmingWizardPage>();
			dimmingPage.PropType = PropType.Arch;
			SummaryWizardPage summaryPage = this.AddPage<SummaryWizardPage>();
			summaryPage.Description = $"Below is a summary of the {Title} selections.";
		}

		public override Task ShowHelpAsync()
		{
			return _messageService.ShowAsync("HELP HANDLER");
		}

		#region HideNavigationSystem property

		/// <summary>
		/// Gets or sets the HideNavigationSystem value.
		/// </summary>
		public bool HideNavigationSystem
		{
			get { return GetValue<bool>(HideNavigationSystemProperty); }
			set { SetValue(HideNavigationSystemProperty, value); }
		}

		/// <summary>
		/// HideNavigationSystem property data.
		/// </summary>
		public static readonly IPropertyData HideNavigationSystemProperty = RegisterProperty<bool>(nameof(HideNavigationSystem));

		#endregion
	}
}
