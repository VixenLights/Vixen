using System.Globalization;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Microsoft.Win32;
using NLog;
using LogManager = NLog.LogManager;
using VixenApplication.Updates;

namespace VixenApplication
{
	public partial class CheckForUpdates : BaseForm
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly string _currentVersionType;
		private readonly IUpdateService _updateService;
		private readonly HttpClient? _githubDownloadClient;
		private readonly CancellationTokenSource _updateCancellation = new();
		private string _currentVersion = string.Empty;
		private string _latestVersion = string.Empty;
		private Uri? _installerDownloadUri;
		private bool _newVersionAvailable;

		public CheckForUpdates() : this(new UnavailableUpdateService(), null)
		{
		}

		internal CheckForUpdates(IUpdateService updateService, HttpClient? githubDownloadClient)
		{
			_updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
			_githubDownloadClient = githubDownloadClient;
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			textBoxReleaseNotes.AutoSize = true;
			pictureBoxIcon.Image = Resources.VixenImage;
			labelHeading.Font = new Font(labelHeading.Font.Name, 20F);
			labelCurrentVersion.Font = new Font(labelCurrentVersion.Font.Name, 10F);
			_currentVersionType = VersionInfo.CurrentVersionType;
			FormClosed += (_, _) => _updateCancellation.Cancel();
		}

		private async void CheckForUpdates_Load(object sender, EventArgs e)
		{
			Text = $@"{VersionInfo.VersionName} Installed";
			labelCurrentVersion.Text = @"Checking for updates, please wait.";
			await CheckUpdates();
		}

		private async Task CheckUpdates()
		{
			Cursor = Cursors.WaitCursor;
			var channel = VersionInfo.IsDevBuild ? UpdateChannel.Development : UpdateChannel.Release;
			_currentVersion = VersionInfo.IsDevBuild ? VersionInfo.BuildNumber.ToString() : VersionInfo.ReleaseVersion;

			UpdateCheckResult? result;
			try
			{
				result = await _updateService.CheckAsync(
					new UpdateCheckRequest(channel, _currentVersion, true), _updateCancellation.Token);
			}
			catch (OperationCanceledException) when (_updateCancellation.IsCancellationRequested)
			{
				return;
			}

			if (result is null)
			{
				labelCurrentVersion.Text = @"Unable to check for updates. Please try again later.";
				labelHeading.Text = @"Update check unavailable.";
				textBoxReleaseNotes.Visible = false;
				lblChangeLog.Visible = false;
				buttonDownload.Visible = false;
				Cursor = Cursors.Arrow;
				return;
			}

			_latestVersion = result.LatestVersion;
			_installerDownloadUri = result.InstallerDownloadUri;
			_newVersionAvailable = result.IsUpdateAvailable;
			textBoxReleaseNotes.Text = NormalizeLineEndings(result.ReleaseNotes ?? string.Empty);
			labelCurrentVersion.Text = $@"{_currentVersionType} {_latestVersion} is the latest.";

			if (_newVersionAvailable)
			{
				labelHeading.Text = $@"An updated {_currentVersionType.ToLower(CultureInfo.CurrentCulture)} build is available.";
				textBoxReleaseNotes.Visible = true;
				labelHeading.Visible = true;
				lblChangeLog.Visible = true;
				buttonDownload.Visible = _installerDownloadUri is not null;
				if (_installerDownloadUri is null)
				{
					labelCurrentVersion.Text = @"An update is available, but its installer is unavailable.";
				}
				textBoxReleaseNotes.AutoSize = false;
				textBoxReleaseNotes.Height = (int)(ScalingTools.GetScaleFactor() * 225);
				SetScrollbars();
			}
			else
			{
				labelHeading.Text = $@"You have the latest {_currentVersionType.ToLower(CultureInfo.CurrentCulture)} build installed.";
				textBoxReleaseNotes.Text = string.Empty;
				buttonDownload.Visible = false;
			}

			Cursor = Cursors.Arrow;
		}

		private static string NormalizeLineEndings(string releaseNotes)
			=> releaseNotes.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

		private async void buttonDownload_Click(object sender, EventArgs e)
		{
			try
			{
				await DownloadFile();
			}
			catch (Exception exception)
			{
				Logging.Error(exception, "An error occurred while downloading the latest version.");
				var messageBox = new MessageBoxForm("The update could not be downloaded. Please try again later.", "Download Failed", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialogThreadSafe(this);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private async Task DownloadFile()
		{
			Cursor = Cursors.WaitCursor;
			var installerDownloadUri = _installerDownloadUri ?? throw new InvalidOperationException("The update installer is unavailable.");
			var fileName = Path.GetFileName(installerDownloadUri.LocalPath);
			var githubDownloadClient = _githubDownloadClient ?? throw new InvalidOperationException("The GitHub download client is unavailable.");
			var fileBytes = await githubDownloadClient.GetByteArrayAsync(installerDownloadUri);
			await File.WriteAllBytesAsync(Path.Combine(GetDownloadFolderPath(), fileName), fileBytes);
			var messageBox = new MessageBoxForm($"Latest version downloaded to {GetDownloadFolderPath()}.", "Info", MessageBoxButtons.OK, SystemIcons.Information);
			messageBox.ShowDialogThreadSafe(this);
		}

		private static string GetDownloadFolderPath()
		{
			using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders");
			var path = key?.GetValue("{374DE290-123F-4565-9164-39C4925E467B}")?.ToString();
			return string.IsNullOrEmpty(path) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") : path;
		}

		private void SetScrollbars()
		{
			var size = TextRenderer.MeasureText(textBoxReleaseNotes.Text, textBoxReleaseNotes.Font);
			var horizontal = textBoxReleaseNotes.ClientSize.Height < size.Height + Convert.ToInt32(textBoxReleaseNotes.Font.Size);
			var vertical = textBoxReleaseNotes.ClientSize.Width < size.Width;
			textBoxReleaseNotes.ScrollBars = (horizontal, vertical) switch
			{
				(true, true) => ScrollBars.Both,
				(true, false) => ScrollBars.Vertical,
				(false, true) => ScrollBars.Horizontal,
				_ => ScrollBars.None
			};
		}
	}
}
