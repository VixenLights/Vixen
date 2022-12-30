#nullable disable

using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.GeneralShapes;
using Vixen.Data.Flow;

namespace VixenApplication.GraphicalPatching;

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
		if (source is DataFlowConnectionLine)
		{
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

	public static DataFlowConnectionLine CreateInstance(ShapeType shapeType, Template template)
	{
		return new DataFlowConnectionLine(shapeType, template);
	}

	public IDataFlowComponentReference SourceDataFlowComponentReference { get; set; }

	public IDataFlowComponent DestinationDataComponent { get; set; }
}