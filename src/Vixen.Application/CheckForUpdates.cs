using System.Diagnostics;
using System.Text;
using Catel.Logging;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using NLog;
using LogManager = NLog.LogManager;

namespace VixenApplication
{
	public partial class CheckForUpdates : BaseForm
	{
		private readonly string _currentVersionType;
		private string _currentVersion = string.Empty;
		private string _latestVersion = string.Empty;
		private bool _newVersionAvailable;
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		public CheckForUpdates()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			textBoxReleaseNotes.AutoSize = false;
			textBoxReleaseNotes.Height = (int)(ScalingTools.GetScaleFactor() * 225);
			pictureBoxIcon.Image = Resources.VixenImage;
			labelHeading.Font = new Font(labelHeading.Font.Name, 20F);
			labelCurrentVersion.Font = new Font(labelCurrentVersion.Font.Name, 10F);
			buttonDownload.Font = new Font(buttonDownload.Font.Name, 20F);
			_currentVersionType = VersionInfo.CurrentVersionType;
			
		}

		private async void CheckForUpdates_Load(object sender, EventArgs e)
		{
			//Add Installed version and type to the Form Title
			Text = @" " + _currentVersionType + @" " + VersionInfo.VersionName + @" Installed"; 
			//display a message to the user that we are doing stuff
			labelCurrentVersion.Text = @"Checking for updates, please wait.";

			await CheckUpdates();
		}

		private async Task CheckUpdates()
		{
			//Turn on the wait cursor while we do stuff
			Cursor = Cursors.WaitCursor;

			if (VersionInfo.IsDevBuild)
			{
				_currentVersion = VersionInfo.BuildNumber.ToString();
				var buildVersion = await VersionInfo.GetLatestBuildVersionAsync();
				_latestVersion = buildVersion.ToString();
			}
			else
			{
				_currentVersion = VersionInfo.ReleaseVersion;
				_latestVersion = await VersionInfo.GetLatestReleaseVersionAsync();
			}

			await PopulateChangeLog(); //Add relevant Tickets and Descriptions to the TextBox.

			if (_newVersionAvailable)
			{
				labelCurrentVersion.Text = String.Empty;
				textBoxReleaseNotes.Visible = true;
				labelHeading.Visible = true;
				lblChangeLog.Visible = true;
				buttonDownload.Text = $@"Download {_currentVersionType} version {_latestVersion}";
				buttonDownload.Visible = true;
			}
			else
			{
				labelCurrentVersion.Text = 
					@"Vixen " + _currentVersionType + @" " + _currentVersion + @" is the latest " + _currentVersionType;
				labelHeading.Text = @"You have the latest " + _currentVersionType + @" installed!";
				textBoxReleaseNotes.Text = "";
				buttonDownload.Visible = false;
			}

			//Set the cursor back
			Cursor = Cursors.Arrow;
		}

		private async Task PopulateChangeLog()
		{
			try
			{

				if (VersionInfo.IsDevBuild)
				{
					using (HttpClient wc = new HttpClient())
					{
						wc.Timeout = TimeSpan.FromMilliseconds(5000);
						//Run the web call as an async call to prevent locking the UI
						//While this occurs, the control will be returned to the caller as this runs in a background thread
						string allBuildResults =
							await wc.GetStringAsync(
								$"http://bugs.vixenlights.com//rest/api/3/search/jql?jql=Project='Vixen 3' and 'fix build number[number]' > '{_currentVersion}' AND Status IN 'Closed' ORDER BY Key&startAt=0&maxResults=1000&fields=summary");

						//Execution will resume here after the call is complete
						dynamic allBuildArray = JObject.Parse(allBuildResults);

						foreach (var build in allBuildArray.issues)
						{
							textBoxReleaseNotes.Text += @"    * [" + build.key + @"] -  " + build.fields.summary + "\r\n";
							_newVersionAvailable = true;
						}
					}
				}
				else
				{
					List<string> releaseVersionNames = new List<string>();
					//Get the release date of the installed Version
					HttpClient wc = new HttpClient();
					wc.Timeout = TimeSpan.FromMilliseconds(5000);
					//More async stuff here as above. However, we are going to use an HttpClient here instead as it 
					//will allow us to make more than one call at a time whereas the WebClient will not
					string getReleaseVersion =
						await wc.GetStringAsync(
							"http://bugs.vixenlights.com/rest/api/latest/project/VIX/versions?orderBy=releaseDate");
					//Query returns an array of released versions
					dynamic releaseVersions = JArray.Parse(getReleaseVersion);
					DateTime currentReleaseDate = new DateTime();
					//Go through Versions and store the date when it finds a match. 
					foreach (var releaseVersion in releaseVersions)
					{
						if (releaseVersion.name == _currentVersion)
						{
							if (releaseVersion.releaseDate != null)
							{
								currentReleaseDate = Convert.ToDateTime(releaseVersion.releaseDate.ToString());
							}
							else
							{
								currentReleaseDate = DateTime.Now;
							}
							break;
						}
					}

					foreach (var releaseVersion in releaseVersions)
					{
						if (releaseVersion.releaseDate != null)
						{
							if (currentReleaseDate < Convert.ToDateTime(releaseVersion.releaseDate.ToString()))
							{
								releaseVersionNames.Add(releaseVersion.name.ToString());
							}
						}
					}

					releaseVersionNames.Reverse();

					//We are going to create a list to hold all of our parallel tasks that we spin up
					Dictionary<string, Task<string>> releaseNotesResponses = new Dictionary<string, Task<string>>();

					//loop over our versions and fire off some async tasks to do them in the background
					foreach (var releaseVersionName in releaseVersionNames)
					{
						//Grab all Closed Tickets that are in the release asynchronously and queue up a list of them
						//This call will not block and the loop will go on and get all of them going
						var response = wc.GetStringAsync(
								$"http://bugs.vixenlights.com/rest/api/3/search/jql?jql=Project='Vixen 3' AND status=Closed AND fixVersion='{releaseVersionName}'&maxResults=150&fields=summary,issuetype,fixVersions");

						//Add the background task into a list tied to its version and loop back
						releaseNotesResponses.Add(releaseVersionName, response);
					}

					//Wait for all the parallel requests to finish
					//Control will be returned to the caller and the UI will be free
					await Task.WhenAll(releaseNotesResponses.Values);

					//Get rid of our client once we are done and control resumes here
					wc.Dispose();

					//Now loop over the results and build the notes like before
					foreach (var allBuildResults in releaseNotesResponses)
					{
						dynamic allBuildArray = JObject.Parse(allBuildResults.Value.Result);

						//Lists are used so they can be added to the textbox later in group order
						//Can't work out a better way to do this. I did have it go through the allBuildArray three times and each time grabbing the group type.
						List<string> improvements = new List<string>();
						List<string> newFeatures = new List<string>();

						textBoxReleaseNotes.Text += "\r\nVersion:" + allBuildResults.Key + "\r\n\r\n";

						textBoxReleaseNotes.Text += "** Bugs\r\n   ";
						foreach (var build in allBuildArray.issues)
						{
							if (build.fields.fixVersions[0].releaseDate > currentReleaseDate)
							{
								string issueNumber = build.fields.issuetype.name.ToString();
								switch (issueNumber)
								{
									case "Bug":
										//Add Bugs to the textbox first.
										textBoxReleaseNotes.Text += "    * [" + build.key + "] -  " + build.fields.summary + "\r\n   ";
										break;
									case "Improvement":
										improvements.Add("    * [" + build.key + "] -  " + build.fields.summary);
										break;
									case "New Feature":
										newFeatures.Add("    * [" + build.key + "] -  " + build.fields.summary);
										break;
								}
								_newVersionAvailable = true;
							}
						}

						//Add Improvements to the textbox
						textBoxReleaseNotes.Text += "\r\n** Improvements\r\n   ";
						foreach (var improvement in improvements)
						{
							textBoxReleaseNotes.Text += improvement + "\r\n   ";
						}

						//add New Features to the textbox
						textBoxReleaseNotes.Text += "\r\n** New Features\r\n   ";
						foreach (var newFeature in newFeatures)
						{
							textBoxReleaseNotes.Text += newFeature + "\r\n   ";
						}
					}

				}
				SetScrollbars();

			}
			catch (Exception e)
			{
				Logging.Error(e, "Error trying to get the change log.");
				//If we get here then more then likely there is no internet connection.
			}
		}
		
		private async void buttonDownload_Click(object sender, EventArgs e)
		{
			try
			{
				await DownloadFile();
			}
			catch (Exception ex)
			{
				Logging.Error(ex, "An error occured trying to download the lastest version");
			}
		}
		
		private async Task DownloadFile()
		{
			//https://github.com/VixenLights/Vixen/releases/download/3.12u6/Vixen-3.12.6-Setup-64bit.exe
			//https://github.com/VixenLights/Vixen/releases/download/DevBuild-1380/Vixen-DevBuild-0.0.1380-Setup-64bit.exe
			//Turn on the wait cursor while we do stuff
			Cursor = Cursors.WaitCursor;

			StringBuilder downloadUrl = new StringBuilder(@"https://github.com/VixenLights/Vixen/releases/download/");

			if (VersionInfo.IsDevBuild)
			{
				downloadUrl.Append("DevBuild-");
				downloadUrl.Append(_latestVersion);
				downloadUrl.Append($"/Vixen-DevBuild-0.0.{_latestVersion}-Setup-64bit.exe");
			}
			else
			{
				downloadUrl.Append(_latestVersion);
				downloadUrl.Append($"/Vixen-{ConvertVersion(_latestVersion)}-Setup-64bit.exe");
			}

			String downloadPath = GetDownloadFolderPath();
			String fileToDownload = downloadUrl.ToString();

			var httpClient = new HttpClient();
			string fileName = fileToDownload.Split('/').Last();
			byte[] fileBytes = await httpClient.GetByteArrayAsync(fileToDownload);
			await File.WriteAllBytesAsync(downloadPath + "\\" + fileName, fileBytes);

			var messageBox = new MessageBoxForm($"Latest version download to {downloadPath}.", "Info", MessageBoxButtons.OK, SystemIcons.Information);
			messageBox.ShowDialogThreadSafe(this);

			//Turn off the wait cursor while we do stuff
			Cursor = Cursors.Default;
		}

		private string GetDownloadFolderPath()
		{
			string path = string.Empty;
			RegistryKey? rKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders");
			if (rKey != null)
			{
				// The specific GUID for the Downloads folder
				object? value = rKey.GetValue("{374DE290-123F-4565-9164-39C4925E467B}");
				if (value != null)
				{
					path = value.ToString() ?? string.Empty;
				}
			}

			// Fallback to the general approach if registry key is not found
			if (string.IsNullOrEmpty(path))
			{
				string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
				path = Path.Combine(userProfile, "Downloads");
				if (Directory.Exists(path))
				{
					return path;
				}
				return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			}

			return path;
		}



		private String ConvertVersion(String version)
		{
			return version.Replace('u', '.');
		}

		private void SetScrollbars()
		{

			Size tS = TextRenderer.MeasureText(textBoxReleaseNotes.Text, textBoxReleaseNotes.Font);
			bool hsb = textBoxReleaseNotes.ClientSize.Height < tS.Height + Convert.ToInt32(textBoxReleaseNotes.Font.Size);
			bool vsb = textBoxReleaseNotes.ClientSize.Width < tS.Width;

			if (hsb && vsb)
				textBoxReleaseNotes.ScrollBars = ScrollBars.Both;
			else if (!hsb && !vsb)
				textBoxReleaseNotes.ScrollBars = ScrollBars.None;
			else if (hsb && !vsb)
				textBoxReleaseNotes.ScrollBars = ScrollBars.Vertical;
			else if (!hsb && vsb)
				textBoxReleaseNotes.ScrollBars = ScrollBars.Horizontal;
		}
	}
}
