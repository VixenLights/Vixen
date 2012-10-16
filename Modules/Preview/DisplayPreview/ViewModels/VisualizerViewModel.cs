using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Vixen.Data.Value;

namespace VixenModules.Preview.DisplayPreview.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using Vixen.Sys;
    using VixenModules.Preview.DisplayPreview.Model;

	public class VisualizerViewModel : ViewModelBase
    {
        private DisplayPreviewModuleDataModel _dataModel;
		private BitmapImage _backgroundImage;

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

		public int DisplayHeight
		{
			get
			{
				return _dataModel.DisplayHeight;
			}
		}

		public ObservableCollection<DisplayItem> DisplayItems
		{
			get
			{
				return _dataModel.DisplayItems;
			}
		}

		public int DisplayWidth
		{
			get
			{
				return _dataModel.DisplayWidth;
			}
		}

		public double Opacity
		{
			get
			{
				return _dataModel.Opacity;
			}
		}

		public BitmapImage BackgroundImage
		{
			get
			{
				if (_backgroundImage == null
					&& _dataModel.BackgroundImage != null)
				{
					var image = new BitmapImage();
					image.BeginInit();
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.UriSource = new Uri(_dataModel.BackgroundImage, UriKind.Absolute);
					image.EndInit();

					_backgroundImage = image;
					
				}

				return _backgroundImage;
			}

			//set
			//{
			//    _backgroundImage = value;
			//    _dataModel.BackgroundImage = value == null ? null : value.UriSource.AbsoluteUri;
			//    OnPropertyChanged("BackgroundImage");
			//}
		}

        public void UpdateExecutionStateValues(ChannelIntentStates channelIntentStates)
        {
            
			foreach (var displayItem in DataModel.DisplayItems)
			{
				//How to tell if we are running as ResetColor acts different running or not.
				//Need to reset as we do not get off values, so reset all to off before next set of 
				//states
				displayItem.ResetColor(true);  
				displayItem.UpdateChannelColors(channelIntentStates);
			}
        }

    }
}
