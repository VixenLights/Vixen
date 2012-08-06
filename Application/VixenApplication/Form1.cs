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
using Vixen.Sys;

namespace VixenApplication
{
	public partial class Form1 : Form
	{
		private Project myproject;
		private DiagramSetController dsc;

		private Shape secondlastshape;
		private Shape lastshape;

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

			diagramDisplay.CurrentTool = new SelectionTool();
			diagramDisplay.ShowDefaultContextMenu = false;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			EllipseBase ellipse = (EllipseBase)myproject.ShapeTypes["Ellipse"].CreateInstance();
			//d.Shapes.Add(ellipse);
			diagramDisplay.Diagram.Shapes.Add(ellipse);
			//repository.Insert(ellipse, display1.Diagram);

			secondlastshape = lastshape;
			lastshape = ellipse;
			
			

			diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)ellipse, diagramDisplay.Diagram);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (diagramDisplay.Diagram.Shapes.Count < 2)
				return;

			Polyline line = (Polyline)myproject.ShapeTypes["Polyline"].CreateInstance();
			diagramDisplay.Diagram.Shapes.Add(line);
			line.EndCapStyle = myproject.Design.CapStyles.ClosedArrow;

			line.Connect(ControlPointId.FirstVertex, secondlastshape, ControlPointId.Reference);
			line.Connect(ControlPointId.LastVertex, lastshape, ControlPointId.Reference);
			
		}

		private void button4_Click(object sender, EventArgs e)
		{
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions('A', StandardRole.Guest, Permission.Connect);
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).CurrentRole = StandardRole.Guest;

			//display1.Diagram = d;


			//if (display1.DiagramSetController.Project.Repository.GetDiagram("qwer"))
			//display1.DiagramSetController.Project.Repository.Update(display1.Diagram);
			//display1.DiagramSetController.Project.Repository.InsertAll(display1.Diagram);
			//display1.DiagramSetController.Project.Repository.Update(display1.Diagram);

		}

		private void display1_ShapeClick(object sender, DiagramPresenterShapeClickEventArgs e)
		{
			log("shape click: " + e.Shape.Type);
		}

		private void display1_ShapeDoubleClick(object sender, DiagramPresenterShapeClickEventArgs e)
		{
			log("shape doubleclick: " + e.Shape.Type);
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
			//int nodeXOffset = 30;
			int nodeYOffset = SHAPE_CHANNELS_Y_TOP;

			foreach (var node in VixenSystem.Nodes.GetRootNodes())
			{
				FilterShape shape = MakeChannelNodeShape(node, SHAPE_CHANNELS_WIDTH, nodeYOffset, 1);
				nodeYOffset += shape.Height + SHAPE_CHANNELS_SPACING;

				//diagramDisplay.Diagram.Shapes.Add(shape);
				//diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)shape, diagramDisplay.Diagram);

				/*
				FilterShape nodebox = (FilterShape)myproject.ShapeTypes["FilterShape"].CreateInstance();
				nodebox.SetCaptionText(0, node.Name);
				diagramDisplay.Diagram.Shapes.Add(nodebox);
				diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)nodebox, diagramDisplay.Diagram);

				foreach (var child in node.Children)
				{
					FilterShape childbox = (FilterShape)myproject.ShapeTypes["FilterShape"].CreateInstance();
					childbox.SetCaptionText(0, child.Name);
					diagramDisplay.Diagram.Shapes.Add(childbox);
					diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)childbox, diagramDisplay.Diagram);				
				}


				nodebox.Width = 200;
				nodebox.Height = 30;

				nodebox.X = nodeXOffset + nodebox.Width / 2;
				nodebox.Y = nodeYOffset + nodebox.Height / 2;

				nodeYOffset += nodebox.Height + 10;
				 */

			}
		}

		private void button6_Click(object sender, EventArgs e)
		{

		}

		private void button7_Click(object sender, EventArgs e)
		{

		}

		private FilterShape MakeChannelNodeShape(ChannelNode node, int width, int yTop, int zOrder)
		{
			FilterShape shape = (FilterShape)myproject.ShapeTypes["FilterShape"].CreateInstance();
			shape.Node = node;
			diagramDisplay.Diagram.Shapes.Add(shape, zOrder);
			diagramDisplay.DiagramSetController.Project.Repository.InsertAll((Shape)shape, diagramDisplay.Diagram);

			if (node.Children.Count() > 0) {
				int currentYTop = yTop + SHAPE_CHANNELS_GROUP_HEADER_HEIGHT;

				foreach (var child in node.Children) {
					FilterShape childShape = MakeChannelNodeShape(child, width - SHAPE_CHILD_WIDTH_REDUCTION, currentYTop, zOrder + 1);
					currentYTop += childShape.Height + SHAPE_CHANNELS_SPACING;
					shape.ChildFilterShapes.Add(childShape);
					//shape.Children.Add(childShape, zOrder + 1);
				}

				shape.Width = width;
				shape.Height = (currentYTop - SHAPE_CHANNELS_SPACING + SHAPE_CHANNELS_GROUP_FOOTER_HEIGHT) - yTop;
			}
			else {
				shape.Width = width;
				shape.Height = SHAPE_CHANNELS_HEIGHT;
				shape.OutputCount = 1;
			}
			shape.X = SHAPE_CHANNELS_X_LOCATION;
			shape.Y = yTop + shape.Height / 2;

			return shape;
		}

		// the central X point of all channel shapes
		internal const int SHAPE_CHANNELS_X_LOCATION = 100;
		// the starting top of all channel shapes
		internal const int SHAPE_CHANNELS_Y_TOP = 30;
		// the (base) width of all channels (inner children will be smaller)
		internal const int SHAPE_CHANNELS_WIDTH = 160;
		// the height of all channel shapes
		internal const int SHAPE_CHANNELS_HEIGHT = 32;
		// the vertical spacing between channels
		internal const int SHAPE_CHANNELS_SPACING = 10;
		// how much the width of inner children is reduced
		internal const int SHAPE_CHILD_WIDTH_REDUCTION = 16;
		// how much of a parent shape should be reserved/kept for the wrapping above/below
		internal const int SHAPE_CHANNELS_GROUP_HEADER_HEIGHT = 32;
		internal const int SHAPE_CHANNELS_GROUP_FOOTER_HEIGHT = 8;


		internal const int SHAPE_OUTPUTS_X_LOCATION = 500;

	}





	public class FilterShape : RoundedBox
	{
		private void _init()
		{
			_inputCount = 0;
			_outputCount = 0;
			ChildFilterShapes = new List<FilterShape>();
			_font = new Font("Arial", 14, GraphicsUnit.Pixel);
			_textBrush = new SolidBrush(Color.Black);
		}

		public FilterShape(ShapeType shapeType, Template template) : base(shapeType, template)
		{
			_init();
		}

		public FilterShape(ShapeType shapeType, IStyleSet styleSet) : base(shapeType, styleSet)
		{
			_init();
		}

		public override Shape Clone()
		{
			FilterShape result = new FilterShape(Type, (Template)null);
			result.CopyFrom(this);
			result.InputCount = InputCount;
			result.OutputCount = OutputCount;
			result.ChildFilterShapes = new List<FilterShape>(ChildFilterShapes);
			result.Node = Node;
			return result;
		}

		static public FilterShape CreateInstance(ShapeType shapeType, Template template)
		{
			return new FilterShape(shapeType, template);
		}


		private void _recalcControlPoints()
		{
			controlPoints = new Point[ControlPointCount];
			CalcControlPoints();			
		}

		private int _inputCount;
		public int InputCount
		{
			get { return _inputCount; }
			set { _inputCount = value; _recalcControlPoints(); }
		}

		private int _outputCount;
		public int OutputCount
		{
			get { return _outputCount; }
			set { _outputCount = value; _recalcControlPoints(); }
		}

		public List<FilterShape> ChildFilterShapes { get; private set; }
		public ChannelNode Node { get; set; }
		private Font _font { get; set; }
		private Brush _textBrush { get; set; }


		public override void Draw(Graphics graphics)
		{
			base.Draw(graphics);

			SizeF stringSize = graphics.MeasureString(Node.Name, _font);
			float x = X - (stringSize.Width / 2f);
			float y = Y - (Height / 2f);

			if (ChildFilterShapes.Count > 0)
				y += (Form1.SHAPE_CHANNELS_GROUP_HEADER_HEIGHT - stringSize.Height) / 2f;
			else
				y += (Height - stringSize.Height) / 2f;

			graphics.DrawString(Node.Name, _font, _textBrush, x, y);
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
				return ((controlPointCapability & ControlPointCapabilities.Reference) > 0);
			}

			// default to any other control points not having any capabilities (shouldn't be any left, really)
			return false;
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


	public static class NShapeLibraryInitializer
	{
		public static void Initialize(IRegistrar registrar)
		{
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);
			registrar.RegisterShapeType(
				new ShapeType("FilterShape", namespaceName, namespaceName, FilterShape.CreateInstance, FilterShape.GetPropertyDefinitions)
			);
		}

		private const string namespaceName = "VixenFilterShapes";
		private const int preferredRepositoryVersion = 3;
	}
}
