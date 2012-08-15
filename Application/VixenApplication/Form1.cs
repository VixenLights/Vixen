using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.GeneralShapes;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication
{
	public partial class Form1 : Form
	{
		private Project myproject;
		private DiagramSetController dsc;

		private List<ChannelNodeShape> ChannelShapes;
		private List<ControllerShape> ControllerShapes;

		private Shape lastshape;
		private Shape secondlastshape;

		private Layer _visibleLayer;
		private Layer _hiddenLayer;

		public Form1()
		{
			InitializeComponent();

			dsc = new DiagramSetController();
			myproject = new Project();
			//repository = new CachedRepository();
			//store = new XmlStore();

			//repository.Store = store;
			//myproject.Repository = repository;
			dsc.Project = myproject;
			diagramDisplay.DiagramSetController = dsc;

			myproject.LibrarySearchPaths.Add(@"Common\");
			myproject.AutoLoadLibraries = true;	
			myproject.AddLibraryByName("Dataweb.NShape.GeneralShapes", false);
			myproject.AddLibraryByName("Dataweb.NShape.FlowChartShapes", false);
			myproject.AddLibraryByName("Dataweb.NShape.GeneralModelObjects", false);
			myproject.AddLibraryByName("Dataweb.NShape.SoftwareArchitectureShapes", false);
			myproject.AddLibraryByName("VixenApplication", false);
			
			myproject.Name = "temp";
			myproject.Create();

			diagramDisplay.Diagram = dsc.CreateDiagram("qwer");
			diagramDisplay.Diagram.Size = new Size(600, 600);
			//diagramDisplay.AutoScrollPosition = new Point(0, 0);
			//diagramDisplay.AutoScrollMinSize = diagramDisplay.Size; // Size(600, 800);

			_visibleLayer = new Layer("Visible");
			//_visibleLayer.Id = LayerIds.Layer01;
			_hiddenLayer = new Layer("Hidden");
			//_hiddenLayer.Id = LayerIds.Layer02;

			diagramDisplay.Diagram.Layers.Add(_visibleLayer);
			diagramDisplay.Diagram.Layers.Add(_hiddenLayer);
			diagramDisplay.SetLayerVisibility(_visibleLayer.Id, true);
			diagramDisplay.SetLayerVisibility(_hiddenLayer.Id, false);

			diagramDisplay.CurrentTool = new SelectionTool();
			diagramDisplay.ShowDefaultContextMenu = false;
			diagramDisplay.ClicksOnlyAffectTopShape = true;

			// A: fixed shapes with no connection points: nothing
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS, StandardRole.Operator, Permission.None);
			// B: fixed shapes with connection points: connect only
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS, StandardRole.Operator, Permission.Connect);
			// C: movable shapes (filters): connect, layout (movable)
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS, StandardRole.Operator, Permission.Connect | Permission.Layout);
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).CurrentRole = StandardRole.Operator;

			ControllerShapes = new List<ControllerShape>();
			ChannelShapes = new List<ChannelNodeShape>();

			FillStyle styleChannelGroup = new FillStyle("ChannelGroup",
				new ColorStyle("", Color.FromArgb(120, 160, 240)), new ColorStyle("", Color.FromArgb(90, 120, 180)));
			styleChannelGroup.FillMode = FillMode.Gradient;
			FillStyle styleChannelLeaf = new FillStyle("ChannelLeaf",
				new ColorStyle("", Color.FromArgb(200, 220, 255)), new ColorStyle("", Color.FromArgb(140, 160, 200)));
			styleChannelLeaf.FillMode = FillMode.Gradient;
			FillStyle styleFilter = new FillStyle("Filter",
				new ColorStyle("", Color.FromArgb(255, 220, 150)), new ColorStyle("", Color.FromArgb(230, 200, 100)));
			styleFilter.FillMode = FillMode.Gradient;
			FillStyle styleController = new FillStyle("Controller",
				new ColorStyle("", Color.FromArgb(200, 255, 200)), new ColorStyle("", Color.FromArgb(180, 255, 180)));
			styleController.FillMode = FillMode.Gradient;
			FillStyle styleOutput = new FillStyle("Output",
				new ColorStyle("", Color.FromArgb(220, 255, 220)), new ColorStyle("", Color.FromArgb(200, 255, 200)));
			styleOutput.FillMode = FillMode.Gradient;

			myproject.Design.FillStyles.Add(styleChannelGroup, styleChannelGroup);
			myproject.Design.FillStyles.Add(styleChannelLeaf, styleChannelLeaf);
			myproject.Design.FillStyles.Add(styleFilter, styleFilter);
			myproject.Design.FillStyles.Add(styleController, styleController);
			myproject.Design.FillStyles.Add(styleOutput, styleOutput);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			/*
			EllipseBase ellipse = (EllipseBase)myproject.ShapeTypes["Ellipse"].CreateInstance();
			//d.Shapes.Add(ellipse);
			diagramDisplay.Diagram.Shapes.Add(ellipse);
			//repository.Insert(ellipse, display1.Diagram);

			secondlastshape = lastshape;
			lastshape = ellipse;
			
			

			diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)ellipse, diagramDisplay.Diagram);
			 */
			Template template = new Template("connector", myproject.ShapeTypes["Polyline"].CreateInstance());
			diagramDisplay.CurrentTool = new LinearShapeCreationTool(template);



		}

		private void button3_Click(object sender, EventArgs e)
		{
			diagramDisplay.CurrentTool = new SelectionTool();


			/*
			if
				(diagramDisplay.Diagram.Shapes.Count < 2)
				return;

			Polyline line = (Polyline)myproject.ShapeTypes["Polyline"].CreateInstance();
			diagramDisplay.Diagram.Shapes.Add(line);
			line.EndCapStyle = myproject.Design.CapStyles.ClosedArrow;

			line.Connect(ControlPointId.FirstVertex, secondlastshape, ControlPointId.Reference);
			line.Connect(ControlPointId.LastVertex, lastshape, ControlPointId.Reference);
			*/


		}

		private void button4_Click(object sender, EventArgs e)
		{

		}

		private void display1_ShapeClick(object sender, DiagramPresenterShapeClickEventArgs e)
		{
			log("shape click: " + e.Shape.Type);
		}

		private void display1_ShapeDoubleClick(object sender, DiagramPresenterShapeClickEventArgs e)
		{
			log("shape doubleclick: " + e.Shape.Type);
			var shape = (FilterSetupShapeBase)e.Shape;

			// workaround: only modify the shape if it's currently selected. The diagram likes to
			// send click events to all shapes under the mouse, even if they're not active.
			if (!diagramDisplay.SelectedShapes.Contains(shape)) {
				log("shape doubleclick: NOT acting on shape " + shape.Title + ", as it's not selected");
				return;
			}

			if (shape is NestingSetupShape) {
				NestingSetupShape s = (shape as NestingSetupShape);
				if (s.Expanded) {
					log("closing shape: " + shape.Title);
					s.Expanded = false;
				}
				else {
					log("opening shape: " + shape.Title);
					s.Expanded = true;
				}
			}

			_ResizeAndPositionShapes();
		}

		private void display1_ShapesInserted(object sender, DiagramPresenterShapesEventArgs e)
		{
			log("shapes inserted: " + e.Shapes.Count);
		}

		private void display1_ShapesSelected(object sender, EventArgs e)
		{
			log("shapes selected");
		}

		private void display1_ShapesRemoved(object sender, DiagramPresenterShapesEventArgs e)
		{
			log("shapes removed: " + e.Shapes.Count);
		}

		private void display1_DiagramChanged(object sender, EventArgs e)
		{
			log("diagram changed");
		}

		private void display1_DiagramChanging(object sender, EventArgs e)
		{
			log("diagram changing");
		}

		private void log(string text)
		{
			richTextBox1.AppendText("\n" + text);
			richTextBox1.SelectionStart = richTextBox1.Text.Length;
			richTextBox1.ScrollToCaret();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			_CreateShapesFromChannels();
			_CreateShapesFromControllers();
		}

		private void button6_Click(object sender, EventArgs e)
		{
			_ResizeAndPositionShapes();
		}

		private void button7_Click(object sender, EventArgs e)
		{
//			ControllerShape controllerShape = _MakeControllerShape(Guid.NewGuid(), 1);
//			ControllerShapes.Add(controllerShape);
		}



		private void _CreateShapesFromChannels()
		{
			if (ChannelShapes != null) {
				foreach (ChannelNodeShape channelShape in ChannelShapes) {
					_RemoveShape(channelShape);
				}
			}

			ChannelShapes = new List<ChannelNodeShape>();
			foreach (ChannelNode node in VixenSystem.Nodes.GetRootNodes()) {
				ChannelNodeShape channelShape = _MakeChannelNodeShape(node, 1);
				if (channelShape != null)
					ChannelShapes.Add(channelShape);
			}
		}

		private void _CreateShapesFromControllers()
		{
			if (ControllerShapes != null) {
				foreach (ControllerShape controllerShape in ControllerShapes) {
					_RemoveShape(controllerShape);
				}
			}

			ControllerShapes = new List<ControllerShape>();
			foreach (IOutputDevice controller in VixenSystem.Controllers) {
				ControllerShape controllerShape = _MakeControllerShape(controller);
				if (controllerShape != null)
					ControllerShapes.Add(controllerShape);
			}
		}


		private void _ResizeAndPositionShapes()
		{
			int y = SHAPE_Y_TOP;
			foreach (ChannelNodeShape channelShape in ChannelShapes) {
				_ResizeAndPositionShape(channelShape, SHAPE_CHANNELS_WIDTH, SHAPE_CHANNELS_X_LOCATION, y, true);
				y += channelShape.Height + SHAPE_VERTICAL_SPACING;
			}
			y = SHAPE_Y_TOP;
			foreach (ControllerShape controllerShape in ControllerShapes) {
				_ResizeAndPositionShape(controllerShape, SHAPE_CONTROLLERS_WIDTH, SHAPE_CONTROLLERS_X_LOCATION, y, true);
				y += controllerShape.Height + SHAPE_VERTICAL_SPACING;
			}
		}

		private void _ResizeAndPositionShape(FilterSetupShapeBase shape, int width, int x, int y, bool visible)
		{
			if (visible) {
				_ShowShape(shape);
			} else {
				_HideShape(shape);
			}

			if (visible && (shape is NestingSetupShape) && (shape as NestingSetupShape).Expanded &&
				(shape as NestingSetupShape).ChildFilterShapes.Count > 0)
			{
				int curY = y + SHAPE_GROUP_HEADER_HEIGHT;
				foreach (FilterSetupShapeBase childShape in (shape as NestingSetupShape).ChildFilterShapes) {
					_ResizeAndPositionShape(childShape, width - SHAPE_CHILD_WIDTH_REDUCTION, x, curY, true);
					curY += childShape.Height + SHAPE_VERTICAL_SPACING;
				}
				shape.Width = width;
				shape.Height = (curY - SHAPE_VERTICAL_SPACING + SHAPE_GROUP_FOOTER_HEIGHT) - y;
			} else {
				shape.Width = width;
				shape.Height = SHAPE_CHANNELS_HEIGHT;
				if (shape is NestingSetupShape) {
					foreach (FilterSetupShapeBase childShape in (shape as NestingSetupShape).ChildFilterShapes) {
						_ResizeAndPositionShape(childShape, width, x, y, false);
					}
				}
			}
			shape.X = x;
			shape.Y = y + shape.Height / 2;
		}





		private ChannelNodeShape _MakeChannelNodeShape(ChannelNode node, int zOrder)
		{
			ChannelNodeShape shape = (ChannelNodeShape)myproject.ShapeTypes["ChannelNodeShape"].CreateInstance();
			shape.Node = node;
			shape.Title = node.Name;
			diagramDisplay.Diagram.Shapes.Add(shape, zOrder);
			diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)shape, diagramDisplay.Diagram);
			diagramDisplay.Diagram.AddShapeToLayers(shape, _visibleLayer.Id);

			if (node.Children.Count() > 0) {
				foreach (var child in node.Children) {
					FilterSetupShapeBase childSetupShapeBase = _MakeChannelNodeShape(child, zOrder + 1);
					shape.ChildFilterShapes.Add(childSetupShapeBase);
				}
				shape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS;
				shape.FillStyle = myproject.Design.FillStyles["ChannelGroup"];
			} else {
				shape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS;
				shape.FillStyle = myproject.Design.FillStyles["ChannelLeaf"];
			}
			return shape;
		}

		private void _RemoveShape(FilterSetupShapeBase shape)
		{
			diagramDisplay.Diagram.Shapes.Remove(shape);
			diagramDisplay.Diagram.RemoveShapeFromLayers(shape, _visibleLayer.Id | _hiddenLayer.Id);
			if (shape is NestingSetupShape) {
				foreach (FilterSetupShapeBase child in (shape as NestingSetupShape).ChildFilterShapes) {
					_RemoveShape(child);
				}
			}
		}

		private ControllerShape _MakeControllerShape(IOutputDevice controller)
		{
			OutputController outputController = controller as OutputController;
			if (outputController == null)
				return null;

			ControllerShape controllerShape = (ControllerShape)myproject.ShapeTypes["ControllerShape"].CreateInstance();
			controllerShape.Title = controller.Name;
			controllerShape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS;
			controllerShape.FillStyle = myproject.Design.FillStyles["Controller"];

			diagramDisplay.Diagram.Shapes.Add(controllerShape, 1);
			diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)controllerShape, diagramDisplay.Diagram);
			diagramDisplay.Diagram.AddShapeToLayers(controllerShape, _visibleLayer.Id);

			for (int i = 0; i < outputController.OutputCount; i++) {
				CommandOutput output = outputController.Outputs[i];
				OutputShape outputShape = (OutputShape)myproject.ShapeTypes["OutputShape"].CreateInstance();
				outputShape.Controller = outputController;
				outputShape.Output = output;
				outputShape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS;
				outputShape.FillStyle = myproject.Design.FillStyles["Output"];

				if (output.Name.Length <= 0)
					outputShape.Title = outputController.Name + " [" + (i+1) + "]";
				else
					outputShape.Title = output.Name;

				diagramDisplay.Diagram.Shapes.Add(outputShape, 2);
				diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)outputShape, diagramDisplay.Diagram);
				diagramDisplay.Diagram.AddShapeToLayers(outputShape, _visibleLayer.Id);

				controllerShape.ChildFilterShapes.Add(outputShape);
			}

			return controllerShape;
		}








		private void _HideShape(FilterSetupShapeBase setupShapeBase)
		{
			diagramDisplay.Diagram.AddShapeToLayers(setupShapeBase, _hiddenLayer.Id);
			diagramDisplay.Diagram.RemoveShapeFromLayers(setupShapeBase, _visibleLayer.Id);
		}

		private void _ShowShape(FilterSetupShapeBase setupShapeBase)
		{
			diagramDisplay.Diagram.AddShapeToLayers(setupShapeBase, _visibleLayer.Id);
			diagramDisplay.Diagram.RemoveShapeFromLayers(setupShapeBase, _hiddenLayer.Id);
		}

		private void _HideShapeAndChildren(NestingSetupShape nestingShape)
		{
			_HideShape(nestingShape);
			foreach (var childFilterShape in nestingShape.ChildFilterShapes) {
				if (childFilterShape is NestingSetupShape)
					_HideShapeAndChildren((NestingSetupShape)childFilterShape);
			}
		}

		private void _ShowShapeAndChildren(NestingSetupShape nestingShape)
		{
			_ShowShape(nestingShape);
			foreach (var childFilterShape in nestingShape.ChildFilterShapes) {
				if (childFilterShape is NestingSetupShape)
					_ShowShapeAndChildren((NestingSetupShape)childFilterShape);
			}
		}

		// the central X point of shapes
		internal const int SHAPE_CHANNELS_X_LOCATION = 100;
		internal const int SHAPE_CONTROLLERS_X_LOCATION = 500;

		// the starting top of all shapes
		internal const int SHAPE_Y_TOP = 30;

		// the (base) width of all shapes (inner children will be smaller)
		internal const int SHAPE_CHANNELS_WIDTH = 160;
		internal const int SHAPE_CONTROLLERS_WIDTH = 200;
		internal const int SHAPE_FILTERS_WIDTH = 160;

		// the default height of all shapes
		internal const int SHAPE_CHANNELS_HEIGHT = 32;
		internal const int SHAPE_CONTROLLERS_HEIGHT = 32;
		internal const int SHAPE_FILTERS_HEIGHT = 60;

		// the vertical spacing between channels
		internal const int SHAPE_VERTICAL_SPACING = 10;

		// how much the width of inner children is reduced
		internal const int SHAPE_CHILD_WIDTH_REDUCTION = 16;

		// how much of a parent shape should be reserved/kept for the wrapping above/below
		internal const int SHAPE_GROUP_HEADER_HEIGHT = 32;
		internal const int SHAPE_GROUP_FOOTER_HEIGHT = 8;

		// security domains for different shape types
		internal const char SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS = 'A';
		internal const char SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS = 'B';
		internal const char SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS = 'C';
	}

















	public abstract class FilterSetupShapeBase : RoundedBox
	{
		protected virtual void _init()
		{
			_recalcControlPoints();
		}

		protected FilterSetupShapeBase(ShapeType shapeType, Template template) : base(shapeType, template)
		{
		}

		protected FilterSetupShapeBase(ShapeType shapeType, IStyleSet styleSet) : base(shapeType, styleSet)
		{
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is FilterSetupShapeBase)
			{
				FilterSetupShapeBase src = (FilterSetupShapeBase)source;
				//DataFlowComponent = src.DataFlowComponent;
			}
		}

		protected void _recalcControlPoints()
		{
			controlPoints = new Point[ControlPointCount];
			CalcControlPoints();			
		}

		public virtual int InputCount
		{
			get { return (DataFlowComponent != null) ? 1 : 0; }
		}

		public virtual int OutputCount
		{
			get
			{
				if ((DataFlowComponent == null) || (DataFlowComponent.Outputs == null))
					return 0;
				return DataFlowComponent.Outputs.Length;
			}
		}

		public abstract IDataFlowComponent DataFlowComponent { get; }

		public virtual string Title { get; set; }

		private Font _customFont = null;
		private static readonly Font _defaultFont = new Font("Arial", 14, GraphicsUnit.Pixel);
		protected Font _font
		{
			get
			{
				if (_customFont != null)
					return _customFont;
				else
					return _defaultFont;
			}
			set { _customFont = value; }
		}

		private Brush _customTextBrush = null;
		private static readonly Brush _defaultTextBrush = new SolidBrush(Color.Black);
		protected Brush _textBrush
		{
			get
			{
				if (_customTextBrush != null)
					return _customTextBrush;
				else
					return _defaultTextBrush;
			}
			set { _customTextBrush = value; }
		}

		public override void Draw(Graphics graphics)
		{
			base.Draw(graphics);
			DrawCustom(graphics);
		}

		public virtual void DrawCustom(Graphics graphics)
		{
			SizeF stringSize = graphics.MeasureString(Title, _font);
			float x = X - (stringSize.Width / 2f);
			float y = Y - (Height / 2f);
			y += (Height - stringSize.Height) / 2f;

			graphics.DrawString(Title, _font, _textBrush, x, y);
		}

		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability)
		{
			int index = GetControlPointIndex(controlPointId);
			if (index > 0 && index <= InputCount + OutputCount)
			{
				return ((controlPointCapability & ControlPointCapabilities.Connect) > 0 ||
				        (controlPointCapability & ControlPointCapabilities.Resize) > 0);
			}

			if (controlPointId == ControlPointId.Reference || index == 0)
			{
				return ReferenceControlPointHasCapability(controlPointCapability);
			}

			// default to any other control points not having any capabilities (shouldn't be any left, really)
			return false;
		}

		protected virtual bool ReferenceControlPointHasCapability(ControlPointCapabilities controlPointCapability)
		{
			return ((controlPointCapability & ControlPointCapabilities.Reference) > 0);
		}

		protected override void CalcControlPoints()
		{
			int left = (int)Math.Round(-Width / 2f);
			int right = left + Width;
			int offset = 1;

			ControlPoints[0].X = 0;
			ControlPoints[0].Y = 0;

			for (int i = 0; i < InputCount; i++)
			{
				ControlPoints[offset + i].X = left;
				ControlPoints[offset + i].Y = (int)Math.Round((-Height / 2f) + Height * GetProportionalDistanceForPoint(i, InputCount));
			}

			offset += InputCount;

			for (int i = 0; i < OutputCount; i++)
			{
				ControlPoints[offset + i].X = right;
				ControlPoints[offset + i].Y = (int)Math.Round((-Height / 2f) + Height * GetProportionalDistanceForPoint(i, OutputCount));
			}
		}

		protected override int ControlPointCount
		{
			get { return InputCount + OutputCount + 1; }
		}

		protected float GetProportionalDistanceForPoint(int point, int totalPoints, float reserved = 0.2f)
		{
			if (reserved < 0f || reserved >= 1f)
				throw new ArgumentOutOfRangeException("reserved");

			if (point < 0 || point >= totalPoints)
				throw new ArgumentOutOfRangeException("point");

			if (totalPoints <= 0)
				throw new ArgumentOutOfRangeException("totalPoints");

			if (totalPoints == 1)
				return 0.5f;

			float range = 1.0f - reserved;
			float proportionalPosition = point / (float)(totalPoints - 1);
			float position = (reserved / 2f) + (proportionalPosition * range);
			return position;
		}
	}














	public abstract class NestingSetupShape : FilterSetupShapeBase
	{
		protected override void _init()
		{
			base._init();
			ChildFilterShapes = new List<FilterSetupShapeBase>();
			Expanded = true;
		}

		protected NestingSetupShape(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}

		protected NestingSetupShape(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is NestingSetupShape) {
				NestingSetupShape src = (NestingSetupShape)source;
				ChildFilterShapes = new List<FilterSetupShapeBase>(src.ChildFilterShapes);
			}
		}

		public List<FilterSetupShapeBase> ChildFilterShapes { get; private set; }

		public bool Expanded { get; set; }

		public override void DrawCustom(Graphics graphics)
		{
			if (ChildFilterShapes.Count == 0)
			{
				base.DrawCustom(graphics);
				return;
			}

			SizeF stringSize = graphics.MeasureString(Title, _font);
			float x = X - (stringSize.Width / 2f);
			float y = Y - (Height / 2f);
			y += (Form1.SHAPE_GROUP_HEADER_HEIGHT - stringSize.Height) / 2f;

			graphics.DrawString(Title, _font, _textBrush, x, y);
		}
	}


	






	public class ChannelNodeShape : NestingSetupShape
	{
		public ChannelNodeShape(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			_init();
		}

		public ChannelNodeShape(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			_init();
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is ChannelNodeShape) {
				ChannelNodeShape src = (ChannelNodeShape)source;
				Node = src.Node;
			}
		}

		public override Shape Clone()
		{
			ChannelNodeShape result = new ChannelNodeShape(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}

		static public ChannelNodeShape CreateInstance(ShapeType shapeType, Template template)
		{
			return new ChannelNodeShape(shapeType, template);
		}

		private ChannelNode _node;
		public ChannelNode Node
		{
			get { return _node; }
			set { _node = value; _recalcControlPoints(); }
		}

		public override IDataFlowComponent DataFlowComponent
		{
			get
			{
				if (Node == null || Node.Channel == null)
					return null;
				return VixenSystem.Channels.GetDataFlowComponentForChannel(Node.Channel);
			}
		}

		public override int InputCount
		{
			get { return 0; }
		}

		protected override bool ReferenceControlPointHasCapability(ControlPointCapabilities controlPointCapability)
		{
			return (controlPointCapability & ControlPointCapabilities.Reference) > 0 ||
				(OutputCount == 1 && (controlPointCapability & ControlPointCapabilities.Connect) > 0);
		}

	}








	public class ControllerShape : NestingSetupShape
	{
		public ControllerShape(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			_init();
		}

		public ControllerShape(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			_init();
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is ControllerShape) {
				ControllerShape src = (ControllerShape)source;
			}
		}

		public override Shape Clone()
		{
			ControllerShape result = new ControllerShape(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}

		static public ControllerShape CreateInstance(ShapeType shapeType, Template template)
		{
			return new ControllerShape(shapeType, template);
		}

		public override IDataFlowComponent DataFlowComponent
		{
			get { return null; }
		}
	}









	public class OutputShape : FilterSetupShapeBase
	{
		public OutputShape(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			_init();
		}

		public OutputShape(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			_init();
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is OutputShape) {
				OutputShape src = (OutputShape)source;
			}
		}

		public override Shape Clone()
		{
			OutputShape result = new OutputShape(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}

		static public OutputShape CreateInstance(ShapeType shapeType, Template template)
		{
			return new OutputShape(shapeType, template);
		}

		private CommandOutput _output;
		public CommandOutput Output
		{
			get { return _output; }
			set { _output = value; _recalcControlPoints(); }
		}

		private OutputController _controller;
		public OutputController Controller
		{
			get { return _controller; }
			set { _controller = value; _recalcControlPoints(); }
		}

		public override IDataFlowComponent DataFlowComponent
		{
			get
			{
				if (Output == null || Controller == null)
					return null;
				return Controller.GetDataFlowComponentForOutput(Output);
			}
		}

		protected override bool ReferenceControlPointHasCapability(ControlPointCapabilities controlPointCapability)
		{
			return (controlPointCapability & ControlPointCapabilities.Reference) > 0 ||
				(controlPointCapability & ControlPointCapabilities.Connect) > 0;
		}

	}









	public static class NShapeLibraryInitializer
	{
		public static void Initialize(IRegistrar registrar)
		{
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);
			registrar.RegisterShapeType(
				new ShapeType("ChannelNodeShape", namespaceName, namespaceName, ChannelNodeShape.CreateInstance, ChannelNodeShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("ControllerShape", namespaceName, namespaceName, ControllerShape.CreateInstance, ControllerShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("OutputShape", namespaceName, namespaceName, OutputShape.CreateInstance, OutputShape.GetPropertyDefinitions)
				);
		}

		private const string namespaceName = "VixenFilterShapes";
		private const int preferredRepositoryVersion = 3;
	}
}
