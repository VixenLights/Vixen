#nullable disable

using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace VixenApplication.GraphicalPatching;

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

	public static ControllerShape CreateInstance(ShapeType shapeType, Template template)
	{
		return new ControllerShape(shapeType, template);
	}

	public override IDataFlowComponent DataFlowComponent
	{
		get { return null; }
	}

	public IOutputDevice Controller { get; set; }
}