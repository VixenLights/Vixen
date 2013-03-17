using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VixenApplication
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			bool result;
			var mutex = new System.Threading.Mutex(true, "Vixen3RunningInstance", out result);

			if (!result) {
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
	}
}
