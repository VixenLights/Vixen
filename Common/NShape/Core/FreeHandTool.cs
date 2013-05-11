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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Timers;

using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.Advanced {

#if EXPERIMENTAL
	/// <summary>
	/// Lets the user sketch a shape using a pen.
	/// </summary>
	public class FreeHandTool : Tool {

		/// <ToBeCompleted></ToBeCompleted>
		public FreeHandTool(Project project)
			: base("Standard") {
			Construct(project);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public FreeHandTool(Project project, string category)
			: base(category) {
			Construct(project);
		}


		#region Tool Implementation

		/// <override></override>
		public override void RefreshIcons() {
			// nothing to do
		}


		/// <override></override>
		public override bool ProcessMouseEvent(IDiagramPresenter diagramPresenter, MouseEventArgsDg e) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			bool result = false;

			MouseState newMouseState = MouseState.Empty;
			newMouseState.Buttons = e.Buttons;
			newMouseState.Modifiers = e.Modifiers;
			diagramPresenter.ControlToDiagram(e.Position, out newMouseState.Position);

			diagramPresenter.SuspendUpdate();
			try {
				switch (e.EventType) {
					case MouseEventType.MouseDown:
						timer.Stop();
						break;

					case MouseEventType.MouseMove:
						if (CurrentMouseState.Position != newMouseState.Position) {
							if (newMouseState.IsButtonDown(MouseButtonsDg.Left)
								&& diagramPresenter.Project.SecurityManager.IsGranted(Permission.Insert)) {
								diagramPresenter.ControlToDiagram(e.Position, out p);
								currentStroke.Add(p.X, p.Y);
							}
							diagramPresenter.SetCursor(penCursorId);
						}
						Invalidate(diagramPresenter);
						break;

					case MouseEventType.MouseUp:
						if (newMouseState.IsButtonDown(MouseButtonsDg.Left)
							&& diagramPresenter.Project.SecurityManager.IsGranted(Permission.Insert)) {
							StartToolAction(diagramPresenter, 0, newMouseState, false);

							strokeSet.Add(currentStroke);
							currentStroke = new Stroke();
							timer.Start();
						}
						break;

					default: throw new NShapeUnsupportedValueException(e.EventType);
				}
			} finally { diagramPresenter.ResumeUpdate(); }
			base.ProcessMouseEvent(diagramPresenter, e);
			return result;
		}


		/// <override></override>
		public override bool ProcessKeyEvent(IDiagramPresenter diagramPresenter, KeyEventArgsDg e) {
			// nothing to do
			return false;
		}


		/// <override></override>
		public override void EnterDisplay(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			// nothing to do
		}


		/// <override></override>
		public override void LeaveDisplay(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			// nothing to do
		}


		/// <override></override>
		public override void Draw(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			diagramPresenter.ResetTransformation();
			try {
				// draw stroke(s)
				foreach (Stroke stroke in strokeSet) {
					for (int i = 0; i < stroke.Count - 1; ++i)
						diagramPresenter.DrawLine(stroke[i], stroke[i + 1]);
				}
				// draw stroke(s)
				for (int i = 0; i < currentStroke.Count - 1; ++i)
					diagramPresenter.DrawLine(currentStroke[i], currentStroke[i + 1]);
			} finally { diagramPresenter.RestoreTransformation(); }
		}


		/// <override></override>
		public override void Invalidate(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			int x = int.MaxValue;
			int y = int.MaxValue;
			int width = int.MinValue;
			int height = int.MinValue;
			if (strokeSet.Count > 0)
				GetStrokeSetBounds(out x, out y, out width, out height);

			// invalidate Stroke(s)
			foreach (Point p in currentStroke) {
				if (p.X < x) x = p.X;
				if (p.Y < y) y = p.Y;
				if (p.X > x + width) width = p.X - x;
				if (p.Y > y + height) height = p.Y - y;
			}
			if (diagramPresenter != null) {
				diagramPresenter.ControlToDiagram(rect, out rect);
				if (strokeSet.Count > 0 || currentStroke.Count > 0)
					diagramPresenter.InvalidateDiagram(x, y, width, height);
			}
		}


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			yield break;
		}


		///// <override></override>
		//public override Cursor Cursor {
		//   get { return penCursor; }
		//}


		/// <override></override>
		protected override void CancelCore() {
			//InvalidatePreview(currentDisplay);
			currentStroke.Clear();
			strokeSet.Clear();
		}


		#endregion


		/// <override></override>
		public override void Dispose() {
			base.Dispose();

			timer.Stop();
			timer.Elapsed -= timer_Tick;
			timer.Dispose();
		}


		static FreeHandTool() {
			penCursorId = CursorProvider.RegisterCursor(Properties.Resources.PenCursor);
		}


		private void Construct(Project project) {
			if (project == null) throw new ArgumentNullException("project");

			Title = "Freehand Pen";
			ToolTipText = "Draw the symbol of the object which should be created.";

			SmallIcon = global::Dataweb.NShape.Properties.Resources.FreehandIconSmall;
			SmallIcon.MakeTransparent(Color.Fuchsia);
			LargeIcon = global::Dataweb.NShape.Properties.Resources.FreehandIconLarge;
			LargeIcon.MakeTransparent(Color.Fuchsia);

			polygone = new PathFigureShape();
			strokeSet = new StrokeSequence();
			currentStroke = new Stroke();
			shaper = new Shaper();

			timer = new Timer();
			timer.Enabled = false;
			timer.Interval = timeOut;
			timer.Elapsed += timer_Tick;

			this.project = project;
			project.LibraryLoaded += project_LibraryLoaded;
			RegisterFigures();
		}


		private void project_LibraryLoaded(object sender, LibraryLoadedEventArgs e) {
			RegisterFigures();
		}


		private void RegisterFigures() {
			foreach (ShapeType shapeType in project.ShapeTypes)
				if (!shaper.IsRegistered(shapeType.FullName))
					shaper.RegisterFigure(shapeType.FullName, shapeType.FreehandReferenceImage);
		}


		private void timer_Tick(object sender, EventArgs e) {
			timer.Stop();
			IdentifyFigure(ActionDiagramPresenter);
		}


		private void IdentifyFigure(IDiagramPresenter diagramPresenter) {
			// Das ShapeSet berechnen
			Figure shapeSet = shaper.IdentifyShapes(strokeSet);

			// FeedBack
			foreach (FigureShape s in shapeSet.Shapes) {
				if (s == null)
					Console.WriteLine("NotSupported");
				else
					Console.WriteLine(s.Description);
			}
			Console.Write("=> ");

			Figure figure = shaper.FindFigure(shapeSet);
			List<string> figureNames = new List<string>();
			if (figure != null) {
				figureNames.AddRange(shaper.GetFigureNames(figure));
				Console.WriteLine(figureNames.ToString());
			} else
				Console.Write("No idea" + Environment.NewLine);

			if (diagramPresenter != null && figureNames.Count > 0) {
				if (diagramPresenter.Project.Repository == null)
					throw new NullReferenceException("Unable to access repository of current ownerDisplay.");

				matchingTemplates.Clear();
				foreach (Template t in diagramPresenter.Project.Repository.GetTemplates()) {
					foreach (string figName in figureNames) {
						if (t.Shape.Type.FullName == figName) {
							matchingTemplates.Add(t);
						}
					}
				}

				if (matchingTemplates.Count == 1) {
					CreateShape(diagramPresenter, matchingTemplates[0]);
				} else if (matchingTemplates.Count > 1) {

					// ToDo: Create "CreateShapeFromTemplateAction" and build the ContextMenu from actions here
					// show context menu with matching templates

					// ToDo: Find a solution for displaying a context menu in the display
					//contextMenu.Items.Clear();
					//foreach (Template t in matchingTemplates) {
					//   ToolStripMenuItem item = new ToolStripMenuItem(t.Name, t.CreateThumbnail(16, 2), ContextMenuItem_Click);
					//   item.Tag = t;
					//   contextMenu.Items.Add(item);
					//}

					//int x, y, width, height;
					//this.GetStrokeSetBounds(out x, out y, out width, out height);
					//contextMenu.Show(x, y);
				}
			}

			Invalidate(diagramPresenter);

			strokeSet.Clear();
			currentStroke.Clear();

			OnToolExecuted(ExecutedEventArgs);
		}


		private void ContextMenuItem_Click(object sender, EventArgs e) {
			//if (sender is ToolStripMenuItem) {
			//   Template t = (Template) ((ToolStripMenuItem)sender).Tag;
			//   CreateShape(ActionDisplay, t);
			//}
		}


		private void CreateShape(IDiagramPresenter diagramPresenter, Template template) {
			// create shape
			Shape shape = template.Shape.Clone();
			if (template.Shape.ModelObject != null)
				shape.ModelObject = template.Shape.ModelObject.Clone();

			int x, y, width, height;
			GetStrokeSetBounds(out x, out y, out width, out height);
			shape.Fit(x, y, width, height);

			ICommand cmd = new InsertShapeCommand(diagramPresenter.Diagram, diagramPresenter.ActiveLayers, shape, true, false);
			project.ExecuteCommand(cmd);
		}


		private void GetStrokeSetBounds(out int x, out int y, out int width, out int height) {
			x = y = 0;
			width = height = 1;
			if (strokeSet.Count > 0) {
				rect.X = int.MaxValue;
				rect.Y = int.MaxValue;
				rect.Width = int.MinValue;
				rect.Height = int.MinValue;
				foreach (Stroke stroke in strokeSet) {
					foreach (Point p in stroke) {
						if (p.X < rect.Left) rect.X = p.X;
						if (p.Y < rect.Top) rect.Y = p.Y;
						if (p.X > rect.Right) rect.Width = rect.Width + (p.X - rect.Right);
						if (p.Y > rect.Bottom) rect.Height = rect.Height + (p.Y - rect.Bottom);
					}
				}
				x = rect.X;
				y = rect.Y;
				width = rect.Width;
				height = rect.Height;
			}
		}


		#region Fields

		private static int penCursorId;
		private Project project;

		private readonly Brush[] brushes = new Brush[] { Brushes.Blue, Brushes.Red, Brushes.Green, Brushes.Pink, Brushes.Plum };
		private Shaper shaper;
		private PathFigureShape polygone;
		private StrokeSequence strokeSet;
		private Stroke currentStroke;

		// ToDo: Timer ins Display verlagern
		private Timer timer;
		private const int timeOut = 1250;

		private List<Template> matchingTemplates = new List<Template>();
		private System.Drawing.Rectangle rect;

		//private ContextMenuStrip contextMenu = new ContextMenuStrip();

		// buffer for coordinate conversions
		private Point p;

		#endregion
	}
#endif

}