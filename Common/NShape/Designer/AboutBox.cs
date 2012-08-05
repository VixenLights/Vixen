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
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Dataweb.NShape.WinFormsUI;


namespace Dataweb.NShape.Designer {

	partial class AboutBox : Form {
		
		public AboutBox() {
			InitializeComponent();
			this.Text = String.Format("About {0}", AssemblyTitle);
			this.labelProductName.Text = AssemblyProduct;
			this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
			this.labelCopyright.Text = AssemblyCopyright;
			this.textBoxDescription.Text = AssemblyDescription;
			this.textBoxDescription.Text += Environment.NewLine + Environment.NewLine ;

			this.textBoxDescription.Text += "NShape Framework Version Info:" + Environment.NewLine;
			AssemblyName coreAssemblyName = typeof(Project).Assembly.GetName();
			this.textBoxDescription.Text += 
				coreAssemblyName.Name + ": " + coreAssemblyName.Version.ToString() 
				+ Environment.NewLine;

			AssemblyName uiAssemblyName = typeof(Display).Assembly.GetName();
			this.textBoxDescription.Text += 
				uiAssemblyName.Name + ": " + uiAssemblyName.Version.ToString() 
				+ Environment.NewLine;
		}


		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start("http://www.dataweb.de");
		}


		#region Assembly Attribute Accessors

		public string AssemblyTitle {
			get {
				object[] attributes = Attribute.GetCustomAttributes(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0) {
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "") return titleAttribute.Title;
				}
				return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion {
			get {
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription {
			get {
				object[] attributes = Attribute.GetCustomAttributes(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0) return "";
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct {
			get {
				object[] attributes = Attribute.GetCustomAttributes(Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0) return "";
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright {
			get {
				object[] attributes = Attribute.GetCustomAttributes(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0) return "";
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany {
			get {
				object[] attributes = Attribute.GetCustomAttributes(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0) return "";
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
		#endregion

	}
}
