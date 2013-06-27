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
using System.Drawing;
using System.Windows.Forms;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI
{
	/// <ToBeCompleted></ToBeCompleted>
	[ToolboxItem(false)]
	public partial class ShapeInfoDialog : Form
	{
		/// <summary>
		/// Displays a dialog showing various information on the given shape:
		/// Template, shape type, shape library and the shape's control points including their capabilities and connected shapes.
		/// </summary>
		public ShapeInfoDialog(Project project, Shape shape)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (shape == null) throw new ArgumentNullException("shape");
			InitializeComponent();

			this.project = project;

			Rectangle shapeBounds = shape.GetBoundingRectangle(false);
			this.shape = shape;
			this.shapeClone = shape.Clone();
			this.shapeClone.Fit(0, 0, shapeBounds.Width, shapeBounds.Height);

			this.diagram.Size = shapeBounds.Size;
			this.diagram.Shapes.Add(shapeClone);

			diagramSetController.Project = project;
			display.DrawDiagramSheet = false;
			display.Diagram = diagram;
			display.ShowGrid = false;
			display.GripSize = 5;
			display.HighQualityRendering = true;
			display.RenderingQualityHighQuality = RenderingQuality.MaximumQuality;
			display.CurrentTool = tool;

			UpdateShapeInfo();
		}


		private void UpdateShapeInfo()
		{
			templateNameLbl.Text = (shape.Template != null) ? shape.Template.Title : string.Empty;
			shapeTypeLbl.Text = shape.Type.Name;
			libraryNameLbl.Text = shape.Type.LibraryName;
			fullNameLbl.Text = shape.Type.FullName;

			permissionsLbl.Text = string.Empty;
			foreach (Permission permission in Enum.GetValues(typeof (Permission))) {
				if (permission == Permission.All || permission == Permission.None)
					continue;
				if (project.SecurityManager.IsGranted(permission, shape.SecurityDomainName)) {
					if (permissionsLbl.Text.Length > 0) permissionsLbl.Text += ", ";
					permissionsLbl.Text += permission.ToString();
				}
			}
			if (string.IsNullOrEmpty(permissionsLbl.Text))
				permissionsLbl.Text = "None";

			foreach (ControlPointId ptId in shape.GetControlPointIds(ControlPointCapabilities.All)) {
				string idTxt = ptId.ToString();
				ListViewItem item = ctrlPointListView.Items.Add(idTxt, idTxt);
				item.SubItems[0].Text = idTxt;
				item.SubItems.Add(new ListViewItem.ListViewSubItem(item, GetControlPointCapabilities(ptId)));
				item.SubItems.Add(new ListViewItem.ListViewSubItem(item, GetConnectedShapes(ptId)));
			}
		}


		private string GetControlPointCapabilities(ControlPointId id)
		{
			ControlPointCapabilities capabilities = ControlPointCapabilities.None;
			foreach (ControlPointCapabilities capability in Enum.GetValues(typeof (ControlPointCapabilities))) {
				if (capability == ControlPointCapabilities.All || capability == ControlPointCapabilities.None)
					continue;
				if (shape.HasControlPointCapability(id, capability))
					capabilities |= capability;
			}
			return capabilities.ToString();
		}


		private string GetConnectedShapes(ControlPointId id)
		{
			//msgTxt += "Shape is connected to" + Environment.NewLine;
			string result = string.Empty;
			foreach (ShapeConnectionInfo sci in shape.GetConnectionInfos(id, null))
				result += string.Format("{0}{1} with Point {2}",
				                        (result.Length > 0) ? ", " : "",
				                        (sci.OtherShape.Template != null)
				                        	? sci.OtherShape.Template.ToString()
				                        	: sci.OtherShape.Type.Name,
				                        sci.OtherPointId);
			return result;
		}


		private void ctrlPointListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedPtId = ControlPointId.None;
			if (ctrlPointListView.SelectedItems.Count > 0)
				int.TryParse(ctrlPointListView.SelectedItems[0].SubItems[0].Text, out selectedPtId);
			tool.SelectedPointId = selectedPtId;
			display.Invalidate();
		}


		private Project project = null;
		private Shape shape = null;
		private Shape shapeClone = null;
		private Diagram diagram = new Diagram("");
		private PointHighlightingTool tool = new PointHighlightingTool();
	}


	internal class PointHighlightingTool : Tool
	{
		public ControlPointId SelectedPointId { get; set; }

		public override void EnterDisplay(IDiagramPresenter diagramPresenter)
		{
			// Nothing to do
		}

		public override void LeaveDisplay(IDiagramPresenter diagramPresenter)
		{
			// Nothing to do
		}

		public override IEnumerable<MenuItemDef> GetMenuItemDefs(IDiagramPresenter diagramPresenter)
		{
			// Nothing to do
			yield break;
		}

		public override void Invalidate(IDiagramPresenter diagramPresenter)
		{
			// Nothing to do
		}

		public override void Draw(IDiagramPresenter diagramPresenter)
		{
			Shape shape = null;
			if (diagramPresenter.Diagram != null && diagramPresenter.Diagram.Shapes.Count > 0)
				shape = diagramPresenter.Diagram.Shapes.TopMost;
			if (shape != null) {
				diagramPresenter.ResetTransformation();
				try {
					foreach (ControlPointId id in shape.GetControlPointIds(ControlPointCapabilities.All)) {
						if (id == ControlPointId.Reference) continue;
						IndicatorDrawMode drawMode = (SelectedPointId == id) ? IndicatorDrawMode.Highlighted : IndicatorDrawMode.Normal;
						if (shape.HasControlPointCapability(id, ControlPointCapabilities.Resize))
							diagramPresenter.DrawResizeGrip(drawMode, shape, id);
						if (shape.HasControlPointCapability(id, ControlPointCapabilities.Connect | ControlPointCapabilities.Glue))
							diagramPresenter.DrawConnectionPoint(drawMode, shape, id);
						if (shape.HasControlPointCapability(id, ControlPointCapabilities.Rotate))
							diagramPresenter.DrawRotateGrip(drawMode, shape, id);
					}
				}
				finally {
					diagramPresenter.RestoreTransformation();
				}
			}
		}


		public override void RefreshIcons()
		{
			// Nothing to do
		}

		protected override void CancelCore()
		{
			// Nothing to do
		}
	}
}