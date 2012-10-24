namespace VixenModules.Preview.DisplayPreview.Views
{
    using System;
    using System.Windows.Threading;
    using Vixen.Sys;
    using VixenModules.Preview.DisplayPreview.Model;
    using VixenModules.Preview.DisplayPreview.ViewModels;

    public static class ViewManager
    {
        private static VisualizerView _view;

        private static VisualizerViewModel _visualizerViewModel;

        public static void DisplayPreferences(DisplayPreviewModuleDataModel dataModel)
        {
            var viewModel = new PreferencesViewModel(dataModel);
            var view = new PreferencesView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void DisplaySetupView(DisplayPreviewModuleDataModel dataModel)
        {
            var setupViewModel = new SetupViewModel(dataModel);
            var setupView = new SetupView { DataContext = setupViewModel };
            setupView.ShowDialog();
			StartVisualizer(dataModel); //Refresh the visualizer
		}

        public static void EnsureVisualizerIsClosed()
        {
            if (_view != null)
            {
                _view.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => _view.Close()));
            }
        }

        public static void StartVisualizer(DisplayPreviewModuleDataModel dataModel)
        {
            if (_view != null)
            {
				//Ensure current view is using this datamodel
				_visualizerViewModel = new VisualizerViewModel(dataModel.Clone() as DisplayPreviewModuleDataModel);
				_view.DataContext = _visualizerViewModel;
                _view.Focus();
            }
            else
            {
				_visualizerViewModel = new VisualizerViewModel(dataModel.Clone() as DisplayPreviewModuleDataModel);
                _view = new VisualizerView { DataContext = _visualizerViewModel };
                _view.Closed += VisualizerViewClosed;
                _view.Show();
            }
        }

        public static void UpdatePreviewExecutionStateValues(ChannelIntentStates channelIntentStates)
        {
            if (_visualizerViewModel != null)
            {
                _visualizerViewModel.UpdateExecutionStateValues(channelIntentStates);
            }
        }

        private static void VisualizerViewClosed(object sender, EventArgs e)
        {
            if (_view != null)
            {
                _view.Closed -= VisualizerViewClosed;
                _view = null;
                _visualizerViewModel = null;
            }
        }
    }
}
