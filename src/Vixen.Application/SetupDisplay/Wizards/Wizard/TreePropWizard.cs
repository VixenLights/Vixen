using Catel.Data;
using Catel.IoC;
using Catel.Services;
using Orc.Wizard;
using System.Diagnostics;
using Vixen.Extensions;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.Wizard
{
	public class TreePropWizard : PropWizardBase
	{
		private readonly IMessageService _messageService;

		private const string HelpUrl =
			"https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-mega-tree/";

		public TreePropWizard(ITypeFactory typeFactory, IMessageService messageService) : base(typeFactory)
		{
			ArgumentNullException.ThrowIfNull(messageService);

			_messageService = messageService;

			Title = $"{PropType.Tree.GetEnumDescription()} Prop";
			ShowInTaskbar = true;

			// Set up the Wizard pages
			this.AddPage<TreePropWizardPage>();
			DimmingWizardPage dimmingPage = this.AddPage<DimmingWizardPage>();
			dimmingPage.PropType = PropType.Tree;
			this.AddPage<ColorWizardPage>();
			SummaryWizardPage summaryPage = this.AddPage<SummaryWizardPage>();
			summaryPage.Description = $"Below is a summary of the {Title} selections.";
		}

		#region IWizard

		/// <summary>
		/// Catel Wizard override for showing Tree specific help.
		/// </summary>
		/// <returns>Task for showing the online help</returns>
		public override Task ShowHelpAsync()
		{
			// Launch a browser with the help URL for the current page
			return Task.FromResult(Process.Start(new ProcessStartInfo(HelpUrl) { UseShellExecute = true }));
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

		#endregion


	}
}
