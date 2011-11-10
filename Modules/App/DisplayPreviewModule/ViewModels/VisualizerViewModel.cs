namespace VixenModules.App.DisplayPreview.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Vixen.Commands;
    using VixenModules.App.DisplayPreview.Model;
    using Vixen.Sys;
    using VixenModules.Property.RGB;

    public class VisualizerViewModel : ViewModelBase
    {
        private string _backgroundImage;

        public VisualizerViewModel(DisplayPreviewModuleDataModel displayPreviewModuleDataModel)
        {
            DisplayItems = displayPreviewModuleDataModel.DisplayItems;
            BackgroundImage = displayPreviewModuleDataModel.BackgroundImage;
            DisplayWidth = displayPreviewModuleDataModel.DisplayWidth;
            DisplayHeight = displayPreviewModuleDataModel.DisplayHeight;
        }

        public string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }

            set
            {
                _backgroundImage = value;
                OnPropertyChanged("BackgroundImage");
            }
        }

        public ObservableCollection<DisplayItem> DisplayItems { get; set; }

        public int DisplayHeight { get; set; }

        public int DisplayWidth { get; set; }

        public void UpdateExecutionStateValues(ExecutionStateValues stateValues)
        {
            var colorsByChannel = RGBModule.MapChannelCommandsToColors(stateValues).ToMediaColor();
            foreach (var displayItem in DisplayItems)
            {
                displayItem.UpdateChannelColors(colorsByChannel);
            }
        }
    }
}
