namespace VixenModules.Preview.DisplayPreview.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using Vixen.Sys;
    using VixenModules.Preview.DisplayPreview.Model;

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

        public void UpdateExecutionStateValues(ChannelIntentStates channelIntentStates)
        {
            // TODO: This was commented out to make it compile.
            var colorsByChannel = new Dictionary<ChannelNode, Color>(); // RGBModule.MapChannelCommandsToColors(channelIntentStates).ToMediaColor();
            foreach (var displayItem in DataModel.DisplayItems)
            {
                displayItem.UpdateChannelColors(colorsByChannel);
            }
        }
    }
}
