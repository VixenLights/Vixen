#nullable disable

using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace VixenApplication.GraphicalPatching;

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
			OutputShape src = (OutputShape) source;
			_controller = src.Controller;
			_output = src.Output;
			_CopyControlPointsFrom(src);
		}
	}

	public override Shape Clone()
	{
		OutputShape result = new OutputShape(Type, (Template) null);
		result.CopyFrom(this);
		return result;
	}

	public static OutputShape CreateInstance(ShapeType shapeType, Template template)
	{
		return new OutputShape(shapeType, template);
	}

	private CommandOutput _output;
	public CommandOutput Output
	{
		get { return _output; }
	}

	private int _outputIndex;
	public int OutputIndex
	{
		get { return _outputIndex; }
	}

	public void SetOutput(CommandOutput output, int outputIndex)
	{
		_output = output;
		_outputIndex = outputIndex;
		_recalcControlPoints();
	}

	private OutputController _controller;

	public OutputController Controller
	{
		get { return _controller; }
	}

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