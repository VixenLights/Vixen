using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

			try
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				Application.ThreadException += Application_ThreadException;
			 
				bool result;
				var mutex = new System.Threading.Mutex(true, "Vixen3RunningInstance", out result);

				if (!result)
				{
					MessageBox.Show("Another instance is already running; please close that one before trying to start another.",
									"Vixen 3 already active", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new VixenApplication());

				// mutex shouldn't be released - important line
				GC.KeepAlive(mutex);
			}
			catch (Exception ex)
			{
				Logging.Fatal(ex);
			}
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Logging.Error(e.Exception);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{ 
			var e = (Exception)args.ExceptionObject;
			Logging.Error(e);
		}
	}
}