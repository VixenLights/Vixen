using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenApplication
{
	internal static class Program
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			bool result;
			var mutex = new System.Threading.Mutex(true, "Vixen3RunningInstance", out result);

			if (!result)
			{
				MessageBox.Show(@"Another instance is already running; please close that one before trying to start another.",
					@"Vixen 3 already active", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// Add the event handler for handling UI thread exceptions to the event.
			Application.ThreadException += UIThreadException;

			// Set the unhandled exception mode to force all Windows Forms errors to go through 
			// our handler.
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

			// Add the event handler for handling non-UI thread exceptions to the event. 
			AppDomain.CurrentDomain.UnhandledException +=
				CurrentDomain_UnhandledException;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new VixenApplication());

			// mutex shouldn't be released - important line
			GC.KeepAlive(mutex);
		}

		// Handle the UI exceptions 
		private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
		{	
			LogMessageAndExit(t.Exception);
		}

		// Handle the other exceptions 
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			
			LogMessageAndExit((Exception)e.ExceptionObject);
		}

		private static void LogMessageAndExit(Exception ex)
		{
			const string errorMsg = "An application error occurred. Please contact the Vixen Dev Team " +
									"with the following information:\n\n";

			// Since we can't prevent the app from terminating, log this to the event log. 
			Logging.ErrorException(errorMsg, ex);
			Environment.Exit(1);	
		}
	}

}