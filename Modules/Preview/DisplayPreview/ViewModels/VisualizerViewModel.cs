using System;
using System.Collections.ObjectModel;
using System.IO;
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
					try {
						var image = new BitmapImage();
						image.BeginInit();
						image.CacheOption = BitmapCacheOption.OnLoad;
						image.UriSource = new Uri(_dataModel.BackgroundImage, UriKind.Absolute);
						image.EndInit();

						_backgroundImage = image;
					}
					catch (Exception ex) {
						if (ex is DirectoryNotFoundException || ex is FileNotFoundException) {
							VixenSystem.Logging.Error("DisplayPreview: error loading background image. File not found: " +
							                          _dataModel.BackgroundImage);
						} else {
							throw;
						}
					}				
				}

				return _backgroundImage;
			}
		}

        public void UpdateExecutionStateValues(ChannelIntentStates channelIntentStates)
        {
            
			foreach (var displayItem in DataModel.DisplayItems)
			{
				displayItem.ResetColor(true);  
				displayItem.UpdateChannelColors(channelIntentStates);
			}
        }

    }
}
