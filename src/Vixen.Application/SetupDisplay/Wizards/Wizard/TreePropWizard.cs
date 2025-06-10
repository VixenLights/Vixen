using Catel.Data;
using Catel.IoC;
using Catel.Services;
using Orc.Wizard;
using System.Diagnostics;
using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.Wizard
{
	public class TreePropWizard : SideNavigationWizardBase
	{
		private readonly IMessageService _messageService;

		private const string HelpUrl =
			"https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-mega-tree/";

		public TreePropWizard(ITypeFactory typeFactory, IMessageService messageService) : base(typeFactory)
		{
			ArgumentNullException.ThrowIfNull(messageService);

			_messageService = messageService;

			Title = "Tree Prop";
			ShowInTaskbar = true;
			this.AddPage<TreePropWizardPage>();

			ResizeMode = System.Windows.ResizeMode.CanResize;


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
