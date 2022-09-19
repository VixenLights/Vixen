using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Catel.Data;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Common.WPFCommon.Services;
using Newtonsoft.Json.Linq;
using VixenModules.App.TimingTrackBrowser.Import.XLights;
using VixenModules.App.TimingTrackBrowser.Model.ExternalVendorInventory;
using VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory;
using Song = VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory.Song;

namespace VixenModules.App.TimingTrackBrowser.ViewModels
{
	public class VendorInventoryWindowViewModel : ViewModelBase
	{
		private readonly IProcessService _processService;
        private readonly IDownloadService _downloadService;
        private readonly IMessageBoxService _messageBoxService;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		public VendorInventoryWindowViewModel(IProcessService processService, IDownloadService downloadService, IMessageBoxService messageBoxService)
		{
			_processService = processService;
            _downloadService = downloadService;
            _messageBoxService = messageBoxService;
            SongInventories = new ObservableCollection<SongInventory>();
            SelectedInventory = new SongInventory();
        }

        #region Overrides of ViewModelBase

		/// <inheritdoc />
		protected override async Task InitializeAsync()
        {
            XTimingLyricInventoryImporter li = new XTimingLyricInventoryImporter();

            var vendorLinks = await GetVendorUrls();
            if (!vendorLinks.Any()) { return; }

            foreach (var vendorLink in vendorLinks)
            {
                try
                {
                    var xml = await _downloadService.GetFileAsStringAsync(new Uri(vendorLink.Url));
                    var response = await li.Import(xml);

                    SongInventories.Add(response);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"An error occurred retrieving the inventory from: {vendorLink}");
                    _messageBoxService.ShowError($"Unable to retrieve inventory from {vendorLink.Name}\nEnsure you have an active internet connection.", "Error Retrieving Inventory");
                }
            }

            SelectedInventory = SongInventories.FirstOrDefault();
            //    new Uri("https://www.xlightsfaces.com/wp-content/uploads/xlights_music_free.xml"));

        }

        /// <inheritdoc />
		public override string Title => "Song Inventory";

		#endregion

		#region SelectedInventory property

		/// <summary>
		/// Gets or sets the SelectedInventory value.
		/// </summary>
		public SongInventory SelectedInventory
		{
			get { return GetValue<SongInventory>(SelectedInventoryProperty); }
			set { SetValue(SelectedInventoryProperty, value); }
		}

		/// <summary>
		/// SelectedInventory property data.
		/// </summary>
		public static readonly PropertyData SelectedInventoryProperty = RegisterProperty("SelectedInventory", typeof(SongInventory));

		#endregion

		#region Inventory model property

		/// <summary>
		/// Gets or sets the Inventory value.
		/// </summary>
		[Model]
		public ObservableCollection<SongInventory> SongInventories
		{
			get { return GetValue<ObservableCollection<SongInventory>>(InventoryProperty); }
			private set { SetValue(InventoryProperty, value); }
		}

		/// <summary>
		/// Inventory property data.
		/// </summary>
		public static readonly PropertyData InventoryProperty = RegisterProperty("SongInventories", typeof(ObservableCollection<SongInventory>));

		#endregion

		#region SelectedSong property

		/// <summary>
		/// Gets or sets the SelectedProduct value.
		/// </summary>
		public Song SelectedSong
		{
			get { return GetValue<Song>(SelectedSongProperty); }
			set { SetValue(SelectedSongProperty, value); }
		}

		/// <summary>
		/// SelectedProduct property data.
		/// </summary>
		public static readonly PropertyData SelectedSongProperty = RegisterProperty("SelectedSong", typeof(Song));

		#endregion

        #region IsModelValid property

		/// <summary>
		/// Gets or sets the ShowModelTab value.
		/// </summary>
		public bool IsSongValid
		{
			get { return GetValue<bool>(IsSongValidProperty); }
			set { SetValue(IsSongValidProperty, value); }
		}

		/// <summary>
		/// ShowModelTab property data.
		/// </summary>
		public static readonly PropertyData IsSongValidProperty = RegisterProperty("IsSongValid", typeof(bool));

		#endregion

		#region IsProductVisible property

		#region IsProductVisible property

		/// <summary>
		/// Gets or sets the IsProductVisible value.
		/// </summary>
		public bool IsSongVisible
		{
			get { return GetValue<bool>(IsSongVisibleProperty); }
			set { SetValue(IsSongVisibleProperty, value); }
		}

		/// <summary>
		/// IsProductVisible property data.
		/// </summary>
		public static readonly PropertyData IsSongVisibleProperty = RegisterProperty("IsSongVisible", typeof(bool));

		#endregion

		#endregion

		#region IsProductViewSelected property

		/// <summary>
		/// Gets or sets the ProductTabSelected value.
		/// </summary>
		public bool IsSongViewSelected
		{
			get { return GetValue<bool>(IsSongViewSelectedProperty); }
			set { SetValue(IsSongViewSelectedProperty, value); }
		}

		/// <summary>
		/// ProductTabSelected property data.
		/// </summary>
		public static readonly PropertyData IsSongViewSelectedProperty = RegisterProperty("IsSongViewSelected", typeof(bool));

		#endregion

		#region SelectProduct command

		private Command<Song> _selectProductCommand;

		/// <summary>
		/// Gets the SelectProduct command.
		/// </summary>
		public Command<Song> SelectSongCommand
		{
			get { return _selectProductCommand ?? (_selectProductCommand = new Command<Song>(SelectSong)); }
		}

		/// <summary>
		/// Method to invoke when the SelectSong command is executed.
		/// </summary>
		private void SelectSong(Song s)
		{
            SelectedSong = s;
			IsSongVisible = s != null;
            IsSongValid = s.TimingLink != null;//s.ModelLinks != null && s.ModelLinks.Any(x => x.Link != null);
			IsSongViewSelected = true;

		}

		#endregion

		#region DialogResult property

		/// <summary>
		/// Gets or sets the DialogResult value.
		/// </summary>
		public bool DialogResult
		{
			get { return GetValue<bool>(DialogResultProperty); }
			set { SetValue(DialogResultProperty, value); }
		}

		/// <summary>
		/// DialogResult property data.
		/// </summary>
		public static readonly PropertyData DialogResultProperty = RegisterProperty("DialogResult", typeof(bool));

		#endregion

		#region ShowProductPage command

		private Command<Uri> _navigateToUrlCommand;

		/// <summary>
		/// Gets the ShowProductPage command.
		/// </summary>
		public Command<Uri> NavigateToUrlCommand
		{
			get { return _navigateToUrlCommand ?? (_navigateToUrlCommand = new Command<Uri>(NavigateToUrl)); }
		}

		/// <summary>
		/// Method to invoke when the ShowProductPage command is executed.
		/// </summary>
		private async void NavigateToUrl(Uri url)
		{
			if(url==null)
            {
				Log.Info($"Song link is null");
				return;
            }
            try
            {
	            await _processService.RunAsync(new ProcessContext { FileName = url.AbsoluteUri, UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {

                if (noBrowser.ErrorCode == -2147467259)
                {
                    Log.Error(noBrowser, noBrowser.Message);
				}
            }
            catch (System.Exception e)
            {
				Log.Error(e, "Error opening link");
            }
		}

		#endregion

		#region SendEmail command

		private Command<string> _sendEmailCommand;

		/// <summary>
		/// Gets the SendEmail command.
		/// </summary>
		public Command<string> SendEmailCommand
		{
			get { return _sendEmailCommand ?? (_sendEmailCommand = new Command<string>(SendEmail)); }
		}

		/// <summary>
		/// Method to invoke when the SendEmail command is executed.
		/// </summary>
		private void SendEmail(string address)
		{
			if (!address.StartsWith("mailto:"))
			{
				address = $"mailto:{address}";
			}
			_processService.StartProcess(address);
		}

		#endregion

		#region ImportModel command

		private Command _importSongCommand;

		/// <summary>
		/// Gets the ImportModel command.
		/// </summary>
		public Command ImportSongCommand
		{
			get { return _importSongCommand ?? (_importSongCommand = new Command(ImportSongLyricTrack)); }
		}

		/// <summary>
		/// Method to invoke when the ImportModel command is executed.
		/// </summary>
		private void ImportSongLyricTrack()
		{
		
            
            DialogResult = true;
            CloseViewModelAsync(true);
		}

		#endregion

        public async Task<string> GetSelectedSongTiming()
        {

            if (SelectedSong.TimingLink != null)
            {
                var track = await _downloadService.GetFileAsStringAsync(SelectedSong.TimingLink);
                return track;
            }

			return String.Empty;
        }

		private async Task<VendorLink[]> GetVendorUrls()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var ds = dependencyResolver.Resolve<IDownloadService>();

            try
            {
                var vendorJson = await ds.GetFileAsStringAsync(new Uri(@"https://app.vixenlights.com/singing-faces-vendor.json"));
                var o = JArray.Parse(vendorJson);

                var vendors = o.Select(p => new VendorLink()
                {
                    Name = (string)p["Name"],
                    Url = (string)p["Url"]
                }).ToArray();

                return vendors;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error retrieving the singing faces vendor list.");
                var mbs = dependencyResolver.Resolve<IMessageBoxService>();
                mbs.ShowError("Unable to retrieve singing faces vendor list. Ensure you have an active internet connection.", "Error Retrieving Vendors");
            }

            return new VendorLink[0];
        }

	}
}
