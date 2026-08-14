using Common.Controls;
using Common.Controls.Theme;
using NLog;
using Vixen.Sys;
using VixenApplication.Updates;

namespace VixenApplication
{
	/// <summary>
	/// Displays the GitHub release notes for the running Vixen version.
	/// </summary>
	public partial class ReleaseNotes : BaseForm
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private const string ReleaseNotesFileName = "Release Notes.txt";
		private readonly IUpdateService _updateService;
		private readonly string? _releaseTag;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReleaseNotes"/> class for the Windows Forms designer.
		/// </summary>
		public ReleaseNotes() : this(new UnavailableUpdateService(), null)
		{
		}

		internal ReleaseNotes(IUpdateService updateService, string? releaseTag)
		{
			_updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
			_releaseTag = releaseTag;
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
		}

		private async void ReleaseNotes_Load(object sender, EventArgs e)
		{
			try
			{
				await LoadReleaseNotesAsync();
			}
			catch (Exception exception)
			{
				Logging.Warn(exception, "Release notes failed to load.");
				textBoxReleaseNotes.Text = @"Release notes could not be loaded.";
			}
		}

		private async Task LoadReleaseNotesAsync()
		{
			if (string.IsNullOrEmpty(_releaseTag))
			{
				textBoxReleaseNotes.Text = @"Release notes are unavailable for test builds.";
				return;
			}

			textBoxReleaseNotes.Text = @"Loading release notes...";
			var result = await _updateService.GetReleaseNotesAsync(_releaseTag);
			var releaseNotes = result?.ReleaseNotes;
			if (string.IsNullOrWhiteSpace(releaseNotes))
			{
				releaseNotes = await LoadFallbackReleaseNotesAsync();
			}

			textBoxReleaseNotes.Text = string.IsNullOrWhiteSpace(releaseNotes)
				? "Release notes could not be loaded."
				: NormalizeLineEndings(releaseNotes);
		}

		private static async Task<string?> LoadFallbackReleaseNotesAsync()
		{
			var releaseNotesPath = Path.Combine(Paths.BinaryRootPath, ReleaseNotesFileName);
			if (!File.Exists(releaseNotesPath))
			{
				return null;
			}

			return await File.ReadAllTextAsync(releaseNotesPath);
		}

		private static string NormalizeLineEndings(string text)
			=> text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\n", Environment.NewLine, StringComparison.Ordinal);
	}
}
