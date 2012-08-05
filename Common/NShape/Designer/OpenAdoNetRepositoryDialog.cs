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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Dataweb.NShape.Designer {

	public partial class OpenAdoNetRepositoryDialog : Form {

		public OpenAdoNetRepositoryDialog(Form owner) {
			InitializeComponent();
			if (owner != null) {
				Owner = owner;
				Icon = Owner.Icon;
			}
		}


		public OpenAdoNetRepositoryDialog(Form owner, string serverName, string databaseName, Mode mode)
			: this(owner) {
			this.mode = mode;
			providerNameComboBox.SelectedIndex = 0;
			serverNameTextBox.Text = serverName;
			databaseNameComboBox.Text = databaseName;

			Cursor = Cursors.WaitCursor;
			Application.DoEvents();
			try {
				FillDatabaseNameComboBox();
				databaseNameComboBox.Text = DatabaseName;

				switch (mode) {
					case Mode.CreateSchema:
						projectNameComboBox.Visible =
							projectNameLabel.Visible = false;
						Text = "Select Database";
						break;
					case Mode.CreateProject:
					case Mode.OpenProject:
						projectNameComboBox.Visible =
							projectNameLabel.Visible = true;
						Text = "Select Project";
						break;
				}
				FillProjectNameComboBox();
			} finally { Cursor = Cursors.Default; }
		}


		public OpenAdoNetRepositoryDialog(string serverName, string databaseName, Mode mode)
			: this(null, serverName, databaseName, mode) {
		}


		public enum Mode {
			CreateSchema,
			CreateProject,
			OpenProject
		}


		public string ProviderName {
			get { return providerNameComboBox.Text; }
		}


		public string ServerName {
			get { return serverNameTextBox.Text; }
		}


		public string DatabaseName {
			get { return databaseNameComboBox.Text; }
		}


		public string ProjectName {
			get { return projectNameComboBox.Text; }
		}


		private bool CreateDatabase(string providerName, string serverName, string databaseName) {
			bool result = false;
			if (DatabaseExists(providerName, serverName, databaseName))
				MessageBox.Show("Database already exists.");
			else {
				using (DbConnection connection = GetConnection(providerName, serverName, null)) {
					connection.Open();
					try {
						DbCommand command = connection.CreateCommand();
						command.CommandText = string.Format("CREATE DATABASE {0}", databaseName);
						command.ExecuteNonQuery();

						ClearDatabaseNames();
						result = true;
					} catch (Exception exc) {
						throw exc;
					}
				}
			}
			return result;
		}


		private bool DropDatabase(string providerName, string serverName, string databaseName) {
			bool result = false;
			if (!DatabaseExists(providerName, serverName, databaseName))
				MessageBox.Show(this, "Database does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else {
				using (DbConnection connection = GetConnection(providerName, serverName, null)) {
					if (connection != null) {
						connection.Open();
						try {
							using (DbCommand command = connection.CreateCommand()) {
								command.CommandText = string.Format("DROP DATABASE {0}", databaseName);
								command.ExecuteNonQuery();

								ClearDatabaseNames();
								result = true;
							}
						} catch (Exception exc) {
							MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
			return result;
		}


		private DbConnection GetConnection(string providerName, string serverName, string databaseName) {
			DbConnection connection = null;
			if (!string.IsNullOrEmpty(providerName)
				&& !string.IsNullOrEmpty(serverName)) {
				string connectionString = string.Format(
					"Data Source={0};Initial Catalog={1};Integrated Security=True;MultipleActiveResultSets=True;Pooling=False",
					serverName, string.IsNullOrEmpty(databaseName) ? "master" : databaseName);
				connection = new SqlConnection(connectionString);
			}
			return connection;
		}


		private IEnumerable<string> GetDatabases(string providerName, string serverName, string databaseName) {
			Cursor = Cursors.WaitCursor;
			Application.DoEvents();
			try {
				using (DbConnection connection = GetConnection(providerName, serverName, databaseName)) {
					foreach (string dbname in GetDatabases(connection))
						yield return dbname;
				}
			} finally { Cursor = Cursors.Default; }
		}


		private IEnumerable<string> GetDatabases(DbConnection connection) {
			if (connection != null) {
				connection.Open();
				using (DataTable databases = connection.GetSchema("Databases")) {
					foreach (DataRow row in databases.Rows)
						yield return row[0].ToString();
				}
			}
		}


		private bool DatabaseExists(DbConnection connection, string databaseName) {
			Cursor = Cursors.WaitCursor;
			Application.DoEvents();
			try {
				bool result = false;
				foreach (string dbName in GetDatabases(connection)) {
					if (databaseName == dbName) {
						result = true;
						break;
					}
				}
				return result;
			} finally { Cursor = Cursors.Default; }
		}


		private bool DatabaseExists(string providerName, string serverName, string databaseName) {
			using (DbConnection connection = GetConnection(providerName, serverName, null))
				return DatabaseExists(connection, databaseName);
		}


		private void ClearDatabaseNames() {
			databaseNameComboBox.Items.Clear();
		}


		private void ClearProjectNames() {
			if (projectNameComboBox.Visible) {
				projectNameComboBox.Items.Clear();
				projectNameComboBox.Text = string.Empty;
			}
		}


		private void FillDatabaseNameComboBox() {
			if (!string.IsNullOrEmpty(ProviderName)
				&& !string.IsNullOrEmpty(ServerName)) {
				try {
					foreach (string databaseName in GetDatabases(ProviderName, ServerName, null))
						databaseNameComboBox.Items.Add(databaseName);
				} catch (Exception exc) {
					MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}


		private void FillProjectNameComboBox() {
			if (projectNameComboBox.Visible
				&& !string.IsNullOrEmpty(ProviderName)
				&& !string.IsNullOrEmpty(ServerName)
				&& !string.IsNullOrEmpty(DatabaseName)) {
				string connectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;MultipleActiveResultSets=True;Pooling=False", ServerName, DatabaseName);
				try {
					using (SqlConnection connection = new SqlConnection(connectionString)) {
						connection.Open();
						using (SqlCommand command = connection.CreateCommand()) {
							command.CommandText = "SELECT * FROM Project INNER JOIN ProjectInfo ON Project.ProjectInfo = ProjectInfo.Id ORDER BY LastSavedUtc DESC";
							try {
								using (IDataReader reader = command.ExecuteReader()) {
									projectNameComboBox.Items.Clear();
									while (reader.Read()) {
										string projectname = reader.GetString(reader.GetOrdinal("Name"));
										if (!string.IsNullOrEmpty(projectname)) {
											projectNameComboBox.Items.Add(projectname);
											if (mode == Mode.OpenProject
												&& string.IsNullOrEmpty(projectNameComboBox.Text))
												projectNameComboBox.Text = projectname;
										}
									}
								}
							} catch (SqlException) {
								// If the database server does not exist, the database does not exist 
								// or the database scheme does not exist, we will end up here.+
							}
						}
					}
				} catch (Exception exc) {
					MessageBox.Show(this, exc.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}


		#region Event handler implementations

		private void OpenAdoNetRepositoryDialog_Load(object sender, EventArgs e) {
			//Control ctrl = Owner ?? Parent ?? this;
			//ctrl.Cursor = Cursors.WaitCursor;
			//Application.DoEvents();
			//try {
			//   FillDatabaseNameComboBox();
			//   databaseNameComboBox.Text = DatabaseName;

			//   switch (mode) {
			//      case Mode.CreateSchema:
			//         projectNameComboBox.Visible =
			//            projectNameLabel.Visible = false;
			//         Text = "Select Database";
			//         break;
			//      case Mode.CreateProject:
			//      case Mode.OpenProject:
			//         projectNameComboBox.Visible =
			//            projectNameLabel.Visible = true; ;
			//         Text = "Select Project";
			//         break;
			//   }
			//   FillProjectNameComboBox();
			//} finally { ctrl.Cursor = Cursors.Default; }
		}


		private void serverNameTextBox_TextChanged(object sender, EventArgs e) {
			ClearDatabaseNames();
			ClearProjectNames();
		}


		private void providerNameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			ClearProjectNames();
		}


		private void projectNameComboBox_DropDown(object sender, EventArgs e) {
			if (projectNameComboBox.Items.Count == 0)
				FillProjectNameComboBox();
		}


		private void databaseNameComboBox_TextChanged(object sender, EventArgs e) {
			ClearProjectNames();
		}


		private void databaseNameComboBox_DropDown(object sender, EventArgs e) {
			if (databaseNameComboBox.Items.Count <= 0)
				FillDatabaseNameComboBox();
		}


		private void createDbButton_Click(object sender, EventArgs e) {
			if (mode == Mode.CreateSchema) {
				if (CreateDatabase(ProviderName, ServerName, DatabaseName))
					MessageBox.Show(this, "Database successfully created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			} else MessageBox.Show(this, useDBGeneratorMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}


		private void dropDbButton_Click(object sender, EventArgs e) {
			if (mode == Mode.CreateSchema) {
				if (DropDatabase(ProviderName, ServerName, DatabaseName))
					MessageBox.Show(this, "Database successfully dropped.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			} else MessageBox.Show(this, useDBGeneratorMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}


		private void okButton_Click(object sender, EventArgs e) {
			if (DatabaseExists(ProviderName, ServerName, DatabaseName))
				DialogResult = DialogResult.OK;
			else {
				DialogResult result = MessageBox.Show(
					string.Format("Database '{0}' does not exist. Do you ant to create the database?", DatabaseName),
					"Create Database", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (result) {
					case DialogResult.Cancel:
						DialogResult = DialogResult.None;
						break;
					case DialogResult.Yes:
						CreateDatabase(ProviderName, ServerName, DatabaseName);
						DialogResult = DialogResult.OK;
						break;
					case DialogResult.No:
						DialogResult = DialogResult.OK;
						break;
				}
			}
		}

		#endregion


		private Mode mode;
		private const string useDBGeneratorMsg = "Please use 'Tools / AdoNetStore.NET Database Generator' to drop or create a new database for the project.";
	}

}