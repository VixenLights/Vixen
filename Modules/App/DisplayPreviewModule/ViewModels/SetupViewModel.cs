namespace VixenModules.App.DisplayPreview.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using Microsoft.Win32;
    using VixenModules.App.DisplayPreview.Model;
    using VixenModules.App.DisplayPreview.Views;
    using VixenModules.App.DisplayPreview.WPF;

    public class SetupViewModel : ViewModelBase
    {
        private readonly DisplayPreviewModuleDataModel _dataModel;

        private BitmapImage _backgroundImage;

        private DisplayItem _currentDisplayElement;

        public SetupViewModel(DisplayPreviewModuleDataModel dataModel)
        {
            _dataModel = dataModel;
            AddElementCommand = new RelayCommand(x => AddElement());
            EditElementCommand = new RelayCommand(x => EditDisplayElement(), x => CanEditDisplayElement());
            DeleteElementCommand = new RelayCommand(x => DeleteDisplayElement(), x => CanDeleteDisplayElement());
            SetBackgroundCommand = new RelayCommand(x => SetBackground());
            MoveUpCommand = new RelayCommand(x => MoveUp(), x => CanMoveUp());
            MoveDownCommand = new RelayCommand(x => MoveDown(), x => CanMoveDown());
        }

        public double Opacity
        {
            get
            {
                return _dataModel.Opacity;
            }

            set
            {
                _dataModel.Opacity = value;
                OnPropertyChanged("Opacity");
            }
        }

        public ICommand AddElementCommand { get; private set; }

        public BitmapImage BackgroundImage
        {
            get
            {
                if (_backgroundImage == null && _dataModel.BackgroundImage != null)
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

            set
            {
                _backgroundImage = value;
                _dataModel.BackgroundImage = value.UriSource.AbsoluteUri;
                OnPropertyChanged("BackgroundImage");
            }
        }

        public DisplayItem CurrentDisplayElement
        {
            get
            {
                return _currentDisplayElement;
            }

            set
            {
                _currentDisplayElement = value;
                OnPropertyChanged("CurrentDisplayElement");
            }
        }

        public ICommand DeleteElementCommand { get; private set; }

        public ObservableCollection<DisplayItem> DisplayItems
        {
            get
            {
                return _dataModel.DisplayItems;
            }

            set
            {
                _dataModel.DisplayItems = value;
                OnPropertyChanged("DisplayItems");
            }
        }

        public int DisplayHeight
        {
            get
            {
                return _dataModel.DisplayHeight;
            }

            set
            {
                _dataModel.DisplayHeight = value;
                OnPropertyChanged("DisplayHeight");
            }
        }

        public int DisplayWidth
        {
            get
            {
                return _dataModel.DisplayWidth;
            }

            set
            {
                _dataModel.DisplayWidth = value;
                OnPropertyChanged("DisplayWidth");
            }
        }

        public ICommand EditElementCommand { get; private set; }

        public ICommand MoveDownCommand { get; private set; }
        
        public ICommand MoveUpCommand { get; private set; }
        
        public ICommand SetBackgroundCommand { get; private set; }

        /// <summary>
        ///   The add element.
        /// </summary>
        private void AddElement()
        {
            var displayElement = new DisplayItem();
            var viewModel = new DisplayItemEditorViewModel { DisplayItem = displayElement };
            var editor = new DisplayItemEditorView { DataContext = viewModel };
            editor.ShowDialog();
            DisplayItems.Add(displayElement);
            CurrentDisplayElement = displayElement;
        }

        private bool CanDeleteDisplayElement()
        {
            return CurrentDisplayElement != null;
        }

        private bool CanEditDisplayElement()
        {
            return CurrentDisplayElement != null;
        }

        private bool CanMoveDown()
        {
            var currentDisplayElement = CurrentDisplayElement;
            return currentDisplayElement != null && DisplayItems.IndexOf(currentDisplayElement) != DisplayItems.Count - 1;
        }

        private bool CanMoveUp()
        {
            var currentDisplayElement = CurrentDisplayElement;
            return currentDisplayElement != null && DisplayItems.IndexOf(currentDisplayElement) != 0;
        }

        private void DeleteDisplayElement()
        {
            var displayElement = CurrentDisplayElement;
            if (displayElement == null)
            {
                return;
            }

            if (
                MessageBox.Show(
                                string.Format(
                                              "Are you sure you want to delete the selected display element named '{0}' ?", 
                                              displayElement.Name), 
                                "Confirm delete", 
                                MessageBoxButton.YesNo, 
                                MessageBoxImage.Question,
                                MessageBoxResult.No)
                == MessageBoxResult.Yes)
            {
                DisplayItems.Remove(displayElement);
                CurrentDisplayElement = null;
            }
        }

        private void EditDisplayElement()
        {
            var displayElement = CurrentDisplayElement;
            if (displayElement == null)
            {
                return;
            }

            var viewModel = new DisplayItemEditorViewModel { DisplayItem = displayElement };
            var editor = new DisplayItemEditorView { DataContext = viewModel };
            editor.ShowDialog();
        }

        private void MoveDown()
        {
            var currentDisplayElement = CurrentDisplayElement;
            var index = DisplayItems.IndexOf(currentDisplayElement);
            DisplayItems.Move(index, index + 1);
        }

        private void MoveUp()
        {
            var currentDisplayElement = CurrentDisplayElement;
            var index = DisplayItems.IndexOf(currentDisplayElement);
            DisplayItems.Move(index, index - 1);
        }

        private void SetBackground()
        {
            var openFileDialog = new OpenFileDialog
                                 {
                                     DefaultExt = ".bmp|.jpg|.png", 
                                     Filter = "Image Files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png"
                                 };

            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Open document
                var imageFile = new FileInfo(openFileDialog.FileName);
                var destFileName = Path.Combine(DisplayPreviewModuleDescriptor.ModulePath, "Background" + imageFile.Extension);
                File.Copy(imageFile.FullName, destFileName, true);
                var image = new BitmapImage();
                image.BeginInit();               
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(destFileName, UriKind.Absolute);
                image.EndInit();
                BackgroundImage = image;
                Opacity = 1;
            }
        }
    }
}
