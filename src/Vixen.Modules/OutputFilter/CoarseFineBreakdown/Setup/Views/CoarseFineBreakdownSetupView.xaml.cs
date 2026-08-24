using VixenModules.OutputFilter.CoarseFineBreakdown.Setup.ViewModels;

namespace VixenModules.OutputFilter.CoarseFineBreakdown.Setup.Views
{
	/// <summary>
	/// Provides the setup window for coarse/fine default-value mapping.
	/// </summary>
	internal partial class CoarseFineBreakdownSetupView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CoarseFineBreakdownSetupView"/> class.
		/// </summary>
		/// <param name="viewModel">The view model that stages the setup values.</param>
		public CoarseFineBreakdownSetupView(CoarseFineBreakdownSetupViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
