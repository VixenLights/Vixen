#nullable disable

using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Vixen.Data.Flow;
using Vixen.Sys;

namespace VixenApplication.GraphicalPatching;

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
		if (source is ElementNodeShape)
		{
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

	public static ElementNodeShape CreateInstance(ShapeType shapeType, Template template)
	{
		return new ElementNodeShape(shapeType, template);
	}

	private ElementNode _node;

	public ElementNode Node
	{
		get { return _node; }
	}

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