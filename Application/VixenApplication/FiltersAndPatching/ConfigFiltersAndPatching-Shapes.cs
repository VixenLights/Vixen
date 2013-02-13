using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.GeneralShapes;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication
{
	public abstract class FilterSetupShapeBase : RoundedBox
	{
		protected virtual void _init()
		{
			_recalcControlPoints();
		}

		protected FilterSetupShapeBase(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}

		protected FilterSetupShapeBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is FilterSetupShapeBase) {
				FilterSetupShapeBase src = (FilterSetupShapeBase)source;
			}
		}

		// This is a horrible abomination of a function and purpose. When a shape is duplicated, the control
		// point positions aren't properly copied by the base classes. When we copy the type specific data
		// which gives the DataFlowComponent (in subclasses), the control points are regenerated. However,
		// the problem is that they are generated at a 0-offset, rather than the position-relative offset.
		// So, the easiest way to fix it is just to copy the correct positions from the source shape to update
		// them correctly. Nasty.
		//
		// (Ideally, we would have the NShape system properly transform the points when it's used; but I can't
		// figure out at what point the generated points get transformed from shape-relative points into the
		// absolute position points).
		//
		// (Because these points are (incorrectly) generated when copying the DataFlowComponent in subclasses,
		// this needs to be called AFTER all the other copying is done, so will need to be called in each CopyFrom.
		protected void _CopyControlPointsFrom(FilterSetupShapeBase source)
		{
			controlPoints = new Point[ControlPointCount];
			for (int i = 0; i < source.ControlPoints.Length; i++) {
				controlPoints[i] = source.ControlPoints[i];
			}
		}

		protected void _recalcControlPoints()
		{
			controlPoints = new Point[ControlPointCount];
			// updating the draw cache will call CalcControlPoints, and also translate them appropriately for the shape position.
			InvalidateDrawCache();
			UpdateDrawCache();
		}

		public void ModuleDataUpdated()
		{
			_recalcControlPoints();
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
			_DrawTitle(graphics, 0, 0, true, true);
		}

		// x and y offsets are from the top left of the shape. Text will be centered on the X/Y offset. If center[xy] are true, the offsets are ignored.
		protected void _DrawTitle(Graphics graphics, float xOffset, float yOffset, bool centerX, bool centerY)
		{
			// resize the font smaller until it fits in 90% of the shape width (or until it hits 6 pixels in size)
			SizeF stringSize = graphics.MeasureString(Title, _font);

			while (stringSize.Width > (Width * 0.9) && (_font.Size > 6)) {
				float newSize = (float)(_font.Size * 0.9);
				_font = new Font("Arial", newSize, GraphicsUnit.Pixel);
				stringSize = graphics.MeasureString(Title, _font);
			}

			float x;
			if (centerX) {
				x = X - (stringSize.Width / 2f);
			} else {
				x = X - (Width / 2f) + xOffset - (stringSize.Width / 2f);
			}

			float y;
			if (centerY) {
				y = Y - (stringSize.Height / 2f);
			} else {
				y = Y - (Height / 2f) + yOffset - (stringSize.Height / 2f);
			}

			graphics.DrawString(Title, _font, _textBrush, x, y);
		}

		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability)
		{
			if (controlPointId == ControlPointId.None || controlPointId == ControlPointId.Any)
				return false;

			// all control points have at least the 'none' capability. Hopefully, they might even have more...
			if (controlPointCapability == ControlPointCapabilities.None)
				return true;

			int index = GetControlPointIndex(controlPointId);
			if (index > 0 && index <= InputCount + OutputCount) {
				return ((controlPointCapability & ControlPointCapabilities.Connect) > 0 ||
						(controlPointCapability & ControlPointCapabilities.Resize) > 0);
			}

			if (controlPointId == ControlPointId.Reference || index == 0) {
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

			for (int i = 0; i < InputCount; i++) {
				ControlPoints[offset + i].X = left;
				ControlPoints[offset + i].Y = (int)Math.Round((-Height / 2f) + Height * GetProportionalDistanceForPoint(i, InputCount));
			}

			offset += InputCount;

			for (int i = 0; i < OutputCount; i++) {
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
			if (ChildFilterShapes.Count == 0) {
				base.DrawCustom(graphics);
				return;
			}

			float yoffset = (ConfigFiltersAndPatching.SHAPE_GROUP_HEADER_HEIGHT) / 2f;
			_DrawTitle(graphics, 0, yoffset, true, false);
		}
	}


	public class ElementNodeShape : NestingSetupShape
	{
		public ElementNodeShape(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			_init();
		}

		public ElementNodeShape(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			_init();
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is ElementNodeShape) {
				ElementNodeShape src = (ElementNodeShape)source;
				_node = src.Node;
				_CopyControlPointsFrom(src);
			}
		}

		public override Shape Clone()
		{
			ElementNodeShape result = new ElementNodeShape(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}

		static public ElementNodeShape CreateInstance(ShapeType shapeType, Template template)
		{
			return new ElementNodeShape(shapeType, template);
		}

		private ElementNode _node;
		public ElementNode Node { get { return _node; } }

		public void SetElementNode(ElementNode node)
		{
			_node = node;
			_recalcControlPoints();
		}

		public override IDataFlowComponent DataFlowComponent
		{
			get
			{
				if (Node == null || Node.Element == null)
					return null;
				return VixenSystem.Elements.GetDataFlowComponentForElement(Node.Element);
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
				_controller = src.Controller;
				_output = src.Output;
				_CopyControlPointsFrom(src);
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
		public CommandOutput Output { get { return _output; } }

		public void SetOutput(CommandOutput output)
		{
			_output = output;
			_recalcControlPoints();
		}

		private OutputController _controller;
		public OutputController Controller { get { return _controller; } }

		public void SetController(OutputController controller)
		{
			_controller = controller;
			_recalcControlPoints();
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


	public class FilterShape : FilterSetupShapeBase
	{
		public FilterShape(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			_init();
		}

		public FilterShape(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			_init();
		}

		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is FilterShape) {
				FilterShape src = (FilterShape)source;
				_filterInstance = src.FilterInstance;
				_CopyControlPointsFrom(src);
			}
		}

		public override Shape Clone()
		{
			FilterShape result = new FilterShape(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}

		static public FilterShape CreateInstance(ShapeType shapeType, Template template)
		{
			return new FilterShape(shapeType, template);
		}

		private IOutputFilterModuleInstance _filterInstance;
		public IOutputFilterModuleInstance FilterInstance { get { return _filterInstance; } }

		public void SetFilterInstance(IOutputFilterModuleInstance filterInstance)
		{
			_filterInstance = filterInstance;
			_recalcControlPoints();
		}

		public override IDataFlowComponent DataFlowComponent
		{
			get
			{
				if (FilterInstance == null)
					return null;
				return FilterInstance;
			}
		}

		protected override bool ReferenceControlPointHasCapability(ControlPointCapabilities controlPointCapability)
		{
			return (controlPointCapability & ControlPointCapabilities.Reference) > 0 ||
				(controlPointCapability & ControlPointCapabilities.Connect) > 0;
		}

		public bool RunSetup()
		{
			bool result = false;
			if (FilterInstance.HasSetup) {
				result = FilterInstance.Setup();
				if (result)
					_recalcControlPoints();
			}
			return result;
		}
	}


	public class DataFlowConnectionLine : Polyline
	{
		protected internal DataFlowConnectionLine(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}

		protected internal DataFlowConnectionLine(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
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
				new ShapeType("ElementNodeShape", namespaceName, namespaceName,
					ElementNodeShape.CreateInstance, ElementNodeShape.GetPropertyDefinitions)
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
				new ShapeType("FilterShape", namespaceName, namespaceName,
					FilterShape.CreateInstance, FilterShape.GetPropertyDefinitions)
				);
			registrar.RegisterShapeType(
				new ShapeType("DataFlowConnectionLine", namespaceName, namespaceName,
					DataFlowConnectionLine.CreateInstance, DataFlowConnectionLine.GetPropertyDefinitions)
				);
		}

		private const string namespaceName = "VixenFilterShapes";
		private const int preferredRepositoryVersion = 3;
	}
}
