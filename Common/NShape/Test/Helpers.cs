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
using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.FlowChartShapes;
using Dataweb.NShape.GeneralModelObjects;
using Dataweb.NShape.GeneralShapes;


namespace NShapeTest {

	public static class DiagramHelper {

		public static void CreateLargeDiagram(Project project, string diagramName) {
			const int shapesPerSide = 100;
			CreateDiagram(project, diagramName, shapesPerSide, shapesPerSide, true, false, false, false, true);
		}


		public static void CreateDiagram(Project project, string diagramName, int shapesPerRow, int shapesPerColumn, bool connectShapes, bool withModels, bool withTerminalMappings, bool withModelMappings, bool withLayers) {
			const int shapeSize = 80;
			int lineLength = shapeSize / 2;
			//
			// Create ModelMappings
			NumericModelMapping numericModelMapping = null;
			FormatModelMapping formatModelMapping = null;
			StyleModelMapping styleModelMapping = null;
			if (withModelMappings) {
				// Create numeric- and format model mappings
				numericModelMapping = new NumericModelMapping(2, 4, NumericModelMapping.MappingType.FloatInteger, 10, 0);
				formatModelMapping = new FormatModelMapping(4, 2, FormatModelMapping.MappingType.StringString, "{0}");
				// Create style model mapping
				float range = (shapesPerRow * shapesPerColumn) / 15f;
				styleModelMapping = new StyleModelMapping(1, 4, StyleModelMapping.MappingType.FloatStyle);
				for (int i = 0; i < 15; ++i) {
					IStyle style = null;
					switch (i) {
						case 0: style = project.Design.LineStyles.None; break;
						case 1: style = project.Design.LineStyles.Dotted; break;
						case 2: style = project.Design.LineStyles.Dashed; break;
						case 3: style = project.Design.LineStyles.Special1; break;
						case 4: style = project.Design.LineStyles.Special2; break;
						case 5: style = project.Design.LineStyles.Normal; break;
						case 6: style = project.Design.LineStyles.Blue; break;
						case 7: style = project.Design.LineStyles.Green; break;
						case 8: style = project.Design.LineStyles.Yellow; break;
						case 9: style = project.Design.LineStyles.Red; break;
						case 10: style = project.Design.LineStyles.HighlightDotted; break;
						case 11: style = project.Design.LineStyles.HighlightDashed; break;
						case 12: style = project.Design.LineStyles.Highlight; break;
						case 13: style = project.Design.LineStyles.HighlightThick; break;
						case 14: style = project.Design.LineStyles.Thick; break;
						default: style = null; break;
					}
					if (style != null) styleModelMapping.AddValueRange(i * range, style);
				}
			}
			//
			// Create model obejct for the planar shape's template
			IModelObject planarModel = null;
			if (withModels) planarModel = project.ModelObjectTypes["Core.GenericModelObject"].CreateInstance();
			//
			// Create a shape for the planar shape's template
			Circle circleShape = (Circle)project.ShapeTypes["Circle"].CreateInstance();
			circleShape.Diameter = shapeSize;
			//
			// Create a template for the planar shapes
			Template planarTemplate = new Template("PlanarShape Template", circleShape);
			if (withModels) {
				planarTemplate.Shape.ModelObject = planarModel;
				planarTemplate.MapTerminal(TerminalId.Generic, ControlPointId.Reference);
				if (withTerminalMappings) {
					foreach (ControlPointId id in planarTemplate.Shape.GetControlPointIds(ControlPointCapabilities.Connect))
						planarTemplate.MapTerminal(TerminalId.Generic, id);
				}
				if (withModelMappings) {
					planarTemplate.MapProperties(numericModelMapping);
					planarTemplate.MapProperties(formatModelMapping);
					planarTemplate.MapProperties(styleModelMapping);
				}
			}
			// 
			// Create a template for the linear shapes
			Template linearTemplate = null;
			if (connectShapes)
				linearTemplate = new Template("LinearShape Template", project.ShapeTypes["Polyline"].CreateInstance());
			//
			// Insert the created templates into the repository
			project.Repository.InsertAll(planarTemplate);
			if (connectShapes) project.Repository.InsertAll(linearTemplate);
			//
			// Prepare the connection points
			ControlPointId leftPoint = withModels ? ControlPointId.Reference : 4;
			ControlPointId rightPoint = withModels ? ControlPointId.Reference : 5;
			ControlPointId topPoint = withModels ? ControlPointId.Reference : 2;
			ControlPointId bottomPoint = withModels ? ControlPointId.Reference : 7;
			//
			// Create the diagram
			Diagram diagram = new Diagram(diagramName);
			//
			// Create and add layers
			LayerIds planarLayer = LayerIds.None, linearLayer = LayerIds.None, oddRowLayer = LayerIds.None,
				evenRowLayer = LayerIds.None, oddColLayer = LayerIds.None, evenColLayer = LayerIds.None;
			if (withLayers) {
				const string planarLayerName = "PlanarShapesLayer";
				const string linearLayerName = "LinearShapesLayer";
				const string oddRowsLayerName = "OddRowsLayer";
				const string evenRowsLayerName = "EvenRowsLayer";
				const string oddColsLayerName = "OddColsLayer";
				const string evenColsLayerName = "EvenColsLayer";
				// Create Layers
				Layer planarShapesLayer = new Layer(planarLayerName);
				planarShapesLayer.Title = "Planar Shapes";
				planarShapesLayer.LowerZoomThreshold = 5;
				planarShapesLayer.UpperZoomThreshold = 750;
				diagram.Layers.Add(planarShapesLayer);
				Layer linearShapesLayer = new Layer(linearLayerName);
				linearShapesLayer.Title = "Linear Shapes";
				linearShapesLayer.LowerZoomThreshold = 10;
				linearShapesLayer.UpperZoomThreshold = 500;
				diagram.Layers.Add(linearShapesLayer);
				Layer oddRowsLayer = new Layer(oddRowsLayerName);
				oddRowsLayer.Title = "Odd Rows";
				oddRowsLayer.LowerZoomThreshold = 2;
				oddRowsLayer.UpperZoomThreshold = 1000;
				diagram.Layers.Add(oddRowsLayer);
				Layer evenRowsLayer = new Layer(evenRowsLayerName);
				evenRowsLayer.Title = "Even Rows";
				evenRowsLayer.LowerZoomThreshold = 2;
				evenRowsLayer.UpperZoomThreshold = 1000;
				diagram.Layers.Add(evenRowsLayer);
				Layer oddColsLayer = new Layer(oddColsLayerName);
				oddColsLayer.Title = "Odd Columns";
				oddColsLayer.LowerZoomThreshold = 2;
				oddColsLayer.UpperZoomThreshold = 1000;
				diagram.Layers.Add(oddColsLayer);
				Layer evenColsLayer = new Layer(evenColsLayerName);
				evenColsLayer.Title = "Even Columns";
				evenColsLayer.LowerZoomThreshold = 2;
				evenColsLayer.UpperZoomThreshold = 1000;
				diagram.Layers.Add(evenColsLayer);
				// Assign LayerIds
				planarLayer = diagram.Layers.FindLayer(planarLayerName).Id;
				linearLayer = diagram.Layers.FindLayer(linearLayerName).Id;
				oddRowLayer = diagram.Layers.FindLayer(oddRowsLayerName).Id;
				evenRowLayer = diagram.Layers.FindLayer(evenRowsLayerName).Id;
				oddColLayer = diagram.Layers.FindLayer(oddColsLayerName).Id;
				evenColLayer = diagram.Layers.FindLayer(evenColsLayerName).Id;
			}

			for (int rowIdx = 0; rowIdx < shapesPerRow; ++rowIdx) {
				LayerIds rowLayer = ((rowIdx + 1) % 2 == 0) ? evenRowLayer : oddRowLayer;
				for (int colIdx = 0; colIdx < shapesPerRow; ++colIdx) {
					LayerIds colLayer = ((colIdx + 1) % 2 == 0) ? evenColLayer : oddColLayer;
					int shapePosX = shapeSize + colIdx * (lineLength + shapeSize);
					int shapePosY = shapeSize + rowIdx * (lineLength + shapeSize);

					circleShape = (Circle)planarTemplate.CreateShape();
					circleShape.Text = string.Format("{0} / {1}", rowIdx + 1, colIdx + 1);
					circleShape.MoveTo(shapePosX, shapePosY);
					if (withModels) {
						project.Repository.Insert(circleShape.ModelObject);
						((GenericModelObject)circleShape.ModelObject).IntegerValue = rowIdx;
					}

					diagram.Shapes.Add(circleShape, project.Repository.ObtainNewTopZOrder(diagram));
					if (withLayers) diagram.AddShapeToLayers(circleShape, planarLayer | rowLayer | colLayer);
					if (connectShapes) {
						if (rowIdx > 0) {
							Shape lineShape = linearTemplate.CreateShape();
							lineShape.Connect(ControlPointId.FirstVertex, circleShape, topPoint);
							Assert.AreNotEqual(ControlPointId.None, lineShape.IsConnected(ControlPointId.FirstVertex, circleShape));

							Shape otherShape = diagram.Shapes.FindShape(shapePosX, shapePosY - shapeSize, ControlPointCapabilities.None, 0, null);
							lineShape.Connect(ControlPointId.LastVertex, otherShape, bottomPoint);
							diagram.Shapes.Add(lineShape, project.Repository.ObtainNewBottomZOrder(diagram));
							if (withLayers) diagram.AddShapeToLayers(lineShape, linearLayer);
							Assert.AreNotEqual(ControlPointId.None, lineShape.IsConnected(ControlPointId.LastVertex, otherShape));
						}
						if (colIdx > 0) {
							Shape lineShape = linearTemplate.CreateShape();
							lineShape.Connect(1, circleShape, leftPoint);
							Assert.AreNotEqual(ControlPointId.None, lineShape.IsConnected(ControlPointId.FirstVertex, circleShape));

							Shape otherShape = diagram.Shapes.FindShape(shapePosX - shapeSize, shapePosY, ControlPointCapabilities.None, 0, null);
							lineShape.Connect(2, otherShape, rightPoint);
							diagram.Shapes.Add(lineShape, project.Repository.ObtainNewBottomZOrder(diagram));
							if (withLayers) diagram.AddShapeToLayers(lineShape, linearLayer);
							Assert.AreNotEqual(ControlPointId.None, lineShape.IsConnected(ControlPointId.LastVertex, otherShape));
						}
					}
				}
			}
			diagram.Width = (lineLength + shapeSize) * shapesPerRow + 2 * shapeSize;
			diagram.Height = (lineLength + shapeSize) * shapesPerColumn + 2 * shapeSize;
			project.Repository.InsertAll(diagram);
		}

	}
	
	
	public static class RepositoryHelper {

		public static SqlStore CreateSqlStore() {
			string server = Environment.MachineName + SqlServerName;
			return new SqlStore(server, DatabaseName);
		}


		public static XmlStore CreateXmlStore() {
			return new XmlStore(Path.GetTempPath(), ".xml");
		}


		public static void SQLCreateDatabase() {
			using (SqlStore sqlStore = CreateSqlStore()) {
				// Create database
				string connectionString = string.Format("server={0};Integrated Security=True", sqlStore.ServerName);
				using (SqlConnection conn = new SqlConnection(connectionString)) {
					conn.Open();
					try {
						SqlCommand command = conn.CreateCommand();
						command.CommandText = string.Format("CREATE DATABASE {0}", DatabaseName);
						command.ExecuteNonQuery();
					} catch (SqlException exc) {
						// Ignore "Database already exists" error
						if (exc.ErrorCode != sqlErrDatabaseExists) throw exc;
					}
				}

				// Create Repository
				CachedRepository repository = new CachedRepository();
				repository.Store = CreateSqlStore();

				// Create project
				Project project = new Project();
				project.AutoLoadLibraries = true;
				project.Name = "NShape SQL Test";
				project.Repository = repository;

				// Add and register libraries
				project.RemoveAllLibraries();
				project.AddLibrary(typeof(ValueDevice).Assembly, true);
				project.AddLibrary(typeof(Circle).Assembly, true);
				project.AddLibrary(typeof(ProcessSymbol).Assembly, true);
				project.RegisterEntityTypes();

				// Create schema
				sqlStore.CreateDbCommands(repository);
				sqlStore.CreateDbSchema(repository);

				// Close project
				project.Close();
			}
		}


		public static void SQLDropDatabase() {
			string connectionString = string.Empty;
			using (SqlStore sqlStore = CreateSqlStore()) {
				connectionString = string.Format("server={0};Integrated Security=True", sqlStore.ServerName);
				sqlStore.DropDbSchema();
			}

			// Drop database
			if (!string.IsNullOrEmpty(connectionString)) {
				using (SqlConnection conn = new SqlConnection(connectionString)) {
					conn.Open();
					try {
						using (SqlCommand command = conn.CreateCommand()) {
							command.CommandText = string.Format("DROP DATABASE {0}", DatabaseName);
							command.ExecuteNonQuery();
						}
					} catch (SqlException exc) {
						if (exc.ErrorCode != sqlErrDatabaseExists) throw exc;
					}
				}
			}
		}


		private const int sqlErrDatabaseExists = -2146232060;
		public const string SqlServerName = "\\SQLEXPRESS";
		public const string DatabaseName = "NShapeSQLTest";
	}


	// Enum helper class
	public static class Enum<T> where T : struct, IComparable {

		public static T Parse(string value) {
			return (T)Enum.Parse(typeof(T), value);
		}


		public static IList<T> GetValues() {
			IList<T> list = new List<T>();
			foreach (object value in Enum.GetValues(typeof(T)))
				list.Add((T)value);
			return list;
		}


		public static T GetNextValue(T currentValue) {
			T result = default(T);
			IList<T> values = Enum<T>.GetValues();
			int cnt = values.Count;
			for (int i = 0; i < cnt; ++i) {
				if (values[i].Equals(currentValue)) {
					if (i + 1 < cnt) result = values[i + 1];
					else result = values[0];
					break;
				}
			}
			return result;
		}

	}

}
