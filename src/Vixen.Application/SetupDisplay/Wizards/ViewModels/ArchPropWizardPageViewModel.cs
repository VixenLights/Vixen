using Catel.Data;
using Catel.MVVM;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Props.Models.Arch;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	/// <summary>
	/// Maintains an arch wizard page view model.
	/// </summary>
	public class ArchPropWizardPageViewModel : LightWizardPageViewModel<ArchPropWizardPage, ArchModel>, IPropWizardPageViewModel
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="wizardPage">Type of wizard page</param>
		public ArchPropWizardPageViewModel(ArchPropWizardPage wizardPage) : base(wizardPage)
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Refreshes the 3-D OpenGL graphics.
		/// </summary>
		private void RefreshGraphics()
		{
			// Update the prop nodes
			PropModel.NumPoints = NodeCount;
			PropModel.LightSize = LightSize;			
		}

		#endregion

		#region NodeCount property
		/// <summary>
		/// Gets or sets the number of lights.
		/// </summary>
		/// <remarks>
		/// The number of lights is constrained by <see cref="NodeCountMinimum"/> and <see cref="NodeCountMaximum"/>, consequently
		/// set these values prior to setting NodeCount.
		/// </remarks>
		[ViewModelToModel]
		public int NodeCount
		{
			get { return GetValue<int>(NodeCountProperty); }
			set
			{
				SetValue(NodeCountProperty, value);
				RefreshGraphics();
			}
		}
		private static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(NodeCount));

		[ViewModelToModel]
		public int NodeCountMinimum
		{
			get { return GetValue<int>(NodeCountMinimumProperty); }
			set 
			{ 
				if (NodeCount < value)
				{
					NodeCount = value;
				}
				SetValue(NodeCountMinimumProperty, value); 
			}
		}
		private static readonly IPropertyData NodeCountMinimumProperty = RegisterProperty<int>(nameof(NodeCountMinimum));

		[ViewModelToModel]
		public int NodeCountMaximum
		{
			get { return GetValue<int>(NodeCountMaximumProperty); }
			set 
			{ 
				if (NodeCount > value)
				{
					NodeCount = value;
				}
				SetValue(NodeCountMaximumProperty, value); 
			}
		}
		private static readonly IPropertyData NodeCountMaximumProperty = RegisterProperty<int>(nameof(NodeCountMaximum));
		#endregion
		
		#region ArchWiringStart property
		/// <summary>
		/// Gets or sets the Wiring Start value.
		/// </summary>
		[ViewModelToModel]
		public ArchStartLocation ArchWiringStart
		{
			get { return GetValue<ArchStartLocation>(ArchWiringStartProperty); }
			set { SetValue(ArchWiringStartProperty, value); }
		}

		/// <summary>
		/// ArchWiringStart property data.
		/// </summary>
		private static readonly IPropertyData ArchWiringStartProperty = RegisterProperty<ArchStartLocation>(nameof(ArchWiringStart));
		#endregion

		#region Protected Methods

		protected override async Task InitializeAsync()
		{
			PropModel.NumPoints = NodeCount;
			PropModel.LightSize = LightSize;

			await base.InitializeAsync();

			RefreshGraphics();
		}

		#endregion
	}
}