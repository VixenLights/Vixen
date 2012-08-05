/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Windows.Forms;


namespace Dataweb.NShape.Designer {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			Application.Run(new DiagramDesignerMainForm());
		}


		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
			if (e.Exception is System.ComponentModel.WarningException) throw e.Exception;
			else {
				string errorMessage = e.Exception.Message + Environment.NewLine + "Do you want to terminate the application?";
				if (MessageBox.Show(errorMessage, "Unhandled Exception", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
					Application.Exit();
			}
		}
	}
}