using Catel.IoC;
using Orc.Wizard;

namespace VixenApplication.SetupDisplay.Wizards.Wizard
{
	/// <summary>
	/// Base class for prop wizard.
	/// </summary>
	public abstract class PropWizardBase : SideNavigationWizardBase, IPropWizard
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="typeFactory">Catel type factory</param>
		public PropWizardBase(ITypeFactory typeFactory) :
			base(typeFactory)
		{
		}

		#endregion

		#region IPropWizard

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public INavigationController NavigationControllerWrapper
		{
			get { return NavigationController; }
			set { NavigationController = value; }
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool ShowInTaskbarWrapper
		{
			get { return ShowInTaskbar; }
			set { ShowInTaskbar = value; }
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool ShowHelpWrapper
		{
			get { return IsHelpVisible; }
			set { IsHelpVisible = value; }
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool AllowQuickNavigationWrapper
		{
			get { return AllowQuickNavigation; }
			set { AllowQuickNavigation = value; }
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool HandleNavigationStatesWrapper
		{
			get { return HandleNavigationStates; }
			set { HandleNavigationStates = value; }
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool CacheViewsWrapper
		{
			get { return CacheViews; }
			set { CacheViews = value; }
		}

		#endregion
	}
}
