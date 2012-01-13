namespace VixenModules.Output.E131.Views
{
    using VixenModules.Output.E131.ViewModels;

    public partial class UniverseSetupView
    {
        public UniverseSetupView(UniverseSetupViewModel viewModel)
        {
            viewModel.RequestClose += delegate { Close(); };
            InitializeComponent();
            DataContext = viewModel;            
        }
    }
}
