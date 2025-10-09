using Catel.IoC;
using Orc.Wizard;

namespace VixenModules.Editor.PropWizard
{
	/// <summary>
	/// Base class for prop wizard.
	/// </summary>
	public abstract class PropWizardBase : SideNavigationWizardBase, IPropWizard
	{
		public int MyNumber;

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="typeFactory">Catel type factory</param>
		public PropWizardBase(ITypeFactory typeFactory) :
			base(typeFactory)
		{
			ResizeMode = System.Windows.ResizeMode.CanResize;
			MaxSize = new System.Windows.Size(800, 600);
		}

		#endregion

		#region IPropWizard

		/// <inheritdoc/>		
		public INavigationController NavigationControllerWrapper
		{
			get { return NavigationController; }
			set { NavigationController = value; }
		}
		
		/// <inheritdoc/>	
		public bool ShowInTaskbarWrapper
		{
			get { return ShowInTaskbar; }
			set { ShowInTaskbar = value; }
		}
		
		/// <inheritdoc/>	
		public bool ShowHelpWrapper
		{
			get { return IsHelpVisible; }
			set { IsHelpVisible = value; }
		}

		/// <inheritdoc/>	
		public bool AllowQuickNavigationWrapper
		{
			get { return AllowQuickNavigation; }
			set { AllowQuickNavigation = value; }
		}

		/// <inheritdoc/>	
		public bool HandleNavigationStatesWrapper
		{
			get { return HandleNavigationStates; }
			set { HandleNavigationStates = value; }
		}

		/// <inheritdoc/>	
		public bool CacheViewsWrapper
		{
			get { return CacheViews; }
			set { CacheViews = value; }
		}

		#endregion
	}
}
