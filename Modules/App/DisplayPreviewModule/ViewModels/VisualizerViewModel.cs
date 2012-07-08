namespace VixenModules.App.DisplayPreview.ViewModels
{
    using Vixen.Sys;
    using VixenModules.App.DisplayPreview.Model;
    using VixenModules.Property.RGB;

    public class VisualizerViewModel : ViewModelBase
    {
        private DisplayPreviewModuleDataModel _dataModel;

        public VisualizerViewModel(DisplayPreviewModuleDataModel displayPreviewModuleDataModel)
        {
            _dataModel = displayPreviewModuleDataModel;
        }

        public DisplayPreviewModuleDataModel DataModel
        {
            get
            {
                return _dataModel;
            }

            set
            {
                _dataModel = value;
                OnPropertyChanged("DataModel");
            }
        }

        public void UpdateExecutionStateValues(ExecutionStateValues stateValues)
        {
            var colorsByChannel = RGBModule.MapChannelCommandsToColors(stateValues).ToMediaColor();
            foreach (var displayItem in DataModel.DisplayItems)
            {
                displayItem.UpdateChannelColors(colorsByChannel);
            }
        }
    }
}
