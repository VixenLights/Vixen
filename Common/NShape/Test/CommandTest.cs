using System;
using System.Collections.Generic;
using System.Drawing;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;


namespace NShapeTest {

	[TestClass]
	public class CommandTest {

		// Commands Tested:
		//Dataweb.NShape.Commands.CreateDesignCommand
		//Dataweb.NShape.Commands.CreateDiagramCommand
		//Dataweb.NShape.Commands.createModelObjectsCommand
		//Dataweb.NShape.Commands.CreateShapesCommand
		//Dataweb.NShape.Commands.CreateStyleCommand
		//Dataweb.NShape.Commands.CreateTemplateCommand
		//Dataweb.NShape.Commands.DeleteDesignCommand
		//Dataweb.NShape.Commands.DeleteDiagramCommand
		//Dataweb.NShape.Commands.DeleteModelObjectsCommand
		//Dataweb.NShape.Commands.DeleteShapeCommand
		//Dataweb.NShape.Commands.DeleteStyleCommand
		//Dataweb.NShape.Commands.DeleteTemplateCommand
		[TestMethod]
		public void CommandTest_CreateAndDelete() {
			const string projectName = "Command Test - Create and Delete";

			// Create project, diagram, shapes, ...
			Project project = CreateProject(projectName, true);

			// Store test begin state
			BeginTest(project, projectName);

			// Dataweb.NShape.Commands.CreateDesignCommand
			Design design = new Design("My New Design");
			ExecTest(project, new CreateDesignCommand(design),
				() => Assert.IsTrue(Contains(project.Repository.GetDesigns(), design)),
				() => Assert.IsFalse(Contains(project.Repository.GetDesigns(), design)));

			// Dataweb.NShape.Commands.CreateStyleCommand
			ColorStyle style = new ColorStyle("My New Style", Color.DarkViolet);
			ExecTest(project, new CreateStyleCommand(design, style),
				() => Assert.IsTrue(Contains(design.Styles, style)),
				() => Assert.IsFalse(Contains(design.Styles, style)));

			// Dataweb.NShape.Commands.CreateTemplateCommand
			Template template = new Template("My New Template", project.ShapeTypes["Ellipse"].CreateInstance());
			ExecTest(project, new CreateTemplateCommand(template),
				() => Assert.IsTrue(Contains(project.Repository.GetTemplates(), template)),
				() => Assert.IsFalse(Contains(project.Repository.GetTemplates(), template)));

			// Dataweb.NShape.Commands.CreateModelObjectsCommand
			IModelObject modelObject = project.ModelObjectTypes["GenericModelObject"].CreateInstance();
			ExecTest(project, new CreateModelObjectsCommand(modelObject),
				() => Assert.IsTrue(Contains(project.Repository.GetModelObjects(null), modelObject)),
				() => Assert.IsFalse(Contains(project.Repository.GetModelObjects(null), modelObject)));

			// Dataweb.NShape.Commands.CreateDiagramCommand
			Diagram diagram = new Diagram(diagramName);
			ExecTest(project, new CreateDiagramCommand(diagram),
				() => Assert.IsTrue(Contains(project.Repository.GetDiagrams(), diagram)),
				() => Assert.IsFalse(Contains(project.Repository.GetDiagrams(), diagram)));

			// Dataweb.NShape.Commands.CreateShapeCommand
			List<Shape> shapes = CreateShapes(project);
			ExecTest(project, new CreateShapesCommand(diagram, LayerIds.None, shapes, false, false), 
				() => Assert.IsTrue(Contains(diagram.Shapes, shapes)),
				() => Assert.IsFalse(Contains(diagram.Shapes, shapes)));

			// Dataweb.NShape.Commands.DeleteShapesCommand
			ExecTest(project, new DeleteShapesCommand(diagram, shapes, true),
				() => Assert.IsFalse(Contains(diagram.Shapes, shapes)),
				() => Assert.IsTrue(Contains(diagram.Shapes, shapes)));

			// Dataweb.NShape.Commands.DeleteDiagramCommand
			ExecTest(project, new DeleteDiagramCommand(diagram),
				() => Assert.IsFalse(Contains(project.Repository.GetDiagrams(), diagram)),
				() => Assert.IsTrue(Contains(project.Repository.GetDiagrams(), diagram)));

			// Dataweb.NShape.Commands.DeleteModelObjectsCommand
			ExecTest(project, new DeleteModelObjectsCommand(modelObject),
				() => Assert.IsFalse(Contains(project.Repository.GetModelObjects(null), modelObject)),
				() => Assert.IsTrue(Contains(project.Repository.GetModelObjects(null), modelObject)));

			// Dataweb.NShape.Commands.DeleteTemplateCommand
			ExecTest(project, new DeleteTemplateCommand(template), 
				() => Assert.IsFalse(Contains(project.Repository.GetTemplates(), template)),
				() => Assert.IsTrue(Contains(project.Repository.GetTemplates(), template)));

			// Dataweb.NShape.Commands.DeleteStyleCommand
			ExecTest(project, new DeleteStyleCommand(design, style),
				() => Assert.IsFalse(Contains(design.Styles, style)),
				() => Assert.IsTrue(Contains(design.Styles, style)));

			// Dataweb.NShape.Commands.DeleteDesignCommand
			ExecTest(project, new DeleteDesignCommand(design),
				() => Assert.IsFalse(Contains(project.Repository.GetDesigns(), design)),
				() => Assert.IsTrue(Contains(project.Repository.GetDesigns(), design)));

			// Now check the result, undo all and check again
			EndTest(project, projectName);
		}


		// Commands tested:
		//Dataweb.NShape.Commands.MoveShapeByCommand
		//Dataweb.NShape.Commands.MoveShapesCommand
		//Dataweb.NShape.Commands.MoveShapesToCommand
		//Dataweb.NShape.Commands.RotateShapesCommand
		//Dataweb.NShape.Commands.SetCaptionTextCommand
		//Dataweb.NShape.Commands.LiftShapeCommand
		[TestMethod]
		public void CommandTest_ShapeMovement() {
			const string projectName = "Command Test - Shape Movement";

			// Create project, diagram, shapes, ...
			Project project = CreateProject(projectName, true);
			Diagram diagram = new Diagram(diagramName);
			List<Shape> shapes = CreateShapes(project);
			List<ShapePointPosition> currPositions = null;
			List<ShapePointPosition> newPositions = null;

			foreach (Shape s in shapes) {
				if (s.ModelObject != null) project.Repository.Insert(s.ModelObject);
				diagram.Shapes.Add(s, project.Repository.ObtainNewBottomZOrder(diagram));
			}
			project.Repository.InsertAll(diagram);

			// Store test begin state
			BeginTest(project, projectName);

			// MoveShapeByCommand
			currPositions = GetPointPositions(shapes, ControlPointCapabilities.Reference);
			newPositions = TransformPointPositions(currPositions, deltaX, deltaY);
			ExecTest(project, new MoveShapeByCommand(shapes, deltaX, deltaY),
				() => ComparePointPositions(shapes, newPositions, 0),
				() => ComparePointPositions(shapes, currPositions, 0));

			// MoveShapesCommand
			const int moveCnt = 3;
			currPositions = GetPointPositions(shapes, ControlPointCapabilities.Reference);
			newPositions = TransformPointPositions(currPositions, moveCnt * deltaX, moveCnt * deltaY);
			MoveShapesCommand moveShapesCommand = new MoveShapesCommand();
			for (int d = 1; d <= moveCnt; ++d)
				for (int i = 0; i < shapes.Count; ++i)
					moveShapesCommand.AddMove(shapes[i], deltaX, deltaY);
			ExecTest(project, moveShapesCommand, 
				() => ComparePointPositions(shapes, newPositions, 0),
				() => ComparePointPositions(shapes, currPositions, 0));

			// MoveShapesToCommand
			currPositions = GetPointPositions(shapes, ControlPointCapabilities.Reference);
			MoveShapesToCommand moveShapesToCommand = new MoveShapesToCommand();
			for (int d = moveCnt; d > 0; --d)
				for (int i = 0; i < shapes.Count; ++i)
					moveShapesToCommand.AddMoveTo(shapes[i], shapes[i].X, shapes[i].Y, d * deltaX, d * deltaY);
			ExecTest(project, moveShapesToCommand,
				() => { foreach (Shape s in shapes) Assert.IsTrue(s.X == deltaX && s.Y == deltaY); },
				() => ComparePointPositions(shapes, currPositions, 0));

			// RotateShapeCommand
			currPositions = GetPointPositions(shapes, ControlPointCapabilities.Reference | ControlPointCapabilities.Resize);
			newPositions = TransformPointPositions(currPositions, angle, rotationCenterX, rotationCenterY);
			ExecTest(project, new RotateShapesCommand(shapes, angle, rotationCenterX, rotationCenterY),
				() => ComparePointPositions(shapes, newPositions, 1),
				() => ComparePointPositions(shapes, currPositions, 1));

			// SetCaptionTextCommand
			foreach (Shape s in GetShapes(shapes, s => s is ICaptionedShape)) {
				ICaptionedShape caption = (ICaptionedShape)s;
				string origText = caption.GetCaptionText(0);
				ExecTest(project, new SetCaptionTextCommand(caption, 0, caption.GetCaptionText(0), s.Type.FullName),
					() => Assert.AreEqual(caption.GetCaptionText(0), s.Type.FullName),
					() => Assert.AreEqual(caption.GetCaptionText(0), origText));
			}

			// LiftShapeCommand
			int origTopZOrder = diagram.Shapes.TopMost.ZOrder;
			int origBottomZOrder = diagram.Shapes.Bottom.ZOrder;
			foreach (Shape s in shapes) {
			    if (s == diagram.Shapes.TopMost || s == diagram.Shapes.Bottom) continue;
				int origZOrder;
				
				// Not Yet Implemented
				//origZOrder = s.ZOrder;
				//ExecTest_Command(project, new LiftShapeCommand(diagram, s, ZOrderDestination.Upwards), 
				//    () => Assert.IsTrue(s.ZOrder > origZOrder), () => Assert.AreEqual(s.ZOrder, origZOrder));

				// Not Yet Implemented
				//origZOrder = s.ZOrder;
				//ExecTest_Command(project, new LiftShapeCommand(diagram, s, ZOrderDestination.Downwards), 
				//    () => Assert.IsTrue(s.ZOrder < origZOrder), () => Assert.AreEqual(s.ZOrder, origZOrder));

				origZOrder = s.ZOrder; 
				ExecTest(project, new LiftShapeCommand(diagram, s, ZOrderDestination.ToTop), 
					() => Assert.IsTrue(s.ZOrder > origTopZOrder), 
					() => Assert.AreEqual(s.ZOrder, origZOrder));

				origZOrder = s.ZOrder;
				ExecTest(project, new LiftShapeCommand(diagram, s, ZOrderDestination.ToBottom), 
					() => Assert.IsTrue(s.ZOrder < origBottomZOrder), 
					() => Assert.AreEqual(s.ZOrder, origZOrder));
			}

			// Now check the result, undo all and check again
			EndTest(project, projectName);
		}


		// Commands tested:
		//Dataweb.NShape.Commands.CopyTemplateFromTemplateCommand
		//Dataweb.NShape.Commands.ExchangeTemplateShapeCommand
		[TestMethod]
		public void CommandTest_Templates() {
			string projectName = "Command Test - Templates";
			Project project = CreateProject(projectName, true);
			ICommand command = null;

			// Create a template and some shapes from this template
			Shape shapeA = project.ShapeTypes["RoundedBox"].CreateInstance();
			shapeA.Children.Add(project.ShapeTypes["Square"].CreateInstance());
			Template origTemplate = new Template("Template A", shapeA);
			Diagram diagram = new Diagram(diagramName);
			for (int i = 0; i <= 5; ++i) {
				Shape s = origTemplate.CreateShape();
				s.MoveTo(i * deltaX, i * deltaY);
				diagram.Shapes.Add(s, project.Repository.ObtainNewTopZOrder(diagram));
			}
			project.Repository.InsertAll(origTemplate);
			project.Repository.InsertAll(diagram);
			// Create modified template
			Template templateB = origTemplate.Clone();
			templateB.Name = "Template B";
			templateB.Shape.ModelObject = project.ModelObjectTypes["GenericModelObject"].CreateInstance();
			templateB.Shape.LineStyle = project.Design.LineStyles.Dotted;
			((PathBasedPlanarShape)templateB.Shape).FillStyle = project.Design.FillStyles.Red;
			// Create template with a shape of an other shape type
			Template templateC = templateB.Clone();
			templateC.Name = "Template C";
			Shape shapeC = project.ShapeTypes["Ellipse"].CreateInstance();
			shapeC.Children.Add(project.ShapeTypes["Circle"].CreateInstance());

			//Dataweb.NShape.Commands.CopyTemplateFromTemplateCommand
			command = new CopyTemplateFromTemplateCommand(origTemplate, templateB);
			ExecTest(project, command, 
				() => CheckTemplateCommandResult(templateB, diagram.Shapes), 
				() => CheckTemplateCommandResult(origTemplate, diagram.Shapes));

			//Dataweb.NShape.Commands.ExchangeTemplateShapeCommand
			command = new ExchangeTemplateShapeCommand(origTemplate, templateC);
			ExecTest(project, command, () => CheckTemplateCommandResult(templateC, diagram.Shapes), () => CheckTemplateCommandResult(templateB, diagram.Shapes));
		}


		// Commands tested:
		//Dataweb.NShape.Commands.AggregateCompositeShapeCommand
		//Dataweb.NShape.Commands.GroupShapesCommand
		//Dataweb.NShape.Commands.UngroupShapesCommand
		//Dataweb.NShape.Commands.SplitCompositeShapeCommand
		[TestMethod]
		public void CommandTest_AggregatedShapes() {
			const string projectName = "Command Test - Aggregated Shapes";
			Project project = CreateProject(projectName, true);
			List<Shape> shapes = CreateShapes(project);
			ICommand command = null;
			
			// Create a new Diagram
			Shape looseShape = project.ShapeTypes["Polyline"].CreateInstance();
			Shape parentShape = project.ShapeTypes["Ellipse"].CreateInstance();
			List<Shape> childShapes = new List<Shape>();
			foreach (Shape s in shapes) childShapes.Add(s.Clone());
			Diagram diagram = new Diagram(diagramName);
			diagram.Shapes.Add(looseShape, project.Repository.ObtainNewTopZOrder(diagram));
			diagram.Shapes.Add(parentShape, project.Repository.ObtainNewTopZOrder(diagram));
			foreach (Shape childShape in childShapes)
				diagram.Shapes.Add(childShape, project.Repository.ObtainNewTopZOrder(diagram));
			int i = 0;
			foreach (Shape s in diagram.Shapes.BottomUp) {
				++i;
				s.MoveControlPointTo(ControlPointId.Reference, i * deltaX, i * deltaY, ResizeModifiers.None);
			}
			project.Repository.InsertAll(diagram);
			// Prepare Group
			Shape groupShape = project.ShapeTypes["ShapeGroup"].CreateInstance();
			List<Shape> groupMembers = new List<Shape>();
			groupMembers.Add(parentShape);
			groupMembers.Add(looseShape);

			BeginTest(project, projectName);

			//Dataweb.NShape.Commands.AggregateCompositeShapeCommand
			command = new AggregateCompositeShapeCommand(diagram, LayerIds.None, parentShape, childShapes);
			ExecTest(project, command,
				() => { Assert.IsTrue(Contains(parentShape.Children, childShapes)); Assert.IsFalse(Contains(diagram.Shapes, childShapes)); },
				() => { Assert.IsFalse(Contains(parentShape.Children, childShapes)); Assert.IsTrue(Contains(diagram.Shapes, childShapes)); });

			//Dataweb.NShape.Commands.GroupShapesCommand
			command = new GroupShapesCommand(diagram, LayerIds.None, groupShape, groupMembers);
			ExecTest(project, command,
				() => { Assert.IsTrue(Contains(groupShape.Children, groupMembers)); Assert.IsFalse(Contains(diagram.Shapes, groupMembers)); },
				() => { Assert.IsFalse(Contains(groupShape.Children, groupMembers)); Assert.IsTrue(Contains(diagram.Shapes, groupMembers)); });

			//Dataweb.NShape.Commands.UngroupShapesCommand
			command = new UngroupShapesCommand(diagram, groupShape);
			ExecTest(project, command,
				() => { Assert.IsFalse(Contains(groupShape.Children, groupMembers)); Assert.IsTrue(Contains(diagram.Shapes, groupMembers)); },
				() => { Assert.IsTrue(Contains(groupShape.Children, groupMembers)); Assert.IsFalse(Contains(diagram.Shapes, groupMembers)); });

			//Dataweb.NShape.Commands.SplitCompositeShapeCommand
			command = new SplitCompositeShapeCommand(diagram, LayerIds.None, parentShape);
			ExecTest(project, command,
				() => { Assert.IsFalse(Contains(parentShape.Children, childShapes)); Assert.IsTrue(Contains(diagram.Shapes, childShapes)); },
				() => { Assert.IsTrue(Contains(parentShape.Children, childShapes)); Assert.IsFalse(Contains(diagram.Shapes, childShapes)); });

			EndTest(project, projectName);
		}


		// Commands tested:
		//Dataweb.NShape.Commands.AddVertexCommand
		//Dataweb.NShape.Commands.AddConnectionPointCommand
		//Dataweb.NShape.Commands.InsertVertexCommand
		//Dataweb.NShape.Commands.ConnectCommand
		//Dataweb.NShape.Commands.DisconnectCommand
		//Dataweb.NShape.Commands.MoveControlPointCommand
		//Dataweb.NShape.Commands.MoveGluePointCommand
		//Dataweb.NShape.Commands.RemoveConnectionPointCommand
		//Dataweb.NShape.Commands.RemoveVertexCommand		
		[TestMethod]
		public void CommandTest_ControlPoints() {
			string projectName = "Command Test - Control Points";
			Project project = CreateProject(projectName, true);

			// Create project, diagram, shapes, ...
			DiagramHelper.CreateDiagram(project, diagramName, 2, 2, false, true, true, true, true);
			Diagram diagram = project.Repository.GetDiagram(diagramName);
			PolylineBase line = (PolylineBase)project.ShapeTypes["Polyline"].CreateInstance();
			diagram.Shapes.Add(line, project.Repository.ObtainNewBottomZOrder(diagram));
			project.Repository.InsertAll((Shape)line, diagram);

			// Prepare test data for connection tests
			ShapePointPosition targetA = null;
			ShapePointPosition targetB = null;
			foreach (Shape shape in GetShapes(diagram.Shapes, s => s is IPlanarShape)) {
				if (targetA == null)
					targetA = new ShapePointPosition(shape, 2, shape.GetControlPointPosition(2));
				else if (targetB == null)
					targetB = new ShapePointPosition(shape, 7, shape.GetControlPointPosition(7));
				else break;
			}
			// Buffers
			Point pos = Point.Empty;
			List<ShapePointPosition> currPositions = null;
			List<ShapePointPosition> newPositions = null;

			BeginTest(project, projectName);

			// Test strategy:
			// Add two vertices at the beginning and remove them at the end of the test in the same order as they were created.
			// This is an implicit test for the correct assignment of ControlPointIds when undoing/redoing the commands.
			// The assigned ControlPointIds may not change in this scenario.

			//Dataweb.NShape.Commands.InsertVertexCommand
			pos = new Point(deltaX, deltaY);
			currPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Resize);
			newPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Resize);
			newPositions.Add(new ShapePointPosition(line, ControlPointId.None, pos));
			ExecTest(project, new InsertVertexCommand(line, ControlPointId.LastVertex, pos.X, pos.Y),
				() => CheckControlPointCommandResult(line, newPositions, 0, ControlPointCapabilities.Resize),
				() => CheckControlPointCommandResult(line, currPositions, 0, ControlPointCapabilities.Resize));
			
			//Dataweb.NShape.Commands.AddVertexCommand
			pos = Geometry.VectorLinearInterpolation(
				line.GetControlPointPosition(ControlPointId.FirstVertex), 
				line.GetControlPointPosition(line.GetNextVertexId(ControlPointId.FirstVertex)), 0.5f);
			currPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Resize);
			newPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Resize);
			newPositions.Add(new ShapePointPosition(line, ControlPointId.None, pos));
			ExecTest(project, new AddVertexCommand(line, pos.X, pos.Y),
				() => CheckControlPointCommandResult(line, newPositions, 1, ControlPointCapabilities.Resize),
				() => CheckControlPointCommandResult(line, currPositions, 1, ControlPointCapabilities.Resize));
			
			//Dataweb.NShape.Commands.AddConnectionPointCommand
			pos = Geometry.VectorLinearInterpolation(
				line.GetControlPointPosition(ControlPointId.FirstVertex),
				line.GetControlPointPosition(line.GetNextVertexId(ControlPointId.FirstVertex)), 0.5f);
			currPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Connect);
			newPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Connect);
			newPositions.Add(new ShapePointPosition(line, ControlPointId.None, pos));
			ExecTest(project, new AddConnectionPointCommand(line, pos.X, pos.Y),
				() => CheckControlPointCommandResult(line, newPositions, 1, ControlPointCapabilities.Connect),
				() => CheckControlPointCommandResult(line, currPositions, 1, ControlPointCapabilities.Connect));

			// Test strategy:
			// In order to test as many combinations as possible, the following actions are performed in this order:
			// 1. Move a glue point over a connection point with a "MoveCOntrolPointCommand" (no connect)
			// 2. Connected the shapes
			// 3. Move the (connected) glue point over another shape with a "MoveGluePointCommand" (automatic disconnect and reconnect).
			// 4. Disconnect the glue point
			
			//Dataweb.NShape.Commands.MoveControlPointCommand
			currPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Glue);
			newPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Glue);
			int dx = 0, dy = 0;
			foreach (ShapePointPosition spp in newPositions)
				if (spp.pointId == ControlPointId.FirstVertex) {
					Point oldPos = spp.position;
					spp.position = targetA.shape.GetControlPointPosition(targetA.pointId);
					dx = spp.position.X - oldPos.X;
					dy = spp.position.Y - oldPos.Y;
				}
			ExecTest(project, new MoveControlPointCommand((Shape)line, ControlPointId.FirstVertex, dx, dy, ResizeModifiers.None),
				() => { 
					CheckControlPointCommandResult(line, newPositions, 0, ControlPointCapabilities.Glue); 
					Assert.AreEqual<int>(ControlPointId.None, line.IsConnected(ControlPointId.Any, targetA.shape)); 
				}, () => { 
					CheckControlPointCommandResult(line, currPositions, 0, ControlPointCapabilities.Glue); 
					Assert.AreEqual<int>(ControlPointId.None, line.IsConnected(ControlPointId.Any, targetA.shape)); 
				});

			//Dataweb.NShape.Commands.ConnectCommand
			ExecTest(project, new ConnectCommand(line, ControlPointId.FirstVertex, targetA.shape, targetA.pointId),
				() => Assert.AreEqual(targetA.pointId, line.IsConnected(ControlPointId.FirstVertex, targetA.shape)),
				() => Assert.AreEqual<int>(ControlPointId.None, line.IsConnected(ControlPointId.FirstVertex, targetA.shape)));
			
			//Dataweb.NShape.Commands.MoveGluePointCommand
			currPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Glue);
			newPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Glue);
			dx = dy = 0;
			foreach (ShapePointPosition spp in newPositions)
				if (spp.pointId == ControlPointId.FirstVertex) {
					Point oldPos = spp.position;
					spp.position = targetB.shape.GetControlPointPosition(targetB.pointId);
					dx = spp.position.X - oldPos.X;
					dy = spp.position.Y - oldPos.Y;
				}
			ExecTest(project, new MoveGluePointCommand(line, ControlPointId.FirstVertex, targetB.shape, targetB.pointId, dx, dy, ResizeModifiers.None),
				() => {
					CheckControlPointCommandResult(line, newPositions, 0, ControlPointCapabilities.Glue);
					Assert.AreEqual(targetB.pointId, line.IsConnected(ControlPointId.FirstVertex, targetB.shape));
				}, () => {
					CheckControlPointCommandResult(line, currPositions, 0, ControlPointCapabilities.Glue);
					Assert.AreEqual(targetA.pointId, line.IsConnected(ControlPointId.FirstVertex, targetA.shape));
				});

			//Dataweb.NShape.Commands.DisconnectCommand
			ExecTest(project, new DisconnectCommand(line, ControlPointId.FirstVertex),
				() => Assert.AreEqual<int>(ControlPointId.None, line.IsConnected(ControlPointId.FirstVertex, targetB.shape)),
				() => Assert.AreEqual(targetB.pointId, line.IsConnected(ControlPointId.FirstVertex, targetB.shape)));

			//Dataweb.NShape.Commands.RemoveConnectionPointCommand
			currPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Connect);
			newPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Connect);
			ControlPointId pointId = ControlPointId.None;
			for (int i = newPositions.Count - 1; i >= 0; --i) {
				if (!line.HasControlPointCapability(newPositions[i].pointId, ControlPointCapabilities.Resize)) {
					pointId = newPositions[i].pointId;
					newPositions.RemoveAt(i);
					break;
				}
			}
			ExecTest(project, new RemoveConnectionPointCommand(line, pointId),
				() => CheckControlPointCommandResult(line, newPositions, 0, ControlPointCapabilities.Connect),
				() => CheckControlPointCommandResult(line, currPositions, 0, ControlPointCapabilities.Connect));

			// This test is executed twice in order to test the scenario described at the beginning.
			for (int a = 0; a < 2; ++a) {
				//Dataweb.NShape.Commands.RemoveVertexCommand
				currPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Resize);
				newPositions = GetPointPositions((Shape)line, ControlPointCapabilities.Resize);
				pointId = line.GetPreviousVertexId(ControlPointId.LastVertex);
				for (int i = newPositions.Count - 1; i >= 0; --i)
					if (newPositions[i].pointId == pointId) { newPositions.RemoveAt(i); break; }
				ExecTest(project, new RemoveVertexCommand(line, pointId),
					() => CheckControlPointCommandResult(line, newPositions, 0, ControlPointCapabilities.Resize),
					() => CheckControlPointCommandResult(line, currPositions, 0, ControlPointCapabilities.Resize));
			}

			EndTest(project, projectName);
		}


		// Commands tested:
		//Dataweb.NShape.Commands.AddLayerCommand 
		//Dataweb.NShape.Commands.AddShapesToLayersCommand
		//Dataweb.NShape.Commands.AssignShapesToLayersCommand
		//Dataweb.NShape.Commands.EditLayerCommand
		//Dataweb.NShape.Commands.RemoveLayerCommand
		//Dataweb.NShape.Commands.RemoveShapesFromLayersCommand
		[TestMethod]
		public void CommandTest_Layers() {
			string projectName = "Command Test - Layers";
			Project project = CreateProject(projectName, true);
			DiagramHelper.CreateDiagram(project, diagramName, 2, 2, true, false, false, false, true);
			Diagram diagram = project.Repository.GetDiagram(diagramName);

			BeginTest(project, projectName);

			//Dataweb.NShape.Commands.AddLayerCommand 
			List<Layer> newLayers = new List<Layer>(5);
			for (int i = 0; i <= 5; ++i) {
				string layerName = "Layer " + (char)(65 + i);
				ExecTest(project, new AddLayerCommand(diagram, layerName),
					() => Assert.IsNotNull(diagram.Layers.FindLayer(layerName)),
					() => Assert.IsNull(diagram.Layers.FindLayer(layerName)));
				newLayers.Add(diagram.Layers.FindLayer(layerName));
			}
			
			// Prepare test data
			LayerIds newLayerIds = LayerIds.None;
			foreach (Layer l in newLayers) newLayerIds |= l.Id;
			Dictionary<Shape, LayerIds> origLayerIds = null;

			//Dataweb.NShape.Commands.AddShapesToLayersCommand
			ExecTest(project, new AddShapesToLayersCommand(diagram, diagram.Shapes, newLayerIds),
				() => { foreach (Shape s in diagram.Shapes) Assert.AreEqual(newLayerIds, s.Layers & newLayerIds); },
				() => { foreach (Shape s in diagram.Shapes) Assert.AreEqual(LayerIds.None, s.Layers & newLayerIds); });

			//Dataweb.NShape.Commands.AssignShapesToLayersCommand
			origLayerIds = GetShapeLayers(diagram.Shapes);
			ExecTest(project, new AssignShapesToLayersCommand(diagram, diagram.Shapes, newLayerIds),
				() => { foreach (Shape s in diagram.Shapes) Assert.AreEqual(newLayerIds, s.Layers & LayerIds.All); },
				() => { foreach (Shape s in diagram.Shapes) Assert.AreEqual(origLayerIds[s], s.Layers & LayerIds.All); });

			//Dataweb.NShape.Commands.EditLayerCommand
			foreach (EditLayerCommand.ChangedProperty prop in Enum.GetValues(typeof(EditLayerCommand.ChangedProperty))) {
				Layer layer = newLayers[(int)prop];
				switch (prop) {
					case EditLayerCommand.ChangedProperty.LowerZoomThreshold: {
							int oldValue = layer.LowerZoomThreshold;
							int newValue = oldValue + 10;
							ExecTest(project, new EditLayerCommand(diagram, layer, prop, oldValue, newValue),
								() => Assert.AreEqual(diagram.Layers.FindLayer(layer.Name).LowerZoomThreshold, newValue),
								() => Assert.AreEqual(diagram.Layers.FindLayer(layer.Name).LowerZoomThreshold, oldValue));
						} break;
					case EditLayerCommand.ChangedProperty.UpperZoomThreshold: {
							int oldValue = layer.UpperZoomThreshold;
							int newValue = oldValue - 10;
							ExecTest(project, new EditLayerCommand(diagram, layer, prop, oldValue, newValue),
								() => Assert.AreEqual(diagram.Layers.FindLayer(layer.Name).UpperZoomThreshold, newValue),
								() => Assert.AreEqual(diagram.Layers.FindLayer(layer.Name).UpperZoomThreshold, oldValue));
						} break;
					case EditLayerCommand.ChangedProperty.Name: {
							string oldValue = layer.Name;
							string newValue = layer.Name + string.Format(" ({0})", layer.Id);
							ExecTest(project, new EditLayerCommand(diagram, layer, prop, oldValue, newValue),
								() => Assert.AreEqual(diagram.Layers.FindLayer(newValue).Name, newValue),
								() => Assert.AreEqual(diagram.Layers.FindLayer(oldValue).Name, oldValue));
						} break;
					case EditLayerCommand.ChangedProperty.Title: {
							string oldValue = layer.Title;
							string newValue = "<[ " + layer.Title + "]>";
							ExecTest(project, new EditLayerCommand(diagram, layer, prop, oldValue, newValue),
								() => Assert.AreEqual(diagram.Layers.FindLayer(layer.Name).Title, newValue),
								() => Assert.AreEqual(diagram.Layers.FindLayer(layer.Name).Title, oldValue));
						} break;
					default:
						Assert.Fail(string.Format("Untested {0} value: {1}", prop.GetType().Name, prop));
						break;
				}
			}

			//Dataweb.NShape.Commands.RemoveShapesFromLayersCommand
			LayerIds removedLayerIds = newLayers[0].Id | newLayers[1].Id | newLayers[2].Id;
			origLayerIds = GetShapeLayers(diagram.Shapes);
			ExecTest(project, new RemoveShapesFromLayersCommand(diagram, diagram.Shapes, removedLayerIds),
				() => { foreach (Shape s in diagram.Shapes) Assert.AreEqual(origLayerIds[s] ^ removedLayerIds, s.Layers); },
				() => { foreach (Shape s in diagram.Shapes) Assert.AreEqual(origLayerIds[s], s.Layers); });

			//Dataweb.NShape.Commands.RemoveLayerCommand
			origLayerIds = GetShapeLayers(diagram.Shapes);
			ExecTest(project, new RemoveLayerCommand(diagram, newLayers),
				() => {
					Assert.IsFalse(Contains(diagram.Layers, newLayers));
					foreach (Shape s in diagram.Shapes) Assert.AreEqual(LayerIds.None, s.Layers & newLayerIds);
				}, () => {
					Assert.IsTrue(Contains(diagram.Layers, newLayers));
					foreach (Shape s in diagram.Shapes) Assert.AreEqual(origLayerIds[s], s.Layers & newLayerIds);
				});

			EndTest(project, projectName);
		}


		// Commands tested:
		//Dataweb.NShape.Commands.PropertySetCommand<T>
		//Dataweb.NShape.Commands.DesignPropertySetCommand
		//Dataweb.NShape.Commands.DiagramPropertySetCommand
		//Dataweb.NShape.Commands.LayerPropertySetCommand
		//Dataweb.NShape.Commands.ModelObjectPropertySetCommand
		//Dataweb.NShape.Commands.ShapePropertySetCommand
		//Dataweb.NShape.Commands.StylePropertySetCommand
		[TestMethod]
		public void CommandTest_PropertyController() {
			string projectName = "Command Test - PropertyController Commands";
			Project project = CreateProject(projectName, true);

			// Prepare test data
			DiagramHelper.CreateDiagram(project, diagramName, 2, 2, true, true, false, false, true);
			// Buffers
			PropertyInfo propertyInfo = null;
			object oldValue = null, newValue = null;
			List<Object> oldValues = new List<object>();
			List<Object> newValues = new List<object>();

			BeginTest(project, projectName);

			//Dataweb.NShape.Commands.DesignPropertySetCommand
			Design design = project.Design;
			propertyInfo = typeof(Design).GetProperty("Title");
			oldValue = design.Title;
			newValue = "My Design";
			ExecTest(project, new DesignPropertySetCommand(design, propertyInfo, oldValue, newValue),
				() => Assert.AreEqual(design.Title, (string)newValue),
				() => Assert.AreEqual(design.Title, (string)oldValue));

			//Dataweb.NShape.Commands.StylePropertySetCommand
			IFillStyle fillStyle = design.FillStyles.Blue;
			propertyInfo = typeof(FillStyle).GetProperty("AdditionalColorStyle");
			oldValues.Clear(); newValues.Clear();
			for (int i = 0; i < design.FillStyles.Count; ++i) {
				oldValues.Add(design.FillStyles[i].AdditionalColorStyle);
				newValues.Add(design.ColorStyles.Transparent);
			}
			ExecTest(project, new StylePropertySetCommand(design, ConvertEnumerator<Style>.Create(design.FillStyles), propertyInfo, oldValues, newValues),
				() => { for (int i = 0; i < design.FillStyles.Count; ++i) Assert.AreEqual(design.FillStyles[i].AdditionalColorStyle, (ColorStyle)newValues[i]); },
				() => { for (int i = 0; i < design.FillStyles.Count; ++i) Assert.AreEqual(design.FillStyles[i].AdditionalColorStyle, (ColorStyle)oldValues[i]); });

			//Dataweb.NShape.Commands.DiagramPropertySetCommand
			Diagram diagram = project.Repository.GetDiagram(diagramName);
			propertyInfo = typeof(Diagram).GetProperty("Width");
			oldValue = diagram.Width;
			newValue = diagram.Width * 3;
			ExecTest(project, new DiagramPropertySetCommand(diagram, propertyInfo, oldValue, newValue),
				() => Assert.AreEqual(diagram.Width, (int)newValue),
				() => Assert.AreEqual(diagram.Width, (int)oldValue));

			//Dataweb.NShape.Commands.LayerPropertySetCommand
			Layer layer = diagram.Layers[LayerIds.Layer01];
			propertyInfo = typeof(Layer).GetProperty("UpperZoomThreshold");
			oldValue = layer.UpperZoomThreshold;
			newValue = layer.UpperZoomThreshold - 100;
			ExecTest(project, new LayerPropertySetCommand(diagram, layer, propertyInfo, oldValue, newValue),
				() => Assert.AreEqual(layer.UpperZoomThreshold, (int)newValue),
				() => Assert.AreEqual(layer.UpperZoomThreshold, (int)oldValue));

			//Dataweb.NShape.Commands.ShapePropertySetCommand
			propertyInfo = typeof(PathBasedPlanarShape).GetProperty("LineStyle");
			List<Shape> shapes = new List<Shape>(diagram.Shapes);
			oldValues.Clear();
			foreach (Shape s in shapes) oldValues.Add(s.LineStyle);
			newValue = design.LineStyles.Dotted;
			ExecTest(project, new ShapePropertySetCommand(shapes, propertyInfo, oldValues, newValue),
				() => { for (int i = 0; i < shapes.Count; ++i) Assert.AreEqual(shapes[i].LineStyle, (ILineStyle)newValue); },
				() => { for (int i = 0; i < shapes.Count; ++i) Assert.AreEqual(shapes[i].LineStyle, (ILineStyle)oldValues[i]); });

			//Dataweb.NShape.Commands.ModelObjectPropertySetCommand
			IModelObject modelObject = diagram.Shapes.TopMost.ModelObject;
			propertyInfo = typeof(IModelObject).GetProperty("Name");
			oldValue = modelObject.Name;
			newValue = modelObject.Name;
			ExecTest(project, new ModelObjectPropertySetCommand(modelObject, propertyInfo, oldValue, newValue),
				() => Assert.AreEqual(modelObject.Name, (string)oldValue),
				() => Assert.AreEqual(modelObject.Name, (string)newValue));

			EndTest(project, projectName);
		}


		// Untested Commands:
		//Dataweb.NShape.Commands.AggregatedCommand
		//Dataweb.NShape.Commands.AssignModelObjectCommand
		//Dataweb.NShape.Commands.SetModelObjectParentCommand


		#region [Private] Core Test Methods

		private void BeginTest(Project project, string projectName) {
			project.Name = projectName + " Test Begin";
			project.Repository.SaveChanges();
			project.Name = projectName;
		}


		private void ExecTest(Project project, ICommand command, Action checkExecuteResult, Action checkRevertResult) {
			// Execure the command and compare with expected positions
			project.ExecuteCommand(command);
			checkExecuteResult();
			// Undo the command and compare with the current positions
			project.History.Undo();
			checkRevertResult();
			// Redo the command and compare with the expected positions again
			project.History.Redo();
			checkExecuteResult();
		}


		private void EndTest(Project project, string projectName) {
			// Save changes, load into a new project and compare the repositories
			project.Name = projectName + " Test End";
			project.Repository.SaveChanges();
			Project projectTestEnd = CreateProject(project.Name, false);
			projectTestEnd.Open();
			RepositoryComparer.Compare(project, projectTestEnd);

			// Undo all changes...
			int undoCnt = project.History.UndoCommandCount;
			project.History.Undo(undoCnt);
			// ... and compare with the saved start state
			Project projectTestStart = CreateProject(projectName + " Test Begin", false);
			projectTestStart.Open();
			RepositoryComparer.Compare(project, projectTestStart);

			// Redo all changes...
			int redoCnt = project.History.RedoCommandCount;
			project.History.Redo(redoCnt);
			// ... and compare with the saved start state again
			RepositoryComparer.Compare(project, projectTestEnd);
		}

		#endregion


		#region [Private] Check Test Result Methods

		private void CheckTemplateCommandResult(Template template, IEnumerable<Shape> shapes) {
			foreach (Shape s in shapes) {
				Assert.AreEqual(s.Template.Name, template.Name);
				Assert.AreEqual(s.LineStyle, template.Shape.LineStyle);
				Assert.AreEqual(s.Type, template.Shape.Type);
				if (s.ModelObject != null)
					Assert.IsNotNull(template.Shape.ModelObject != null);
				if (s is PathBasedPlanarShape)
					Assert.AreEqual(((PathBasedPlanarShape)s).FillStyle, ((PathBasedPlanarShape)template.Shape).FillStyle);
			}
		}


		private void CheckControlPointCommandResult(Shape shape, List<ShapePointPosition> expectedPositions, int delta, ControlPointCapabilities capabilities) {
			// Compare point positions and capabilities
			foreach (ShapePointPosition spp in expectedPositions) {
				if (spp.pointId == ControlPointId.None) {
					spp.pointId = spp.shape.FindNearestControlPoint(spp.position.X, spp.position.Y, delta, capabilities);
					Assert.AreNotEqual(ControlPointId.None, spp.pointId);
				} else {
					ComparePointPosition(shape, spp, delta);
					shape.HasControlPointCapability(spp.pointId, capabilities);
				}
			}
			// Check if there are any points that are not expected
			foreach (ControlPointId id in shape.GetControlPointIds(capabilities)) {
				bool found = false;
				foreach (ShapePointPosition spp in expectedPositions)
					if (spp.pointId == id) { found = true; break; }
				Assert.IsTrue(found);
			}
		}

		#endregion


		#region [Private] Helper Methods

		private bool Contains<T>(IEnumerable<T> collection, IEnumerable<T> objects) {
			foreach (T obj in objects)
				if (!Contains(collection, obj))
					return false;
			return true;
		}


		private bool Contains<T>(IEnumerable<T> collection, T obj) {
			foreach (T item in collection) {
				if ((item == null && obj == null) || item.Equals(obj)) 
					return true;
			}
			return false;
		}


		private Project CreateProject(string projectName, bool create) {
			Project project = new Project();
			project.Name = projectName;
			project.AutoLoadLibraries = true;
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = RepositoryHelper.CreateXmlStore();

			if (create) {
				// Delete the current project (if it exists)
				if (project.Repository.Exists())
					project.Repository.Erase();
				project.Create();
				project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			}
			
			return project;
		}


		private List<Shape> CreateShapes(Project project) {
			List<Shape> shapes = new List<Shape>();
			foreach (Template template in project.Repository.GetTemplates())
				shapes.Add(template.CreateShape());
			return shapes;
		}


		private IEnumerable<Shape> GetShapes(IEnumerable<Shape> shapes, Predicate<Shape> predicate) {
			foreach (Shape s in shapes) {
				if (predicate(s)) 
					yield return s;
			}
		}


		private Dictionary<Shape, LayerIds> GetShapeLayers(IEnumerable<Shape> shapes) {
			Dictionary<Shape, LayerIds> result = new Dictionary<Shape, LayerIds>();
			foreach (Shape s in shapes) 
				result.Add(s, s.Layers);
			return result;
		}


		private List<ShapePointPosition> GetPointPositions(Shape shape, ControlPointCapabilities capabilities) {
			return GetPointPositions(SingleInstanceEnumerator<Shape>.Create(shape), capabilities);
		}


		private List<ShapePointPosition> GetPointPositions(IEnumerable<Shape> shapes, ControlPointCapabilities capabilities) {
			List<ShapePointPosition> shapePositions = new List<ShapePointPosition>();
			foreach (Shape shape in shapes) {
				foreach (ControlPointId pointId in shape.GetControlPointIds(capabilities))
					shapePositions.Add(new ShapePointPosition(shape, pointId, shape.GetControlPointPosition(pointId)));
			}
			return shapePositions;
		}


		private List<ShapePointPosition> TransformPointPositions(List<ShapePointPosition> pointPositions, int offsetX, int offsetY) {
			return TransformPointPositions(pointPositions, offsetX, offsetY, 0, 0, 0);
		}


		private List<ShapePointPosition> TransformPointPositions(List<ShapePointPosition> pointPositions, int angleTenthsOfDeg, int aroundX, int aroundY) {
			return TransformPointPositions(pointPositions, 0, 0, angleTenthsOfDeg, aroundX, aroundY);
		}


		private List<ShapePointPosition> TransformPointPositions(List<ShapePointPosition> pointPositions, int offsetX, int offsetY, int angleTenthsOfDeg, int aroundX, int aroundY) {
			List<ShapePointPosition> result = new List<ShapePointPosition>(pointPositions.Count);
			Point p = Point.Empty;
			for (int i = pointPositions.Count - 1; i >= 0; --i) {
				p = pointPositions[i].position;
				p.Offset(offsetX, offsetY);
				if (angleTenthsOfDeg != 0)
					p = Geometry.RotatePoint(aroundX, aroundY, Geometry.TenthsOfDegreeToDegrees(angleTenthsOfDeg), p);
				result.Add(new ShapePointPosition(pointPositions[i].shape, pointPositions[i].pointId, p));
			}
			return result;
		}


		private void ComparePointPositions(List<Shape> shapes, List<ShapePointPosition> expectedPositions, int delta) {
			foreach (ShapePointPosition shapePointPos in expectedPositions) {
				if (shapePointPos.shape is LabelBase && shapePointPos.shape.HasControlPointCapability(shapePointPos.pointId, ControlPointCapabilities.Glue))
					continue;
				int i = shapes.IndexOf(shapePointPos.shape);
				Assert.IsTrue(i >= 0);
				ComparePointPosition(shapes[i], shapePointPos, delta);
			}
		}


		private void ComparePointPositions(Shape shape, List<ShapePointPosition> expectedPositions, int delta) {
			foreach (ShapePointPosition shapePointPos in expectedPositions) {
				if (shapePointPos.shape is LabelBase && shapePointPos.shape.HasControlPointCapability(shapePointPos.pointId, ControlPointCapabilities.Glue))
					continue;
				ComparePointPosition(shape, shapePointPos, delta);
			}
		}


		private void ComparePointPosition(Shape shape, ShapePointPosition shapePointPos, int delta) {
			Assert.AreEqual(shape, shapePointPos.shape);
			Point ptPos = shape.GetControlPointPosition(shapePointPos.pointId);
			Assert.IsTrue(Math.Abs(ptPos.X - shapePointPos.position.X) <= Math.Abs(delta));
			Assert.IsTrue(Math.Abs(ptPos.Y - shapePointPos.position.Y) <= Math.Abs(delta));
		}

		#endregion


		#region [Private] Types and Fields

		private class ShapePointPosition {

			public ShapePointPosition() {
				shape = null;
				pointId = ControlPointId.None;
				position = Geometry.InvalidPoint;
			}

			public ShapePointPosition(Shape shape, ControlPointId pointId, Point position) {
				this.shape = shape;
				this.pointId = pointId;
				this.position = position;
			}

			public Shape shape;
			public ControlPointId pointId;
			public Point position;
		}


		const string diagramName = "Test Diagram";
		const int deltaX = 50;
		const int deltaY = 100;
		const int angle = 333;
		const int rotationCenterX = 0;
		const int rotationCenterY = 1000;

		#endregion

	}

}
