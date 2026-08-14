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
