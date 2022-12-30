#nullable disable

using Dataweb.NShape;
using Dataweb.NShape.Advanced;

namespace VixenApplication.GraphicalPatching;

public abstract class NestingSetupShape : FilterSetupShapeBase
{
	// how much of a parent shape should be reserved/kept for the wrapping above/below
	internal const int SHAPE_GROUP_HEADER_HEIGHT = 32;
	internal const int SHAPE_GROUP_FOOTER_HEIGHT = 8;

	protected override void _init()
	{
		base._init();
		ChildFilterShapes = new List<FilterSetupShapeBase>();
		Expanded = true;
		HeaderHeight = SHAPE_GROUP_HEADER_HEIGHT;
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
			NestingSetupShape src = (NestingSetupShape) source;
			ChildFilterShapes = new List<FilterSetupShapeBase>(src.ChildFilterShapes);
		}
	}

	public List<FilterSetupShapeBase> ChildFilterShapes { get; private set; }

	public bool Expanded { get; set; }

	public int HeaderHeight { get; set; }

	public override void DrawCustom(Graphics graphics)
	{
		if (ChildFilterShapes.Count == 0) {
			base.DrawCustom(graphics);
			return;
		}

		float yoffset = HeaderHeight/2f;
		_DrawTitle(graphics, 0, yoffset, true, false);
	}
}