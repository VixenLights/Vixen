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
using Dataweb.NShape.Commands;
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

		private readonly Layer _visibleLayer;
		private readonly Layer _hiddenLayer;

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
				SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS, StandardRole.Operator, Permission.Insert);
			// B: fixed shapes with connection points: connect only
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS, StandardRole.Operator, Permission.Connect | Permission.Insert);
			// C: movable shapes (filters): connect, layout (movable)
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS, StandardRole.Operator, Permission.Connect | Permission.Insert | Permission.Layout);

			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(StandardRole.Operator,
				Permission.All);
			//	Permission.Connect | Permission.Layout | Permission.Insert);

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
				new ColorStyle("", Color.FromArgb(100, 200, 100)), new ColorStyle("", Color.FromArgb(50, 200, 50)));
			styleController.FillMode = FillMode.Gradient;
			FillStyle styleOutput = new FillStyle("Output",
				new ColorStyle("", Color.FromArgb(150, 220, 150)), new ColorStyle("", Color.FromArgb(80, 200, 80)));
			styleOutput.FillMode = FillMode.Gradient;

			myproject.Design.FillStyles.Add(styleChannelGroup, styleChannelGroup);
			myproject.Design.FillStyles.Add(styleChannelLeaf, styleChannelLeaf);
			myproject.Design.FillStyles.Add(styleFilter, styleFilter);
			myproject.Design.FillStyles.Add(styleController, styleController);
			myproject.Design.FillStyles.Add(styleOutput, styleOutput);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//Template template = new Template("connector", myproject.ShapeTypes["Polyline"].CreateInstance());
			//diagramDisplay.CurrentTool = new LinearShapeCreationTool(template);
			diagramDisplay.CurrentTool = new ConnectionTool();

		}

		private void button3_Click(object sender, EventArgs e)
		{

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

		public void log(string text)
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
			if (controlPointId == ControlPointId.None || controlPointId == ControlPointId.Any)
				return false;

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

		// gets the control point ID for the given input number. Given number should be 0-indexed.
		public ControlPointId GetControlPointIdForInput(int inputNumber)
		{
			if (inputNumber >= InputCount)
				throw new ArgumentOutOfRangeException("inputNumber");

			// the first control point is the reference point, then all the inputs, then the outputs.
			// to get the given input, offset it by 1, to account for the central reference point.
			return GetControlPointId(1 + inputNumber);
		}

		// gets the control point ID for the given output number. Given number should be 0-indexed.
		public ControlPointId GetControlPointIdForOutput(int outputNumber)
		{
			if (outputNumber >= OutputCount)
				throw new ArgumentOutOfRangeException("outputNumber");

			// the first control point is the reference point, then all the inputs, then the outputs.
			// to get the given output, offset it by 1, to account for the central reference point, then all the inputs.
			return GetControlPointId(1 + InputCount + outputNumber);			
		}

		// returns the (0-indexed) output number for the given control point
		public int GetOutputNumberForControlPoint(ControlPointId controlPoint)
		{
			if (controlPoint == ControlPointId.None || controlPoint == ControlPointId.Any) {
				Debugger.Break();
				return 0;
			}

			int index = GetControlPointIndex(controlPoint);
			if (index <= InputCount || index > InputCount + OutputCount)
				throw new ArgumentOutOfRangeException("controlPoint");

			// take off the inputs, and the reference point to get it 0-indexed
			return index - InputCount - 1;
		}

		// returns the (0-indexed) input number for the given control point
		public int GetInputNumberForControlPoint(ControlPointId controlPoint)
		{
			if (controlPoint == ControlPointId.None || controlPoint == ControlPointId.Any) {
				Debugger.Break();
				return 0;
			}

			int index = GetControlPointIndex(controlPoint);
			if (index <= 0 || index > InputCount)
				throw new ArgumentOutOfRangeException("controlPoint");

			// take off the reference point to get it 0-indexed
			return index - 1;
		}

		public FilterShapeControlPointType GetTypeForControlPoint(ControlPointId controlPoint)
		{
			if (controlPoint == ControlPointId.None || controlPoint == ControlPointId.Any) {
				Debugger.Break();
				return FilterShapeControlPointType.Other;
			}

			int index = GetControlPointIndex(controlPoint);
			if (index == 0)
				return FilterShapeControlPointType.Reference;
			if (index > 0 && index <= InputCount)
				return FilterShapeControlPointType.Input;
			if (index > InputCount && index <= InputCount + OutputCount)
				return FilterShapeControlPointType.Output;

			return FilterShapeControlPointType.Other;
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

		public enum FilterShapeControlPointType
		{
			Reference,
			Input,
			Output,
			Other
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



	public class DataFlowConnectionLine : Polyline
	{
		protected internal DataFlowConnectionLine(ShapeType shapeType, Template template) : base(shapeType, template)
		{
		}

		protected internal DataFlowConnectionLine(ShapeType shapeType, IStyleSet styleSet) : base(shapeType, styleSet)
		{
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is DataFlowConnectionLine) {
				DataFlowConnectionLine src = (DataFlowConnectionLine)source;
				SourceDataFlowComponentReference = src.SourceDataFlowComponentReference;
				DestinationDataComponent = src.DestinationDataComponent;
			}
		}

		public override Shape Clone()
		{
			DataFlowConnectionLine result = new DataFlowConnectionLine(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}

		static public DataFlowConnectionLine CreateInstance(ShapeType shapeType, Template template)
		{
			return new DataFlowConnectionLine(shapeType, template);
		}

		public IDataFlowComponentReference SourceDataFlowComponentReference { get; set; }

		public IDataFlowComponent DestinationDataComponent { get; set; }
	}












	public static class NShapeLibraryInitializer
	{
		public static void Initialize(IRegistrar registrar)
		{
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);
			registrar.RegisterShapeType(
				new ShapeType("ChannelNodeShape", namespaceName, namespaceName,
					ChannelNodeShape.CreateInstance, ChannelNodeShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("ControllerShape", namespaceName, namespaceName,
					ControllerShape.CreateInstance, ControllerShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("OutputShape", namespaceName, namespaceName,
					OutputShape.CreateInstance, OutputShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("DataFlowConnectionLine", namespaceName, namespaceName,
					DataFlowConnectionLine.CreateInstance, DataFlowConnectionLine.GetPropertyDefinitions)
				);
		}

		private const string namespaceName = "VixenFilterShapes";
		private const int preferredRepositoryVersion = 3;
	}






























	public class ConnectionTool : Tool
	{
		public ConnectionTool()
			: base("Standard")
		{
			Construct();
		}



		#region [Public] Tool Members

		public override void RefreshIcons()
		{
			// nothing to do...
		}


		public override bool ProcessMouseEvent(IDiagramPresenter diagramPresenter, MouseEventArgsDg e)
		{
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			bool result = false;
			// get new mouse state
			MouseState newMouseState = MouseState.Empty;
			newMouseState.Buttons = e.Buttons;
			newMouseState.Modifiers = e.Modifiers;
			diagramPresenter.ControlToDiagram(e.Position, out newMouseState.Position);

			diagramPresenter.SuspendUpdate();
			try {
				// Only process mouse action if the position of the mouse or a mouse button state changed
				//if (e.EventType != MouseEventType.MouseMove || newMouseState.Position != CurrentMouseState.Position) {
				// Process the mouse event
				switch (e.EventType) {
					case MouseEventType.MouseDown:
						// Start drag action such as drawing a SelectionFrame or moving selectedShapes/shape handles
						result = ProcessMouseDown(diagramPresenter, newMouseState);
						break;

					case MouseEventType.MouseMove:
						// Set cursors depending on HotSpots or draw moving/resizing preview
						result = ProcessMouseMove(diagramPresenter, newMouseState);
						break;

					case MouseEventType.MouseUp:
						// perform selection/moving/resizing
						result = ProcessMouseUp(diagramPresenter, newMouseState);
						break;

					default: throw new NShapeUnsupportedValueException(e.EventType);
				}
				//}
			} finally { diagramPresenter.ResumeUpdate(); }
			base.ProcessMouseEvent(diagramPresenter, e);
			return result;
		}


		public override bool ProcessKeyEvent(IDiagramPresenter diagramPresenter, KeyEventArgsDg e)
		{
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			// if the keyPress was not handled by the base class, try to handle it here
			bool result = false;
			switch (e.EventType) {
				case KeyEventType.PreviewKeyDown:
				case KeyEventType.KeyPress:
					// do nothing
					break;
				case KeyEventType.KeyDown:
				case KeyEventType.KeyUp:
					if ((KeysDg)e.KeyCode == KeysDg.Delete) {
						// Update selected shape unter the mouse cursor because it was propably deleted
						if (!selectedShapeAtCursorInfo.IsEmpty && !diagramPresenter.SelectedShapes.Contains(selectedShapeAtCursorInfo.Shape)) {
							SetSelectedShapeAtCursor(diagramPresenter, CurrentMouseState.X, CurrentMouseState.Y, diagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
							Invalidate(diagramPresenter);
						}
					}

					// Update Cursor when modifier keys are pressed or released
					if (((KeysDg)e.KeyCode & KeysDg.Shift) == KeysDg.Shift
						|| ((KeysDg)e.KeyCode & KeysDg.ShiftKey) == KeysDg.ShiftKey
						|| ((KeysDg)e.KeyCode & KeysDg.Control) == KeysDg.Control
						|| ((KeysDg)e.KeyCode & KeysDg.ControlKey) == KeysDg.ControlKey
						|| ((KeysDg)e.KeyCode & KeysDg.Alt) == KeysDg.Alt) {
						MouseState mouseState = CurrentMouseState;
						mouseState.Modifiers = (KeysDg)e.Modifiers;
						int cursorId = DetermineCursor(diagramPresenter, mouseState);
						diagramPresenter.SetCursor(cursorId);
					}
					break;
				default: throw new NShapeUnsupportedValueException(e.EventType);
			}
			if (base.ProcessKeyEvent(diagramPresenter, e)) result = true;
			return result;
		}


		public override void EnterDisplay(IDiagramPresenter diagramPresenter)
		{
			// nothing to do
		}


		public override void LeaveDisplay(IDiagramPresenter diagramPresenter)
		{
			// nothing to do
		}


		public override IEnumerable<MenuItemDef> GetMenuItemDefs(IDiagramPresenter diagramPresenter)
		{
			return null;
		}


		public override void Draw(IDiagramPresenter diagramPresenter)
		{
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			switch (CurrentAction) {
				case Action.Select:
					// nothing to do
					break;

				case Action.None:
					break;

				case Action.SelectWithFrame:
					diagramPresenter.ResetTransformation();
					try {
						diagramPresenter.DrawSelectionFrame(frameRect);
					} finally { diagramPresenter.RestoreTransformation(); }
					break;

				case Action.MoveShape:
				case Action.MoveHandle:
					// Draw shape previews first
					diagramPresenter.DrawShapes(Previews.Values);

					// Then draw snap-lines and -points
					if (selectedShapeAtCursorInfo != null && (snapPtId > 0 || snapDeltaX != 0 || snapDeltaY != 0)) {
						Shape previewAtCursor = FindPreviewOfShape(selectedShapeAtCursorInfo.Shape);
						diagramPresenter.DrawSnapIndicators(previewAtCursor);
					}
					// Finally, draw highlighted connection points and/or highlighted shape outlines
					if (diagramPresenter.SelectedShapes.Count == 1 && selectedShapeAtCursorInfo.ControlPointId != ControlPointId.None) {
						Shape preview = FindPreviewOfShape(diagramPresenter.SelectedShapes.TopMost);
						if (preview.HasControlPointCapability(selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Glue)) {
							// Find and highlight valid connection targets in range
							Point p = preview.GetControlPointPosition(selectedShapeAtCursorInfo.ControlPointId);
							DrawConnectionTargets(ActionDiagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, p, ActionDiagramPresenter.SelectedShapes);
						}
					}
					break;

				case Action.ConnectShapes:
					// is there anything to do for this?

					// TODO: highlight hovered connection points, shapes, etc.
					break;

				default:
					throw new NShapeUnsupportedValueException(CurrentAction);
			}
		}


		public override void Invalidate(IDiagramPresenter diagramPresenter)
		{
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			switch (CurrentAction) {
				case Action.None:
				case Action.Select:
					if (!selectedShapeAtCursorInfo.IsEmpty) {
						selectedShapeAtCursorInfo.Shape.Invalidate();
						diagramPresenter.InvalidateGrips(selectedShapeAtCursorInfo.Shape, ControlPointCapabilities.All);
					}
					break;

				case Action.SelectWithFrame:
					diagramPresenter.DisplayService.Invalidate(frameRect);
					break;

				case Action.MoveHandle:
				case Action.MoveShape:
					if (Previews.Count > 0) {
						InvalidateShapes(diagramPresenter, Previews.Values, false);
						if (diagramPresenter.SnapToGrid) {
							Shape previewAtCursor = FindPreviewOfShape(selectedShapeAtCursorInfo.Shape);
							diagramPresenter.InvalidateSnapIndicators(previewAtCursor);
						}
						if (CurrentAction == Action.MoveHandle && selectedShapeAtCursorInfo.IsCursorAtGluePoint)
							InvalidateConnectionTargets(diagramPresenter, CurrentMouseState.X, CurrentMouseState.Y);
					}
					break;

				case Action.ConnectShapes:
					if (currentConnectionLine != null)
						currentConnectionLine.Invalidate();
					break;

				default:
					throw new NShapeUnsupportedValueException(typeof(MenuItemDef), CurrentAction);
			}
		}


		protected override void CancelCore()
		{
			frameRect = Rectangle.Empty;

			//currentToolAction = ToolAction.None;
			selectedShapeAtCursorInfo.Clear();
		}

		#endregion


		#region [Protected] Tool Members

		protected override void StartToolAction(IDiagramPresenter diagramPresenter, int action, MouseState mouseState, bool wantAutoScroll)
		{
			base.StartToolAction(diagramPresenter, action, mouseState, wantAutoScroll);
			// Empty selection frame
			frameRect.Location = mouseState.Position;
			frameRect.Size = Size.Empty;
		}


		protected override void EndToolAction()
		{
			base.EndToolAction();
			//currentToolAction = ToolAction.None;
			if (!IsToolActionPending)
				ClearPreviews();
		}

		#endregion


		#region [Private] Properties

		private Action CurrentAction
		{
			get
			{
				if (IsToolActionPending)
					return (Action)CurrentToolAction.Action;
				else return Action.None;
			}
		}


		#endregion


		#region [Private] MouseEvent processing implementation

		private bool ProcessMouseDown(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			bool result = false;

			// Check if the selected shape at cursor is still valid
			if (!selectedShapeAtCursorInfo.IsEmpty
			    && (!diagramPresenter.SelectedShapes.Contains(selectedShapeAtCursorInfo.Shape)
			        || selectedShapeAtCursorInfo.Shape.HitTest(mouseState.X, mouseState.Y, ControlPointCapabilities.All, diagramPresenter.ZoomedGripSize) == ControlPointId.None)) {
			    selectedShapeAtCursorInfo.Clear();
			}

			// If no action is pending, try to start a new one...
			if (CurrentAction == Action.None) {
				// Get suitable action (depending on the currently selected shape under the mouse cursor)
				Action newAction = DetermineMouseDownAction(diagramPresenter, mouseState);
				if (newAction != Action.None) {
					//currentToolAction = newAction;
					bool wantAutoScroll;
					switch (newAction) {
						case Action.SelectWithFrame:
						case Action.MoveHandle:
						case Action.MoveShape:
						case Action.ConnectShapes:
							// do we ever get here?! shouldn't a click always start with a select?!
							wantAutoScroll = true;
							result = true;
							break;

						default:
							wantAutoScroll = false;
							break;
					}
					StartToolAction(diagramPresenter, (int)newAction, mouseState, wantAutoScroll);

					// If the action requires preview shapes, create them now...
					switch (CurrentAction) {
						case Action.None:
						case Action.Select:
						case Action.SelectWithFrame:
							break;

						case Action.MoveHandle:
						case Action.MoveShape:
							CreatePreviewShapes(diagramPresenter);
							result = true;
							break;

						case Action.ConnectShapes:
							// I don't think we should ever get here?!
							break;

						default:
							throw new NShapeUnsupportedValueException(CurrentAction);
					}

					Invalidate(ActionDiagramPresenter);
				}
			}
			return result;
		}


		private bool ProcessMouseMove(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			bool result = true;

			if (!selectedShapeAtCursorInfo.IsEmpty &&
				!diagramPresenter.SelectedShapes.Contains(selectedShapeAtCursorInfo.Shape))
				selectedShapeAtCursorInfo.Clear();

			switch (CurrentAction) {
				case Action.None:
					result = false;
					SetSelectedShapeAtCursor(diagramPresenter, mouseState.X, mouseState.Y, diagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
					Invalidate(diagramPresenter);
					break;

				case Action.Select:

					ShapeAtCursorInfo shapeAtActionStartInfo =
						FindShapeAtCursor(ActionDiagramPresenter, ActionStartMouseState.X, ActionStartMouseState.Y, ControlPointCapabilities.None, 0, false);

					Action newAction = DetermineMouseMoveAction(ActionDiagramPresenter, mouseState, shapeAtActionStartInfo);

					// If the action has changed, prepare and start the new action
					if (newAction != CurrentAction) {
						switch (newAction) {
							// Select --> SelectWithFrame
							case Action.SelectWithFrame:
								StartToolAction(diagramPresenter, (int)newAction, ActionStartMouseState, true);
								PrepareSelectionFrame(ActionDiagramPresenter, ActionStartMouseState);
								break;

							// Select --> (Select shape and) move shape
							case Action.MoveShape:
							case Action.MoveHandle:
								if (selectedShapeAtCursorInfo.IsEmpty) {
									// Select shape at cursor before start dragging it
									PerformSelection(ActionDiagramPresenter, ActionStartMouseState, shapeAtActionStartInfo);
									SetSelectedShapeAtCursor(diagramPresenter, ActionStartMouseState.X, ActionStartMouseState.Y, 0, ControlPointCapabilities.None);
								}
								// Init moving shape
								CreatePreviewShapes(ActionDiagramPresenter);
								StartToolAction(diagramPresenter, (int)newAction, ActionStartMouseState, true);
								PrepareMoveShapePreview(ActionDiagramPresenter, ActionStartMouseState);
								break;

							case Action.ConnectShapes:
								PrepareConnection(diagramPresenter, mouseState, shapeAtActionStartInfo, newAction);
								StartToolAction(diagramPresenter, (int)newAction, ActionStartMouseState, true);
								break;

							case Action.None:
							case Action.Select:
								throw new Exception("Unhandled state change in MouseMove");

							default:
								throw new Exception(string.Format("Unexpected {0} value: {1}", CurrentAction.GetType().Name, CurrentAction));
						}
					}
					Invalidate(ActionDiagramPresenter);
					break;

				case Action.SelectWithFrame:
					PrepareSelectionFrame(ActionDiagramPresenter, mouseState);
					break;

				case Action.MoveHandle:
					PrepareMoveHandlePreview(ActionDiagramPresenter, mouseState);
					break;

				case Action.MoveShape:
					PrepareMoveShapePreview(diagramPresenter, mouseState);
					break;

				case Action.ConnectShapes:
					ShapeAtCursorInfo shapeAtCursorInfo = FindShapeAtCursor(ActionDiagramPresenter, mouseState.X, mouseState.Y, ControlPointCapabilities.None, 0, false);
					UpdateConnection(diagramPresenter, mouseState, shapeAtCursorInfo);
					break;

				default:
					throw new NShapeUnsupportedValueException(typeof(Action), CurrentAction);
			}

			int cursorId = DetermineCursor(diagramPresenter, mouseState);
			if (CurrentAction == Action.None) diagramPresenter.SetCursor(cursorId);
			else ActionDiagramPresenter.SetCursor(cursorId);

			return result;
		}


		private bool ProcessMouseUp(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			bool result = false;

			Console.WriteLine("Processing mouse UP event. Current action is " + CurrentAction.ToString());

			if (!selectedShapeAtCursorInfo.IsEmpty &&
				!diagramPresenter.SelectedShapes.Contains(selectedShapeAtCursorInfo.Shape))
				selectedShapeAtCursorInfo.Clear();

			switch (CurrentAction) {
				case Action.None:
					// do nothing
					break;

				case Action.Select:
					// Perform selection
					ShapeAtCursorInfo shapeAtCursorInfo = FindShapeAtCursor(diagramPresenter, mouseState.X, mouseState.Y, ControlPointCapabilities.None, 0, false);
					result = PerformSelection(ActionDiagramPresenter, mouseState, shapeAtCursorInfo);
					SetSelectedShapeAtCursor(ActionDiagramPresenter, mouseState.X, mouseState.Y, ActionDiagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
					EndToolAction();
					break;

				case Action.SelectWithFrame:
					// select all selectedShapes within the frame
					result = PerformFrameSelection(ActionDiagramPresenter, mouseState);
					while (IsToolActionPending)
						EndToolAction();
					break;

				case Action.MoveHandle:
					result = PerformMoveHandle(ActionDiagramPresenter);
					while (IsToolActionPending)
						EndToolAction();
					break;

				case Action.MoveShape:
					result = PerformMoveShape(ActionDiagramPresenter);
					while (IsToolActionPending)
						EndToolAction();
					break;

				case Action.ConnectShapes:
					result = FinishConnection(ActionDiagramPresenter);
					while (IsToolActionPending)
						EndToolAction();
					break; 
					
				default:
					throw new NShapeUnsupportedValueException(CurrentAction);
			}

			SetSelectedShapeAtCursor(diagramPresenter, mouseState.X, mouseState.Y, diagramPresenter.ZoomedGripSize, ControlPointCapabilities.All);
			diagramPresenter.SetCursor(DetermineCursor(diagramPresenter, mouseState));

			OnToolExecuted(ExecutedEventArgs);
			return result;
		}


		#endregion


		#region [Private] Determine action depending on mouse state and event type

		/// <summary>
		/// Decide which tool action is suitable for the current mouse state.
		/// </summary>
		private Action DetermineMouseDownAction(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			if (mouseState.IsButtonDown(MouseButtonsDg.Left) || mouseState.IsButtonDown(MouseButtonsDg.Right)) {
				// if left or right buttons are down, start a 'select' action. This will refine to an
				// appropriate drag action later on (when mouse moving) based on the mouse button, otherwise
				// it will (de)select the shape on mouseup
				return Action.Select;
			}
			// Ignore other pressed mouse buttons
			return CurrentAction;
		}


		/// <summary>
		/// Decide which tool action is suitable for the current mouse state.
		/// </summary>
		private Action DetermineMouseMoveAction(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo)
		{
			switch (CurrentAction) {
				case Action.None:
				case Action.MoveHandle:
				case Action.MoveShape:
				case Action.SelectWithFrame:
				case Action.ConnectShapes:
					// Do not change the current action
					return CurrentAction;

				case Action.Select:
					if (mouseState.IsButtonDown(MouseButtonsDg.Left)) {
						// if we're doing something with the left mouse button, it will be a 'drag' style action
						if (!IsDragActionFeasible(mouseState)) {
							return CurrentAction;
						}

						// Check if cursor is over a control point and moving grips is feasible
						if (selectedShapeAtCursorInfo.IsCursorAtGrip) {
							if (IsMoveHandleFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
								return Action.MoveHandle;
							else
								return Action.SelectWithFrame;
						} else {
							// If there is no shape under the cursor, start a SelectWithFrame action,
							// otherwise start a MoveShape action
							bool canMove = false;
							if (!selectedShapeAtCursorInfo.IsEmpty) {
								// If there are selected shapes, check these shapes...
								canMove = IsMoveShapeFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo);
							} else {
								// ... otherwise check the shape under the cursor as it will be selected 
								// before starting a move action
								canMove = IsMoveShapeFeasible(diagramPresenter, mouseState, shapeAtCursorInfo);
							}
							return canMove ? Action.MoveShape : Action.SelectWithFrame;
						}
					} else if (mouseState.IsButtonDown(MouseButtonsDg.Right)) {
						if (IsConnectFromShapeFeasible(diagramPresenter, mouseState, shapeAtCursorInfo)) {
							return Action.ConnectShapes;
						}
					}
					return CurrentAction;

				default:
					throw new NShapeUnsupportedValueException(CurrentAction);
			}
		}

		#endregion


		#region [Private] Action implementations

		#region Selecting Shapes

		// (Un)Select shape unter the mouse pointer
		private bool PerformSelection(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo)
		{
			bool result = false;
			bool multiSelect = mouseState.IsKeyPressed(KeysDg.Control) || mouseState.IsKeyPressed(KeysDg.Shift);

			// When selecting shapes conteolpoints should be ignored as the user does not see them 
			// until a shape is selected
			const ControlPointCapabilities capabilities = ControlPointCapabilities.None;
			const int range = 0;

			// Determine the shape that has to be selected:
			Shape shapeToSelect = null;
			if (!selectedShapeAtCursorInfo.IsEmpty) {
				// When in multiSelection mode, unselect the selected shape under the cursor
				if (multiSelect) shapeToSelect = selectedShapeAtCursorInfo.Shape;
				else {
					// First, check if the selected shape under the cursor has children that can be selected
					shapeToSelect = selectedShapeAtCursorInfo.Shape.Children.FindShape(mouseState.X, mouseState.Y, capabilities, range, null);
					// Second, check if the selected shape under the cursor has siblings that can be selected
					if (shapeToSelect == null && selectedShapeAtCursorInfo.Shape.Parent != null) {
						shapeToSelect = selectedShapeAtCursorInfo.Shape.Parent.Children.FindShape(mouseState.X, mouseState.Y, capabilities, range, selectedShapeAtCursorInfo.Shape);
						// Discard found shape if it is the selected shape at cursor
						if (shapeToSelect == selectedShapeAtCursorInfo.Shape) shapeToSelect = null;
						if (shapeToSelect == null) {
							foreach (Shape shape in selectedShapeAtCursorInfo.Shape.Parent.Children.FindShapes(mouseState.X, mouseState.Y, capabilities, range)) {
								if (shape == selectedShapeAtCursorInfo.Shape) continue;
								// Ignore layer visibility for child shapes
								shapeToSelect = shape;
								break;
							}
						}
					}
				}
			}

			// If there was a shape to select related to the selected shape under the cursor
			// (a child or a sibling of the selected shape or a shape below it),
			// try to select the first non-selected shape under the cursor
			if (shapeToSelect == null && shapeAtCursorInfo.Shape != null
				&& shapeAtCursorInfo.Shape.ContainsPoint(mouseState.X, mouseState.Y))
				shapeToSelect = shapeAtCursorInfo.Shape;

			// If a new shape to select was found, perform selection
			if (shapeToSelect != null) {
				// (check if multiselection mode is enabled (Shift + Click or Ctrl + Click))
				if (multiSelect) {
					// if multiSelect is enabled, add/remove to/from selected selectedShapes...
					if (diagramPresenter.SelectedShapes.Contains(shapeToSelect)) {
						// if object is selected -> remove from selection
						diagramPresenter.UnselectShape(shapeToSelect);
						RemovePreviewOf(shapeToSelect);
						result = true;
					} else {
						// If object is not selected -> add to selection
						diagramPresenter.SelectShape(shapeToSelect, true);
						result = true;
					}
				} else {
					// ... otherwise deselect all selectedShapes but the clicked object
					ClearPreviews();
					// check if the clicked shape is a child of an already selected shape
					Shape childShape = null;
					if (diagramPresenter.SelectedShapes.Count == 1
						&& diagramPresenter.SelectedShapes.TopMost.Children != null
						&& diagramPresenter.SelectedShapes.TopMost.Children.Count > 0) {
						childShape = diagramPresenter.SelectedShapes.TopMost.Children.FindShape(mouseState.X, mouseState.Y, ControlPointCapabilities.None, 0, null);
					}
					if (childShape != null) diagramPresenter.SelectShape(childShape, false);
					else diagramPresenter.SelectShape(shapeToSelect, false);
					result = true;
				}

				// validate if the desired shape or its parent was selected
				if (shapeToSelect.Parent != null) {
					if (!diagramPresenter.SelectedShapes.Contains(shapeToSelect))
						if (diagramPresenter.SelectedShapes.Contains(shapeToSelect.Parent))
							shapeToSelect = shapeToSelect.Parent;
				}
			} else if (selectedShapeAtCursorInfo.IsEmpty) {
				// if there was no other shape to select and none of the selected shapes is under the cursor,
				// clear selection
				if (!multiSelect) {
					if (diagramPresenter.SelectedShapes.Count > 0) {
						diagramPresenter.UnselectAll();
						ClearPreviews();
						result = true;
					}
				}
			} else {
				// if there was no other shape to select and a selected shape is under the cursor,
				// do nothing
			}
			return result;
		}


		// Calculate new selection frame
		private void PrepareSelectionFrame(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			Invalidate(ActionDiagramPresenter);

			frameRect.X = Math.Min(ActionStartMouseState.X, mouseState.X);
			frameRect.Y = Math.Min(ActionStartMouseState.Y, mouseState.Y);
			frameRect.Width = Math.Max(ActionStartMouseState.X, mouseState.X) - frameRect.X;
			frameRect.Height = Math.Max(ActionStartMouseState.Y, mouseState.Y) - frameRect.Y;

			Invalidate(ActionDiagramPresenter);
		}


		// Select shapes inside the selection frame
		private bool PerformFrameSelection(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			bool multiSelect = mouseState.IsKeyPressed(KeysDg.Control) || mouseState.IsKeyPressed(KeysDg.Shift);
			diagramPresenter.SelectShapes(frameRect, multiSelect);
			return true;
		}

		#endregion


		#region Connecting / Disconnecting Shapes

		private void PrepareConnection(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo, Action action)
		{
			// if we're not dealing with filter shapes... well, what the fuck?! get out of here!
			FilterSetupShapeBase shape = shapeAtCursorInfo.Shape as FilterSetupShapeBase;
			if (shape == null)
				throw new Exception("Can't connect a shape that isn't a FilterSetup shape!");

			StartToolAction(diagramPresenter, (int)action, ActionStartMouseState, true);

			DataFlowConnectionLine line = (DataFlowConnectionLine)diagramPresenter.Project.ShapeTypes["DataFlowConnectionLine"].CreateInstance();
			diagramPresenter.Diagram.Shapes.Add(line, 100);		// yaaay, arbitrarily high Z order to draw above everything else
			line.EndCapStyle = diagramPresenter.Project.Design.CapStyles.ClosedArrow;

			// get the starting control point for the line; ie. the first output for the shape, or the selected control point.
			// TODO: filter out already-connected control points, and pick the first unused one (for a generic shape-click
			//       only: dragging from a specific point will select that point below)
			ControlPointId connectionPoint = shape.GetControlPointIdForOutput(0);
			line.SourceDataFlowComponentReference = new DataFlowComponentReference(shape.DataFlowComponent, 0);
			line.DestinationDataComponent = null;

			if (shapeAtCursorInfo.IsCursorAtConnectionPoint) {
				if (shape.GetTypeForControlPoint(shapeAtCursorInfo.ControlPointId) == FilterSetupShapeBase.FilterShapeControlPointType.Output)
					connectionPoint = shapeAtCursorInfo.ControlPointId;
			}
			line.Connect(ControlPointId.FirstVertex, shape, connectionPoint);
			line.Disconnect(ControlPointId.LastVertex);
			line.MoveControlPointTo(ControlPointId.LastVertex, mouseState.X, mouseState.Y, ResizeModifiers.None);
			currentConnectionLine = line;
		}

		private void UpdateConnection(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo)
		{
			if (currentConnectionLine == null)
				throw new Exception("expecting to have a connection line when in CONNECT mode on drag!");

VixenSystem.Logging.Info("UpdateConnection: given shape info was: " + shapeAtCursorInfo.Shape + ". (Is Empty: " + shapeAtCursorInfo.IsEmpty + ")");
			bool connectionLineTargetConnected = false;
			FilterSetupShapeBase filtershape = null;
			if (!shapeAtCursorInfo.IsEmpty)
				filtershape = shapeAtCursorInfo.Shape as FilterSetupShapeBase;

			if (filtershape != null) {
				ControlPointId point = filtershape.HitTest(mouseState.X, mouseState.Y, ControlPointCapabilities.Connect, 10);

				if (point != ControlPointId.Any && point != ControlPointId.None) {
					bool skipConnection = false;

					if (filtershape.GetTypeForControlPoint(point) != FilterSetupShapeBase.FilterShapeControlPointType.Input) {
						// TODO: find an appropriate input connection point for the shape; don't just assume it's the first
						if (filtershape.InputCount > 0) {
							point = filtershape.GetControlPointIdForInput(0);
						}
						else {
							skipConnection = true;
						}
					}

					if (!skipConnection) {
VixenSystem.Logging.Info("UpdateConnection: connecting to shape " + filtershape.Title + ". point " + point.ToString());
						// TODO: check the input of the shape it's connecting to; ensure there's only a single connection. (can't have more than one, currently)
						if (currentConnectionLine.GetConnectionInfo(ControlPointId.LastVertex, null).OtherPointId != point) {
							currentConnectionLine.Disconnect(ControlPointId.LastVertex);
							currentConnectionLine.Connect(ControlPointId.LastVertex, filtershape, point);
							currentConnectionLine.DestinationDataComponent = filtershape.DataFlowComponent;
						}

						connectionLineTargetConnected = true;
					}
				}
			}

			if (!connectionLineTargetConnected) {
VixenSystem.Logging.Info("UpdateConnection: connecting to no shape.");
				currentConnectionLine.Disconnect(ControlPointId.LastVertex);
				currentConnectionLine.MoveControlPointTo(ControlPointId.LastVertex, mouseState.X, mouseState.Y, ResizeModifiers.None);
				currentConnectionLine.DestinationDataComponent = null;
			}
		}


		private bool FinishConnection(IDiagramPresenter diagramPresenter)
		{
			bool result = false;
			if (currentConnectionLine.DestinationDataComponent != null && currentConnectionLine.SourceDataFlowComponentReference != null) {
				// TODO: split this out properly, removing all the Vixen and Filter specific stuff (events, or somesuch?)

				//ShapeConnectionInfo sourceConnectionInfo = currentConnectionLine.GetConnectionInfo(ControlPointId.FirstVertex, null);
				//ShapeConnectionInfo destConnectionInfo = currentConnectionLine.GetConnectionInfo(ControlPointId.LastVertex, null);
				//FilterSetupShapeBase sourceShape = sourceConnectionInfo.OtherShape as FilterSetupShapeBase;
				//FilterSetupShapeBase destShape = destConnectionInfo.OtherShape as FilterSetupShapeBase;

				//if (sourceShape == null || destShape == null)
				//    throw new Exception("Expecting a filter shape when making a connection");

				//int outputIndex = sourceShape.GetOutputNumberForControlPoint(sourceConnectionInfo.OtherPointId);

				//// TODO: check that everything is OK first before making connections -- index is OK, data flow is OK, etc.
				//destShape.DataFlowComponent.Source = new DataFlowComponentReference(sourceShape.DataFlowComponent, outputIndex);


				currentConnectionLine.DestinationDataComponent.Source = currentConnectionLine.SourceDataFlowComponentReference;

				result = true;
			} else {
				currentConnectionLine.Disconnect(ControlPointId.FirstVertex);
				currentConnectionLine.Disconnect(ControlPointId.LastVertex);
				diagramPresenter.Diagram.Shapes.Remove(currentConnectionLine);
			}

			return result;
		}


		#endregion


		#region Moving Shapes

		// prepare drawing preview of move action
		private void PrepareMoveShapePreview(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			// calculate the movement
			int distanceX = mouseState.X - ActionStartMouseState.X;
			int distanceY = mouseState.Y - ActionStartMouseState.Y;
			// calculate "Snap to Grid" offset
			snapDeltaX = snapDeltaY = 0;
			if (diagramPresenter.SnapToGrid && !selectedShapeAtCursorInfo.IsEmpty) {
				FindNearestSnapPoint(diagramPresenter, selectedShapeAtCursorInfo.Shape, distanceX, distanceY, out snapDeltaX, out snapDeltaY);
				distanceX += snapDeltaX;
				distanceY += snapDeltaY;
			}

			// Reset all shapes to start values
			ResetPreviewShapes(diagramPresenter);

			// Move (preview copies of) the selected shapes
			Rectangle shapeBounds = Rectangle.Empty;
			foreach (Shape originalShape in diagramPresenter.SelectedShapes) {
				// Get preview of the shape to move...
				Shape previewShape = FindPreviewOfShape(originalShape);
				// ...and move the preview shape to the new position
				previewShape.MoveTo(originalShape.X + distanceX, originalShape.Y + distanceY);
			}
		}


		// Apply the move action
		private bool PerformMoveShape(IDiagramPresenter diagramPresenter)
		{
			bool result = false;
			if (selectedShapeAtCursorInfo.IsEmpty) {
				// This should never happen...
				Debug.Assert(!selectedShapeAtCursorInfo.IsEmpty);
			}

			if (ActionStartMouseState.Position != CurrentMouseState.Position) {
				// calculate the movement
				int distanceX = CurrentMouseState.X - ActionStartMouseState.X;
				int distanceY = CurrentMouseState.Y - ActionStartMouseState.Y;
				//snapDeltaX = snapDeltaY = 0;
				//if (diagramPresenter.SnapToGrid)
				//   FindNearestSnapPoint(diagramPresenter, SelectedShapeAtCursorInfo.Shape, distanceX, distanceY, out snapDeltaX, out snapDeltaY, ControlPointCapabilities.All);

				ICommand cmd = new MoveShapeByCommand(diagramPresenter.SelectedShapes, distanceX + snapDeltaX, distanceY + snapDeltaY);
				diagramPresenter.Project.ExecuteCommand(cmd);

				snapDeltaX = snapDeltaY = 0;
				snapPtId = ControlPointId.None;
				result = true;
			}
			return result;
		}

		#endregion


		#region Moving ControlPoints

		// prepare drawing preview of resize action 
		private void PrepareMoveHandlePreview(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			InvalidateConnectionTargets(diagramPresenter, CurrentMouseState.X, CurrentMouseState.Y);

			int distanceX = mouseState.X - ActionStartMouseState.X;
			int distanceY = mouseState.Y - ActionStartMouseState.Y;

			// calculate "Snap to Grid/ControlPoint" offset
			snapDeltaX = snapDeltaY = 0;
			if (selectedShapeAtCursorInfo.IsCursorAtGluePoint) {
				ControlPointId targetPtId;
				Shape targetShape = FindNearestControlPoint(diagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Connect, distanceX, distanceY, out snapDeltaX, out snapDeltaY, out targetPtId);
			} else
				FindNearestSnapPoint(diagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, distanceX, distanceY, out snapDeltaX, out snapDeltaY);
			distanceX += snapDeltaX;
			distanceY += snapDeltaY;

			// Reset all preview shapes to start values
			ResetPreviewShapes(diagramPresenter);

			// Move selected shapes
			ResizeModifiers resizeModifier = GetResizeModifier(mouseState);
			Point originalPtPos = Point.Empty;
			foreach (Shape selectedShape in diagramPresenter.SelectedShapes) {
				Shape previewShape = FindPreviewOfShape(selectedShape);
				// Perform movement
				if (previewShape.HasControlPointCapability(selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Resize))
					previewShape.MoveControlPointBy(selectedShapeAtCursorInfo.ControlPointId, distanceX, distanceY, resizeModifier);
			}

			InvalidateConnectionTargets(diagramPresenter, mouseState.X, mouseState.Y);
		}


		// apply the resize action
		private bool PerformMoveHandle(IDiagramPresenter diagramPresenter)
		{
			bool result = false;
			Invalidate(diagramPresenter);

			int distanceX = CurrentMouseState.X - ActionStartMouseState.X;
			int distanceY = CurrentMouseState.Y - ActionStartMouseState.Y;

			// if the moved ControlPoint is a single GluePoint, snap to ConnectionPoints
			bool isGluePoint = false;
			if (diagramPresenter.SelectedShapes.Count == 1)
				isGluePoint = selectedShapeAtCursorInfo.Shape.HasControlPointCapability(selectedShapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Glue);

			// Snap to Grid or ControlPoint
			bool calcSnapDistance = true;
			ShapeAtCursorInfo targetShapeInfo = ShapeAtCursorInfo.Empty;
			if (isGluePoint) {
				Point currentPtPos = selectedShapeAtCursorInfo.Shape.GetControlPointPosition(selectedShapeAtCursorInfo.ControlPointId);
				Point newPtPos = Point.Empty;
				newPtPos.Offset(currentPtPos.X + distanceX, currentPtPos.Y + distanceY);
				targetShapeInfo = FindConnectionTarget(ActionDiagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, newPtPos, true, true);
				if (!targetShapeInfo.IsEmpty) {
					// If there is a target shape to connect to, get the position of the target connection point 
					// and move the gluepoint exactly to this position
					calcSnapDistance = false;
					if (targetShapeInfo.ControlPointId != ControlPointId.Reference) {
						Point pt = targetShapeInfo.Shape.GetControlPointPosition(targetShapeInfo.ControlPointId);
						distanceX = pt.X - currentPtPos.X;
						distanceY = pt.Y - currentPtPos.Y;
					} else {
						// If the target point is the reference point, use the previously calculated snap distance
						// ToDo: We need a solution for calculating the nearest point on the target shape's outline
						distanceX += snapDeltaX;
						distanceY += snapDeltaY;
					}
				}
			}
			if (calcSnapDistance) {
				FindNearestSnapPoint(diagramPresenter, selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, distanceX, distanceY, out snapDeltaX, out snapDeltaY);
				distanceX += snapDeltaX;
				distanceY += snapDeltaY;
			}

			ResizeModifiers resizeModifier = GetResizeModifier(CurrentMouseState);
			if (isGluePoint) {
				ICommand cmd = new MoveGluePointCommand(selectedShapeAtCursorInfo.Shape, selectedShapeAtCursorInfo.ControlPointId, targetShapeInfo.Shape, targetShapeInfo.ControlPointId, distanceX, distanceY, resizeModifier);
				diagramPresenter.Project.ExecuteCommand(cmd);
			} else {
				ICommand cmd = new MoveControlPointCommand(ActionDiagramPresenter.SelectedShapes, selectedShapeAtCursorInfo.ControlPointId, distanceX, distanceY, resizeModifier);
				diagramPresenter.Project.ExecuteCommand(cmd);
			}

			snapDeltaX = snapDeltaY = 0;
			snapPtId = ControlPointId.None;
			result = true;

			return result;
		}

		#endregion


		#endregion


		#region [Private] Preview management implementation

		/// <summary>
		/// The dictionary of preview shapes: The key is the original shape, the value is the preview shape.
		/// </summary>
		private IDictionary<Shape, Shape> Previews
		{
			get { return previewShapes; }
		}


		private Shape FindPreviewOfShape(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			return previewShapes[shape];
		}


		private Shape FindShapeOfPreview(Shape previewShape)
		{
			if (previewShape == null) throw new ArgumentNullException("previewShape");
			return originalShapes[previewShape];
		}


		private void AddPreview(Shape shape, Shape previewShape, IDisplayService displayService)
		{
			if (originalShapes.ContainsKey(previewShape)) return;
			if (previewShapes.ContainsKey(shape)) return;
			// Set DisplayService for the preview shape
			if (previewShape.DisplayService != displayService)
				previewShape.DisplayService = displayService;

			// Add shape and its preview to the appropriate dictionaries
			previewShapes.Add(shape, previewShape);
			originalShapes.Add(previewShape, shape);

			// Add shape's children and their previews to the appropriate dictionaries
			if (previewShape.Children.Count > 0) {
				IEnumerator<Shape> previewChildren = previewShape.Children.TopDown.GetEnumerator();
				IEnumerator<Shape> originalChildren = shape.Children.TopDown.GetEnumerator();

				previewChildren.Reset();
				originalChildren.Reset();
				bool processNext = false;
				if (previewChildren.MoveNext() && originalChildren.MoveNext())
					processNext = true;
				while (processNext) {
					AddPreview(originalChildren.Current, previewChildren.Current, displayService);
					processNext = (previewChildren.MoveNext() && originalChildren.MoveNext());
				}
			}
		}

		private void RemovePreviewOf(Shape originalShape)
		{
			if (previewShapes.ContainsKey(originalShape)) {
				// Invalidate Preview Shape
				Shape previewShape = Previews[originalShape];
				previewShape.Invalidate();

				// remove previews of the shape and its children from the preview's dictionary
				previewShapes.Remove(originalShape);
				if (originalShape.Children.Count > 0) {
					foreach (Shape childShape in originalShape.Children)
						previewShapes.Remove(childShape);
				}
				// remove the shape and its children from the shape's dictionary
				originalShapes.Remove(previewShape);
				if (previewShape.Children.Count > 0) {
					foreach (Shape childShape in previewShape.Children)
						originalShapes.Remove(childShape);
				}
			}
		}

		private void ClearPreviews()
		{
			foreach (KeyValuePair<Shape, Shape> item in previewShapes) {
				Shape previewShape = item.Value;
				DeletePreviewShape(ref previewShape);
			}
			previewShapes.Clear();
			originalShapes.Clear();
		}


		private bool IsConnectedToNonSelectedShape(IDiagramPresenter diagramPresenter, Shape shape)
		{
			foreach (ControlPointId gluePointId in shape.GetControlPointIds(ControlPointCapabilities.Glue)) {
				ShapeConnectionInfo sci = shape.GetConnectionInfo(gluePointId, null);
				if (!sci.IsEmpty
					&& !diagramPresenter.SelectedShapes.Contains(sci.OtherShape))
					return true;
			}
			return false;
		}


		/// <summary>
		/// Create previews of shapes connected to the given shape (and it's children) and connect them to the
		/// shape's preview (or the preview of it's child)
		/// </summary>
		private void ConnectPreviewOfShape(IDiagramPresenter diagramPresenter, Shape shape)
		{
			// process shape's children
			if (shape.Children != null && shape.Children.Count > 0) {
				foreach (Shape childShape in shape.Children)
					ConnectPreviewOfShape(diagramPresenter, childShape);
			}

			Shape preview = FindPreviewOfShape(shape);
			foreach (ShapeConnectionInfo connectionInfo in shape.GetConnectionInfos(ControlPointId.Any, null)) {
				if (!diagramPresenter.IsLayerVisible(connectionInfo.OtherShape.Layers)) continue;
				if (diagramPresenter.SelectedShapes.Contains(connectionInfo.OtherShape)) {
					// Do not connect previews if BOTH of the connected shapes are part of the selection because 
					// this would restrict movement of the connector shapes and decreases performance (many 
					// unnecessary FollowConnectionPointWithGluePoint() calls)
					if (shape.HasControlPointCapability(connectionInfo.OwnPointId, ControlPointCapabilities.Glue)) {
						if (IsConnectedToNonSelectedShape(diagramPresenter, shape)) {
							Shape targetPreview = FindPreviewOfShape(connectionInfo.OtherShape);
							preview.Connect(connectionInfo.OwnPointId, targetPreview, connectionInfo.OtherPointId);
						}
					}
				} else {
					// Connect preview of shape to a non-selected shape if it is a single shape 
					// that has a glue point (e.g. a Label)
					if (preview.HasControlPointCapability(connectionInfo.OwnPointId, ControlPointCapabilities.Glue)) {
						// Only connect if the control point to be connected is not the control point to be moved
						if (shape == selectedShapeAtCursorInfo.Shape && connectionInfo.OwnPointId != selectedShapeAtCursorInfo.ControlPointId)
							preview.Connect(connectionInfo.OwnPointId, connectionInfo.OtherShape, connectionInfo.OtherPointId);
					} else
						// Create a preview of the shape that is connected to the preview (recursive call)
						CreateConnectedTargetPreviewShape(diagramPresenter, preview, connectionInfo);
				}
			}
		}


		/// <summary>
		/// Creates (or finds) a preview of the connection's PassiveShape and connects it to the current preview shape
		/// </summary>
		private void CreateConnectedTargetPreviewShape(IDiagramPresenter diagramPresenter, Shape previewShape, ShapeConnectionInfo connectionInfo)
		{
			// Check if any other selected shape is connected to the same non-selected shape
			Shape previewTargetShape;
			// If the current passiveShape is already connected to another shape of the current selection,
			// connect the current preview to the other preview's passiveShape
			if (!targetShapeBuffer.TryGetValue(connectionInfo.OtherShape, out previewTargetShape)) {
				// If the current passiveShape is not connected to any other of the selected selectedShapes,
				// create a clone of the passiveShape and connect it to the corresponding preview
				// If the preview exists, abort connecting (in this case, the shape is a preview of a child shape)
				if (previewShapes.ContainsKey(connectionInfo.OtherShape))
					return;
				else {
					previewTargetShape = connectionInfo.OtherShape.Type.CreatePreviewInstance(connectionInfo.OtherShape);
					AddPreview(connectionInfo.OtherShape, previewTargetShape, diagramPresenter.DisplayService);
				}
				// Add passive shape and it's clone to the passive shape dictionary
				targetShapeBuffer.Add(connectionInfo.OtherShape, previewTargetShape);
			}
			// Connect the (new or existing) preview shapes
			// Skip connecting if the preview is already connected.
			if (previewTargetShape.IsConnected(connectionInfo.OtherPointId, null) == ControlPointId.None) {
				previewTargetShape.Connect(connectionInfo.OtherPointId, previewShape, connectionInfo.OwnPointId);
				// check, if any shapes are connected to the connector (that is connected to the selected shape)
				foreach (ShapeConnectionInfo connectorCI in connectionInfo.OtherShape.GetConnectionInfos(ControlPointId.Any, null)) {
					if (!diagramPresenter.IsLayerVisible(connectorCI.OtherShape.Layers)) continue;
					// skip if the connector is connected to the shape with more than one glue point
					if (connectorCI.OtherShape == FindShapeOfPreview(previewShape)) continue;
					if (connectorCI.OwnPointId != connectionInfo.OtherPointId) {
						// Check if the shape on the other end is selected.
						// If it is, connect to it's preview or skip connecting if the target preview does 
						// not exist yet (it will be connected when creating the targt's preview)
						if (diagramPresenter.SelectedShapes.Contains(connectorCI.OtherShape)) {
							if (previewShapes.ContainsKey(connectorCI.OtherShape)) {
								Shape s = FindPreviewOfShape(connectorCI.OtherShape);
								if (s.IsConnected(connectorCI.OtherPointId, previewTargetShape) == ControlPointId.None)
									previewTargetShape.Connect(connectorCI.OwnPointId, s, connectorCI.OtherPointId);
							} else continue;
						} else if (connectorCI.OtherShape.HasControlPointCapability(connectorCI.OtherPointId, ControlPointCapabilities.Glue))
							// Connect connectors connected to the previewTargetShape
							CreateConnectedTargetPreviewShape(diagramPresenter, previewTargetShape, connectorCI);
						else if (connectorCI.OtherPointId == ControlPointId.Reference) {
							// Connect the other end of the previewTargetShape if the connection is a Point-To-Shape connection
							if (previewTargetShape.IsConnected(connectorCI.OwnPointId, null) == ControlPointId.None)
								previewTargetShape.Connect(connectorCI.OwnPointId, connectorCI.OtherShape, connectorCI.OtherPointId);
						}
					}
				}
			}
		}


		#endregion


		#region [Private] Helper Methods

		private void SetSelectedShapeAtCursor(IDiagramPresenter diagramPresenter, int mouseX, int mouseY, int handleRadius, ControlPointCapabilities handleCapabilities)
		{
			// Find the shape under the cursor
			selectedShapeAtCursorInfo.Clear();
			selectedShapeAtCursorInfo.Shape = diagramPresenter.SelectedShapes.FindShape(mouseX, mouseY, handleCapabilities, handleRadius, null);

			// If there is a shape under the cursor, find the nearest control point and caption
			if (!selectedShapeAtCursorInfo.IsEmpty) {
				ControlPointCapabilities capabilities;
				if (CurrentMouseState.IsKeyPressed(KeysDg.Control)) capabilities = ControlPointCapabilities.Resize /*| ControlPointCapabilities.Movable*/;
				else if (CurrentMouseState.IsKeyPressed(KeysDg.Shift)) capabilities = ControlPointCapabilities.Rotate;
				else capabilities = gripCapabilities;

				// Find control point at cursor that belongs to the selected shape at cursor
				selectedShapeAtCursorInfo.ControlPointId = selectedShapeAtCursorInfo.Shape.FindNearestControlPoint(mouseX, mouseY, diagramPresenter.ZoomedGripSize, capabilities);
				// Find caption at cursor (if the shape is a captioned shape)
				if (selectedShapeAtCursorInfo.Shape is ICaptionedShape && ((ICaptionedShape)selectedShapeAtCursorInfo.Shape).CaptionCount > 0)
					selectedShapeAtCursorInfo.CaptionIndex = ((ICaptionedShape)selectedShapeAtCursorInfo.Shape).FindCaptionFromPoint(mouseX, mouseY);
			}
		}

		private int DetermineCursor(IDiagramPresenter diagramPresenter, MouseState mouseState)
		{
			switch (CurrentAction) {
				case Action.None:
					// If no action is pending, the folowing cursors are possible:
					// - Default (no selected shape under cursor or action not granted)
					// - Move shape cursor
					// - Move grip cursor
					if (!selectedShapeAtCursorInfo.IsEmpty) {
						// Check if cursor is over a control point and moving grips is feasible
						if (selectedShapeAtCursorInfo.IsCursorAtGrip) {
							if (IsMoveHandleFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
								return cursors[ToolCursor.MoveHandle];
							else return cursors[ToolCursor.Default];
						}
						// Check if cursor is inside the shape and move shape is feasible
						if (IsMoveShapeFeasible(diagramPresenter, mouseState, selectedShapeAtCursorInfo))
							return cursors[ToolCursor.MoveShape];
					}
					return cursors[ToolCursor.Default];

				case Action.Select:
				case Action.SelectWithFrame:
					return cursors[ToolCursor.Default];

				case Action.MoveHandle:
					if (selectedShapeAtCursorInfo.IsCursorAtGluePoint) {
						Shape previewShape = FindPreviewOfShape(selectedShapeAtCursorInfo.Shape);
						Point ptPos = previewShape.GetControlPointPosition(selectedShapeAtCursorInfo.ControlPointId);
						ShapeAtCursorInfo shapeAtCursorInfo = FindConnectionTarget(
							diagramPresenter,
							selectedShapeAtCursorInfo.Shape,
							selectedShapeAtCursorInfo.ControlPointId,
							ptPos,
							true,
							false);
						if (!shapeAtCursorInfo.IsEmpty && shapeAtCursorInfo.IsCursorAtConnectionPoint)
							return cursors[ToolCursor.Connect];
					}
					return cursors[ToolCursor.MoveHandle];

				case Action.MoveShape:
					return cursors[ToolCursor.MoveShape];

				case Action.ConnectShapes:
					return cursors[ToolCursor.Connect];

				default:
					throw new NShapeUnsupportedValueException(CurrentAction);
			}
		}


		/// <summary>
		/// Create Previews of all shapes selected in the CurrentDisplay.
		/// These previews are connected to all the shapes the original shapes are connected to.
		/// </summary>
		private void CreatePreviewShapes(IDiagramPresenter diagramPresenter)
		{
			if (diagramPresenter == null) throw new ArgumentNullException("diagramPresenter");
			if (Previews.Count == 0 && diagramPresenter.SelectedShapes.Count > 0) {
				// first, clone all selected shapes...
				foreach (Shape shape in diagramPresenter.SelectedShapes) {
					if (!diagramPresenter.IsLayerVisible(shape.Layers)) continue;
					AddPreview(shape, shape.Type.CreatePreviewInstance(shape), diagramPresenter.DisplayService);
				}
				// ...then restore connections between previews and connections between previews and non-selected shapes
				targetShapeBuffer.Clear();
				foreach (Shape selectedShape in diagramPresenter.SelectedShapes.BottomUp) {
					if (!diagramPresenter.IsLayerVisible(selectedShape.Layers)) continue;
					// AttachGluePointToConnectionPoint the preview shape (and all it's cildren) to all the shapes the original shape was connected to
					// Additionally, create previews for all connected shapes and connect these to the appropriate target shapes
					ConnectPreviewOfShape(diagramPresenter, selectedShape);
				}
				targetShapeBuffer.Clear();
			}
		}


		private void ResetPreviewShapes(IDiagramPresenter diagramPresenter)
		{
			// Dictionary "originalShapes" contains the previewShape as "Key" and the original shape as "Value"
			foreach (KeyValuePair<Shape, Shape> pair in originalShapes) {
				// Copy all properties of the original shape to the preview shape 
				// (assigns the original model object to the preview shape!)
				IModelObject modelObj = pair.Value.ModelObject;
				pair.Key.CopyFrom(pair.Value);
				// If the original shape has a model object, clone and assign it.
				// Then, transform the copied shape into a preview shape
				pair.Key.ModelObject = null;
				pair.Key.MakePreview(diagramPresenter.Project.Design);
				if (modelObj != null) pair.Key.ModelObject = modelObj.Clone();
			}
		}


		private void InvalidateShapes(IDiagramPresenter diagramPresenter, IEnumerable<Shape> shapes, bool invalidateGrips)
		{
			foreach (Shape shape in shapes)
				DoInvalidateShape(diagramPresenter, shape, invalidateGrips);
		}


		private void DoInvalidateShape(IDiagramPresenter diagramPresenter, Shape shape, bool invalidateGrips)
		{
			if (shape.Parent != null)
				DoInvalidateShape(diagramPresenter, shape.Parent, false);
			else {
				shape.Invalidate();
				if (invalidateGrips)
					diagramPresenter.InvalidateGrips(shape, ControlPointCapabilities.All);
			}
		}


		private bool IsDragActionFeasible(MouseState mouseState)
		{
			int dx = 0, dy = 0;
			if (IsToolActionPending) {
				// Check the minimum drag distance before switching to a drag action
				dx = Math.Abs(mouseState.X - ActionStartMouseState.X);
				dy = Math.Abs(mouseState.Y - ActionStartMouseState.Y);
Console.WriteLine("IsDragActionFeasible: tool action is pending");
			}
Console.WriteLine("IsDragActionFeasible: dx is " + dx + " and dy is " + dy);
			return (dx >= MinimumDragDistance.Width || dy >= MinimumDragDistance.Height);
		}


		private bool IsMoveShapeFeasible(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo)
		{
			if (shapeAtCursorInfo.IsEmpty || mouseState.IsEmpty)
				return false;
			if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, shapeAtCursorInfo.Shape))
				return false;
			if (diagramPresenter.SelectedShapes.Count > 0 && !diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, diagramPresenter.SelectedShapes))
				return false;
			if (!shapeAtCursorInfo.Shape.ContainsPoint(mouseState.X, mouseState.Y))
				return false;
			if (shapeAtCursorInfo.Shape.HasControlPointCapability(ControlPointId.Reference, ControlPointCapabilities.Glue))
				if (shapeAtCursorInfo.Shape.IsConnected(ControlPointId.Reference, null) != ControlPointId.None) return false;

			// Check if the shape is connected to other shapes with its glue points
			if (diagramPresenter.SelectedShapes.Count == 0) {
				// Check if the non-selected shape at cursor (which will be selected) owns glue points connected to other shapes
				foreach (ControlPointId id in shapeAtCursorInfo.Shape.GetControlPointIds(ControlPointCapabilities.Glue))
					if (shapeAtCursorInfo.Shape.IsConnected(id, null) != ControlPointId.None) return false;
			} else if (diagramPresenter.SelectedShapes.Contains(shapeAtCursorInfo.Shape)) {
				// ToDo: If there are *many* shapes selected (e.g. 10000), this check will be extremly slow...
				if (diagramPresenter.SelectedShapes.Count < 10000) {
					// LinearShapes that own connected gluePoints may not be moved.
					foreach (Shape shape in diagramPresenter.SelectedShapes) {
						if (shape is ILinearShape) {
							foreach (ControlPointId gluePointId in shape.GetControlPointIds(ControlPointCapabilities.Glue)) {
								ShapeConnectionInfo sci = shape.GetConnectionInfo(gluePointId, null);
								if (!sci.IsEmpty) {
									// Allow movement if the connected shapes are moved together
									if (!diagramPresenter.SelectedShapes.Contains(sci.OtherShape))
										return false;
								}
							}
						}
					}
				}
			}
			return true;
		}

		private bool IsConnectFromShapeFeasible(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo)
		{
			if (shapeAtCursorInfo.IsEmpty || mouseState.IsEmpty)
				return false;
			if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Connect, shapeAtCursorInfo.Shape))
				return false;

			if (shapeAtCursorInfo.Shape is FilterSetupShapeBase) {
				FilterSetupShapeBase filterShape = shapeAtCursorInfo.Shape as FilterSetupShapeBase;
				// only check OUTPUT count; we want to enforce the 'drag from OUTPUT, to INPUT' paradigm
				if (filterShape.OutputCount > 0)
					return true;
			}

			return false;
		}


		private bool IsMoveHandleFeasible(IDiagramPresenter diagramPresenter, MouseState mouseState, ShapeAtCursorInfo shapeAtCursorInfo)
		{
			if (shapeAtCursorInfo.IsEmpty)
				return false;
			// Collides with the resize modifiers
			//if (mouseState.IsKeyPressed(KeysDg.Shift)) return false;
			if (shapeAtCursorInfo.Shape.HasControlPointCapability(shapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Glue)) {
				if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Connect, diagramPresenter.SelectedShapes))
					return false;
			} else {
				if (!diagramPresenter.Project.SecurityManager.IsGranted(Permission.Layout, diagramPresenter.SelectedShapes))
					return false;
			}
			if (!shapeAtCursorInfo.Shape.HasControlPointCapability(shapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Resize | ControlPointCapabilities.Glue /*| ControlPointCapabilities.Movable*/))
				return false;
			if (diagramPresenter.SelectedShapes.Count > 1) {
				// GluePoints may only be moved alone
				if (shapeAtCursorInfo.Shape.HasControlPointCapability(shapeAtCursorInfo.ControlPointId, ControlPointCapabilities.Glue))
					return false;
				// Check if all shapes that are going to be resizes are of the same type
				Shape lastShape = null;
				foreach (Shape shape in diagramPresenter.SelectedShapes) {
					if (lastShape != null && lastShape.Type != shape.Type)
						return false;
					lastShape = shape;
				}
			}
			return true;
		}

		#endregion


		#region [Private] Construction

		static ConnectionTool()
		{
			cursors = new Dictionary<ToolCursor, int>(8);
			// Register cursors
			cursors.Add(ToolCursor.Default, CursorProvider.DefaultCursorID);
			cursors.Add(ToolCursor.MoveShape, CursorProvider.RegisterCursor(Properties.Resources.MoveShapeCursor));
			cursors.Add(ToolCursor.MoveHandle, CursorProvider.RegisterCursor(Properties.Resources.MovePointCursor));
			cursors.Add(ToolCursor.Connect, CursorProvider.RegisterCursor(Properties.Resources.HandCursor));
		}


		private void Construct()
		{
			Title = "Pointer";
			ToolTipText = "Select one or more objects by holding shift while clicking or drawing a frame."
				+ Environment.NewLine
				+ "Selected objects can be moved by dragging them to the target position or resized by dragging "
				+ "a control point to the target position.";

			//SmallIcon = global::Dataweb.NShape.Properties.Resources.PointerIconSmall;
			//SmallIcon.MakeTransparent(Color.Fuchsia);

			//LargeIcon = global::Dataweb.NShape.Properties.Resources.PointerIconLarge;
			//LargeIcon.MakeTransparent(Color.Fuchsia);

			frameRect = Rectangle.Empty;
		}

		#endregion


		#region [Private] Types

		private enum Action
		{
			None,
			Select,
			SelectWithFrame,
			MoveShape,
			MoveHandle,
			ConnectShapes,
		}


		private enum ToolCursor
		{
			Default,
			MoveHandle,
			MoveShape,
			Connect,
		}


		// connection handling stuff
		private struct ConnectionInfoBuffer : IEquatable<ConnectionInfoBuffer>
		{

			public static readonly ConnectionInfoBuffer Empty;

			public static bool operator ==(ConnectionInfoBuffer x, ConnectionInfoBuffer y) { return (x.connectionInfo == y.connectionInfo && x.shape == y.shape); }

			public static bool operator !=(ConnectionInfoBuffer x, ConnectionInfoBuffer y) { return !(x == y); }

			public Shape shape;

			public ShapeConnectionInfo connectionInfo;

			/// <override></override>
			public override bool Equals(object obj) { return obj is ConnectionInfoBuffer && this == (ConnectionInfoBuffer)obj; }

			/// <ToBeCompleted></ToBeCompleted>
			public bool Equals(ConnectionInfoBuffer other)
			{
				return other == this;
			}

			/// <override></override>
			public override int GetHashCode() { return base.GetHashCode(); }

			static ConnectionInfoBuffer()
			{
				Empty.shape = null;
				Empty.connectionInfo = ShapeConnectionInfo.Empty;
			}
		}

		#endregion


		#region Fields

		// --- Description of the tool ---
		private static Dictionary<ToolCursor, int> cursors;
		//
		private ControlPointCapabilities gripCapabilities = ControlPointCapabilities.Resize | ControlPointCapabilities.Rotate /*| ControlPointCapabilities.Movable*/;

		// --- State after the last ProcessMouseEvent ---
		// selected shape under the mouse cursor, being highlighted in the next drawing
		private ShapeAtCursorInfo selectedShapeAtCursorInfo;
		// Rectangle that represents the transformed selection area in control coordinates
		private Rectangle frameRect;
		// Stores the distance the SelectedShape was moved on X-axis for snapping the nearest gridpoint
		private int snapDeltaX;
		// Stores the distance the SelectedShape was moved on Y-axis for snapping the nearest gridpoint
		private int snapDeltaY;
		// Index of the controlPoint that snapped to grid/point/swimline
		private int snapPtId;

		// preview shapes (Key = original shape, Value = preview shape)
		private Dictionary<Shape, Shape> previewShapes = new Dictionary<Shape, Shape>();
		// original shapes (Key = preview shape, Value = original shape)
		private Dictionary<Shape, Shape> originalShapes = new Dictionary<Shape, Shape>();

		private DataFlowConnectionLine currentConnectionLine;


		// Buffers
		// used for buffering selectedShapes connected to the preview selectedShapes: key = passiveShape, values = targetShapes's clone
		private Dictionary<Shape, Shape> targetShapeBuffer = new Dictionary<Shape, Shape>();

		#endregion
	}





}
