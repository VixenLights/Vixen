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
using System.ComponentModel;
using System.Windows.Forms;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI {

	/// <summary>
	/// A dialog used for creating and editing designs and styles
	/// </summary>
	[ToolboxItem(false)]
	public partial class DesignEditorDialog : Form {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.DesignEditorDialog" />.
		/// </summary>
		public DesignEditorDialog() {
			SetStyle(ControlStyles.ResizeRedraw
				| ControlStyles.AllPaintingInWmPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.SupportsTransparentBackColor
				, true);
			UpdateStyles();
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);

			// Initialize Components
			InitializeComponent();
			RegisterDesignControllerEventHandlers();
			RegisterDesignPresenterEventHandlers();
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.DesignEditorDialog" />.
		/// </summary>
		public DesignEditorDialog(Project project)
			: this() {
			if (project == null) throw new ArgumentNullException("project");
			this.Project = project;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.DesignEditorDialog" />.
		/// </summary>
		public DesignEditorDialog(Project project, StyleCategory styleCategory)
			: this(project) {
			designPresenter.SelectedStyleCategory = styleCategory;
		}


		/// <summary>
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		[Category("NShape")]
		public Project Project {
			get { return designController.Project; }
			set { 
				designController.Project = value;
				if (designController.Project != null) {
					// Deactivate Creating new Designs for XMLStore because currently it does 
					// not implement saving multiple designs.
					if (designController.Project.Repository is CachedRepository
						&& ((CachedRepository)designController.Project.Repository).Store is XmlStore) {
						newDesignButton.Enabled = false;
						newDesignButton.ToolTipText = "XML Stores do not support multiple designs.";
					}
				}
			}
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				// Unregister all registered event handlers
				UnregisterDesignPresenterEventHandlers();
				UnregisterDesignControllerEventHandlers();
				if (Project != null) Project = null;

				if (components != null) components.Dispose();
			}
			base.Dispose(disposing);
		}


		private void InitializeDesignList() {
			designsComboBox.Items.Clear();
			foreach (Design design in Project.Repository.GetDesigns())
				if (design != null) designsComboBox.Items.Add(design);
		}


		private void ClearDesignList() {
			designsComboBox.Items.Clear();
		}


		private void SetButtonStates() {
			designsComboBox.Enabled = false;
			for (int i = 0; i < designsComboBox.Items.Count; ++i) {
				if (designsComboBox.Items[i] != Project.Design) {
					designsComboBox.Enabled = true;
					break;
				}
			}
			if (designPresenter != null) {
				if (designPresenter.Project != null)
					activateButton.Enabled = designsComboBox.SelectedItem != designPresenter.Project.Design;
				deleteDesignButton.Enabled = designController.CanDelete(designPresenter.SelectedDesign);
				deleteStyleButton.Enabled = designController.CanDelete(designPresenter.SelectedDesign, designPresenter.SelectedStyle);
			}
		}


		#region [Private] Methods: (Un)Registering event handlers

		private void RegisterDesignControllerEventHandlers() {
			designController.Initialized += designController_Initialized;
			designController.Uninitialized += designController_Uninitialized;
			designController.DesignCreated += designController_DesignCreated;
			designController.DesignChanged += designController_DesignChanged;
			designController.DesignDeleted += designController_DesignDeleted;
			designController.StyleCreated += designController_StyleCreated;
			designController.StyleChanged += designController_StyleChanged;
			designController.StyleDeleted += designController_StyleDeleted;
		}


		private void UnregisterDesignControllerEventHandlers() {
			designController.Initialized -= designController_Initialized;
			designController.Uninitialized -= designController_Uninitialized;
			designController.DesignCreated -= designController_DesignCreated;
			designController.DesignChanged -= designController_DesignChanged;
			designController.DesignDeleted -= designController_DesignDeleted;
			designController.StyleCreated -= designController_StyleCreated;
			designController.StyleChanged -= designController_StyleChanged;
			designController.StyleDeleted -= designController_StyleDeleted;
		}


		private void RegisterDesignPresenterEventHandlers() {
			designPresenter.DesignSelected += designPresenter_DesignSelected;
			designPresenter.StyleSelected += designPresenter_StyleSelected;
		}


		private void UnregisterDesignPresenterEventHandlers() {
			designPresenter.DesignSelected -= designPresenter_DesignSelected;
			designPresenter.StyleSelected -= designPresenter_StyleSelected;
		}

		#endregion


		#region [Private] Methods: DesignController event handler implementations

		private void designController_Initialized(object sender, EventArgs e) {
			InitializeDesignList();
			designPresenter.SelectedDesign = Project.Design;
		}


		private void designController_Uninitialized(object sender, EventArgs e) {
			ClearDesignList();
		}


		private void designController_DesignCreated(object sender, DesignEventArgs e) {
			if (designsComboBox.Items != null)
				designsComboBox.SelectedIndex = designsComboBox.Items.Add(e.Design);
		}


		private void designController_DesignChanged(object sender, DesignEventArgs e) {
			// nothing to do
		}


		private void designController_DesignDeleted(object sender, DesignEventArgs e) {
			if (designsComboBox.Items.Contains(e.Design))
				designsComboBox.Items.Remove(e.Design);
			designsComboBox.SelectedItem = Project.Design;
		}


		private void designController_StyleCreated(object sender, StyleEventArgs e) {
			// nothing to do
		}


		private void designController_StyleChanged(object sender, StyleEventArgs e) {
			// nothing to do
		}


		private void designController_StyleDeleted(object sender, StyleEventArgs e) {
			// nothing to do
		}

		#endregion


		#region [Private] Methods: DesignPresenter event handler implementations

		private void designPresenter_StyleSelected(object sender, EventArgs e) {
			SetButtonStates();
		}


		private void designPresenter_DesignSelected(object sender, EventArgs e) {
			StyleUITypeEditor.Design = designPresenter.SelectedDesign;
			designsComboBox.SelectedItem = designPresenter.SelectedDesign;
			SetButtonStates();
		}

		#endregion


		#region [Private] Methods: Event handler implementations

		private void designsComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			designPresenter.SelectedDesign = designsComboBox.SelectedItem as Design;
			SetButtonStates();
		}


		private void activateButton_Click(object sender, EventArgs e) {
			designPresenter.ActivateDesign(designPresenter.SelectedDesign);
		}
		
		
		private void newDesignButton_Click(object sender, EventArgs e) {
			designController.CreateDesign();
		}


		private void deleteDesignButton_Click(object sender, EventArgs e) {
			if (MessageBox.Show("All styles in this design will be lost." + Environment.NewLine + "Do you really want do to delete this design?", "Delete Design", MessageBoxButtons.YesNo) == DialogResult.Yes)
				designPresenter.DeleteSelectedDesign();
		}


		private void newStyleButton_Click(object sender, EventArgs e) {
			designPresenter.CreateStyle();
		}


		private void deleteStyleButton_Click(object sender, EventArgs e) {
			if (MessageBox.Show("Do you really want do to delete this style?", "Delete Style", MessageBoxButtons.YesNo) == DialogResult.Yes)
				designPresenter.DeleteSelectedStyle();
		}


		private void closeButton_Click(object sender, EventArgs e) {
			if (Modal) DialogResult = DialogResult.OK;
			else Close();
		}

		
		private void DesignEditorDialog_FormClosed(object sender, FormClosedEventArgs e) {
			Project = null;
		}

		#endregion

	}
}