using Catel.Logging;
using Vixen.Sys;
using VixenApplication.Updates;

namespace VixenApplication
{
	internal static class Program
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private const string ErrorMsg = "An application error occurred. Please contact the Vixen Dev Team " +
									"with the following information:\n\n";
		private static VixenApplication? _app;
		private static HttpClient? _githubClient;
		private static SocketsHttpHandler? _githubHandler;
		internal static string LockFilePath = string.Empty;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			try
			{
				Logging.Info("Vixen app starting.");

				LogManager.AddListener(new NLogListener());
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				Application.ThreadException += Application_ThreadException;

				// To customize application configuration such as set high DPI settings or default font,
				// see https://aka.ms/applicationconfiguration.
				ApplicationConfiguration.Initialize();
				_githubHandler = new SocketsHttpHandler
				{
					PooledConnectionLifetime = TimeSpan.FromMinutes(10)
				};
				_githubClient = new HttpClient(_githubHandler)
				{
					BaseAddress = new Uri("https://api.github.com/repos/VixenLights/Vixen/"),
					Timeout = TimeSpan.FromSeconds(5)
				};
				_githubClient.DefaultRequestHeaders.UserAgent.ParseAdd("Vixen-UpdateChecker/1.0");
				_githubClient.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
				_githubClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2026-03-10");

				_app = new VixenApplication(new GitHubUpdateService(_githubClient));
				Application.Run(_app);
			}
			catch (Exception ex)
			{
				LogMessageAndExit(ex);
			}
			finally
			{
				_githubClient?.Dispose();
				_githubHandler?.Dispose();
			}
		}

		static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			LogMessageAndExit(e.Exception);

		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			var e = (Exception)args.ExceptionObject;
			LogMessageAndExit(e);
		}

		private static void LogMessageAndExit(Exception ex)
		{
			// Since we can't prevent the app from terminating, log this to the event log. 
			Logging.Fatal(ex, ErrorMsg);
			if (VixenSystem.IsSaving())
			{
				Logging.Fatal("Save was in progress during the fatal crash. Trying to pause 5 seconds to give it a chance to complete.");
				Thread.Sleep(5000);
			}
			if (_app != null)
			{
				_app.RemoveLockFile();
			}
			else
			{
				//try the failsafe to clean up the lock file.
				VixenApplication.RemoveLockFile(LockFilePath);
			}
			Environment.Exit(1);
		}

	}
}
