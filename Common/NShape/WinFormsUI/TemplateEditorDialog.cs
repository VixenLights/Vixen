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


namespace Dataweb.NShape.WinFormsUI
{
	/// <summary>
	/// Dialog used for creating and editing templates.
	/// </summary>
	[ToolboxItem(false)]
	public partial class TemplateEditorDialog : Form
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TemplateEditorDialog" />.
		/// </summary>
		public TemplateEditorDialog()
		{
			InitializeComponent();
			DoubleBuffered = true;
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TemplateEditorDialog" />.
		/// </summary>
		public TemplateEditorDialog(Project project, Template template)
			: this()
		{
			if (project == null) throw new ArgumentNullException("project");
			templateController.Initialize(project, template);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TemplateEditorDialog" />.
		/// </summary>
		public TemplateEditorDialog(Project project)
			: this(project, null)
		{
		}


		/// <summary>
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		[Category("NShape")]
		public Project Project
		{
			get { return templateController.Project; }
			set { templateController.Project = value; }
		}


		/// <summary>
		/// Specifies the template being edited.
		/// </summary>
		public Template Template
		{
			get { return template; }
			set
			{
				template = value;
				if (template != null && Project != null)
					templateController.Initialize(Project, template);
			}
		}


		private void EnableButtons()
		{
			if (templateController.Project != null) {
				okButton.Enabled =
					applyButton.Enabled =
					(templatePresenter.TemplateWasModified
					 && Project.SecurityManager.IsGranted(Permission.Templates));
			}
			else {
				okButton.Enabled =
					applyButton.Enabled = false;
			}
			cancelButton.Enabled = true;
		}

		#region [Private] Methods: TemplateController event handler implementations

		private void templateController_TemplateShapeChanged(object sender, TemplateControllerTemplateShapeReplacedEventArgs e)
		{
			EnableButtons();
		}


		private void templateController_TemplateModelObjectChanged(object sender,
		                                                           TemplateControllerModelObjectReplacedEventArgs e)
		{
			EnableButtons();
		}


		private void templateController_TemplateShapeControlPointMappingModified(object sender,
		                                                                         TemplateControllerPointMappingChangedEventArgs
		                                                                         	e)
		{
			EnableButtons();
		}


		private void templateController_TemplateShapePropertyMappingDeleted(object sender,
		                                                                    TemplateControllerPropertyMappingChangedEventArgs
		                                                                    	e)
		{
			EnableButtons();
		}


		private void templateController_TemplateShapePropertyMappingSet(object sender,
		                                                                TemplateControllerPropertyMappingChangedEventArgs e)
		{
			EnableButtons();
		}


		private void templateController_TemplateModified(object sender, EventArgs e)
		{
			EnableButtons();
		}

		#endregion

		#region [Private] Methods: Form and control event handler implementations

		private void TemplateEditorDialog_Shown(object sender, EventArgs e)
		{
			EnableButtons();
		}


		private void TemplateEditorDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			templateController.Clear();
			templateController.Project = null;
			template = null;
		}


		private void applyButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(templatePresenter.TemplateController.WorkTemplate.Name))
				MessageBox.Show(this, "The template name must not be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else {
				if (templatePresenter.TemplateWasModified)
					templatePresenter.ApplyChanges();
				EnableButtons();
			}
		}


		private void okButton_Click(object sender, EventArgs e)
		{
			applyButton_Click(sender, e);
			if (!string.IsNullOrEmpty(templatePresenter.TemplateController.WorkTemplate.Name)) {
				if (Modal) DialogResult = DialogResult.OK;
				else Close();
			}
		}


		private void cancelButton_Click(object sender, EventArgs e)
		{
			templatePresenter.DiscardChanges();
			if (Modal) DialogResult = DialogResult.Cancel;
			else Close();
		}

		#endregion

		private Template template = null;
	}
}