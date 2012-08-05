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

	public partial class DiagramSettingsForm : Form {

		public DiagramSettingsForm() {
			InitializeComponent();
		}


		public DiagramSettingsForm(Form owner)
			: this() {
			if (owner != null) {
				Owner = owner;
				Icon = Owner.Icon;
			}
		}


		public DiagramSettingsForm(Form owner, Diagram diagram)
			: this(owner) {
			propertyGrid.SelectedObject = diagram;
		}


		public DiagramSettingsForm(Diagram diagram)
			: this(null, diagram) {
		}


		private void cancelButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}


		private void okButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
	}
}