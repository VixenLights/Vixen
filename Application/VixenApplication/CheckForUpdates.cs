using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Newtonsoft.Json.Linq;
using Vixen.Sys;

namespace VixenApplication
{
	public partial class CheckForUpdates : BaseForm
	{
		private readonly string _currentVersionType;
		private readonly string _currentVersion;
		private readonly string _latestVersion;
		private bool _newVersionAvailable;

		public CheckForUpdates(string currentVersion, string latestVersion, string currentVersionType)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			pictureBoxIcon.Image = Resources.VixenImage;
			labelHeading.Font = new Font(labelHeading.Font.Name, 20F);
			labelCurrentVersion.Font = new Font(labelCurrentVersion.Font.Name, 10F);
			linkLabelVixenDownLoadPage.Font = new Font(linkLabelVixenDownLoadPage.Font.Name, 10F);
			_currentVersion = currentVersion;
			_latestVersion = latestVersion;
			_currentVersionType = currentVersionType;
			Text = " " + _currentVersionType + " " + currentVersion + " Installed"; //Add Installed version and type to the Form Title
		}

		private void CheckForUpdates_Load(object sender, EventArgs e)
		{
			PopulateChangeLog(); //Add relevant Tickets and Descriptions to the TextBox.

			if (_newVersionAvailable)
			{
				labelCurrentVersion.Text = "Vixen " + _currentVersionType + " " + _latestVersion + " is now available for Download at";
			}
			else
			{
				labelCurrentVersion.Text = "Vixen " + _currentVersionType + " " + _currentVersion + " is the latest " + _currentVersionType;
				labelHeading.Text = "You have the latest " + _currentVersionType + " installed!";
				textBoxReleaseNotes.Text = "";
			}
			linkLabelVixenDownLoadPage.Text = "www.vixenlights.com/downloads/vixen-3-downloads/";
		}

		private void PopulateChangeLog()
		{
			try
			{
				using (WebClient wc = new WebClient())
				{
					if (_currentVersionType == "Build")
					{
						string allBuildResults =
							wc.DownloadString(
								"http://bugs.vixenlights.com/rest/api/latest/search?jql=Project='Vixen 3' AND fixVersion=DevBuild ORDER BY Key&startAt=0&maxResults=1000");
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
					else
					{
						List<string> releaseVersionNames = new List<string>();
						//Get the release date of the installed Version
						string getReleaseVersion =
							wc.DownloadString("http://bugs.vixenlights.com/rest/api/latest/project/VIX/versions?orderBy=releaseDate");
						//Query returns an array of released versions
						dynamic releaseVersions = JArray.Parse(getReleaseVersion);
						DateTime currentReleaseDate = new DateTime();
						//Go through Versions and store the date when it finds the a match. 
						foreach (var releaseVersion in releaseVersions)
						{
							if (releaseVersion.name == _currentVersion)
							{
								currentReleaseDate = Convert.ToDateTime(releaseVersion.releaseDate.ToString());
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
						int i = 0;
						releaseVersionNames.Reverse();


						//This for loop needs to be multithreading. The way it is now works but takes time.
						//I have tried parellal.foreach with no luck.
						//I have tried to use lists and arrays to  store the data instead of going straight to the textbox
						//then after the parellel add to textbox


						foreach (var releaseVersionName in releaseVersionNames)
						{
							textBoxReleaseNotes.Text += "\r\nVersion:" + releaseVersionName + "\r\n\r\n";
							//Grab all Closed Tickets that are in the release.
							string allBuildResults =
								wc.DownloadString(
									"http://bugs.vixenlights.com/rest/api/latest/search?jql=Project='Vixen 3' AND status=Closed AND fixVersion=\"" +
									releaseVersionName + "\"");
							dynamic allBuildArray = JObject.Parse(allBuildResults);

							//Lists are used so they can be added to the textbox later in group order
							//Can't work out a better way to do this. I did have it go through the allBuildArray three times and each time grabbing the group type.
							List<string> improvements = new List<string>();
							List<string> newFeatures = new List<string>();

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
				}
			}
			catch (Exception e)
			{
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
	}
}
