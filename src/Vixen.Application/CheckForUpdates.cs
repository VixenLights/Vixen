using System.Globalization;
using System.Text;
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
		private readonly CancellationTokenSource _updateCancellation = new();
		private string _currentVersion = string.Empty;
		private string _latestVersion = string.Empty;
		private bool _newVersionAvailable;

		public CheckForUpdates() : this(new UnavailableUpdateService())
		{
		}

		internal CheckForUpdates(IUpdateService updateService)
		{
			_updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
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
				labelCurrentVersion.Text = "Unable to check for updates. Please try again later.";
				labelHeading.Text = "Update check unavailable.";
				textBoxReleaseNotes.Visible = false;
				lblChangeLog.Visible = false;
				buttonDownload.Visible = false;
				Cursor = Cursors.Arrow;
				return;
			}

			_latestVersion = result.LatestVersion;
			_newVersionAvailable = result.IsUpdateAvailable;
			textBoxReleaseNotes.Text = NormalizeLineEndings(result.ReleaseNotes ?? string.Empty);
			labelCurrentVersion.Text = $@"{_currentVersionType} {_latestVersion} is the latest.";

			if (_newVersionAvailable)
			{
				labelHeading.Text = $@"An updated {_currentVersionType.ToLower(CultureInfo.CurrentCulture)} build is available.";
				textBoxReleaseNotes.Visible = true;
				labelHeading.Visible = true;
				lblChangeLog.Visible = true;
				buttonDownload.Visible = true;
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
			}
		}

		private async Task DownloadFile()
		{
			Cursor = Cursors.WaitCursor;
			var downloadUrl = new StringBuilder("https://github.com/VixenLights/Vixen/releases/download/");
			if (VersionInfo.IsDevBuild)
			{
				downloadUrl.Append($"DevBuild-{_latestVersion}/Vixen-DevBuild-0.0.{_latestVersion}-Setup-64bit.exe");
			}
			else
			{
				downloadUrl.Append($"{_latestVersion}/Vixen-{ConvertVersion(_latestVersion)}-Setup-64bit.exe");
			}

			var fileToDownload = downloadUrl.ToString();
			var fileName = fileToDownload.Split('/').Last();
			using var httpClient = new HttpClient();
			var fileBytes = await httpClient.GetByteArrayAsync(fileToDownload);
			await File.WriteAllBytesAsync(Path.Combine(GetDownloadFolderPath(), fileName), fileBytes);
			var messageBox = new MessageBoxForm($"Latest version downloaded to {GetDownloadFolderPath()}.", "Info", MessageBoxButtons.OK, SystemIcons.Information);
			messageBox.ShowDialogThreadSafe(this);
			Cursor = Cursors.Default;
		}

		private static string GetDownloadFolderPath()
		{
			using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders");
			var path = key?.GetValue("{374DE290-123F-4565-9164-39C4925E467B}")?.ToString();
			return string.IsNullOrEmpty(path) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") : path;
		}

		private static string ConvertVersion(string version) => version.Replace('u', '.');

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
