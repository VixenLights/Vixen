using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Catel.Logging;
using Common.Controls;
using Vixen.Sys;

namespace VixenApplication
{
	internal static class Program
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private const string ErrorMsg = "An application error occurred. Please contact the Vixen Dev Team " +
									"with the following information:\n\n";
		private static VixenApplication _app;
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
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				_app = new VixenApplication();
				Application.Run(_app);
			}
			catch (Exception ex)
			{
				LogMessageAndExit(ex);
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