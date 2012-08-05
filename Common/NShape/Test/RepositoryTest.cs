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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dataweb.NShape;
using Dataweb.NShape.Advanced;


namespace NShapeTest {

	/// <summary>
	/// Summary description for UnitTest
	/// </summary>
	[TestClass]
	public class RepositoryTest {

		public RepositoryTest() {
			// TODO: Add constructor logic here
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
		public void XMLRepository_EditWithContents_Test() {
			// Test inserting, modifying and deleting objects from repository
			string timerName = "RepositoryTest XMLStore Timer (edit with contents)";
			TestContext.BeginTimer(timerName);
			RepositoryEditTestCore(RepositoryHelper.CreateXmlStore(), RepositoryHelper.CreateXmlStore(), true);
			TestContext.EndTimer(timerName);
		}


		[TestMethod]
		public void XMLRepository_EditWithoutContents_Test() {
			// Test inserting, modifying and deleting objects from repository
			string timerName = "RepositoryTest XMLStore Timer (edit without contents)";
			TestContext.BeginTimer(timerName);
			RepositoryEditTestCore(RepositoryHelper.CreateXmlStore(), RepositoryHelper.CreateXmlStore(), false);
			TestContext.EndTimer(timerName);
		}


		[TestMethod]
		public void XMLRepository_FunctionSetComparison_Test() {
			// Test inserting, modifying and deleting objects from repository
			string timerName = "RepositoryTest XMLStore Timer (Function set comparison test)";
			TestContext.BeginTimer(timerName);
			RepositoryFunctionSetTestCore(RepositoryHelper.CreateXmlStore(), RepositoryHelper.CreateXmlStore());
			TestContext.EndTimer(timerName);
		}


		[TestMethod]
		public void XMLRepository_LargeDiagram_Test() {
			// Test inserting large diagrams
			string timerName = "LargeDiagramTest XMLStore Timer";
			TestContext.BeginTimer(timerName);
			LargeDiagramCore(RepositoryHelper.CreateXmlStore());
			TestContext.EndTimer(timerName);
		}


		[TestMethod]
		public void SQLRepository_EditWithContents_Test() {
			try {
				RepositoryHelper.SQLCreateDatabase();
				// Test inserting, modifying and deleting objects from repository
				string timerName = "RepositoryTest SqlStore Timer (edit with contents)";
				TestContext.BeginTimer(timerName);
				RepositoryEditTestCore(RepositoryHelper.CreateSqlStore(), RepositoryHelper.CreateSqlStore(), true);
				TestContext.EndTimer(timerName);
			} finally {
				RepositoryHelper.SQLDropDatabase();
			}
		}

		
		[TestMethod]
		public void SQLRepository_EditWithoutContents_Test() {
			try {
				RepositoryHelper.SQLCreateDatabase();
				// Test inserting, modifying and deleting objects from repository
				string timerName = "RepositoryTest SqlStore Timer (edit without contents)";
				TestContext.BeginTimer(timerName);
				RepositoryEditTestCore(RepositoryHelper.CreateSqlStore(), RepositoryHelper.CreateSqlStore(), false);
				TestContext.EndTimer(timerName);
			} finally {
				RepositoryHelper.SQLDropDatabase();
			}
		}


		[TestMethod]
		public void SQLRepository_FunctionSetComparison_Test() {
			try {
				RepositoryHelper.SQLCreateDatabase();
				// Test inserting, modifying and deleting objects from repository
				string timerName = "RepositoryTest SqlStore Timer (Function set comparison test)";
				TestContext.BeginTimer(timerName);
				RepositoryFunctionSetTestCore(RepositoryHelper.CreateSqlStore(), RepositoryHelper.CreateSqlStore());
				TestContext.EndTimer(timerName);
			} finally {
				RepositoryHelper.SQLDropDatabase();
			}
		}

		
		[TestMethod]
		public void SQLRepository_LargeDiagram_Test() {
			try {
				RepositoryHelper.SQLCreateDatabase();
				// Test inserting large diagrams
				string timerName = "LargeDiagramTest SqlStore Timer";
				TestContext.BeginTimer(timerName);
				LargeDiagramCore(RepositoryHelper.CreateSqlStore());
				TestContext.EndTimer(timerName);
			} finally {
				RepositoryHelper.SQLDropDatabase();
			}
		}

		#endregion


		#region Repository test core methods

		private void RepositoryEditTestCore(Store store1, Store store2, bool withContents) {
			RepositoryComparer.CompareIds = true;
			Project project1 = new Project();
			Project project2 = new Project();
			try {
				const string projectName = "Repository Test";
				const int shapesPerRow = 10;
				// Create two projects and repositories, one for modifying and 
				// saving, one for loading and comparing the result
				project1.Name =
				project2.Name = projectName;
				project1.Repository = new CachedRepository();
				project2.Repository = new CachedRepository();
				((CachedRepository)project1.Repository).Store = store1;
				((CachedRepository)project2.Repository).Store = store2;
				project1.AutoLoadLibraries = project2.AutoLoadLibraries = true;

				// Delete the current project (if it exists)
				project1.Repository.Erase();
				project1.Create();
				project1.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);

				// Create test data, populate repository and save repository
				string diagramName = "Diagram";
				DiagramHelper.CreateDiagram(project1, diagramName, shapesPerRow, shapesPerRow, true, true, true, true, true);
				Diagram diagram = project1.Repository.GetDiagram(diagramName);
				diagram.BackgroundImage = new NamedImage(new Bitmap(100, 100), "TestImage");
				diagram.BackgroundImageLayout = ImageLayoutMode.Center;
				project1.Repository.SaveChanges();

				// Compare the saved data with the loaded data
				project2.Open();
				RepositoryComparer.Compare(project1, project2);
				project2.Close();

				// Modify (and insert) content of the repository and save it
				ModifyAndUpdateContent(project1, withContents);
				CreateAndInsertContent(project1, withContents);
				project1.Repository.SaveChanges();

				// Compare the saved data with the loaded data
				project2.Open();
				RepositoryComparer.Compare(project1, project2);
				project2.Close();

				// Delete various data from project
				DeleteContent(project1, withContents);
				project1.Repository.SaveChanges();

				// Compare the saved data with the loaded data
				project2.Open();
				RepositoryComparer.Compare(project1, project2);
				project2.Close();
				
				// If the store is a XML store, check files and file names
				if (store1 is XmlStore) {
					// Save project again and check if backup files were created
					XmlStore xmlStore = (XmlStore)store1;
					project1.Repository.SaveChanges();
					Assert.IsTrue(File.Exists(Path.Combine(xmlStore.DirectoryName, project1.Name + ".bak")));
					Assert.IsTrue(File.Exists(Path.Combine(xmlStore.DirectoryName, project1.Name + xmlStore.FileExtension)));
					Assert.IsTrue(Directory.Exists(Path.Combine(xmlStore.DirectoryName, project1.Name + " Images")));
					Assert.IsTrue(Directory.Exists(Path.Combine(xmlStore.DirectoryName, project1.Name + " Images.bak")));
					// Save project another time and check if backup files can be deleted
					project1.Repository.SaveChanges();
					Assert.IsTrue(File.Exists(Path.Combine(xmlStore.DirectoryName, project1.Name + ".bak")));
					Assert.IsTrue(File.Exists(Path.Combine(xmlStore.DirectoryName, project1.Name + xmlStore.FileExtension)));
					Assert.IsTrue(Directory.Exists(Path.Combine(xmlStore.DirectoryName, project1.Name + " Images")));
					Assert.IsTrue(Directory.Exists(Path.Combine(xmlStore.DirectoryName, project1.Name + " Images.bak")));
				}
			} finally {
				project1.Close();
				project2.Close();
			}
		}


		private void RepositoryFunctionSetTestCore(Store store1, Store store2) {
			RepositoryComparer.CompareIds = false;
			Project project1 = new Project();
			Project project2 = new Project();
			try {
				// Create two projects and repositories, one for modifying and 
				// saving, one for loading and comparing the result
				const string projectName = "Repository Test";
				const string diagramName = "Diagram";
				const int shapesPerRow = 10;
				const bool connectShapes = true;
				const bool withModels = true;
				const bool withTerminalMappings = true;
				const bool withModelMappings = true;
				const bool withLayers = true;
				Prepare(project1, store1, projectName + " 1", diagramName, shapesPerRow, connectShapes, withModels, withTerminalMappings, withModelMappings, withLayers);
				Prepare(project2, store2, projectName + " 2", diagramName, shapesPerRow, connectShapes, withModels, withTerminalMappings, withModelMappings, withLayers);
				SaveAndClose(project1);
				SaveAndClose(project2);

				// Compare repositories
				project1.Open();
				project2.Open();
				RepositoryComparer.Compare(project1, project2);

				// Modify (and insert) content of the repository and save it
				ModifyAndUpdateContent(project1, true);
				ModifyAndUpdateContent(project2, false);
				SaveAndClose(project1);
				SaveAndClose(project2);

				// Compare repositories
				project1.Open();
				project2.Open();
				RepositoryComparer.Compare(project1, project2);

				// Create new content
				CreateAndInsertContent(project1, true);
				CreateAndInsertContent(project2, false);
				SaveAndClose(project1);
				SaveAndClose(project2);

				// Compare repositories
				project1.Open();
				project2.Open();
				RepositoryComparer.Compare(project1, project2);

				// Delete various data from project
				DeleteContent(project1, true);
				DeleteContent(project2, false);
				SaveAndClose(project1);
				SaveAndClose(project2);

				// Compare the saved data with the loaded data
				project1.Open();
				project2.Open();
				RepositoryComparer.Compare(project1, project2);
			} finally {
				project1.Close();
				project2.Close();
			}
		}


		private void LargeDiagramCore(Store store) {
			string projectName = "Large Diagram Test";
			Project project = new Project();
			try {
				project.AutoLoadLibraries = true;
				project.Name = projectName;
				project.Repository = new CachedRepository();
				((CachedRepository)project.Repository).Store = store;
				project.Repository.Erase();
				project.Create();
				project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);

				string diagramName = "Large Diagram";
				DiagramHelper.CreateLargeDiagram(project, diagramName);

				project.Repository.SaveChanges();
				Trace.WriteLine("Saved!");
			} finally {
				project.Close();
			}
		}

		#endregion


		#region Repository test helper methods - Insert content

		private void CreateAndInsertContent(Project project, bool withContent) {
			IRepository repository = project.Repository;

			// Insert styles into the project's design
			// Insert new color styles at least in order to prevent other styles from using them (they will be deleted later)
			CreateAndInsertStyles(project.Design, repository);

			// ToDo: Currently, the XML repository does not support more than one design.
			if (repository is CachedRepository && ((CachedRepository)repository).Store is AdoNetStore) {
				List<Design> designs = new List<Design>(repository.GetDesigns());
				foreach (Design design in designs)
					CreateAndInsertDesign(project.Design, repository, withContent);
			}

			// Insert templates
			List<Template> templates = new List<Template>(repository.GetTemplates());
			foreach (Template template in templates)
				CreateAndInsertTemplate(template, repository, withContent);

			// Insert model objects
			List<IModelObject> modelObjects = new List<IModelObject>(repository.GetModelObjects(null));
			foreach (IModelObject modelObject in modelObjects)
				CreateAndInsertModelObject(modelObject, repository);

			// Insert diagrams and shapes
			List<Diagram> diagrams = new List<Diagram>(repository.GetDiagrams());
			foreach (Diagram diagram in diagrams)
				CreateAndInsertDiagram(diagram, repository, withContent);
		}


		private void CreateAndInsertDesign(Design sourceDesign, IRepository repository, bool withContent) {
			// Create new design from source design
			Design newDesign = new Design(GetName(sourceDesign.Name, EditContentMode.Insert));
			foreach (IColorStyle style in sourceDesign.ColorStyles)
				newDesign.AddStyle(CreateStyle(style, newDesign));
			foreach (ICapStyle style in sourceDesign.CapStyles)
				newDesign.AddStyle(CreateStyle(style, newDesign));
			foreach (ICharacterStyle style in sourceDesign.CharacterStyles)
				newDesign.AddStyle(CreateStyle(style, newDesign));
			foreach (IFillStyle style in sourceDesign.FillStyles)
				newDesign.AddStyle(CreateStyle(style, newDesign));
			foreach (ILineStyle style in sourceDesign.LineStyles)
				newDesign.AddStyle(CreateStyle(style, newDesign));
			foreach (IParagraphStyle style in sourceDesign.ParagraphStyles)
				newDesign.AddStyle(CreateStyle(style, newDesign));
			// Insert new design into repository
			if (withContent)
				repository.InsertAll(newDesign);
			else {
				repository.Insert(newDesign);
				foreach (IStyle style in newDesign.Styles)
					repository.Insert(newDesign, style);
			}
		}


		private void CreateAndInsertStyles(Design design, IRepository repository) {
			List<IStyle> styleBuffer = new List<IStyle>();
			foreach (IColorStyle style in design.ColorStyles)
				styleBuffer.Add(CreateStyle(style, design));
			foreach (IStyle style in styleBuffer) {
				design.AddStyle(style);
				repository.Insert(design, style);
			}
			styleBuffer.Clear();
			
			foreach (ICapStyle style in design.CapStyles)
				styleBuffer.Add(CreateStyle(style, design));
			foreach (IStyle style in styleBuffer) {
				design.AddStyle(style);
				repository.Insert(design, style);
			}
			styleBuffer.Clear();

			foreach (ICharacterStyle style in design.CharacterStyles)
				styleBuffer.Add(CreateStyle(style, design));
			foreach (IStyle style in styleBuffer) {
				design.AddStyle(style);
				repository.Insert(design, style);
			}
			styleBuffer.Clear();

			foreach (IFillStyle style in design.FillStyles)
				styleBuffer.Add(CreateStyle(style, design));
			foreach (IStyle style in styleBuffer) {
				design.AddStyle(style);
				repository.Insert(design, style);
			}
			styleBuffer.Clear();

			foreach (ILineStyle style in design.LineStyles)
				styleBuffer.Add(CreateStyle(style, design));
			foreach (IStyle style in styleBuffer) {
				design.AddStyle(style);
				repository.Insert(design, style);
			}
			styleBuffer.Clear();

			foreach (IParagraphStyle style in design.ParagraphStyles)
				styleBuffer.Add(CreateStyle(style, design));
			foreach (IStyle style in styleBuffer) {
				design.AddStyle(style);
				repository.Insert(design, style);
			}
			styleBuffer.Clear();
		}


		private ICapStyle CreateStyle(ICapStyle sourceStyle, Design design) {
			if (sourceStyle == null) throw new ArgumentNullException("baseStyle");
			string newName = GetNewStyleName(sourceStyle, design, EditContentMode.Insert);
			CapStyle newStyle = new CapStyle(newName);
			newStyle.Title = GetName(sourceStyle.Title, EditContentMode.Insert).ToLower();
			newStyle.CapShape = sourceStyle.CapShape;
			newStyle.CapSize = sourceStyle.CapSize;
			if (sourceStyle.ColorStyle != null) {
				string colorStyleName = GetName(sourceStyle.ColorStyle.Name, EditContentMode.Insert);
				if (!design.ColorStyles.Contains(colorStyleName))
					CreateStyle(sourceStyle.ColorStyle, design);
				newStyle.ColorStyle = design.ColorStyles[colorStyleName];
			}
			return newStyle;
		}


		private IColorStyle CreateStyle(IColorStyle sourceStyle, Design design) {
			if (sourceStyle == null) throw new ArgumentNullException("baseStyle");
			string newName = GetNewStyleName(sourceStyle, design, EditContentMode.Insert);
			ColorStyle newStyle = new ColorStyle(newName);
			newStyle.Title = GetName(sourceStyle.Title, EditContentMode.Insert).ToLower();
			newStyle.Color = sourceStyle.Color;
			newStyle.Transparency = sourceStyle.Transparency;
			newStyle.ConvertToGray = sourceStyle.ConvertToGray;
			return newStyle;
		}


		private IFillStyle CreateStyle(IFillStyle sourceStyle, Design design) {
			if (sourceStyle == null) throw new ArgumentNullException("baseStyle");
			string newName = GetNewStyleName(sourceStyle, design, EditContentMode.Insert);
			FillStyle newStyle = new FillStyle(newName);
			newStyle.Title = GetName(sourceStyle.Title, EditContentMode.Insert).ToLower();
			if (sourceStyle.AdditionalColorStyle != null) {
				string colorStyleName = GetName(sourceStyle.AdditionalColorStyle.Name, EditContentMode.Insert);
				if (!design.ColorStyles.Contains(colorStyleName))
					CreateStyle(sourceStyle.AdditionalColorStyle, design);
				newStyle.AdditionalColorStyle = design.ColorStyles[colorStyleName];
			}
			if (sourceStyle.BaseColorStyle != null) {
				string colorStyleName = GetName(sourceStyle.BaseColorStyle.Name, EditContentMode.Insert);
				if (!design.ColorStyles.Contains(colorStyleName))
					CreateStyle(sourceStyle.BaseColorStyle, design);
				newStyle.BaseColorStyle = design.ColorStyles[colorStyleName];
			}
			newStyle.ConvertToGrayScale = sourceStyle.ConvertToGrayScale;
			newStyle.FillMode = sourceStyle.FillMode;
			newStyle.FillPattern = sourceStyle.FillPattern;
			if (sourceStyle.Image != null) {
				NamedImage namedImg = new NamedImage((Image)sourceStyle.Image.Image.Clone(),
					GetName(sourceStyle.Image.Name, EditContentMode.Insert));
				newStyle.Image = namedImg;
			} else newStyle.Image = sourceStyle.Image;
			newStyle.ImageGammaCorrection = sourceStyle.ImageGammaCorrection;
			newStyle.ImageLayout = sourceStyle.ImageLayout;
			newStyle.ImageTransparency = sourceStyle.ImageTransparency;
			return newStyle;
		}


		private ICharacterStyle CreateStyle(ICharacterStyle sourceStyle, Design design) {
			if (sourceStyle == null) throw new ArgumentNullException("baseStyle");
			string newName = GetNewStyleName(sourceStyle, design, EditContentMode.Insert);
			CharacterStyle newStyle = new CharacterStyle(newName);
			newStyle.Title = GetName(sourceStyle.Title, EditContentMode.Insert).ToLower();
			if (sourceStyle.ColorStyle != null) {
				string colorStyleName = GetName(sourceStyle.ColorStyle.Name, EditContentMode.Insert);
				if (!design.ColorStyles.Contains(colorStyleName))
					CreateStyle(sourceStyle.ColorStyle, design);
				newStyle.ColorStyle = design.ColorStyles[colorStyleName];
			}
			newStyle.FontName = sourceStyle.FontName;
			newStyle.SizeInPoints = sourceStyle.SizeInPoints;
			newStyle.Style = sourceStyle.Style;
			return newStyle;
		}


		private ILineStyle CreateStyle(ILineStyle sourceStyle, Design design) {
			if (sourceStyle == null) throw new ArgumentNullException("baseStyle");
			string newName = GetNewStyleName(sourceStyle, design, EditContentMode.Insert);
			LineStyle newStyle = new LineStyle(newName);
			newStyle.Title = GetName(sourceStyle.Title, EditContentMode.Insert).ToLower();
			if (sourceStyle.ColorStyle != null) {
				string colorStyleName = GetName(sourceStyle.ColorStyle.Name, EditContentMode.Insert);
				if (!design.ColorStyles.Contains(colorStyleName))
					CreateStyle(sourceStyle.ColorStyle, design);
				newStyle.ColorStyle = design.ColorStyles[colorStyleName];
			}
			newStyle.DashCap = sourceStyle.DashCap;
			newStyle.DashType = sourceStyle.DashType;
			newStyle.LineJoin = sourceStyle.LineJoin;
			newStyle.LineWidth = sourceStyle.LineWidth;
			return newStyle;
		}


		private IParagraphStyle CreateStyle(IParagraphStyle sourceStyle, Design design) {
			if (sourceStyle == null) throw new ArgumentNullException("baseStyle");
			string newName = GetNewStyleName(sourceStyle, design, EditContentMode.Insert);
			ParagraphStyle newStyle = new ParagraphStyle(newName);
			newStyle.Title = GetName(sourceStyle.Title, EditContentMode.Insert).ToLower();
			newStyle.Alignment = sourceStyle.Alignment;
			newStyle.Padding = sourceStyle.Padding;
			newStyle.Trimming = sourceStyle.Trimming;
			return newStyle;
		}


		private void CreateAndInsertTemplate(Template sourceTemplate, IRepository repository, bool withContent) {
			Template newTemplate = sourceTemplate.Clone();
			newTemplate.Description = GetName(sourceTemplate.Description, EditContentMode.Insert);
			newTemplate.Name = GetName(sourceTemplate.Name, EditContentMode.Insert);
			// Clone ModelObject and insert terminal mappings
			if (sourceTemplate.Shape.ModelObject != null) {
				// Copy terminal mapping of reference point
				TerminalId terminalId = sourceTemplate.GetMappedTerminalId(ControlPointId.Reference);
				if (terminalId != TerminalId.Invalid) newTemplate.MapTerminal(terminalId, ControlPointId.Reference);
				// Copy other terminal mappings
				foreach (ControlPointId pointId in sourceTemplate.Shape.GetControlPointIds(ControlPointCapabilities.All)) {
					terminalId = sourceTemplate.GetMappedTerminalId(pointId);
					if (terminalId != TerminalId.Invalid) newTemplate.MapTerminal(terminalId, pointId);
				}
			}
			if (withContent)
				repository.InsertAll(newTemplate);
			else {
				repository.Insert(newTemplate);
				if (newTemplate.Shape.ModelObject != null)
					repository.Insert(newTemplate.Shape.ModelObject, newTemplate);
				InsertShape(newTemplate.Shape, newTemplate, repository);
				// Insert property mappings
				foreach (IModelMapping newModelMapping in newTemplate.GetPropertyMappings())
					repository.Insert(newModelMapping, newTemplate);
			}
		}


		private void CreateAndInsertDiagram(Diagram sourceDiagram, IRepository repository, bool withContent) {
			Diagram newDiagram = new Diagram(GetName(sourceDiagram.Name, EditContentMode.Insert));
			newDiagram.BackgroundColor = sourceDiagram.BackgroundColor;
			newDiagram.BackgroundGradientColor = sourceDiagram.BackgroundGradientColor;
			newDiagram.BackgroundImageGamma = sourceDiagram.BackgroundImageGamma;
			newDiagram.BackgroundImageGrayscale = sourceDiagram.BackgroundImageGrayscale;
			newDiagram.BackgroundImageLayout = sourceDiagram.BackgroundImageLayout;
			newDiagram.BackgroundImageTransparency = sourceDiagram.BackgroundImageTransparency;
			newDiagram.BackgroundImageTransparentColor = sourceDiagram.BackgroundImageTransparentColor;
			newDiagram.Height = sourceDiagram.Height;
			newDiagram.Width = sourceDiagram.Width;
			newDiagram.Title = GetName(sourceDiagram.Title, EditContentMode.Insert).ToLower();
			foreach (Layer sourceLayer in sourceDiagram.Layers) {
				Layer newLayer = new Layer(GetName(sourceLayer.Name, EditContentMode.Insert));
				newLayer.Title = GetName(sourceLayer.Title, EditContentMode.Insert).ToLower();
				newLayer.UpperZoomThreshold = sourceLayer.UpperZoomThreshold;
				newLayer.LowerZoomThreshold = sourceLayer.LowerZoomThreshold;
				newDiagram.Layers.Add(newLayer);
			}
			foreach (Shape sourceShape in sourceDiagram.Shapes.BottomUp) {
				Shape newShape = sourceShape.Clone();
				newDiagram.Shapes.Add(newShape, sourceShape.ZOrder);
				newDiagram.AddShapeToLayers(newShape, sourceShape.Layers);
			}

			if (withContent)
				repository.InsertAll(newDiagram);
			else {
				// Insert diagram only
				repository.Insert(newDiagram);
				// Insert shapes
				foreach (Shape sourceShape in newDiagram.Shapes.BottomUp)
					InsertShape(sourceShape, newDiagram, repository);
			}
		}


		private void InsertShape(Shape shape, Diagram owner, IRepository repository) {
			repository.Insert(shape, owner);
			foreach (Shape childShape in shape.Children)
				InsertShape(childShape, shape, repository);
		}


		private void InsertShape(Shape shape, Template owner, IRepository repository) {
			repository.Insert(shape, owner);
			foreach (Shape childShape in shape.Children)
				InsertShape(childShape, shape, repository);
		}


		private void InsertShape(Shape shape, Shape parent, IRepository repository) {
			repository.Insert(shape, parent);
			foreach (Shape childShape in shape.Children)
				InsertShape(childShape, shape, repository);
		}


		private void CreateAndInsertModelObject(IModelObject sourceModelObject, IRepository repository) {
			IModelObject newModelObject = sourceModelObject.Clone();
			newModelObject.Name = GetName(sourceModelObject.Name, EditContentMode.Insert);
			repository.Insert(newModelObject);
		}

		#endregion


		#region Repository test helper methods - Modify content

		private void ModifyAndUpdateContent(Project project, bool withContent) {
			IRepository repository = project.Repository;

			// Modify designs and styles
			foreach (Design design in repository.GetDesigns())
				ModifyAndUpdateDesign(design, repository);

			// Modify templates
			foreach (Template template in repository.GetTemplates())
				ModifyAndUpdateTemplate(template, repository, withContent);

			// Modify model objects
			foreach (IModelObject modelObject in repository.GetModelObjects(null))
				ModifyAndUpdateModelObject(modelObject, repository, withContent);

			// Modify diagrams and shapes
			foreach (Diagram diagram in repository.GetDiagrams())
				ModifyAndUpdateDiagram(diagram, repository, withContent);
		}


		private void ModifyAndUpdateDesign(Design design, IRepository repository) {
			foreach (CapStyle style in design.CapStyles) ModifyAndUpdateStyle(style, design, repository);
			foreach (CharacterStyle style in design.CharacterStyles) ModifyAndUpdateStyle(style, design, repository);
			foreach (FillStyle style in design.FillStyles) ModifyAndUpdateStyle(style, design, repository);
			foreach (LineStyle style in design.LineStyles) ModifyAndUpdateStyle(style, design, repository);
			foreach (ParagraphStyle style in design.ParagraphStyles) ModifyAndUpdateStyle(style, design, repository);
			foreach (ColorStyle style in design.ColorStyles) ModifyAndUpdateStyle(style, design, repository);
		}


		private void ModifyAndUpdateStyle(Style style, IRepository repository) {
			if (!repository.GetDesign(null).IsStandardStyle(style))
				style.Name = GetName(style.Name, EditContentMode.Modify);
			style.Title = GetName(style.Title, EditContentMode.Modify).ToLower();
			repository.Update(style);
		}


		private void ModifyAndUpdateStyle(CapStyle style, Design design, IRepository repository) {
			style.CapShape = Enum<CapShape>.GetNextValue(style.CapShape);
			style.CapSize += 1;
			style.ColorStyle = GetNextValue(design.ColorStyles, style.ColorStyle);
			ModifyAndUpdateStyle(style, repository);
		}


		private void ModifyAndUpdateStyle(CharacterStyle style, Design design, IRepository repository) {
			style.ColorStyle = GetNextValue(design.ColorStyles, style.ColorStyle);
			style.FontFamily = GetNextValue(FontFamily.Families, style.FontFamily);
			style.Size += 1;
			style.Style = Enum<FontStyle>.GetNextValue(style.Style);
			ModifyAndUpdateStyle(style, repository);
		}


		private void ModifyAndUpdateStyle(ColorStyle style, Design design, IRepository repository) {
			int r = style.Color.R;
			int g = style.Color.G;
			int b = style.Color.B;
			style.Color = Color.FromArgb(g, b, r);
			style.ConvertToGray = !style.ConvertToGray;
			style.Transparency = (style.Transparency <= 50) ? (byte)75 : (byte)25;
			ModifyAndUpdateStyle(style, repository);
		}


		private void ModifyAndUpdateStyle(FillStyle style, Design design, IRepository repository) {
			style.AdditionalColorStyle = GetNextValue(design.ColorStyles, style.AdditionalColorStyle);
			style.BaseColorStyle = GetNextValue(design.ColorStyles, style.BaseColorStyle);
			style.ConvertToGrayScale = !style.ConvertToGrayScale;
			style.FillMode = Enum<FillMode>.GetNextValue(style.FillMode);
			style.FillPattern = Enum<System.Drawing.Drawing2D.HatchStyle>.GetNextValue(style.FillPattern);
			style.ImageGammaCorrection += 0.1f;
			style.ImageLayout = Enum<ImageLayoutMode>.GetNextValue(style.ImageLayout);
			style.ImageTransparency = (style.ImageTransparency < 100) ?
				(byte)(style.ImageTransparency + 1) : (byte)(style.ImageTransparency - 1);
			ModifyAndUpdateStyle(style, repository);
		}


		private void ModifyAndUpdateStyle(LineStyle style, Design design, IRepository repository) {
			style.ColorStyle = GetNextValue(design.ColorStyles, style.ColorStyle);
			style.DashCap = Enum<System.Drawing.Drawing2D.DashCap>.GetNextValue(style.DashCap);
			style.DashType = Enum<DashType>.GetNextValue(style.DashType);
			style.LineJoin = Enum<System.Drawing.Drawing2D.LineJoin>.GetNextValue(style.LineJoin);
			style.LineWidth += 1;
			ModifyAndUpdateStyle(style, repository);
		}


		private void ModifyAndUpdateStyle(ParagraphStyle style, Design design, IRepository repository) {
			style.Alignment = Enum<ContentAlignment>.GetNextValue(style.Alignment);
			style.Padding = new TextPadding(style.Padding.Left + 1, style.Padding.Top + 1, style.Padding.Right + 1, style.Padding.Bottom + 1);
			style.Trimming = Enum<StringTrimming>.GetNextValue(style.Trimming);
			style.WordWrap = !style.WordWrap;
			ModifyAndUpdateStyle(style, repository);
		}


		private void ModifyAndUpdateTemplate(Template template, IRepository repository, bool withContent) {
			template.Description = GetName(template.Description, EditContentMode.Modify);
			template.Name = GetName(template.Name, EditContentMode.Modify);

			// Assign a new child shape with a new child modelObject
			Shape newChildShape = template.Shape.Clone();
			if (newChildShape.ModelObject != null) newChildShape.ModelObject = null;
			template.Shape.Children.Add(newChildShape);
			if (template.Shape.ModelObject != null) {
				// ToDo: ModelObjects of child shapes and child model objects are not supported in the current version
				
				//newShape.ModelObject = template.Shape.ModelObject.Clone();
				//newShape.ModelObject.Parent = template.Shape.ModelObject;
				//repository.InsertModelObject(newShape.ModelObject);
			}

			// Modify property mappings
			foreach (IModelMapping modelMapping in template.GetPropertyMappings())
			    ModifyModelMapping(modelMapping, repository.GetDesign(null));

			// Modify terminal mappings
			if (template.Shape.ModelObject != null) {
				// Get all mapped point- and terminal ids
				List<ControlPointId> pointIds = new List<ControlPointId>();
				List<TerminalId> terminalIds = new List<TerminalId>();
				foreach (ControlPointId pointId in template.Shape.GetControlPointIds(ControlPointCapabilities.All)) {
					TerminalId terminalId = template.GetMappedTerminalId(pointId);
					if (terminalId != TerminalId.Invalid) {
						pointIds.Add(pointId);
						terminalIds.Add(terminalId);
					}
				}
				// Now reverse the mappings
				Assert.AreEqual(pointIds.Count, terminalIds.Count);
				int maxIdx = pointIds.Count - 1;
				for (int i = 0; i <= maxIdx; ++i)
					template.MapTerminal(terminalIds[i], pointIds[maxIdx - i]);
				// If there are no terminal mappings, map all connection points to the generic terminal
				if (pointIds.Count == 0) {
					GenericModelObject genericModel = template.Shape.ModelObject as GenericModelObject;
					TerminalId terminalId = genericModel.Type.MaxTerminalId;
					foreach (ControlPointId pointId in template.Shape.GetControlPointIds(ControlPointCapabilities.Connect)) {
						template.MapTerminal(terminalId, pointId);
						terminalId = (terminalId > 1) ? (TerminalId)(terminalId - 1) : genericModel.Type.MaxTerminalId;
					}
				}
			}

			// Update repository
			//if (withContent) {
			//    repository.UpdateAll(template);
			//} else {
				repository.Insert(newChildShape, template.Shape);
				repository.Update(template);
				repository.Update(template.GetPropertyMappings());
			//}
		}


		private void ModifyModelMapping(IModelMapping modelMapping, Design design) {
			if (modelMapping is NumericModelMapping) {
				NumericModelMapping numericMapping = (NumericModelMapping)modelMapping;
				numericMapping.Intercept += 10;
				numericMapping.Slope += 1;
			} else if (modelMapping is FormatModelMapping) {
				FormatModelMapping formatMapping = (FormatModelMapping)modelMapping;
				formatMapping.Format = GetName(formatMapping.Format, EditContentMode.Modify);
			} else if (modelMapping is StyleModelMapping) {
				StyleModelMapping styleMapping = (StyleModelMapping)modelMapping;
				List<object> ranges = new List<object>(styleMapping.ValueRanges);
				foreach (object range in ranges) {
					IStyle currentStyle = null;
					if (range is float)
						currentStyle = styleMapping[(float)range];
					else if (range is int)
						currentStyle = styleMapping[(int)range];

					IStyle newStyle = null;
					if (currentStyle is CapStyle)
						newStyle = GetNextValue(design.CapStyles, (CapStyle)currentStyle);
					else if (currentStyle is CharacterStyle)
						newStyle = GetNextValue(design.CharacterStyles, (CharacterStyle)currentStyle);
					else if (currentStyle is ColorStyle)
						newStyle = GetNextValue(design.ColorStyles, (ColorStyle)currentStyle);
					else if (currentStyle is FillStyle)
						newStyle = GetNextValue(design.FillStyles, (FillStyle)currentStyle);
					else if (currentStyle is LineStyle)
						newStyle = GetNextValue(design.LineStyles, (LineStyle)currentStyle);
					else if (currentStyle is ParagraphStyle)
						newStyle = GetNextValue(design.ParagraphStyles, (ParagraphStyle)currentStyle);

					if (range is float) {
						styleMapping.RemoveValueRange((float)range);
						styleMapping.AddValueRange((float)range, newStyle);
					} else if (range is int) {
						styleMapping.RemoveValueRange((int)range);
						styleMapping.AddValueRange((int)range, newStyle);
					}
				}
			}
		}


		private void ModifyAndUpdateDiagram(Diagram diagram, IRepository repository, bool withContents) {
			// Modify "base" properties
			Color backColor = diagram.BackgroundColor;
			Color gradientColor = diagram.BackgroundGradientColor;
			diagram.BackgroundGradientColor = backColor;
			diagram.BackgroundColor = gradientColor;
			diagram.BackgroundImageGamma += 0.1f;
			diagram.BackgroundImageGrayscale = !diagram.BackgroundImageGrayscale;
			diagram.BackgroundImageLayout = Enum<ImageLayoutMode>.GetNextValue(diagram.BackgroundImageLayout);
			diagram.BackgroundImageTransparency = (diagram.BackgroundImageTransparency <= 50) ? (byte)75 : (byte)25;
			diagram.BackgroundImageTransparentColor = Color.FromArgb(
				diagram.BackgroundImageTransparentColor.G,
				diagram.BackgroundImageTransparentColor.B,
				diagram.BackgroundImageTransparentColor.R);
			diagram.Height += 200;
			diagram.Width += 200;
			diagram.Name = GetName(diagram.Name, EditContentMode.Modify);
			diagram.Title = GetName(diagram.Title, EditContentMode.Modify).ToLower();

			// Modify layers
			foreach (Layer layer in diagram.Layers) {
				layer.LowerZoomThreshold += 1;
				layer.Title = GetName(layer.Title, EditContentMode.Modify).ToLower();
				layer.UpperZoomThreshold += 1;
			}
			repository.Update(diagram);

			// Modify shapes
			foreach (Shape shape in diagram.Shapes)
				ModifyAndUpdateShape(shape, repository, withContents);
		}


		private void ModifyAndUpdateModelObject(IModelObject modelObject, IRepository repository, bool withContent) {
			if (modelObject is GenericModelObject)
				ModifyModelObject((GenericModelObject)modelObject);

			// ToDo: ModelObjects of child shapes and child model objects are not supported in the current version
			//if (repository is CachedRepository) {
			//    CachedRepository cachedRepository = (CachedRepository)repository;
			//    // Add child model objects only for non-XMLStores
			//    if (!(cachedRepository.Store is XmlStore)) {
			//        IModelObject child = modelObject.Clone();
			//        child.Parent = modelObject;

			//        // Update repository
			//        repository.AddModelObject(child);
			//        repository.UpdateOwner(child, modelObject);
			//    }
			//}
			
			// Update repository
			// ToDo: Implement ModelObjectWithContents methods in Repository
			repository.Update(modelObject);
		}


		private void ModifyModelObject(GenericModelObject modelObject) {
			modelObject.FloatValue += 1.1f;
			modelObject.IntegerValue += 1;
			modelObject.StringValue += " Modified";
		}


		private void ModifyAndUpdateShape(Shape shape, IRepository repository, bool withContent) {
			Design design = repository.GetDesign(null);
			
			Shape childShape = shape.Clone();
			shape.Children.Add(childShape);
			shape.LineStyle = GetNextValue(design.LineStyles, shape.LineStyle);
			shape.MoveBy(100, 100);
			shape.SecurityDomainName = (char)(((int)shape.SecurityDomainName) + 1);
			if (shape is ILinearShape) 
				ModifyAndUpdateShape((ILinearShape)shape, design);
			else if (shape is IPlanarShape) 
				ModifyAndUpdateShape((IPlanarShape)shape, design);
			else if (shape is ICaptionedShape) 
				ModifyAndUpdateShape((ICaptionedShape)shape, design);
			
			// Update repository
			//if (withContent)
			//    repository.UpdateAll(shape);
			//else {
				repository.Insert(childShape, shape);
				repository.Update(shape);
			//}
		}


		private void ModifyAndUpdateShape(ILinearShape line, Design design) {
			// ToDo: Modify line specific properties
		}


		private void ModifyAndUpdateShape(IPlanarShape shape, Design design) {
			shape.Angle += 10;
			shape.FillStyle = GetNextValue(design.FillStyles, shape.FillStyle);
		}


		private void ModifyAndUpdateShape(ICaptionedShape shape, Design design) {
			int cnt = shape.CaptionCount;
			for (int i = 0; i < cnt; ++i) {
				string txt = shape.GetCaptionText(i);
				shape.SetCaptionText(i, txt + "Modified");
				ICharacterStyle characterStyle = shape.GetCaptionCharacterStyle(i);
				shape.SetCaptionCharacterStyle(i, GetNextValue(design.CharacterStyles, characterStyle));
				IParagraphStyle paragraphStyle = shape.GetCaptionParagraphStyle(i);
				shape.SetCaptionParagraphStyle(i, GetNextValue(design.ParagraphStyles, paragraphStyle));
			}
		}

		#endregion


		#region Repository test helper methods - Delete content

		private void DeleteContent(Project project, bool withContents) {
			IRepository repository = project.Repository;

			// Delete diagrams and shapes
			DeleteDiagrams(repository, withContents);

			// Delete templates
			DeleteTemplates(repository, withContents);

			// Delete all additional designs
			DeleteDesigns(repository, withContents);

			// Delete model objects
			DeleteModelObjects(repository);
		}


		private void DeleteDesigns(IRepository repository, bool withContents) {
			Design projectDesign = repository.GetDesign(null);
			List<Design> designs = new List<Design>(repository.GetDesigns());
			foreach (Design design in designs) {
				if (design == projectDesign) continue;
				if (withContents)
					repository.DeleteAll(design);
				else {
					// Delete only non-standard styles
					DeleteStyles(design, design.CapStyles, repository);
					DeleteStyles(design, design.CharacterStyles, repository);
					DeleteStyles(design, design.FillStyles, repository);
					DeleteStyles(design, design.LineStyles, repository);
					DeleteStyles(design, design.ParagraphStyles, repository);
					DeleteStyles(design, design.ColorStyles, repository);
					// Detete design
					repository.Delete(design);
				}
			}
		}


		private void DeleteStyles<TStyle>(Design design, IEnumerable<TStyle> styles, IRepository repository) where TStyle : IStyle {
			List<TStyle> styleList = new List<TStyle>(styles);
			foreach (TStyle style in styleList) {
				if (!design.IsStandardStyle(style)) {
					design.RemoveStyle(style);
					repository.Delete(style);
				}
			}
		}


		private void DeleteTemplates(IRepository repository, bool withContent) {
			List<Template> templates = new List<Template>(repository.GetTemplates());
			foreach (Template t in templates) {
				if (IsNameOf(EditContentMode.Insert, t.Name)) {
					// If the template was inserted, delete it
					if (withContent)
						repository.DeleteAll(t);
					else {
						repository.Delete(t.GetPropertyMappings());
						IModelObject modelObj = t.Shape.ModelObject;
						if (modelObj != null) {
							t.Shape.ModelObject = null;
							repository.Delete(modelObj);
						}
						DeleteShape(t.Shape, repository, withContent);
						repository.Delete(t);
					}
				} else if (IsNameOf(EditContentMode.Modify, t.Name)) {
					// If the template was modified, delete some of it's content 
					//
					// Delete child model objects
					IModelObject modelObject = t.Shape.ModelObject;
					if (modelObject != null) {
						DeleteChildModelObjects(modelObject, repository);

						// Delete ModelMappings
						List<IModelMapping> modelMappings = new List<IModelMapping>(t.GetPropertyMappings());
						t.UnmapAllProperties();
						repository.Delete(modelMappings);
					}

					// Delete child shapes
					if (t.Shape.Children.Count > 0) {
						List<Shape> children = new List<Shape>(t.Shape.Children);
						if (withContent)
							repository.DeleteAll(children);
						else {
							foreach (Shape childShape in children) {
								DeleteShape(childShape, repository, withContent);
								t.Shape.Children.Remove(childShape);
							}
						}
						t.Shape.Children.Clear();
					}
					repository.Update(t);
				}
			}
		}


		private void DeleteModelObjects(IRepository repository) {
			List<IModelObject> modelObjects = new List<IModelObject>(repository.GetModelObjects(null));
			foreach (IModelObject m in modelObjects) {
				if (IsNameOf(EditContentMode.Insert, m.Name)) {
					// If the model object was inserted, delete it
					DeleteModelObject(m, repository);
				} else if (IsNameOf(EditContentMode.Modify, m.Name)) {
					// If the model object was modified, delete it's children 
					DeleteChildModelObjects(m, repository);
				}
			}
		}


		private void DeleteModelObject(IModelObject modelObject, IRepository repository) {
			Assert.IsNotNull(modelObject);
			DeleteChildModelObjects(modelObject, repository);
			List<Shape> shapes = new List<Shape>(modelObject.Shapes);
			for (int i = shapes.Count - 1; i >= 0; --i)
				shapes[i].ModelObject = null;
			repository.Delete(modelObject);
		}


		private void DeleteChildModelObjects(IModelObject parent, IRepository repository) {
			List<IModelObject> children = new List<IModelObject>(repository.GetModelObjects(parent));
			for (int i = children.Count - 1; i >= 0; --i)
				DeleteChildModelObjects(children[i], repository);
			repository.Delete(children);
		}


		private void DeleteDiagrams(IRepository repository, bool withContent) {
			List<Diagram> diagrams = new List<Diagram>(repository.GetDiagrams());
			for (int i = diagrams.Count - 1; i >= 0; --i) {
				if (IsNameOf(EditContentMode.Insert, diagrams[i].Name)) {
					if (withContent) {
						foreach (Shape shape in diagrams[i].Shapes)
							DetachModelObjects(shape);
						repository.DeleteAll(diagrams[i]);
					} else {
						// Delete diagram manually:
						// First, delete all shape connections and all shapes of the diagram
						List<Shape> shapes = new List<Shape>(diagrams[i].Shapes.BottomUp);
						foreach (Shape shape in shapes) {
							foreach (ShapeConnectionInfo sci in shape.GetConnectionInfos(ControlPointId.Any, null)) {
								if (shape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Glue))
									repository.DeleteConnection(shape, sci.OwnPointId, sci.OtherShape, sci.OtherPointId);
							}
							DeleteShape(shape, repository, withContent);
						}
						// Afterwards, the diagram itself can be deleted.
						repository.Delete(diagrams[i]);
					}
				} else if (IsNameOf(EditContentMode.Modify, diagrams[i].Name)) {
					// Delete every second shapes
					DeleteSomeShapes(diagrams[i].Shapes.TopDown, repository, withContent);

					// Delete layers
					List<Layer> layers = new List<Layer>();
					for (int j = layers.Count - 1; j >= 0; --j) {
						if (j % 2 == 0) diagrams[j].Layers.Remove(layers[j]);
					}
				}
			}
		}


		private void DetachModelObjects(Shape shape) {
			foreach (Shape childShape in shape.Children)
				if (childShape.ModelObject != null) DetachModelObjects(childShape);
			if (shape.ModelObject != null) shape.ModelObject = null;
		}


		private void DeleteSomeShapes(IEnumerable<Shape> shapes, IRepository repository, bool withContent) {
			List<Shape> shapeList = new List<Shape>(shapes);
			for (int i = shapeList.Count - 1; i >= 0; --i) {
				if (i % 2 == 0)
					DeleteShape(shapeList[i], repository, withContent);
			}
		}


		private void DeleteShape(Shape shape, IRepository repository, bool withContent) {
			// Delete shape from repository (connections to other shapes will NOT be deleted automatically)
			List<ShapeConnectionInfo> connections = new List<ShapeConnectionInfo>(shape.GetConnectionInfos(ControlPointId.Any, null));
			foreach (ShapeConnectionInfo ci in connections) {
				Assert.IsFalse(ci == ShapeConnectionInfo.Empty);
				if (ci.OtherShape.Diagram == null) continue;	// Skip connections to shapes that are already deleted
				if (shape.HasControlPointCapability(ci.OwnPointId, ControlPointCapabilities.Glue)) {
					repository.DeleteConnection(shape, ci.OwnPointId, ci.OtherShape, ci.OtherPointId);
					shape.Disconnect(ci.OwnPointId);
				} else {
					repository.DeleteConnection(ci.OtherShape, ci.OtherPointId, shape, ci.OwnPointId);
					shape.Disconnect(ci.OtherPointId);
				}
			}

			if (withContent) {
				DetachModelObjects(shape);
				repository.DeleteAll(shape);
			} else {
				// Delete child shapes
				foreach (Shape childShape in shape.Children)
					DeleteShape(childShape, repository, withContent);
				// Delete shape
				DetachModelObjects(shape);
				repository.Delete(shape);
			}
			
			// Disconnect shapes
			foreach (ShapeConnectionInfo ci in connections) {
				if (shape.HasControlPointCapability(ci.OwnPointId, ControlPointCapabilities.Glue))
					shape.Disconnect(ci.OwnPointId);
				else ci.OtherShape.Disconnect(ci.OtherPointId);
			}
			// Clear children
			shape.Children.Clear();
			// Remove from diagram
			if (shape.Diagram != null)
				shape.Diagram.Shapes.Remove(shape);
			else if (shape.Parent != null)
				shape.Parent.Children.Remove(shape);
		}

		#endregion


		#region Repository test helper methods

		private static void Prepare(Project project, Store store, string projectName, string diagramName, int shapesPerRow, bool connectShapes, bool withModels, bool withTerminalMappings, bool withModelMappings, bool withLayers) {
			project.Name = projectName;
			project.AutoLoadLibraries = true;
			project.Repository = new CachedRepository();
			((CachedRepository)project.Repository).Store = store;

			// Delete the current project (if it exists)
			if (project.Repository.Exists()) 
				project.Repository.Erase();
			project.Create();
			project.AddLibrary(typeof(Dataweb.NShape.GeneralShapes.Circle).Assembly, true);

			// Create test data, populate repository and save repository
			DiagramHelper.CreateDiagram(project, diagramName, shapesPerRow, shapesPerRow, connectShapes, withModels, withTerminalMappings, withModelMappings, withLayers);
		}


		private void SaveAndClose(Project project) {
			project.Repository.SaveChanges();
			project.Close();
		}


		private T GetNextValue<T>(IEnumerable<T> collection, T currentValue)
			where T : class {
			T result = null;
			IEnumerator<T> enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext()) {
				if (result == null) result = enumerator.Current;
				if (enumerator.Current == currentValue) {
					if (enumerator.MoveNext()) {
						result = enumerator.Current;
						break;
					}
				}
			}
			return result;
		}


		private string GetNewStyleName<TStyle>(TStyle style, Design design, EditContentMode mode)
			where TStyle : IStyle {
			string result = GetName(style.Name, mode);
			if (design.ColorStyles.Contains(result)) {
				result = result + " ({0})";
				int i = 1;
				while (design.ColorStyles.Contains(string.Format(result, i))) ++i;
				result = string.Format(result, i);
			}
			return result;
		}


		private string GetName(string name, EditContentMode mode) {
			string result;
			switch (mode) {
				case EditContentMode.Insert:
					result = NewNamePrefix + " " + name; break;
				case EditContentMode.Modify:
					result = ModifiedNamePrefix + " " + name; break;
				default:
					Debug.Fail(string.Format("Unexpected {0} value '{1}'", typeof(EditContentMode).Name, mode));
					result = name; break;
			}
			return result;
		}


		private bool IsNameOf(EditContentMode mode, string name) {
			bool result;
			switch (mode) {
				case EditContentMode.Insert:
					result = name.StartsWith(NewNamePrefix); break;
				case EditContentMode.Modify:
					result = name.StartsWith(ModifiedNamePrefix);
					break;
				default:
					Debug.Fail(string.Format("Unexpected {0} value '{1}'", typeof(EditContentMode).Name, mode));
					result = false; break;
			}
			return result;
		}

		#endregion


		private TestContext testContextInstance;

		private enum EditContentMode { Insert, Modify };

		private const string NewNamePrefix = "Copy of";
		private const string ModifiedNamePrefix = "Modified";
	}

}
