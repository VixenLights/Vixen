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
using System.Drawing;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace NShapeTest {

	public class RepositoryComparer {

		public static bool CompareIds {
			get;
			set;
		}


		public static void CompareId(IEntity entityA, IEntity entityB) {
			if (CompareIds) {
				Assert.AreEqual<bool>(entityA != null, entityB != null);
				if (entityA != null && entityB != null) {
					Assert.IsNotNull(entityA.Id);
					Assert.IsNotNull(entityB.Id);
					Assert.AreEqual(entityA.Id, entityB.Id);
				}
			}
		}


		public static void Compare(Project projectA, Project projectB) {
			IRepository repositoryA = projectA.Repository;
			IRepository repositoryB = projectB.Repository;

			// Compare versions
			Assert.AreEqual<int>(repositoryA.Version, repositoryB.Version);
			int version = repositoryA.Version;

			// Compare Designs
			RepositoryComparer.Compare(((IStyleSetProvider)projectA).StyleSet, ((IStyleSetProvider)projectB).StyleSet, version);
			Compare(repositoryA.GetDesign(null), repositoryB.GetDesign(null), version);
			CompareObjectCount(repositoryA.GetDesigns(), repositoryB.GetDesigns());
			Compare(repositoryA.GetDesigns(), repositoryB.GetDesigns(), version);

			// Compare Templates including TerminalMappings and ModelMappings
			CompareObjectCount(repositoryA.GetTemplates(), repositoryB.GetTemplates());
			foreach (Template templateA in repositoryA.GetTemplates())
				RepositoryComparer.Compare(templateA, repositoryB.GetTemplate(templateA.Name), version);

			// Compare ModelObjects
			CompareModelObjects(repositoryA, null, repositoryB, null, version);

			// Compare diagrams including layers and shapes
			CompareObjectCount(repositoryA.GetDiagrams(), repositoryB.GetDiagrams());
			foreach (Diagram diagramA in repositoryA.GetDiagrams()) {
				Diagram diagramB = repositoryB.GetDiagram(diagramA.Name);
				// Shapes must not be loaded after loading the diagram
				if (repositoryB is CachedRepository && ((CachedRepository)repositoryB).Store is AdoNetStore)
					Assert.IsTrue(diagramB.Shapes.Count == 0);
				// Now load the shapes
				repositoryA.GetDiagramShapes(diagramA);
				repositoryB.GetDiagramShapes(diagramB);
				// Compare
				RepositoryComparer.Compare(diagramA, diagramB, version);
			}
		}


		#region Compare designs and styles

		public static void Compare(IEnumerable<Design> designsA, IEnumerable<Design> designsB, int version) {
			Dictionary<string, Design> designsBDict = new Dictionary<string, Design>();
			foreach (Design d in designsB) designsBDict.Add(d.Name, d);

			foreach (Design designA in designsA) {
				Design designB = null;
				Assert.IsTrue(designsBDict.TryGetValue(designA.Name, out designB));

				Assert.AreEqual<string>(designA.Name, designB.Name);
				Compare(designA, designB, version);
				Assert.AreEqual(designA.CapStyles.Count, designB.CapStyles.Count);
				Assert.AreEqual(designA.CharacterStyles.Count, designB.CharacterStyles.Count);
				Assert.AreEqual(designA.ColorStyles.Count, designB.ColorStyles.Count);
				Assert.AreEqual(designA.FillStyles.Count, designB.FillStyles.Count);
				Assert.AreEqual(designA.LineStyles.Count, designB.LineStyles.Count);
				Assert.AreEqual(designA.ParagraphStyles.Count, designB.ParagraphStyles.Count);
				CompareObjectCount(designB.Styles, designA.Styles);
			}
		}


		public static void Compare(IStyleSet designA, IStyleSet designB, int version) {
			foreach (ICapStyle styleA in designA.CapStyles) {
				ICapStyle styleB = designB.CapStyles[styleA.Name];
				Compare(styleA, styleB, version);
			}
			foreach (ICharacterStyle styleA in designA.CharacterStyles) {
				ICharacterStyle styleB = designB.CharacterStyles[styleA.Name];
				Compare(styleA, styleB, version);
			}
			foreach (IColorStyle styleA in designA.ColorStyles) {
				IColorStyle styleB = designB.ColorStyles[styleA.Name];
				Compare(styleA, styleB, version);
			}
			foreach (IFillStyle styleA in designA.FillStyles) {
				IFillStyle styleB = designB.FillStyles[styleA.Name];
				Compare(styleA, styleB, version);
			}
			foreach (ILineStyle styleA in designA.LineStyles) {
				ILineStyle styleB = designB.LineStyles[styleA.Name];
				Compare(styleA, styleB, version);
			}
			foreach (IParagraphStyle styleA in designA.ParagraphStyles) {
				IParagraphStyle styleB = designB.ParagraphStyles[styleA.Name];
				Compare(styleA, styleB, version);
			}
		}


		public static void Compare(IStyle styleA, IStyle styleB, int version) {
			if (styleA is ICapStyle && styleB is ICapStyle)
				Compare((ICapStyle)styleA, (ICapStyle)styleB, version);
			else if (styleA is ICharacterStyle && styleB is ICharacterStyle)
				Compare((ICharacterStyle)styleA, (ICharacterStyle)styleB, version);
			else if (styleA is IColorStyle && styleB is IColorStyle)
				Compare((IColorStyle)styleA, (IColorStyle)styleB, version);
			else if (styleA is IFillStyle && styleB is IFillStyle)
				Compare((IFillStyle)styleA, (IFillStyle)styleB, version);
			else if (styleA is ILineStyle && styleB is ILineStyle)
				Compare((ILineStyle)styleA, (ILineStyle)styleB, version);
			else if (styleA is IParagraphStyle && styleB is IParagraphStyle)
				Compare((IParagraphStyle)styleA, (IParagraphStyle)styleB, version);
			else Assert.Fail("Different style types.");
		}


		public static void Compare(ICapStyle styleA, ICapStyle styleB, int version) {
			CompareBaseStyle(styleA, styleB, version);
			Compare(styleA.ColorStyle, styleB.ColorStyle, version);
			Assert.AreEqual<CapShape>(styleA.CapShape, styleB.CapShape);
			Assert.AreEqual<short>(styleA.CapSize, styleB.CapSize);
		}


		public static void Compare(ICharacterStyle styleA, ICharacterStyle styleB, int version) {
			CompareBaseStyle(styleA, styleB, version);
			Compare(styleA.ColorStyle, styleB.ColorStyle, version);
			Assert.AreEqual<FontFamily>(styleA.FontFamily, styleB.FontFamily);
			Assert.AreEqual<string>(styleA.FontName, styleB.FontName);
			Assert.AreEqual<int>(styleA.Size, styleB.Size);
			Assert.AreEqual<float>(styleA.SizeInPoints, styleB.SizeInPoints);
			Assert.AreEqual<FontStyle>(styleA.Style, styleB.Style);
		}


		public static void Compare(IColorStyle styleA, IColorStyle styleB, int version) {
			if (styleA == ColorStyle.Empty && styleB == ColorStyle.Empty) return;
			CompareBaseStyle(styleA, styleB, version);
			Assert.AreEqual<int>(styleA.Color.ToArgb(), styleB.Color.ToArgb());
			if (version >= 3) Assert.AreEqual<bool>(styleA.ConvertToGray, styleB.ConvertToGray);
			Assert.AreEqual<byte>(styleA.Transparency, styleB.Transparency);
		}


		public static void Compare(IFillStyle styleA, IFillStyle styleB, int version) {
			CompareBaseStyle(styleA, styleB, version);
			Compare(styleA.BaseColorStyle, styleB.BaseColorStyle, version);
			Compare(styleA.AdditionalColorStyle, styleB.AdditionalColorStyle, version);
			if (version >= 3) Assert.AreEqual<bool>(styleA.ConvertToGrayScale, styleB.ConvertToGrayScale);
			Assert.AreEqual<FillMode>(styleA.FillMode, styleB.FillMode);
			Assert.AreEqual<System.Drawing.Drawing2D.HatchStyle>(styleA.FillPattern, styleB.FillPattern);
			Assert.AreEqual<short>(styleA.GradientAngle, styleB.GradientAngle);
			CompareNamedImage(styleA.Image, styleB.Image, version);
			Assert.AreEqual<float>(styleA.ImageGammaCorrection, styleB.ImageGammaCorrection);
			Assert.AreEqual<ImageLayoutMode>(styleA.ImageLayout, styleB.ImageLayout);
			Assert.AreEqual<byte>(styleA.ImageTransparency, styleB.ImageTransparency);
		}


		public static void Compare(ILineStyle styleA, ILineStyle styleB, int version) {
			CompareBaseStyle(styleA, styleB, version);
			Compare(styleA.ColorStyle, styleB.ColorStyle, version);
			Assert.AreEqual<System.Drawing.Drawing2D.DashCap>(styleA.DashCap, styleB.DashCap);
			Assert.IsNotNull(styleA.DashPattern);
			Assert.IsNotNull(styleB.DashPattern);
			Assert.AreEqual<int>(styleA.DashPattern.Length, styleB.DashPattern.Length);
			for (int i = styleA.DashPattern.Length - 1; i >= 0; --i)
				Assert.AreEqual<float>(styleA.DashPattern[i], styleB.DashPattern[i]);
			Assert.AreEqual<DashType>(styleA.DashType, styleB.DashType);
			Assert.AreEqual<System.Drawing.Drawing2D.LineJoin>(styleA.LineJoin, styleB.LineJoin);
			Assert.AreEqual<int>(styleA.LineWidth, styleB.LineWidth);
		}


		public static void Compare(IParagraphStyle styleA, IParagraphStyle styleB, int version) {
			CompareBaseStyle(styleA, styleB, version);
			Assert.AreEqual<ContentAlignment>(styleA.Alignment, styleB.Alignment);
			Assert.AreEqual<TextPadding>(styleA.Padding, styleB.Padding);
			Assert.AreEqual<StringTrimming>(styleA.Trimming, styleB.Trimming);
			Assert.AreEqual<bool>(styleA.WordWrap, styleB.WordWrap);
		}

		#endregion


		#region Compare templates

		public static void Compare(Template templateA, Template templateB, int version) {
			CompareId(templateA, templateB);
			CompareString(templateA.Description, templateB.Description, false);
			CompareString(templateA.Name, templateB.Name, false);
			CompareString(templateA.Title, templateB.Title, false);
			CompareObjectCount(templateA.GetPropertyMappings(), templateB.GetPropertyMappings());
			foreach (IModelMapping mappingA in templateA.GetPropertyMappings()) {
				IModelMapping mappingB = templateB.GetPropertyMapping(mappingA.ModelPropertyId);
				Compare(mappingA, mappingB, version);
			}
			Compare(templateA.Shape, templateB.Shape, version);
			Compare(templateA.Shape.ModelObject, templateB.Shape.ModelObject, version);
			CompareObjectCount(templateA.Shape.GetControlPointIds(ControlPointCapabilities.All), templateB.Shape.GetControlPointIds(ControlPointCapabilities.All));
			int pointCount = Count(templateA.Shape.GetControlPointIds(ControlPointCapabilities.All));
			for (ControlPointId ptId = pointCount; ptId >= ControlPointId.Reference; --ptId) {
				Assert.AreEqual<TerminalId>(templateA.GetMappedTerminalId(ptId), templateB.GetMappedTerminalId(ptId));
				Assert.AreEqual<string>(templateA.GetMappedTerminalName(ptId), templateB.GetMappedTerminalName(ptId));
			}
		}


		public static void Compare(IModelMapping mappingA, IModelMapping mappingB, int version) {
			CompareId(mappingA, mappingB);
			Assert.AreEqual<int>(mappingA.ModelPropertyId, mappingB.ModelPropertyId);
			Assert.AreEqual<int>(mappingA.ShapePropertyId, mappingB.ShapePropertyId);
			if (mappingA is NumericModelMapping && mappingB is NumericModelMapping)
				Compare((NumericModelMapping)mappingA, (NumericModelMapping)mappingB, version);
			else if (mappingA is FormatModelMapping && mappingB is FormatModelMapping)
				Compare((FormatModelMapping)mappingA, (FormatModelMapping)mappingB, version);
			else if (mappingA is StyleModelMapping && mappingB is StyleModelMapping)
				Compare((StyleModelMapping)mappingA, (StyleModelMapping)mappingB, version);
			else Assert.Fail("Model mappings A and B are of different types");
		}


		public static void Compare(NumericModelMapping mappingA, NumericModelMapping mappingB, int version) {
			Assert.AreEqual<NumericModelMapping.MappingType>(mappingA.Type, mappingB.Type);
			Assert.IsTrue(Math.Abs(mappingA.Intercept - mappingB.Intercept) < floatEqualityDelta);
			Assert.IsTrue(Math.Abs(mappingA.Slope - mappingB.Slope) < floatEqualityDelta);
		}


		public static void Compare(FormatModelMapping mappingA, FormatModelMapping mappingB, int version) {
			Assert.AreEqual<FormatModelMapping.MappingType>(mappingA.Type, mappingB.Type);
			Assert.AreEqual<string>(mappingA.Format, mappingB.Format);
		}


		public static void Compare(StyleModelMapping mappingA, StyleModelMapping mappingB, int version) {
			Assert.AreEqual<StyleModelMapping.MappingType>(mappingA.Type, mappingB.Type);
			Assert.AreEqual<int>(mappingA.ValueRangeCount, mappingB.ValueRangeCount);
			switch (mappingA.Type) {
				case StyleModelMapping.MappingType.IntegerStyle:
					List<int> intRangesA = new List<int>(mappingA.ValueRangeCount);
					foreach (object obj in mappingA.ValueRanges) intRangesA.Add((int)obj);
					List<int> intRangesB = new List<int>(mappingB.ValueRangeCount);
					foreach (object obj in mappingB.ValueRanges) intRangesB.Add((int)obj);
					for (int i = mappingA.ValueRangeCount - 1; i >= 0; --i)
						Assert.AreEqual<int>(intRangesA[i], intRangesB[i]);
					break;

				case StyleModelMapping.MappingType.FloatStyle:
					List<float> floatRangesA = new List<float>(mappingA.ValueRangeCount);
					foreach (object obj in mappingA.ValueRanges) floatRangesA.Add((float)obj);
					List<float> floatRangesB = new List<float>(mappingB.ValueRangeCount);
					foreach (object obj in mappingB.ValueRanges) floatRangesB.Add((float)obj);
					for (int i = mappingA.ValueRangeCount - 1; i >= 0; --i)
						CompareFloat(floatRangesA[i], floatRangesB[i]);
					break;

				default: Assert.Fail("Unsupported mapping type"); break;
			}
		}

		#endregion


		#region Compare diagrams

		public static void Compare(Diagram diagramA, Diagram diagramB, int version) {
			Assert.AreEqual<bool>(diagramA != null, diagramB != null);
			if (diagramA != null && diagramB != null) {
				CompareId(diagramA, diagramB);
				Assert.AreEqual<int>(diagramA.BackgroundColor.ToArgb(), diagramB.BackgroundColor.ToArgb());
				Assert.AreEqual<int>(diagramA.BackgroundGradientColor.ToArgb(), diagramB.BackgroundGradientColor.ToArgb());
				if (diagramA.BackgroundImage != null && diagramB.BackgroundImage != null) {
					CompareString(diagramA.BackgroundImage.Name, diagramB.BackgroundImage.Name, false);
					Assert.AreEqual<int>(diagramA.BackgroundImage.Width, diagramB.BackgroundImage.Width);
					Assert.AreEqual<int>(diagramA.BackgroundImage.Height, diagramB.BackgroundImage.Height);
				}
				Assert.AreEqual<float>(diagramA.BackgroundImageGamma, diagramB.BackgroundImageGamma);
				Assert.AreEqual<bool>(diagramA.BackgroundImageGrayscale, diagramB.BackgroundImageGrayscale);
				Assert.AreEqual<ImageLayoutMode>(diagramA.BackgroundImageLayout, diagramB.BackgroundImageLayout);
				Assert.AreEqual<byte>(diagramA.BackgroundImageTransparency, diagramB.BackgroundImageTransparency);
				Assert.AreEqual<int>(diagramA.BackgroundImageTransparentColor.ToArgb(), diagramB.BackgroundImageTransparentColor.ToArgb());
				Assert.AreEqual<IDisplayService>(diagramA.DisplayService, diagramB.DisplayService);
				Assert.AreEqual<int>(diagramA.Height, diagramB.Height);
				Assert.AreEqual<int>(diagramA.Width, diagramB.Width);
				Assert.AreEqual<bool>(diagramA.HighQualityRendering, diagramB.HighQualityRendering);
				CompareString(diagramA.Name, diagramB.Name, false);
				if (version >= 3) CompareString(diagramA.Title, diagramB.Title, false);
				//
				// Compare Layers
				Assert.AreEqual<int>(diagramA.Layers.Count, diagramB.Layers.Count);
				SortedList<LayerIds, Layer> layersA = new SortedList<LayerIds, Layer>();
				foreach (Layer l in diagramA.Layers) layersA.Add(l.Id, l);
				SortedList<LayerIds, Layer> layersB = new SortedList<LayerIds, Layer>();
				foreach (Layer l in diagramB.Layers) layersB.Add(l.Id, l);
				foreach (KeyValuePair<LayerIds, Layer> pair in layersA) {
					Layer layerA = pair.Value;
					Layer layerB = layersB[pair.Key];
					Assert.AreEqual<LayerIds>(layerA.Id, layerB.Id);
					Assert.AreEqual<int>(layerA.LowerZoomThreshold, layerB.LowerZoomThreshold);
					Assert.AreEqual<int>(layerA.UpperZoomThreshold, layerB.UpperZoomThreshold);
					CompareString(layerA.Name, layerB.Name, false);
					CompareString(layerA.Title, layerB.Title, false);
				}
				//
				// Compare Shapes
				IEnumerator<Shape> shapesA = diagramA.Shapes.BottomUp.GetEnumerator();
				IEnumerator<Shape> shapesB = diagramB.Shapes.BottomUp.GetEnumerator();
				Assert.AreEqual<int>(diagramA.Shapes.Count, diagramB.Shapes.Count);
				for (int i = diagramA.Shapes.Count; i >= 0; --i) {
					Compare(shapesA.Current, shapesB.Current, version);
					Assert.AreEqual(shapesA.MoveNext(), shapesB.MoveNext());
				}
			}
		}

		#endregion


		#region Compare shapes

		public static void Compare(Shape shapeA, Shape shapeB, int version) {
			Assert.AreEqual<bool>(shapeA != null, shapeB != null);
			if (shapeA != null && shapeB != null) {
				// Compare base properties
				Assert.AreEqual<string>(shapeA.Type.FullName, shapeB.Type.FullName);
				CompareId(shapeA, shapeB);
				CompareId(shapeA.Diagram, shapeB.Diagram);
				CompareId(shapeA.Template, shapeB.Template);
				Assert.AreEqual<LayerIds>(shapeA.Layers, shapeB.Layers);
				Assert.AreEqual<IDisplayService>(shapeA.DisplayService, shapeB.DisplayService);
				Assert.AreEqual<Rectangle>(shapeA.GetBoundingRectangle(true), shapeB.GetBoundingRectangle(true));
				Assert.AreEqual<Rectangle>(shapeA.GetBoundingRectangle(false), shapeB.GetBoundingRectangle(false));
				Compare(shapeA.LineStyle, shapeB.LineStyle, version);
				Compare(shapeA.ModelObject, shapeB.ModelObject, version);
				Compare(shapeA.Parent, shapeB.Parent, version);
				Assert.AreEqual<char>(shapeA.SecurityDomainName, shapeB.SecurityDomainName);
				Assert.AreEqual<bool>(shapeA.Tag != null, shapeB.Tag != null);
				CompareString(shapeA.Type.FullName, shapeB.Type.FullName, true);
				Assert.AreEqual<int>(shapeA.X, shapeB.X);
				Assert.AreEqual<int>(shapeA.Y, shapeB.Y);
				// 
				// Compare ZOrder and Layers
				// ToDo: Implement this
				//
				// Compare children
				Assert.AreEqual<int>(shapeA.Children.Count, shapeB.Children.Count);
				IEnumerator<Shape> childrenA = shapeA.Children.GetEnumerator();
				IEnumerator<Shape> childrenB = shapeB.Children.GetEnumerator();
				for (int i = shapeA.Children.Count - 1; i >= 0; --i) {
					Compare(childrenA.Current, childrenB.Current, version);
					Assert.AreEqual<bool>(childrenA.MoveNext(), childrenB.MoveNext());
				}
				//
				// Compare connections
				bool shapeAConnected = (shapeA.IsConnected(ControlPointId.Any, null) != ControlPointId.None);
				bool shapeBConnected = (shapeB.IsConnected(ControlPointId.Any, null) != ControlPointId.None);
				Assert.AreEqual<bool>(shapeAConnected, shapeBConnected);
				if (shapeAConnected && shapeBConnected) {
					if (CompareIds)
						CompareConnectionsById(shapeA, shapeB);
					else {
						CompareConnectionsByControlPointIds(shapeA, shapeB);
					}
				}
				//
				// Compare specific properties
				Assert.AreEqual<bool>(shapeA is ILinearShape, shapeB is ILinearShape);
				if (shapeA is ILinearShape && shapeB is ILinearShape)
					Compare((ILinearShape)shapeA, (ILinearShape)shapeB, version);
				Assert.AreEqual<bool>(shapeA is IPlanarShape, shapeB is IPlanarShape);
				if (shapeA is IPlanarShape && shapeB is IPlanarShape)
					Compare((IPlanarShape)shapeA, (IPlanarShape)shapeB, version);
				Assert.AreEqual<bool>(shapeA is ICaptionedShape, shapeB is ICaptionedShape);
				if (shapeA is ICaptionedShape && shapeB is ICaptionedShape)
					Compare((ICaptionedShape)shapeA, (ICaptionedShape)shapeB, version);
			}
		}


		private static void CompareConnectionsById(Shape shapeA, Shape shapeB) {
			List<ShapeConnectionInfo> connectionsA = new List<ShapeConnectionInfo>(shapeA.GetConnectionInfos(ControlPointId.Any, null));
			List<ShapeConnectionInfo> connectionsB = new List<ShapeConnectionInfo>(shapeB.GetConnectionInfos(ControlPointId.Any, null));
			if (connectionsA.Count != connectionsB.Count) {
			}
			Assert.AreEqual<int>(connectionsA.Count, connectionsB.Count);
			for (int sIdx = connectionsA.Count - 1; sIdx >= 0; --sIdx) {
				bool connectionFound = false;
				for (int lIdx = connectionsB.Count - 1; lIdx >= 0; --lIdx) {
					IEntity entityA = (IEntity)connectionsA[sIdx].OtherShape;
					IEntity entityB = (IEntity)connectionsB[lIdx].OtherShape;
					if (entityA.Id.Equals(entityB.Id)) {
						CompareConnection(shapeA, connectionsA[lIdx], shapeB, connectionsB[sIdx]);
						connectionFound = true;
						break;
					}
				}
				Assert.IsTrue(connectionFound);
			}
		}


		private static void CompareConnection(Shape shapeA, ShapeConnectionInfo connectionA, Shape shapeB, ShapeConnectionInfo connectionB) {
			CompareId(shapeA, shapeB);
			CompareId(connectionB.OtherShape, connectionB.OtherShape);
			if (connectionA != ShapeConnectionInfo.Empty && connectionB != ShapeConnectionInfo.Empty) {
				if (connectionA.OwnPointId >= ControlPointId.Reference && connectionB.OwnPointId >= ControlPointId.Reference)
					Assert.IsTrue(connectionA.OwnPointId == connectionB.OwnPointId);
				if (connectionA.OtherPointId >= ControlPointId.Reference && connectionB.OtherPointId >= ControlPointId.Reference)
					Assert.IsTrue(connectionA.OtherPointId == connectionB.OtherPointId);
				bool ownPointPosIsEqual = false;
				bool otherPointPosIsEqual = false;
				if (shapeA.HasControlPointCapability(connectionA.OwnPointId, ControlPointCapabilities.Glue)) {
					Point pointA = shapeA.GetControlPointPosition(connectionA.OwnPointId);
					Point pointB = shapeB.GetControlPointPosition(connectionB.OwnPointId);
					ownPointPosIsEqual = (pointA == pointB);
					otherPointPosIsEqual = (connectionA.OtherPointId == connectionB.OtherPointId);
				} else {
					ownPointPosIsEqual = (connectionA.OwnPointId == connectionB.OwnPointId);
					Point pointA = connectionA.OtherShape.GetControlPointPosition(connectionA.OtherPointId);
					Point pointB = connectionB.OtherShape.GetControlPointPosition(connectionB.OtherPointId);
					otherPointPosIsEqual = (pointA == pointB);
				}
				Assert.IsTrue(ownPointPosIsEqual);
				Assert.IsTrue(otherPointPosIsEqual);
			} else Assert.IsTrue(connectionA == ShapeConnectionInfo.Empty && connectionB == ShapeConnectionInfo.Empty);
		}


		private static void CompareConnectionsByControlPointIds(Shape shapeA, Shape shapeB) {
			foreach (ControlPointId gluePtId in shapeB.GetControlPointIds(ControlPointCapabilities.Glue)) {
				ShapeConnectionInfo connectionA = shapeB.GetConnectionInfo(gluePtId, null);
				ShapeConnectionInfo connectionB = shapeB.GetConnectionInfo(gluePtId, null);
				CompareConnection(shapeA, connectionA, shapeB, connectionB);
			}
		}


		private static void Compare(ILinearShape shapeA, ILinearShape shapeB, int version) {
			Assert.AreEqual<bool>(shapeA != null, shapeB != null);
			if (shapeA != null && shapeB != null) {
				Assert.AreEqual<bool>(shapeA.IsDirected, shapeB.IsDirected);
				Assert.AreEqual<int>(shapeA.MaxVertexCount, shapeB.MaxVertexCount);
				Assert.AreEqual<int>(shapeA.MinVertexCount, shapeB.MinVertexCount);
				Assert.AreEqual<int>(shapeA.VertexCount, shapeB.VertexCount);
			}
		}


		private static void Compare(IPlanarShape shapeA, IPlanarShape shapeB, int version) {
			Assert.AreEqual<bool>(shapeA != null, shapeB != null);
			if (shapeA != null && shapeB != null) {
				Assert.AreEqual<int>(shapeA.Angle, shapeB.Angle);
				Compare(shapeA.FillStyle, shapeB.FillStyle, version);
			}
		}


		private static void Compare(ICaptionedShape shapeA, ICaptionedShape shapeB, int version) {
			Assert.AreEqual<bool>(shapeA != null, shapeB != null);
			if (shapeA != null && shapeB != null) {
				Assert.AreEqual<int>(shapeA.CaptionCount, shapeB.CaptionCount);
				for (int i = shapeA.CaptionCount - 1; i >= 0; --i) {
					Compare(shapeA.GetCaptionCharacterStyle(i), shapeB.GetCaptionCharacterStyle(i), version);
					Compare(shapeA.GetCaptionParagraphStyle(i), shapeB.GetCaptionParagraphStyle(i), version);
					CompareString(shapeA.GetCaptionText(i), shapeB.GetCaptionText(i), false);
				}
			}
		}

		#endregion


		#region Compare model objects

		public static void Compare(IModelObject modelObjectA, IModelObject modelObjectB, int version) {
			Assert.AreEqual<bool>(modelObjectA != null, modelObjectB != null);
			if (modelObjectA != null && modelObjectB != null) {
				Assert.AreEqual<string>(modelObjectA.Type.FullName, modelObjectB.Type.FullName);
				CompareId((IEntity)modelObjectA, (IEntity)modelObjectB);
				Assert.AreEqual(modelObjectA.Name, modelObjectB.Name);

				// Currently, there are orphaned template shapes that will be referenced by the savedModelObject, so this 
				// assertion will always fail. 
				// ToDo: Check how the template shapes became orphans and reactivate this check.
				//Assert.AreEqual(Count(modelObjectA.Shapes), Count(modelObjectB.Shapes));
				// ToDo: Compare Id's of shapes

				Compare(modelObjectA.Parent, modelObjectB.Parent, version);
				// Compare specific model object types
				if (modelObjectA is GenericModelObject) {
					Assert.AreEqual<bool>(modelObjectA is GenericModelObject, modelObjectB is GenericModelObject);
					Compare((GenericModelObject)modelObjectA, (GenericModelObject)modelObjectB, version);
				}
			}
		}


		public static void Compare(GenericModelObject modelObjectA, GenericModelObject modelObjectB, int version) {
			Assert.AreEqual<int>(modelObjectA.IntegerValue, modelObjectB.IntegerValue);
			CompareFloat(modelObjectA.FloatValue, modelObjectB.FloatValue);
			CompareString(modelObjectA.StringValue, modelObjectB.StringValue, false);
		}


		public static void CompareModelObjects(IRepository repositoryA, IModelObject parentA, IRepository repositoryB, IModelObject parentB, int version) {
			IEnumerable<IModelObject> modelObjectsA = repositoryA.GetModelObjects(parentA);
			IEnumerable<IModelObject> modelObjectsB = repositoryB.GetModelObjects(parentB);
			CompareObjectCount(modelObjectsA, modelObjectsB);
			if (CompareIds) {
				foreach (IModelObject modelObjectA in modelObjectsA) {
					IModelObject modelObjectB = repositoryB.GetModelObject(modelObjectA.Id);
					Compare(modelObjectA, modelObjectB, version);
					CompareModelObjects(repositoryA, modelObjectA, repositoryB, modelObjectB, version);
				}
			}
		}


		#endregion


		private static void CompareBaseStyle(IStyle styleA, IStyle styleB, int version) {
			CompareId(styleA, styleB);
			Assert.AreEqual<string>(styleA.Name, styleB.Name);
			Assert.AreEqual<string>(styleA.Title, styleB.Title);
		}


		private static void CompareString(string stringA, string stringB) {
			CompareString(stringA, stringB, false);
		}


		private static void CompareString(string stringA, string stringB, bool exact) {
			if (exact) Assert.AreEqual<string>(stringA, stringB);
			else {
				if (!string.IsNullOrEmpty(stringA) && !string.IsNullOrEmpty(stringB))
					Assert.IsTrue(stringA.Equals(stringB, StringComparison.InvariantCultureIgnoreCase));
				else Assert.IsTrue(string.IsNullOrEmpty(stringA) == string.IsNullOrEmpty(stringB));
			}
		}


		private static void CompareFloat(float valueA, float valueB) {
			if (valueA == 0 || valueB == 0)
				Assert.AreEqual(valueA, valueB);
			else {
				// Calculate the number of significant digits
				float valueDeltaA = floatEqualityDelta * (valueA / (valueA / 10));
				float valueDeltaB = floatEqualityDelta * (valueB / (valueB / 10));
				Assert.AreEqual(valueDeltaA, valueDeltaB);
				// Compare the significant digits
				Assert.IsTrue(Math.Abs(valueA - valueB) < valueDeltaA);
			}
		}


		private static void CompareNamedImage(NamedImage imageA, NamedImage imageB, int version) {
			if (imageA == null && imageB == null) return;
			CompareString(imageA.Name, imageB.Name, true);
			if (imageA.Image == null && imageB.Image == null) return;
			Assert.AreEqual<Size>(imageA.Image.Size, imageB.Image.Size);
			CompareFloat(imageA.Image.HorizontalResolution, imageB.Image.HorizontalResolution);
			CompareFloat(imageA.Image.VerticalResolution, imageB.Image.VerticalResolution);
			Assert.AreEqual<System.Drawing.Imaging.PixelFormat>(imageA.Image.PixelFormat, imageB.Image.PixelFormat);
			Assert.AreEqual<System.Drawing.Imaging.ImageFormat>(imageA.Image.RawFormat, imageB.Image.RawFormat);
		}


		private static void CompareObjectCount<T>(IEnumerable<T> objectsA, IEnumerable<T> objectsB) {
			int objectCntA = Count(objectsA);
			int objectCntB = Count(objectsB);
			Assert.AreEqual<int>(objectCntA, objectCntB);
		}


		private static int Count<T>(IEnumerable<T> items) {
			int result = 0;
			if (items is ICollection<T>)
				result = ((ICollection<T>)items).Count;
			else {
				IEnumerator<T> enumerator = items.GetEnumerator();
				while (enumerator.MoveNext()) ++result;
			}
			return result;
		}


		private const float floatEqualityDelta = 0.000001f;
	}

}
