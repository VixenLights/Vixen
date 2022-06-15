using Catel.Data;
using System;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Zoom view model.  This view model maintains whether the zoom increases or decreases.
	/// </summary>
	public class ZoomViewModel : FixtureViewModelBase, IFixtureSaveable
	{
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ZoomViewModel()
		{			
		}

		#endregion
				
		#region Public Methods

		/// <summary>
		/// Initializes the view model with zoom model data.
		/// </summary>		
		/// <param name="narrowToWide">Rotation limits to edit</param>
		/// <param name="raiseCanExecuteChanged">Action to refresh the command status of the parent</param>
		public void InitializeViewModel(bool narrowToWide, Action raiseCanExecuteChanged)
		{			
			// Store off whether the fixture zooms from narrow to wide
			NarrowToWide = narrowToWide;

			// Store off whetehr the fixture zoom from wide to narrow
			WideToNarrow = !narrowToWide;
			
			// Give the VM the ability to refresh the command enable / disable status
			RaiseCanExecuteChanged = raiseCanExecuteChanged;
		}
				
		#endregion

		#region Public Catel Properties

		/// <summary>
		/// True when the fixture zooms from narrow to wide.
		/// </summary>
		public bool NarrowToWide
		{
			get { return GetValue<bool>(NarrowToWideProperty); }
			set
			{
				SetValue(NarrowToWideProperty, value);

				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// Narrow to Wide zoom property data.
		/// </summary>
		public static readonly PropertyData NarrowToWideProperty = RegisterProperty(nameof(NarrowToWide), typeof(bool), null);

		/// <summary>
		/// True when the fixture zooms from wide to narrow.
		/// </summary>
		public bool WideToNarrow
		{
			get { return GetValue<bool>(WideToNarrowProperty); }
			set
			{
				SetValue(WideToNarrowProperty, value);

				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// Wide to Narrow zoom property data.
		/// </summary>
		public static readonly PropertyData WideToNarrowProperty = RegisterProperty(nameof(WideToNarrow), typeof(bool), null);
		
		#endregion		
	}
}
