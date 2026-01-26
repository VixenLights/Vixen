using Newtonsoft.Json.Linq;
using NLog;
using Vixen.Sys;

namespace VixenApplication
{
	public sealed class VersionInfo
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private static readonly Version Version;

		static VersionInfo()
		{
			Version = GetRunningVersion();
		}

		/// <summary>
		/// Retrieves the version information of the currently running application assembly.
		/// </summary>
		/// <remarks>This method loads the application's main assembly to obtain its version. The returned version may
		/// be used for display, logging, or compatibility checks.</remarks>
		/// <returns>A <see cref="System.Version"/> object representing the version of the running application. Returns a version of 0.0.0 if
		/// the version information cannot be determined.</returns>
		private static Version GetRunningVersion()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(VixenSystem.AssemblyFileName);
			Version? version = assembly.GetName().Version;
			if (version == null)
			{
				//Something bad happened if we don't have a version.
				Logging.Error("Version string is null!");
				return new Version(0, 0, 0);
			}

			return version;
		}

		/// <summary>
		/// Asynchronously retrieves the latest available development build version for Vixen from the remote bug tracking
		/// service.
		/// </summary>
		/// <remarks>This method attempts to connect to the Vixen bug tracking website to determine if a newer
		/// development build is available. If the service is unreachable or an error occurs, the method logs the error and
		/// returns an empty string. The method enforces a 5-second timeout for the HTTP request.</remarks>
		/// <returns>A string representing the latest development build version number if available; otherwise, an empty string if no
		/// newer build is found or if the request fails.</returns>
		public static async Task<int> GetLatestBuildVersionAsync()
		{
			try
			{
				if (await CheckForConnectionToWebsite())
				{
					using HttpClient wc = new HttpClient();
					wc.Timeout = TimeSpan.FromMilliseconds(5000);
					//Get Latest Build
					string getLatestDevelopmentBuild =
						await wc.GetStringAsync(
							$"http://bugs.vixenlights.com/rest/api/latest/search/jql?jql=project='Vixen 3' AND 'Fix Build Number'>{Version.Build} ORDER BY 'Fix Build Number' DESC&startAt=0&maxResults=1&fields=summary,customfield_10032");
					//This will parse the latest development build number
					dynamic developmentBuild = JObject.Parse(getLatestDevelopmentBuild);
					if (developmentBuild.issues.Count > 0)
					{
						if (developmentBuild.issues[0].fields.customfield_10032 != null)
						{
							int latestDevelopmentBuild = developmentBuild.issues[0].fields.customfield_10032;
							//This does not return an array as the results are contained in a wrapper object for paging info
							//There results are in an array called issues, with in that is a set of fields that contain our custom field 
							return latestDevelopmentBuild;
						}
					}
				}
			}
			catch (Exception e)
			{
				//Should only get here if there is no internet connection and e will stipulate that it can't get to the http://bugs.vixenlights.com website.
				Logging.Error("Checking for the latest Development Build failed - " + e);
			}
			return 0;
		}

		/// <summary>
		/// Asynchronously retrieves the version name of the latest released Vixen project from the remote server.
		/// </summary>
		/// <remarks>This method attempts to connect to the Vixen project issue tracker and fetches the most recent
		/// released version. If the server is unreachable or an error occurs during the request, the method logs the error
		/// and returns an empty string. The operation has a timeout of 5 seconds for the HTTP request.</remarks>
		/// <returns>A string containing the name of the latest released version. Returns an empty string if the version cannot be
		/// determined or if the request fails.</returns>
		public static async Task<string> GetLatestReleaseVersionAsync()
		{
			try
			{
				if (await CheckForConnectionToWebsite())
				{
					using HttpClient wc = new HttpClient();
					wc.Timeout = TimeSpan.FromMilliseconds(5000);
					//Get the Latest Release
					string getReleaseVersion =
						await wc.GetStringAsync("http://bugs.vixenlights.com/rest/api/latest/project/VIX/version?status=released&orderBy=-releaseDate&maxResults=1");
					//Query returns an array of release version objects
					dynamic releaseVersion = JObject.Parse(getReleaseVersion);
					//get the last one that has released == true as they are in asending order
					var releaseVersionName = (string)releaseVersion.values[0]["name"];
					return releaseVersionName;
				}
			}
			catch (Exception e)
			{
				//Should only get here if there is no internet connection and e will stipulate that it can't get to the http://bugs.vixenlights.com website.
				Logging.Error("Checking for the latest Release Version failed - " + e);
			}
			return "";
		}

		/// <summary>
		/// Asynchronously checks whether a connection can be established to the Vixen Lights bug tracking website.
		/// </summary>
		/// <remarks>This method attempts to connect to the specified website with a 5-second timeout. It returns <see
		/// langword="false"/> if the connection fails for any reason, including network errors or timeouts.</remarks>
		/// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if a connection to
		/// http://bugs.vixenlights.com is successful; otherwise, <see langword="false"/>.</returns>
		public static async Task<bool> CheckForConnectionToWebsite()
		{
			try
			{
				using var client = new HttpClient();
				client.Timeout = TimeSpan.FromMilliseconds(5000);
				using (await client.GetAsync("http://bugs.vixenlights.com"))
				{
					return true;
				}
			}
			catch(Exception ex)
			{
				Logging.Error(ex, "Unable to connect to the Vixen bug tracker website");
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current build is a test build.
		/// </summary>
		/// <remarks>A test build is identified by a version number where the major, minor, and build components are
		/// all zero. Use this property to conditionally enable or disable features intended only for test
		/// environments.</remarks>
		public static bool IsTestBuild => Version is { Major: 0, Minor: 0, Build: 0 };

		/// <summary>
		/// Gets a value indicating whether the current build is a development build.
		/// </summary>
		/// <remarks>A development build is identified by a major version of 0 and a build number greater than 0. This
		/// property can be used to enable or disable features specific to development environments.</remarks>
		public static bool IsDevBuild => Version is { Major: 0, Build: > 0 };
		
		/// <summary>
		/// Gets a value indicating whether the current build is a release build.
		/// </summary>
		public static bool IsReleaseBuild => Version is { Major: > 0 };

		/// <summary>
		/// Gets the current application version type as a string identifier.
		/// </summary>
		/// <remarks>Use this method to determine the build configuration of the running application at runtime. The
		/// returned value can be used for logging, diagnostics, or conditional feature toggling based on the version
		/// type.</remarks>
		/// <value>
		///   A string that indicates the current version type. Returns "Test" if the application is a test build, "Development"
		///   if it is a development build, or "Release" for all other cases.
		/// </value>
		public static string CurrentVersionType
		{
			get
			{
				if (IsTestBuild)
				{
					return "Test";
				}

				if (IsDevBuild)
				{
					return "Development";
				}

				return "Release";
			}
		}

		/// <summary>
		/// Returns the release version string if this is a release build, or just the build number if this is a dev build.
		/// </summary>
		/// <value></value>
		public static string ReleaseVersion
		{
			get
			{
				if (IsReleaseBuild)
				{
					var releaseVersion = $"{Version.Major}.{Version.Minor}";
					if (Version.Revision > 0)
					{
						releaseVersion += $@"u{Version.Revision}";
					}

					return releaseVersion;
				}
				else if (IsDevBuild)
				{
					return $@"{Version.Build}";
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// Gets a user-friendly name representing the current application version or build type.
		/// </summary>
		/// <value>
		///   A string containing the version name. Returns "Test Build" for test builds, "Development Build" for development
		///   builds, or "Release X.Y.Z" for release builds, where X.Y.Z is the release version number.
		/// </value>
		public static string VersionName
		{
			get
			{
				if (IsTestBuild)
				{
					return "Test Build";
				}
				else if (IsDevBuild)
				{
					return @$"Development Build #{BuildNumber}";
				}

				return $@"Release {ReleaseVersion}";
			}
		}

		/// <summary>
		/// Gets the build component of the current version number.
		/// </summary>
		public static int BuildNumber => Version.Build;

		/// <summary>
		/// Gets the build number of the currently running application as a formatted string.
		/// </summary>
		/// <value>
		///   A string containing the build number in the format "Build #(number)" if the build number is greater than zero;
		///   otherwise, "Build #".
		/// </value>
		public static string BuildName
		{
			get
			{
				if (BuildNumber > 0)
				{
					return $@"Build #{BuildNumber}";
				}

				return @"Build #";
			}
		}
	}
}