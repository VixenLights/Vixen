using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Newtonsoft.Json.Linq;
using NLog;
using Vixen.Sys;

namespace VixenApplication
{
	public partial class CheckForUpdates : BaseForm
	{
		private readonly string _currentVersionType;
		private readonly string _currentVersion;
		private readonly string _latestVersion;
		private bool _newVersionAvailable;
		private static NLog.Logger Logging = LogManager.GetCurrentClassLogger();

		public CheckForUpdates(string currentVersion, string latestVersion, string currentVersionType)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			textBoxReleaseNotes.AutoSize = false;
			textBoxReleaseNotes.Height = (int)(ScalingTools.GetScaleFactor() * 225);
			pictureBoxIcon.Image = Resources.VixenImage;
			labelHeading.Font = new Font(labelHeading.Font.Name, 20F);
			labelCurrentVersion.Font = new Font(labelCurrentVersion.Font.Name, 10F);
			linkLabelVixenDownLoadPage.Font = new Font(linkLabelVixenDownLoadPage.Font.Name, 10F);
			_currentVersion = currentVersion;
			_latestVersion = latestVersion;
			_currentVersionType = currentVersionType;
			Text = " " + _currentVersionType + " " + currentVersion + " Installed"; //Add Installed version and type to the Form Title
		}

		private async void CheckForUpdates_Load(object sender, EventArgs e)
		{
			//display a message to the user that we are doing stuff
			labelCurrentVersion.Text = @"Checking for updates, please wait.";
			linkLabelVixenDownLoadPage.Text = @"www.vixenlights.com/downloads/vixen-3-downloads/";
			
			await CheckUpdates();
		}

		private async Task CheckUpdates()
		{
			//Turn on the wait cursor while we do stuff
			Cursor = Cursors.WaitCursor;

			await PopulateChangeLog(); //Add relevant Tickets and Descriptions to the TextBox.

			if (_newVersionAvailable)
			{
				labelCurrentVersion.Text = @"Vixen " + _currentVersionType + " " + _latestVersion + " is now available for download.";
				textBoxReleaseNotes.Visible = true;
				labelHeading.Visible = true;
				lblChangeLog.Visible = true;
			}
			else
			{
				labelCurrentVersion.Text =
					@"Vixen " + _currentVersionType + " " + _currentVersion + " is the latest " + _currentVersionType;
				labelHeading.Text = @"You have the latest " + _currentVersionType + " installed!";
				textBoxReleaseNotes.Text = "";
			}

			//Set the cursor back
			Cursor = Cursors.Arrow;
		}

		private async Task PopulateChangeLog()
		{
			try
			{
				
					if (_currentVersionType == "DevBuild")
					{
						using (HttpClient wc = new HttpClient())
						{
							wc.Timeout = TimeSpan.FromMilliseconds(5000);
						    //Run the web call as an asyc call to prevent locking the UI
							//While this occurs, the control will be returned to the caller as this runs in a background thread
							string allBuildResults =
								await wc.GetStringAsync(
									"http://bugs.vixenlights.com/rest/api/latest/search?jql=Project='Vixen 3' AND fixVersion=DevBuild ORDER BY Key&startAt=0&maxResults=1000");

							//Execution will resume here after the call is complete
							dynamic allBuildArray = JObject.Parse(allBuildResults);

							foreach (var build in allBuildArray.issues)
							{
								if (build.fields.customfield_10112 > _currentVersion)
								{
									textBoxReleaseNotes.Text += "    * [" + build.key + "] -  " + build.fields.summary + "\r\n";
									_newVersionAvailable = true;
								}
							}
						}
					}
					else
					{
						List<string> releaseVersionNames = new List<string>();
						//Get the release date of the installed Version
						HttpClient wc = new HttpClient();
						wc.Timeout = TimeSpan.FromMilliseconds(5000);
						//More async stuff here as above. However we are going to use an HttpClient here instead as it 
						//will allow us to make more than one call at a time whereas the WebClient will not
						string getReleaseVersion =
							await wc.GetStringAsync(
								"http://bugs.vixenlights.com/rest/api/latest/project/VIX/versions?orderBy=releaseDate");
						//Query returns an array of released versions
						dynamic releaseVersions = JArray.Parse(getReleaseVersion);
						DateTime currentReleaseDate = new DateTime();
						//Go through Versions and store the date when it finds the a match. 
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
							//Grab all Closed Tickets that are in the release asyncronously and queue up a list of them
							//This call will not block and the loop will go on and get all of them going
							var response = wc.GetStringAsync(
									"http://bugs.vixenlights.com/rest/api/latest/search?jql=Project='Vixen 3' AND status=Closed AND fixVersion=\"" +
									releaseVersionName + "\"&maxResults=150");

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

							//Add Improvemnets to the textbox
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

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}

		private void linkLabelVixenDownLoadPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("www.vixenlights.com/downloads/vixen-3-downloads/");
		}

		private void SetScrollbars()
		{
			
			Size tS = TextRenderer.MeasureText(textBoxReleaseNotes.Text, textBoxReleaseNotes.Font);
			bool Hsb = textBoxReleaseNotes.ClientSize.Height < tS.Height + Convert.ToInt32(textBoxReleaseNotes.Font.Size);
			bool Vsb = textBoxReleaseNotes.ClientSize.Width < tS.Width;

			if (Hsb && Vsb)
				textBoxReleaseNotes.ScrollBars = ScrollBars.Both;
			else if (!Hsb && !Vsb)
				textBoxReleaseNotes.ScrollBars = ScrollBars.None;
			else if (Hsb && !Vsb)
				textBoxReleaseNotes.ScrollBars = ScrollBars.Vertical;
			else if (!Hsb && Vsb)
				textBoxReleaseNotes.ScrollBars = ScrollBars.Horizontal;
		}
	}
}
