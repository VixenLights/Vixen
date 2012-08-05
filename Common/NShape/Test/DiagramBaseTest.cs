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
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.GeneralShapes;
using Dataweb.NShape.Commands;


namespace NShapeTest {

	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class DiagramBaseTest {

		public DiagramBaseTest() {
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}


		#region Additional test attributes

		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext) { }

		// Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup() { }

		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize() {
			TestContext.BeginTimer(TestContext.TestName + " Timer");
		}

		// Use TestCleanup to run code after each test has run
		[TestCleanup()]
		public void MyTestCleanup() {
			TestContext.EndTimer(TestContext.TestName + " Timer");
		}

		#endregion


		#region Test methods

		[TestMethod]
		public void BaseTest() {
			// -- Create a project --
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Name = "Test";
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = RepositoryHelper.CreateXmlStore();
			project.Repository.Erase();
			project.Create();

			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			//
			Diagram diagram = new Diagram("All Shapes");
			diagram.Width = 800;
			diagram.Height = 600;
			project.Repository.InsertAll(diagram);
			int x = 0;
			int shapeCount = 0;
			foreach (ShapeType st in project.ShapeTypes) {
				x += 50;
				Shape s = st.CreateInstance();
				s.X = x;
				s.Y = 50;
				if (s is IPlanarShape) {
					((IPlanarShape)s).Angle = -300;
					//((IPlanarShape)s).FillStyle
					// ((ILinearShape)s).LineStyle
				}
				diagram.Shapes.Add(s, shapeCount + 1);
				project.Repository.InsertAll(s, diagram);
				++shapeCount;
			}
			//
			Diagram diagram2 = new Diagram("Connections");
			diagram2.Width = 800;
			diagram2.Height = 600;
			Circle circle = (Circle)project.ShapeTypes["Circle"].CreateInstance();
			circle.X = 50;
			circle.Y = 50;
			circle.Diameter = 10;
			diagram2.Shapes.Add(circle, diagram.Shapes.MaxZOrder + 10);
			Box box = (Box)project.ShapeTypes["Box"].CreateInstance();
			box.X = 100;
			box.Y = 50;
			box.Width = 20;
			box.Height = 10;
			box.Angle = 450;
			diagram2.Shapes.Add(box, diagram2.Shapes.MaxZOrder + 10);
			Polyline polyLine = (Polyline)project.ShapeTypes["Polyline"].CreateInstance();
			polyLine.Connect(1, box, 3);
			polyLine.Connect(2, circle, 7);
			diagram2.Shapes.Add(polyLine, diagram2.Shapes.MaxZOrder + 10);
			project.Repository.InsertAll(diagram2);
			//
			project.Repository.SaveChanges();
			project.Close();
			//
			// -- Reload project and modify --
			project.Open();
			diagram = project.Repository.GetDiagram("All Shapes");
			project.Repository.GetDiagramShapes(diagram);
			foreach (Shape s in diagram.Shapes) {
				s.MoveBy(300, 300);
				if (s is ICaptionedShape)
					((ICaptionedShape)s).SetCaptionText(0, s.Type.Name);
				project.Repository.Update(s);
			}
			project.Repository.SaveChanges();
			project.Close();
			//
			// -- Reload and check --
			project.Open();
			diagram = project.Repository.GetDiagram("All Shapes");
			int c = 0;
			foreach (Shape s in diagram.Shapes) {
				Assert.AreEqual(350, s.Y);
				if (s is IPlanarShape) Assert.AreEqual(3300, ((IPlanarShape)s).Angle, s.Type.Name);
				if (s is ICaptionedShape)
					Assert.AreEqual(((ICaptionedShape)s).GetCaptionText(0), s.Type.Name);
				++c;
			}
			Assert.AreEqual(shapeCount, diagram.Shapes.Count);
			Assert.AreEqual(shapeCount, c);
			project.Close();
		}


		/// <summary>
		/// Calls all methods and properties of a shape for each available shape type and does some plausibility checks.
		/// </summary>
		[TestMethod]
		public void ShapeTest() {
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Name = "Test";
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = RepositoryHelper.CreateXmlStore();
			project.Repository.Erase();
			project.Create();
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			//
			foreach (ShapeType st in project.ShapeTypes) {
				Shape s = st.CreateInstance();
				if (s is TextBase) ((TextBase)s).AutoSize = false;
				//
				// -- Test Properties --
				s.Fit(0, 0, 40, 40);	// ensure that the shape has a defined size
				s.X = 100;
				Assert.AreEqual(100, s.X);
				s.Y = -100;
				Assert.AreEqual(-100, s.Y);
				Assert.ReferenceEquals(st, s.Type);
				Assert.ReferenceEquals(null, s.Template);
				Assert.ReferenceEquals(null, s.Tag);
				s.Tag = "Hello";
				Assert.AreEqual("Hello", (string)s.Tag);
				Assert.AreEqual('A', s.SecurityDomainName);
				s.SecurityDomainName = 'K';
				Assert.AreEqual('K', s.SecurityDomainName);
				Assert.ReferenceEquals(null, s.Parent);
				s.Parent = null;
				Assert.ReferenceEquals(null, s.ModelObject);
				// s.ModelObject = project.ModelObjectTypes["Generic ModelObjectTypes Object"].CreateInstance();
				// Assert
				Assert.ReferenceEquals(project.Design.LineStyles.Normal, s.LineStyle);
				s.LineStyle = project.Design.LineStyles.HighlightDashed;
				Assert.ReferenceEquals(project.Design.LineStyles.HighlightDashed, s.LineStyle);
				Assert.ReferenceEquals(null, ((IEntity)s).Id);
				Assert.ReferenceEquals(null, s.DisplayService);
				Assert.AreEqual(0, s.Children.Count);
				Rectangle bounds1 = s.GetBoundingRectangle(true);
				//
				// -- Test Methods --
				RelativePosition relPos = s.CalculateRelativePosition(100, -100);
				Point absPos = s.CalculateAbsolutePosition(relPos);
				Assert.AreEqual(absPos.X, 100);
				Assert.AreEqual(absPos.Y, -100);
				//
				Shape clone = s.Clone();
				Assert.AreEqual(clone.Y, s.Y);
				Assert.ReferenceEquals(clone.LineStyle, s.LineStyle);
				//
				// TODO 2: Do not know, how to test the result.
				s.ContainsPoint(0, 0);
				s.ContainsPoint(105, -95);
				s.ContainsPoint(-10000, +10000);
				//
				int cpc = 0;
				foreach (ControlPointId id in s.GetControlPointIds(ControlPointCapabilities.Resize)) {
					++cpc;
					s.GetControlPointPosition(id);
					Assert.IsTrue(s.HasControlPointCapability(id, ControlPointCapabilities.Resize));
				}
				//
				s.MoveBy(-320, 1000);
				Rectangle bounds2 = s.GetBoundingRectangle(true);
				bounds2.Offset(320, -1000);
				Assert.AreEqual(bounds1, bounds2);
				s.MoveTo(100, -100);
				bounds2 = s.GetBoundingRectangle(true);
				Assert.AreEqual(bounds2, bounds1);
				//
				Point cPos = s.GetControlPointPosition(ControlPointId.Reference);
				SortedList<ControlPointId, Point> ctrlPoints = new SortedList<ControlPointId, Point>();
				foreach (ControlPointId id in s.GetControlPointIds(ControlPointCapabilities.Resize | ControlPointCapabilities.Reference))
					ctrlPoints.Add(id, s.GetControlPointPosition(id));
				bounds1 = s.GetBoundingRectangle(true);
				Point rotateCenter = new Point(0, 1000);
				int rotateAngle = 333;
				s.Rotate(rotateAngle, rotateCenter.X, rotateCenter.Y);
				foreach (ControlPointId id in s.GetControlPointIds(ControlPointCapabilities.Resize | ControlPointCapabilities.Reference)) {
					if (s is LabelBase && s.HasControlPointCapability(id, ControlPointCapabilities.Glue)) continue;
					Point refPos = Geometry.RotatePoint(rotateCenter, Geometry.TenthsOfDegreeToDegrees(rotateAngle), ctrlPoints[id]);
					Point ptPos = s.GetControlPointPosition(id);
					Assert.IsTrue(Math.Abs(refPos.X - ptPos.X) <= 1 && Math.Abs(refPos.Y - ptPos.Y) <= 1);
				}
				s.Rotate(-rotateAngle, rotateCenter.X, rotateCenter.Y);
				int deltaX = cPos.X - s.X; int deltaY = cPos.Y - s.Y;
				foreach (ControlPointId id in s.GetControlPointIds(ControlPointCapabilities.Resize | ControlPointCapabilities.Reference)) {
					Point ptPos = s.GetControlPointPosition(id);
					if (ptPos != ctrlPoints[id]) ptPos.Offset(deltaX, deltaY);
					Assert.IsTrue(ptPos == ctrlPoints[id]);
				}
				bounds2 = s.GetBoundingRectangle(true);
				Assert.IsTrue(Math.Abs(bounds1.X - bounds2.X) <= Math.Abs(deltaX));
				Assert.IsTrue(Math.Abs(bounds1.Y - bounds2.Y) <= Math.Abs(deltaY));
				Assert.IsTrue(Math.Abs(bounds1.Width - bounds2.Width) <= Math.Abs(deltaX));
				Assert.IsTrue(Math.Abs(bounds1.Height - bounds2.Height) <= Math.Abs(deltaY));
				//
				cPos = s.GetControlPointPosition(ControlPointId.Reference);
				ControlPointId cpId = s.FindNearestControlPoint(cPos.X, cPos.Y, 1, ControlPointCapabilities.All);
				Assert.IsTrue(s.HasControlPointCapability(cpId, ControlPointCapabilities.Reference));
				//
				Point oiPoint = s.CalculateConnectionFoot(1000, 1000);
				bounds1 = s.GetBoundingRectangle(true);
				bounds1.Inflate(2, 2);
				Assert.IsTrue(Geometry.RectangleContainsPoint(bounds1, oiPoint));
				//
				Assert.IsTrue(s.IntersectsWith(bounds1.X, bounds1.Y, bounds1.Width, bounds1.Height));
				//
				bounds1 = new Rectangle(500, 500, 100, 100);
				s.Fit(bounds1.X, bounds1.Y, bounds1.Width, bounds1.Height);
				bounds1.Inflate(10, 10);
				Assert.IsTrue(Geometry.RectangleContainsRectangle(bounds1, s.GetBoundingRectangle(true)));
				//
				foreach (MenuItemDef a in s.GetMenuItemDefs(0, 0, 0))
					Assert.IsNotNull(a.Name);
				//
				// Connections
				// One connection from each connection point to the shape
				// One connection from each connection point to the next connection point
				foreach (ControlPointId cpi in s.GetControlPointIds(ControlPointCapabilities.Connect)) {
					Polyline polyline = (Polyline)project.ShapeTypes["Polyline"].CreateInstance();
					polyline.Connect(1, s, cpi);
					polyline.Connect(2, s, ControlPointId.Reference);
					Assert.AreNotEqual(ControlPointId.None, s.IsConnected(ControlPointId.Any, polyline));
					Assert.AreNotEqual(ControlPointId.None, s.IsConnected(cpi, polyline));
					Assert.AreNotEqual(ControlPointId.None, s.IsConnected(ControlPointId.Reference, polyline));
				}
				// Control point manipulation
				Point p = s.GetControlPointPosition(1);
				if (s.MoveControlPointBy(1, -10, -10, ResizeModifiers.None)) {
					p.Offset(-10, -10);
					Assert.AreEqual(p, s.GetControlPointPosition(1));
					s.MoveControlPointTo(1, p.X + 10, p.Y + 10, ResizeModifiers.None);
					p.Offset(+10, +10);
					Assert.AreEqual(p, s.GetControlPointPosition(1));
				}

				// ToDo: Improve this test case for handling resize modifiers.
				foreach (ControlPointId pointId in s.GetControlPointIds(ControlPointCapabilities.Resize)) {
				    MoveShapeControlPoint(s, pointId, 10, 10, ResizeModifiers.None);
				//    MoveShapeControlPoint(s, pointId, 10, 10, ResizeModifiers.MirroredResize);
				//    MoveShapeControlPoint(s, pointId, 10, 10, ResizeModifiers.MaintainAspect);
				//    MoveShapeControlPoint(s, pointId, 10, 10, ResizeModifiers.MaintainAspect | ResizeModifiers.MirroredResize);
				}
			}
			project.Close();
		}


		private void MoveShapeControlPoint(Shape shape, ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers) {
			Point origPointPos = shape.GetControlPointPosition(pointId);
			Point p = origPointPos, ptPos = origPointPos;
			// Move point to new position
			if (shape.MoveControlPointBy(pointId, deltaX, deltaY, modifiers)) {
				p.Offset(deltaX, deltaY);
				ptPos = shape.GetControlPointPosition(pointId);
				Assert.IsTrue(Math.Abs(p.X - ptPos.X) <= 1);
				Assert.IsTrue(Math.Abs(p.Y - ptPos.Y) <= 1);
				
				if (shape.MoveControlPointBy(pointId, -deltaX, -deltaY, modifiers)) {
					p.Offset(-deltaX,-deltaY);
					ptPos = shape.GetControlPointPosition(pointId);
					Assert.IsTrue(Math.Abs(p.X - ptPos.X) <= 1);
					Assert.IsTrue(Math.Abs(p.Y - ptPos.Y) <= 1);
				}
			}
			// Move point back to original position
			if (shape.MoveControlPointTo(pointId, origPointPos.X, origPointPos.Y, modifiers))
				Assert.AreEqual(origPointPos, shape.GetControlPointPosition(pointId));
			//ptPos = shape.GetControlPointPosition(pointId);
			//Assert.IsTrue(Math.Abs(origPointPos.X - ptPos.X) <= 1);
			//Assert.IsTrue(Math.Abs(origPointPos.Y - ptPos.Y) <= 1);
		}


		[TestMethod]
		public void AggregationTest() {
			// -- Create a project --
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Name = "Test";
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = RepositoryHelper.CreateXmlStore();
			project.Repository.Erase();
			project.Create();
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			Diagram diagram = new Diagram("Diagram A");
			// Create a group
			ShapeGroup group = (ShapeGroup)project.ShapeTypes["ShapeGroup"].CreateInstance();
			group.Children.Add(project.ShapeTypes["Circle"].CreateInstance(), 1);
			group.Children.Add(project.ShapeTypes["Square"].CreateInstance(), 2);
			group.Children.Add(project.ShapeTypes["Diamond"].CreateInstance(), 3);
			group.MoveTo(200, 200);
			diagram.Shapes.Add(group, 1);
			// Create an aggregation
			Box box = (Box)project.ShapeTypes["Box"].CreateInstance();
			box.Children.Add(project.ShapeTypes["Circle"].CreateInstance(), 1);
			box.Children.Add(project.ShapeTypes["Square"].CreateInstance(), 2);
			box.Children.Add(project.ShapeTypes["Diamond"].CreateInstance(), 3);
			box.MoveTo(400, 200);
			diagram.Shapes.Add(box, 2);
			// Save everything
			project.Repository.InsertAll(diagram);
			project.Repository.SaveChanges();
			project.Close();
			//
			// -- Reload and modify --
			project.Open();
			foreach (Diagram d in project.Repository.GetDiagrams())
				diagram = d;
			group = (ShapeGroup)diagram.Shapes.Bottom;
			Shape shape = null;
			foreach (Shape s in group.Children)
				shape = s;
			group.Children.Remove(shape);
			project.Repository.DeleteAll(shape);
			box = (Box)diagram.Shapes.TopMost;
			foreach (Shape s in box.Children)
				shape = s;
			box.Children.Remove(shape);
			project.Repository.DeleteAll(shape);
			project.Repository.SaveChanges();
			project.Close();
			//
			// -- Check --
			project.Open();
			foreach (Diagram d in project.Repository.GetDiagrams()) {
				project.Repository.GetDiagramShapes(d);
				foreach (Shape s in d.Shapes)
					Assert.AreEqual(2, s.Children.Count);
			}
			project.Close();
		}


		[TestMethod]
		public void TemplateTest() {
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Name = "Test";
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = RepositoryHelper.CreateXmlStore();
			project.Repository.Erase();
			project.Create();
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			Template template = new Template("Template1", project.ShapeTypes["RoundedBox"].CreateInstance());
			((IPlanarShape)template.Shape).FillStyle = project.Design.FillStyles.Red;
			project.Repository.InsertAll(template);
			Diagram diagram = new Diagram("Diagram A");
			diagram.Shapes.Add(template.CreateShape(), 1);
			project.Repository.InsertAll(diagram);
			Assert.ReferenceEquals(((IPlanarShape)diagram.Shapes.Bottom).FillStyle, ((IPlanarShape)template.Shape).FillStyle);
			IFillStyle fillStyle = project.Design.FillStyles.Green;
			((IPlanarShape)template.Shape).FillStyle = fillStyle;
			Assert.ReferenceEquals(((IPlanarShape)diagram.Shapes.Bottom).FillStyle, ((IPlanarShape)template.Shape).FillStyle);
			project.Repository.SaveChanges();
			project.Close();
			//
			project.Open();
			template = project.Repository.GetTemplate("Template1");
			diagram = project.Repository.GetDiagram("Diagram A");
			Assert.AreEqual(((IPlanarShape)diagram.Shapes.Bottom).FillStyle.BaseColorStyle, ((IPlanarShape)template.Shape).FillStyle.BaseColorStyle);
			project.Close();
		}


		[TestMethod]
		public void CommandTest() {
			// Initialize the project
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = RepositoryHelper.CreateXmlStore();
			project.Name = "Test";
			project.Repository.Erase();
			project.Create();
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			// Create a diagram with one shape
			Diagram diagram = new Diagram("Diagram A");
			project.Repository.InsertAll(diagram);
			Shape shape = project.ShapeTypes["Square"].CreateInstance();
			shape.X = 100;
			shape.Y = 100;
			// Execute commands
			project.ExecuteCommand(new CreateShapesCommand(diagram, LayerIds.None, shape, true, false));
			project.ExecuteCommand(new MoveShapeByCommand(shape, 200, 200));
			project.History.Undo();
			project.History.Undo();
			Assert.AreEqual(diagram.Shapes.Count, 0);
			project.History.Redo();
			Assert.AreEqual(diagram.Shapes.Count, 1);
			Assert.AreEqual(diagram.Shapes.Bottom.X, 100);
			project.History.Redo();
			Assert.AreEqual(diagram.Shapes.Bottom.X, 300);
			project.Repository.SaveChanges();
			project.Close();
		}


		[TestMethod]
		public void StylesTest() {
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = RepositoryHelper.CreateXmlStore();
			project.Name = "Test";
			project.Repository.Erase();
			project.Create();
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			ColorStyle colorStyle = (ColorStyle)project.Design.ColorStyles.Blue;
			colorStyle.Color = Color.LightBlue;
			project.Repository.Update(colorStyle);
			project.Repository.SaveChanges();
			project.Close();
			project.Open();
			colorStyle = (ColorStyle)project.Design.ColorStyles.Blue;
			Assert.AreEqual(Color.LightBlue.ToArgb(), colorStyle.Color.ToArgb());
			project.Close();
		}


		[TestMethod]
		public void ModelMappingTest() {
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = RepositoryHelper.CreateXmlStore();
			project.Name = "ModelMappingTest";
			project.Repository.Erase();
			project.Create();
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);

			// Create test data
			object[] floatTestData = CreateFloatTestData();
			object[] intTestData = CreateIntTestData();
			object[] stringTestData = CreateStringTestData();

			// Create model object and retrieve all properties
			GenericModelObject templateModelObject = (GenericModelObject)project.ModelObjectTypes["GenericModelObject"].CreateInstance();
			// Define property lists
			List<PropertyInfo> modelProperties = new List<PropertyInfo>();
			List<PropertyInfo> shapeProperties = new List<PropertyInfo>();
			GetPropertyInfos(templateModelObject.GetType(), modelProperties);
			
			// Test a shape of each shapeType
			foreach (ShapeType shapeType in project.ShapeTypes) {
				using (Shape templateShape = shapeType.CreateInstance()) {
					if (templateShape is IShapeGroup) continue;
					templateShape.ModelObject = templateModelObject;

					// Get mappable shape and model properties
					GetPropertyInfos(templateShape.GetType(), shapeProperties);

					Template template = new Template(shapeType.Name, templateShape);
					foreach (PropertyInfo modelProperty in modelProperties) {
						foreach (PropertyInfo shapeProperty in shapeProperties) {
							IModelMapping modelMapping = CreateModelMapping(shapeProperty, modelProperty);
							if (modelMapping == null) continue;
							//
							// Assign suitable test data
							object[] testValues = null;
							if (modelMapping.CanSetFloat) testValues = floatTestData;
							else if (modelMapping.CanSetInteger) testValues = intTestData;
							else if (modelMapping.CanSetString) testValues = stringTestData;
							//
							// Fill Model mappings and set expected results
							object[] testResults = null;
							if (modelMapping is NumericModelMapping)
								testResults = GetNumericMappingResults((NumericModelMapping)modelMapping, testValues);
							else if (modelMapping is FormatModelMapping)
								testResults = GetFormatMappingResults((FormatModelMapping)modelMapping, testValues);
							else if (modelMapping is StyleModelMapping)
								testResults = GetStyleMappingResults((StyleModelMapping)modelMapping, shapeProperty.PropertyType, testValues, project);
							//
							// Map properties
							template.UnmapAllProperties();
							template.MapProperties(modelMapping);

							// Test property mapping
							using (Shape testShape = template.CreateShape()) {
								IModelObject testModel = testShape.ModelObject;

								// Assign all test values to the model object and check the mapped shape property value.
								object templateShapePropValue = shapeProperty.GetValue(testShape, null);
								int cnt = testValues.Length;
								for (int i = 0; i < cnt; ++i) {
									try {
										// Assign the current test value to the model object's property value.
										// The shape's property has to be changed by the ModelMapping.
										object shapeValue = shapeProperty.GetValue(testShape, null);
										modelProperty.SetValue(testModel, testValues[i], null);
										object resultValue = shapeProperty.GetValue(testShape, null);
										object expectedValue = testResults[i] ?? templateShapePropValue;
										// 
										if (testShape is IPlanarShape && modelMapping.ShapePropertyId == 2) {
											// Angle property: All values are vonverted to positive angles < 360° (0 <= value <= 3600)
											if (expectedValue is int)
												expectedValue = ((3600 + (int)expectedValue) % 3600);
											else if (expectedValue is float)
												expectedValue = ((3600 + (float)expectedValue) % 3600);
										}
										//
										// Check mapping result
										bool areEqual = false;
										if (IsIntegerType(resultValue.GetType()) && IsIntegerType(expectedValue.GetType()))
											areEqual = object.Equals(Convert.ToInt64(resultValue), Convert.ToInt64(expectedValue));
										else if (IsFloatType(resultValue.GetType()) && IsFloatType(expectedValue.GetType()))
											areEqual = object.Equals(Convert.ToDouble(resultValue), Convert.ToDouble(expectedValue));
										else
											areEqual = object.Equals(resultValue, expectedValue);
										// Error reporting
										if (!areEqual) {
											if (object.Equals(shapeValue, resultValue))
												Assert.Fail(
													"Assigning '{0}' to {1} had no effect on {2}'s property '{3}': Property value '{4}' did not change.",
													testValues[i],
													testModel.Type.Name,
													testShape.Type.Name,
													shapeProperty.Name,
													shapeValue);
											else
												Assert.Fail(
													"Assigning '{0}' to {1} had not the expected effect on {2}'s property '{3}': Property value '{4}' changed to '{5}' instead of '{6}'.",
													testValues[i],
													testModel.Type.Name,
													testShape.Type.Name,
													shapeProperty.Name,
													shapeValue,
													resultValue,
													expectedValue);
										}
									} catch (TargetInvocationException exc) {
										bool throwInnerException = true;
										if (exc.InnerException is OverflowException && modelMapping is NumericModelMapping) {
											double testValue = Convert.ToDouble(testValues[i]);
											if (double.IsNaN(testValue))
												throwInnerException = false;
											else {
												double resultValue = (double)(((NumericModelMapping)modelMapping).Intercept + (testValue * ((NumericModelMapping)modelMapping).Slope));
												double minValue, maxValue;
												GetMinAndMaxValue(shapeProperty.PropertyType, out minValue, out maxValue);
												if (resultValue < minValue || resultValue > maxValue)
													throwInnerException = false;
											}
										} else if (exc.InnerException is ArgumentOutOfRangeException)
											throwInnerException = false;

										// throw inner exception if the exception was not handled.
										if (throwInnerException) throw exc.InnerException;
									}
								}	// foreach test value
							}	// using block (Shape testShape)
						}	// foreach Shape property
					}	// foreach IModelObject property
				}	// using block (Shape templateShape)
			}	// foreach ShapeType (...)

			project.Close();
		}


		[TestMethod]
		public void BoundingRectangleTest() {
			// -- Create a project --
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Name = "BoundingRectangleTest";
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = new XmlStore(@"C:\Temp", ".xml");
			project.Repository.Erase();
			project.Create();

			// Add Libraries:
			// GeneralShapes
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			// ElectricalShapes
			project.AddLibrary(typeof(Dataweb.NShape.ElectricalShapes.AutoDisconnectorSymbol).Assembly, true);
			// FlowChartShapes
			project.AddLibrary(typeof(Dataweb.NShape.FlowChartShapes.ProcessSymbol).Assembly, true);
			// SoftwareArchitectureShapes
			project.AddLibrary(typeof(Dataweb.NShape.SoftwareArchitectureShapes.CloudSymbol).Assembly, true);

			//
			Diagram diagram = new Diagram("All Shapes");
			diagram.Width = 500;
			diagram.Height = 500;
			project.Repository.InsertAll(diagram);
			int shapeCount = 0;
			foreach (ShapeType st in project.ShapeTypes) {
				Shape s = st.CreateInstance();
				diagram.Shapes.Add(s, shapeCount + 1);
				project.Repository.InsertAll(s, diagram);
				++shapeCount;
			}

			BoundingRectangleTestCore(diagram.Shapes, 0, 100, 0, ResizeModifiers.None);
			BoundingRectangleTestCore(diagram.Shapes, 200, 100, 300, ResizeModifiers.None);
			BoundingRectangleTestCore(diagram.Shapes, 100, 2, 600, ResizeModifiers.None);

			project.Close();
		}


		[TestMethod]
		public void ExportToImageTest() {
			// -- Create a project --
			Project project = new Project();
			project.AutoLoadLibraries = true;
			project.Name = "ExportToImageTest";
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = new XmlStore(@"C:\Temp", ".xml");
			project.Repository.Erase();
			project.Create();

			// Add Libraries:
			// GeneralShapes
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);
			// ElectricalShapes
			project.AddLibrary(typeof(Dataweb.NShape.ElectricalShapes.AutoDisconnectorSymbol).Assembly, true);
			// FlowChartShapes
			project.AddLibrary(typeof(Dataweb.NShape.FlowChartShapes.ProcessSymbol).Assembly, true);
			// SoftwareArchitectureShapes
			project.AddLibrary(typeof(Dataweb.NShape.SoftwareArchitectureShapes.CloudSymbol).Assembly, true);

			//
			DiagramHelper.CreateDiagram(project, "Export Test Diagram", 10, 10, true, false, false, false, false);
			string filePathFmt = System.IO.Path.Combine(Environment.GetEnvironmentVariable("Temp"), "ExportTest {0}.{1}");
			const int dpi = 150;
			foreach (Diagram diagram in project.Repository.GetDiagrams()) {
				IEnumerable<Shape> shapes = diagram.Shapes.FindShapes(diagram.Width / 4, diagram.Height / 4, diagram.Width / 2, diagram.Height / 2, false);
				foreach (ImageFileFormat fileFormat in Enum.GetValues(typeof(ImageFileFormat))) {
					try {
						string fileExt = (fileFormat == ImageFileFormat.EmfPlus) ? "Plus.emf" : fileFormat.ToString();

						using (Image img = diagram.CreateImage(fileFormat))
							GdiHelpers.SaveImageToFile(img, string.Format(filePathFmt, "Whole Diagram", fileExt), fileFormat);

						using (Image img = diagram.CreateImage(fileFormat, shapes))
							GdiHelpers.SaveImageToFile(img, string.Format(filePathFmt, "Shapes", fileExt), fileFormat);

						using (Image img = diagram.CreateImage(fileFormat, shapes, true))
							GdiHelpers.SaveImageToFile(img, string.Format(filePathFmt, "Shapes with Background", fileExt), fileFormat);
						
						using (Image img = diagram.CreateImage(fileFormat, shapes, 5))
							GdiHelpers.SaveImageToFile(img, string.Format(filePathFmt, "Shapes with Margin", fileExt), fileFormat);
						
						using (Image img = diagram.CreateImage(fileFormat, shapes, 5, false, Color.AliceBlue))
							GdiHelpers.SaveImageToFile(img, string.Format(filePathFmt, "Shapes with Background Color", fileExt), fileFormat);
						
						using (Image img = diagram.CreateImage(fileFormat, shapes, 5, false, Color.AliceBlue, dpi))
							GdiHelpers.SaveImageToFile(img, string.Format(filePathFmt, string.Format("Shapes with Background Color {0} Dpi", dpi), fileExt), fileFormat);
					} catch (NotImplementedException) {
						// Do nothing
					}
				}
			}

			project.Close();
		}

		#endregion


		#region Test core methods

		private void BoundingRectangleTestCore(IShapeCollection shapes, int shapePos, int shapeSize, int shapeAngle, ResizeModifiers resizeModifier) {
			foreach (Shape s in shapes) {
				// Move shape
				s.MoveTo(shapePos, shapePos);

				// Resize shape
				int halfShapeSize = shapeSize / 2;
				if (s is RectangleBase) {
					((RectangleBase)s).Width = shapeSize;
					((RectangleBase)s).Height = shapeSize;
				} else if (s is DiamondBase) {
					((DiamondBase)s).Width = shapeSize;
					((DiamondBase)s).Height = shapeSize;
				} else if (s is CircleBase)
					((CircleBase)s).Diameter = shapeSize;
				else if (s is SquareBase) {
					((SquareBase)s).Size = shapeSize;
				} else if (s is ImageBasedShape) {
					Rectangle r = s.GetBoundingRectangle(true);	// Get current size
					Point p = s.GetControlPointPosition(8);		// Get current control point pos
					((ImageBasedShape)s).MoveControlPointTo(8, 0, r.Bottom + (shapeSize - r.Height), resizeModifier);
				} else if (s is PolylineBase || s is RectangularLineBase) {
					s.MoveControlPointTo(ControlPointId.FirstVertex, shapePos - halfShapeSize, shapePos - halfShapeSize, resizeModifier);
					s.MoveControlPointTo(ControlPointId.LastVertex, shapePos - halfShapeSize, shapePos - halfShapeSize, resizeModifier);
				} else if (s is TriangleBase) {
					s.MoveControlPointTo(1, shapePos, shapePos - halfShapeSize, resizeModifier);
					s.MoveControlPointTo(2, shapePos - halfShapeSize, shapePos, resizeModifier);
					s.MoveControlPointTo(3, shapePos + halfShapeSize, shapePos + halfShapeSize, resizeModifier);
				} else if (s is RegularPolygoneBase) {
					s.MoveControlPointTo(1, shapePos, shapePos - halfShapeSize, resizeModifier);
				} else if (s is CircularArcBase) {
					// ToDo: Add third point
					s.MoveControlPointTo(ControlPointId.FirstVertex, -shapeSize / 2, -shapeSize / 2, resizeModifier);
					s.MoveControlPointTo(ControlPointId.LastVertex, shapeSize / 2, shapeSize / 2, resizeModifier);
				} else if (s is ShapeGroup) {
					s.Children.Add(shapes.TopMost);
				} else throw new Exception(string.Format("Untested {0} '{1}'!", s.Type.GetType().Name, s.Type.FullName));

				// rotate shape
				if (shapeAngle != 0 && s is IPlanarShape)
					((IPlanarShape)s).Angle = shapeAngle;

				Rectangle tightBounds = s.GetBoundingRectangle(true);
				Rectangle looseBounds = s.GetBoundingRectangle(false);

				Assert.IsTrue(Geometry.IsValid(tightBounds));
				Assert.IsTrue(Geometry.IsValid(looseBounds));
				Assert.IsTrue(!tightBounds.IsEmpty);
				Assert.IsTrue(!looseBounds.IsEmpty);

				Assert.IsTrue(tightBounds.Width >= 0);
				Assert.IsTrue(tightBounds.Height >= 0);
				Assert.IsTrue(looseBounds.Width >= 0);
				Assert.IsTrue(looseBounds.Height >= 0);

				Assert.IsTrue(tightBounds.X >= looseBounds.X);
				Assert.IsTrue(tightBounds.Y >= looseBounds.Y);
				Assert.IsTrue(tightBounds.Width <= looseBounds.Width);
				Assert.IsTrue(tightBounds.Height <= looseBounds.Height);

				// ToDo: Add more precise tests
				// - Calculate expected (rotated) size of the shape and compare with tightBounds
				// - Calculate expected (rotated) size of the shape's control points and compare with looseBounds
			}
		}

		#endregion


		#region ModelMappingTest helper methods

		private object[] CreateFloatTestData() {
			object[] result = new object[16];
			result[0] = float.NaN;
			result[1] = float.NegativeInfinity;
			result[2] = int.MinValue;
			result[3] = int.MinValue + 1;
			result[4] = short.MinValue - 0.00001f;
			result[5] = short.MinValue;
			result[6] = short.MinValue + 0.00001f;
			result[7] = -float.Epsilon;
			result[8] = 0;
			result[9] = float.Epsilon;
			result[10] = short.MaxValue - 0.00001f;
			result[11] = short.MaxValue;
			result[12] = short.MaxValue + 0.00001f;
			result[13] = int.MaxValue - 1;
			result[14] = int.MaxValue;
			result[15] = float.PositiveInfinity;
			return result;
		}


		private object[] CreateIntTestData() {
			object[] result = new object[13];
			result[0] = int.MinValue;
			result[1] = int.MinValue + 1;
			result[2] = short.MinValue - 1;
			result[3] = short.MinValue;
			result[4] = short.MinValue + 1;
			result[5] = -1;
			result[6] = 0;
			result[7] = 1;
			result[8] = short.MaxValue - 1;
			result[9] = short.MaxValue;
			result[10] = short.MaxValue + 1;
			result[11] = int.MaxValue - 1;
			result[12] = int.MaxValue;
			return result;
		}


		private object[] CreateStringTestData() {
			object[] result = new object[4];
			result[0] = null;
			result[1] = string.Empty;
			result[2] = "abcÄÖÜäöüß";
			result[3] = new string('ä', short.MaxValue);
			return result;
		}


		private object[] GetNumericMappingResults(NumericModelMapping mapping, object[] testData) {
			if (mapping == null) throw new ArgumentNullException("maping");
			if (testData == null) throw new ArgumentNullException("testData");
			// Define mapping
			const float intercept = 100;
			const float slope = 2;
	
			// Fill mapping
			mapping.Intercept = intercept;
			mapping.Slope = slope;

			// Set expected results
			int cnt = testData.Length;
			object[] result = new object[cnt];
			for (int i = 0; i < cnt; ++i) {
				float value = Convert.ToSingle(testData[i]);
				switch (mapping.Type) {
					case NumericModelMapping.MappingType.FloatFloat:
					case NumericModelMapping.MappingType.IntegerFloat:
						result[i] = intercept + (value * slope);
						break;
					case NumericModelMapping.MappingType.FloatInteger:
					case NumericModelMapping.MappingType.IntegerInteger:
						result[i] = (int)Math.Round(mapping.Intercept + (value * mapping.Slope));
						break;
					default: throw new NShapeUnsupportedValueException(mapping.Type);
				}
			}
			return result;
		}


		private object[] GetFormatMappingResults(FormatModelMapping mapping, object[] testData) {
			if (mapping == null) throw new ArgumentNullException("maping");
			if (testData == null) throw new ArgumentNullException("testData");
			// Define mapping
			string formatString = modelMappingFormatPrefix + "{0}" + modelMappingFormatSuffix;
			
			// Fill mapping
			mapping.Format = formatString;
			
			// Set expected results
			int cnt = testData.Length;
			object[] result = new object[cnt];
			for (int i = 0; i < cnt; ++i) {
				if (mapping.CanSetFloat) {
					float value = Convert.ToSingle(testData[i]);
					result[i] = string.Format(formatString, value);
				} else if (mapping.CanSetInteger) {
					int value = Convert.ToInt32(testData[i]);
					result[i] = string.Format(formatString, value);
				} else
					result[i] = string.Format(formatString, testData[i]);
			}
			return result;
		}


		private object[] GetStyleMappingResults(StyleModelMapping mapping, Type propertyType, object[] testData, Project project) {
			if (mapping == null) throw new ArgumentNullException("maping");
			if (testData == null) throw new ArgumentNullException("testData");
			// Define mapping
			int valueCnt = 5;
			int[] values = new int[valueCnt];
			values[0] = int.MinValue;
			values[1] = -1000;
			values[2] = 0;
			values[3] = 1000;
			values[4] = int.MaxValue;
			IStyle[] styles = new IStyle[valueCnt];
			if (IsOfType(propertyType, typeof(ICapStyle))) {
				styles[0] = project.Design.CapStyles.ClosedArrow;
				styles[1] = project.Design.CapStyles.Special1;
				styles[2] = project.Design.CapStyles.None;
				styles[3] = project.Design.CapStyles.Special2;
				styles[4] = project.Design.CapStyles.OpenArrow;
			} else if (IsOfType(propertyType, typeof(ICharacterStyle))) {
				styles[0] = project.Design.CharacterStyles.Caption;
				styles[1] = project.Design.CharacterStyles.Heading1;
				styles[2] = project.Design.CharacterStyles.Heading2;
				styles[3] = project.Design.CharacterStyles.Heading3;
				styles[4] = project.Design.CharacterStyles.Normal;
			} else if (IsOfType(propertyType, typeof(IColorStyle))) {
				styles[0] = project.Design.ColorStyles.Black;
				styles[1] = project.Design.ColorStyles.Red;
				styles[2] = project.Design.ColorStyles.Yellow;
				styles[3] = project.Design.ColorStyles.Green;
				styles[4] = project.Design.ColorStyles.White;
			} else if (IsOfType(propertyType, typeof(IFillStyle))) {
				styles[0] = project.Design.FillStyles.Black;
				styles[1] = project.Design.FillStyles.Red;
				styles[2] = project.Design.FillStyles.Yellow;
				styles[3] = project.Design.FillStyles.Green;
				styles[4] = project.Design.FillStyles.White;
			} else if (IsOfType(propertyType, typeof(ILineStyle))) {
				styles[0] = project.Design.LineStyles.Special1;
				styles[1] = project.Design.LineStyles.Red;
				styles[2] = project.Design.LineStyles.Yellow;
				styles[3] = project.Design.LineStyles.Green;
				styles[4] = project.Design.LineStyles.Special2;
			} else if (IsOfType(propertyType, typeof(IParagraphStyle))) {
				styles[0] = project.Design.ParagraphStyles.Label;
				styles[1] = project.Design.ParagraphStyles.Text;
				styles[2] = project.Design.ParagraphStyles.Title;
				styles[3] = project.Design.ParagraphStyles.Text;
				styles[4] = project.Design.ParagraphStyles.Label;
			}

			// Fill mapping
			mapping.ClearValueRanges();
			for (int i = 0; i < valueCnt; ++i) {
				if (mapping.CanSetInteger) mapping.AddValueRange(values[i], styles[i]);
				else if (mapping.CanSetFloat) mapping.AddValueRange((float)values[i], styles[i]);
			}

			// Set expected results
			int cnt = testData.Length;
			object[] result = new object[cnt];
			for (int i = 0; i < cnt; ++i) {
				double value;
				if (mapping.CanSetFloat)
					value = Convert.ToSingle(testData[i]);
				else value = Convert.ToDouble(testData[i]);

				if (double.IsNaN(value) || double.IsNegativeInfinity(value))
					result[i] = null;
				else if (value < values[1])
					result[i] = styles[0];
				else if (value >= values[1] && value < values[2])
					result[i] = styles[1];
				else if (value >= values[2] && value < values[3])
					result[i] = styles[2];
				else if (value >= values[3] && value < values[4])
					result[i] = styles[3];
				else if (value >= values[4])
					result[i] = styles[4];
				else 
					result[i] = null;
			}
			return result;
		}


		private int? GetPropertyId(PropertyInfo propertyInfo) {
			if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
			object[] idAttribs = Attribute.GetCustomAttributes(propertyInfo, typeof(PropertyMappingIdAttribute), true);
			if (idAttribs.Length == 1) {
				Debug.Assert(idAttribs[0] is PropertyMappingIdAttribute);
				return ((PropertyMappingIdAttribute)idAttribs[0]).Id;
			} else if (idAttribs.Length == 0) return null;
			else throw new NShapeException("Property {0} of {1} has more than 1 {2}.", propertyInfo.Name, propertyInfo.DeclaringType.Name, typeof(PropertyMappingIdAttribute).Name);
		}


		private void GetPropertyInfos(Type type, IList<PropertyInfo> propertyInfos) {
			propertyInfos.Clear();
			PropertyInfo[] piArray = type.GetProperties(bindingFlags);
			for (int i = 0; i < piArray.Length; ++i) {
				if (GetPropertyId(piArray[i]).HasValue)
					propertyInfos.Add(piArray[i]);
			}
		}


		// Find PropertyInfo with the given name
		private PropertyInfo FindPropertyInfo(IList<PropertyInfo> propertyInfos, string propertyName) {
			PropertyInfo result = null;
			for (int i = 0; i < propertyInfos.Count; ++i) {
				if (propertyInfos[i].Name == propertyName) {
					result = propertyInfos[i];
					break;
				}
			}
			return result;
		}


		// Find propertyInfo with the given PropertyId
		private PropertyInfo FindPropertyInfo(IList<PropertyInfo> propertyInfos, int propertyId) {
			PropertyInfo result = null;
			for (int i = 0; i < propertyInfos.Count; ++i) {
				int? id = GetPropertyId(propertyInfos[i]);
				if (id.HasValue && id.Value == propertyId) {
					result = propertyInfos[i];
					break;
				}
			}
			return result;
		}


		// Create a ModelMapping from the given PropertyInfos
		private IModelMapping CreateModelMapping(PropertyInfo shapePropertyInfo, PropertyInfo modelPropertyInfo) {
			// Get PropertyId's
			int? shapePropertyId = GetPropertyId(shapePropertyInfo);
			int? modelPropertyId = GetPropertyId(modelPropertyInfo);
			if (!shapePropertyId.HasValue || !modelPropertyId.HasValue)
				return null;
			//
			// Determine property types
			MappedPropertyType shapePropType;
			if (IsIntegerType(shapePropertyInfo.PropertyType)) shapePropType = MappedPropertyType.Int;
			else if (IsFloatType(shapePropertyInfo.PropertyType)) shapePropType = MappedPropertyType.Float;
			else if (IsStringType(shapePropertyInfo.PropertyType)) shapePropType = MappedPropertyType.String;
			else if (IsStyleType(shapePropertyInfo.PropertyType)) shapePropType = MappedPropertyType.Style;
			else return null;
			//
			MappedPropertyType modelPropType;
			if (IsIntegerType(modelPropertyInfo.PropertyType)) modelPropType = MappedPropertyType.Int;
			else if (IsFloatType(modelPropertyInfo.PropertyType)) modelPropType = MappedPropertyType.Float;
			else if (IsStringType(modelPropertyInfo.PropertyType)) modelPropType = MappedPropertyType.String;
			else return null;
			//
			// Create a suitable ModelMapping:
			IModelMapping result = null;
			switch (modelPropType) {
				case MappedPropertyType.Float:
					switch (shapePropType) {
						case MappedPropertyType.Float: result = new NumericModelMapping(shapePropertyId.Value, modelPropertyId.Value, NumericModelMapping.MappingType.FloatFloat); break;
						case MappedPropertyType.Int: result = new NumericModelMapping(shapePropertyId.Value, modelPropertyId.Value, NumericModelMapping.MappingType.FloatInteger); break;
						case MappedPropertyType.String: result = new FormatModelMapping(shapePropertyId.Value, modelPropertyId.Value, FormatModelMapping.MappingType.FloatString); break;
						case MappedPropertyType.Style: result = new StyleModelMapping(shapePropertyId.Value, modelPropertyId.Value, StyleModelMapping.MappingType.FloatStyle); break;
						default: break;
					}
					break;
				case MappedPropertyType.Int:
					switch (shapePropType) {
						case MappedPropertyType.Float: result = new NumericModelMapping(shapePropertyId.Value, modelPropertyId.Value, NumericModelMapping.MappingType.IntegerFloat); break;
						case MappedPropertyType.Int: result = new NumericModelMapping(shapePropertyId.Value, modelPropertyId.Value, NumericModelMapping.MappingType.IntegerInteger); break;
						case MappedPropertyType.String: result = new FormatModelMapping(shapePropertyId.Value, modelPropertyId.Value, FormatModelMapping.MappingType.IntegerString); break;
						case MappedPropertyType.Style: result = new StyleModelMapping(shapePropertyId.Value, modelPropertyId.Value, StyleModelMapping.MappingType.IntegerStyle); break;
						default: break;
					}
					break;
				case MappedPropertyType.String:
					if (shapePropType == MappedPropertyType.String)
						result = new FormatModelMapping(shapePropertyId.Value, modelPropertyId.Value, FormatModelMapping.MappingType.StringString);
					break;
				default: throw new NotSupportedException();
			}
			return result;
		}


		/// <summary>
		/// Gets the min and max values of a numeric type
		/// </summary>
		private void GetMinAndMaxValue(Type type, out double minValue, out double maxValue) {
			minValue = maxValue = double.NaN;
			if (type == typeof(Byte)) {
				minValue = Byte.MinValue;
				maxValue = Byte.MaxValue;
			} else if (type == typeof(Int16)) {
				minValue = Int16.MinValue;
				maxValue = Int16.MaxValue;
			} else if (type == typeof(Int32)) {
				minValue = Int32.MinValue;
				maxValue = Int32.MaxValue;
			} else if (type == typeof(Int64)) {
				minValue = Int64.MinValue;
				maxValue = Int64.MaxValue;
			} else if (type == typeof(SByte)) {
				minValue = SByte.MinValue;
				maxValue = SByte.MaxValue;
			} else if (type == typeof(UInt16)) {
				minValue = UInt16.MinValue;
				maxValue = UInt16.MaxValue;
			} else if (type == typeof(UInt32)
				|| type == typeof(Enum)) {
				minValue = UInt32.MinValue;
				maxValue = UInt32.MaxValue;
			} else if (type == typeof(UInt64)) {
				minValue = UInt64.MinValue;
				maxValue = UInt64.MaxValue;
			} else if (type == typeof(Single)) {
				minValue = Single.MinValue;
				maxValue = Single.MaxValue;
			} else if (type == typeof(Double)) {
				minValue = Double.MinValue;
				maxValue = Double.MaxValue;
			} else if (type == typeof(Decimal)) {
				minValue = (double)Decimal.MinValue;
				maxValue = (double)Decimal.MaxValue;
			}
		}
		
		
		// Check if the given Type is an integer type (byte, int16, int32, int64, enum)
		private bool IsIntegerType(Type type) {
			return (type == typeof(Byte)
				|| type == typeof(Int16)
				|| type == typeof(Int32)
				|| type == typeof(Int64)
				|| type == typeof(SByte)
				|| type == typeof(UInt16)
				|| type == typeof(UInt32)
				|| type == typeof(UInt64)
				|| type == typeof(Enum));
		}


		// Check if the given Type is a float type (single, double or decimal)
		private bool IsFloatType(Type type) {
			return (type == typeof(Single)
				|| type == typeof(Double)
				|| type == typeof(Decimal));
		}


		// Check if the given Type is a string type (char or string)
		private bool IsStringType(Type type) {
			return (type == typeof(String)
				|| type == typeof(Char));
		}


		// Check if the given type is based on IStyle
		private bool IsStyleType(Type type) {
			return IsOfType(type, typeof(IStyle));
		}


		// Check if the given type is based on targetType
		private bool IsOfType(Type type, Type targetType) {
			return (type == targetType || type.IsSubclassOf(targetType) || type.GetInterface(targetType.Name, true) != null);
		}

		
		private enum MappedPropertyType { Int, Float, String, Style };
		
		#endregion


		private TestContext testContextInstance;

		// Stuff for ModelMapping Test
		private const string modelMappingFormatPrefix = "Property Value = '";
		private const string modelMappingFormatSuffix = "'.";
		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;
	}

}
