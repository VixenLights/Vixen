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

	public partial class TestDataGeneratorDialog : Form {

		public TestDataGeneratorDialog(Project project) {
			InitializeComponent();
			if (project == null) throw new ArgumentNullException("project");
			this.project = project;

			// ToDo: Check if all needed libraries are loaded 

			diagramNameTextBox.Text = string.Format("Diagram {0}", GetDiagramCount() + 1);
			shapeSizeUpDown.Value = 80;
			shapesPerRowUpDown.Value = 10;
			shapesPerColUpDown.Value = 10;
			connectShapesChk.Checked = true;
			useLayersChk.Checked = true;
			// ToDo: Deactivate check boxes when ModelObject library is not loaded
			useModelObjectsChk.Checked = false;
			useModelMappingsChk.Checked = false;
		}


		private int GetDiagramCount() {
			int diagramCnt = 0;
			foreach (Diagram diagram in project.Repository.GetDiagrams())
				++diagramCnt;
			return diagramCnt;
		}


		private void TestDataGeneratorDialog_FormClosing(object sender, FormClosingEventArgs e) {
			if (DialogResult == System.Windows.Forms.DialogResult.OK) {
				Owner.Refresh();
				try {
					Cursor = Cursors.WaitCursor;
					string diagramName = diagramNameTextBox.Text;
					if (string.IsNullOrEmpty(diagramName))
						diagramName = string.Format("Diagram {0}", GetDiagramCount() + 1);

					// Create test diagram
					TestDataGenerator.CreateDiagram(project,
						diagramName,
						(int)shapeSizeUpDown.Value,
						(int)shapesPerRowUpDown.Value,
						(int)shapesPerColUpDown.Value,
						connectShapesChk.Checked,
						useModelObjectsChk.Checked,
						useModelObjectsChk.Checked ? useModelMappingsChk.Checked : false,
						useLayersChk.Checked);
				} catch (Exception exc) {
					MessageBox.Show(this, exc.Message, "Error while creating test data", MessageBoxButtons.OK, MessageBoxIcon.Error);
				} finally {
					Cursor = Cursors.Default;
				}
			}
		}


		private void TestDataGeneratorDialog_FormClosed(object sender, FormClosedEventArgs e) {
		}


		private Project project = null;
	}

}
