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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;


namespace Dataweb.NShape.WinFormsUI {
	
	/// <summary>
	/// Dialog used for loading NShape libraries into the current NShape project.
	/// </summary>
	[ToolboxItem(false)]
	public partial class LibraryManagementDialog : Form {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.LibraryManagementDialog" />.
		/// </summary>
		/// <param name="project"></param>
		public LibraryManagementDialog(Project project) {
			InitializeComponent();
			if (project == null) throw new ArgumentNullException("project");
			this.project = project;
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}


		private void RefreshList() {
			// Collect currently used libraries
			currentLibraries.Clear();
			foreach (Assembly assembly in project.Libraries) {
				string assemblyPath = GetAssemblyFilePath(assembly);
				currentLibraries.Add(assemblyPath, assembly);
				// Remove library from the "add" list
				if (addedLibraries.ContainsKey(assemblyPath))
					addedLibraries.Remove(assemblyPath);
			}
			// (Re-)Fill list view
			try {
				libraryListView.SuspendLayout();
				libraryListView.Items.Clear();
				foreach (KeyValuePair<string, Assembly> item in currentLibraries) {
					// Skip assemblies in the "remove" list
					if (removedLibraries.ContainsKey(item.Key)) continue;
					// Create and add list view item
					ListViewItem lvItem = CreateListViewItem(item.Key, item.Value);
					libraryListView.Items.Add(lvItem);
				}
				foreach (KeyValuePair<string, Assembly> item in addedLibraries) {
					Debug.Assert(!removedLibraries.ContainsKey(item.Key));
					// Create and add list view item
					ListViewItem lvItem = CreateListViewItem(item.Key, item.Value);
					libraryListView.Items.Add(lvItem);
				}
			} finally {
				for (int i = libraryListView.Columns.Count - 1; i >= 0; --i)
					libraryListView.Columns[i].Width = -1;
				libraryListView.ResumeLayout();
			}
		}


		private ListViewItem CreateListViewItem(string assemblyPath, Assembly assembly) {
			if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentNullException("assemblyPath");
			if (assembly == null) throw new ArgumentNullException("assembly");
			// Get assembly path, name and version
			AssemblyName assemblyName = assembly.GetName();
			ListViewItem item = new ListViewItem(assemblyName.Name);
			item.SubItems.Add(assemblyName.Version.ToString());
			item.SubItems.Add(assemblyPath);
			return item;
		}


		private string GetAssemblyFilePath(Assembly assembly) {
			// Get assembly file path
			UriBuilder uriBuilder = new UriBuilder(assembly.CodeBase);
			return Uri.UnescapeDataString(uriBuilder.Path);
		}

		
		private void LibraryManagementDialog_Load(object sender, EventArgs e) {
			project.LibraryLoaded += Project_LibraryLoaded;
			RefreshList();
		}


		private void LibraryManagementDialog_Shown(object sender, EventArgs e) {
			bool librariesLoaded = false;
			foreach (Assembly a in project.Libraries) {
				librariesLoaded = true; 
				break;
			}
			if (!librariesLoaded) addLibraryButton_Click(this, null);
		}

	
		private void LibraryManagementDialog_FormClosed(object sender, FormClosedEventArgs e) {
			project.LibraryLoaded -= Project_LibraryLoaded;
		}


		private void addLibraryButton_Click(object sender, EventArgs e) {
			openFileDialog.Filter = "Assembly Files|*.dll|All Files|*.*";
			openFileDialog.FileName = "";
			openFileDialog.Multiselect = true;
			if (string.IsNullOrEmpty(openFileDialog.InitialDirectory))
				openFileDialog.InitialDirectory = Application.StartupPath;
			
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				List<string> fileNames = new List<string>(openFileDialog.FileNames);
				for (int i = 0; i < fileNames.Count; ++i) {
					try {
						if (!project.IsValidLibrary(fileNames[i])) {
							MessageBox.Show(this, string.Format(InvalidLibraryMessage, fileNames[i]), "Invalid file type", MessageBoxButtons.OK, MessageBoxIcon.Error);
						} else {
							Assembly assembly = Assembly.LoadFile(fileNames[i]);
							string assemblyPath = GetAssemblyFilePath(assembly);
							// Remove library from the "remove" list
							if (removedLibraries.ContainsKey(assemblyPath))
								removedLibraries.Remove(assemblyPath);
							// If not currently in use, add library to the "Add" list 
							if (!currentLibraries.ContainsKey(assemblyPath)
								&& !addedLibraries.ContainsKey(assemblyPath))
								addedLibraries.Add(assemblyPath, assembly);
						}
					} catch (Exception ex) {
						RefreshList();
						MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				RefreshList();
			}
		}


		private void removeLibraryButton_Click(object sender, EventArgs e) {
			bool removeConfirmed = false, removeLibrary = false;
			for (int i = libraryListView.SelectedItems.Count - 1; i >= 0; --i) {
				string assemblyPath = libraryListView.SelectedItems[i].SubItems[2].Text;
				// If assembly is on the "add" list, remove it from there
				if (addedLibraries.ContainsKey(assemblyPath))
					addedLibraries.Remove(assemblyPath);
				// If assembly is currently in use, add it to the "remove" list
				else if (currentLibraries.ContainsKey(assemblyPath)) {
					if (!removeConfirmed) {
						string msg = "When removing a library, the undo history will be cleared. You will not be able to undo changes made until now.";
						msg += Environment.NewLine + "Do you really want to remove loaded libraries?";
						DialogResult res = MessageBox.Show(this, msg, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
						removeLibrary = (res == DialogResult.Yes);
						removeConfirmed = true;
					}
					if (removeLibrary) removedLibraries.Add(assemblyPath, currentLibraries[assemblyPath]);
				}
			}
			RefreshList();
		}

	
		private void okButton_Click(object sender, EventArgs e) {
			project.LibraryLoaded -= Project_LibraryLoaded;

			// Repaint windows under the file dialog before starting with adding libraries
			if (Owner != null) Owner.Refresh();
			Application.DoEvents();

			// Remove obsolete Libraries
			foreach (KeyValuePair<string, Assembly> item in removedLibraries) {
				try {
					project.RemoveLibrary(item.Value);
				} catch (Exception exc) {
					MessageBox.Show(this, exc.Message, "Cannot remove Library", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			// Add new libraries
			foreach (KeyValuePair<string, Assembly> item in addedLibraries) {
				try {
					project.AddLibraryByFilePath(item.Key, true);
				} catch (Exception exc) {
					MessageBox.Show(this, exc.Message, "Cannot add Library", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}

			DialogResult = DialogResult.OK;
		}


		private void Project_LibraryLoaded(object sender, LibraryLoadedEventArgs e) {
			RefreshList();
		}


		private void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = !project.IsValidLibrary(openFileDialog.FileName);
			string msg = string.Format(InvalidLibraryMessage, openFileDialog.FileName);
			if (e.Cancel) MessageBox.Show(this, msg, "Invalid file type", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}


		private Project project;
		private SortedList<string, Assembly> addedLibraries = new SortedList<string, Assembly>();
		private SortedList<string, Assembly> currentLibraries = new SortedList<string,Assembly>();
		private SortedList<string, Assembly> removedLibraries = new SortedList<string, Assembly>();

		private const string InvalidLibraryMessage = "'{0}' is not a valid NShape library.";
	}
}