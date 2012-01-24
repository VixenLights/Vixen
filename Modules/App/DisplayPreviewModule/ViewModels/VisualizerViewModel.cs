namespace VixenModules.App.DisplayPreview.ViewModels
{
    using Vixen.Sys;
    using VixenModules.App.DisplayPreview.Model;
    using VixenModules.Property.RGB;
	using System.Collections.Generic;
	using Vixen.Commands;

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

		//private static int temp = 0;
		//private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        public void UpdateExecutionStateValues(ExecutionStateValues stateValues)
        {
			//if (temp % 100 == 0) {
			//    VixenSystem.Logging.Debug("DP: logged " + temp + " updates in " + watch.ElapsedMilliseconds + "ms, which is " + (temp / (double)(watch.ElapsedMilliseconds / 1000.0)) + " Hz");
			//    watch.Reset();
			//    watch.Start();
			//    temp = 0;
			//}

			//temp++;

			//bool allNull = true;
			//foreach (KeyValuePair<Channel, Command> kvp in stateValues) {
			//    if (kvp.Value != null) {
			//        allNull = false;
			//        break;
			//    }
			//}

			//if (allNull && stateValues.Count > 0) {
			//    VixenSystem.Logging.Debug("DP: All null commands. state values has " + stateValues.Count + " items. Count is " + temp);
			//}

			var colorsByChannel = RGBModule.MapChannelCommandsToColors(stateValues).ToMediaColor();
            foreach (var displayItem in DataModel.DisplayItems)
            {
                displayItem.UpdateChannelColors(colorsByChannel);
            }
        }
    }
}
