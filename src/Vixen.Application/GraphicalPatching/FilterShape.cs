#nullable disable

using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;

namespace VixenApplication.GraphicalPatching;

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
			FilterShape src = (FilterShape) source;
			_filterInstance = src.FilterInstance;
			_CopyControlPointsFrom(src);
		}
	}

	public override Shape Clone()
	{
		FilterShape result = new FilterShape(Type, (Template) null);
		result.CopyFrom(this);
		return result;
	}

	public static FilterShape CreateInstance(ShapeType shapeType, Template template)
	{
		return new FilterShape(shapeType, template);
	}

	private IOutputFilterModuleInstance _filterInstance;

	public IOutputFilterModuleInstance FilterInstance
	{
		get { return _filterInstance; }
	}

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

	// The number of layers this filter is from its source element (by tracing parents back).  If it can't
	// find one, the value will be 0.
	public int LevelsFromElementSource { get; set; }
}