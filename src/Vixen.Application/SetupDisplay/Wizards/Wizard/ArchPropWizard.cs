using Catel.Data;
using Catel.IoC;
using Catel.Services;
using Orc.Wizard;
using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.Wizard
{
	public class ArchPropWizard : SideNavigationWizardBase
	{
		private readonly IMessageService _messageService;

		public ArchPropWizard(ITypeFactory typeFactory, IMessageService messageService) : base(typeFactory)
		{
			ArgumentNullException.ThrowIfNull(messageService);

			_messageService = messageService;

			Title = "Arch Prop";
			ShowInTaskbar = true;
			this.AddPage<ArchPropWizardPage>();

			ResizeMode = System.Windows.ResizeMode.CanResize;


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
