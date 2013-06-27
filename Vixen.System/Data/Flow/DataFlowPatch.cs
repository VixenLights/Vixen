using System;

namespace Vixen.Data.Flow
{
	public class DataFlowPatch
	{
		public DataFlowPatch(Guid componentId, Guid? sourceComponentId, int sourceComponentOutputIndex)
		{
			ComponentId = componentId;
			SourceComponentId = sourceComponentId;
			SourceComponentOutputIndex = sourceComponentOutputIndex;
		}

		public DataFlowPatch(IDataFlowComponent component)
		{
			if (component == null) throw new ArgumentNullException("component");

			ComponentId = component.DataFlowComponentId;
			if (component.Source != null) {
				SourceComponentId = component.Source.Component.DataFlowComponentId;
				SourceComponentOutputIndex = component.Source.OutputIndex;
			}
		}

		public Guid ComponentId { get; private set; }
		public Guid? SourceComponentId { get; private set; }
		public int SourceComponentOutputIndex { get; private set; }
	}

	//public class DataFlowPatch {
	//    public DataFlowPatch(Guid componentId, IDataFlowComponentReference componentSource) {
	//        ComponentId = componentId;
	//        Source = componentSource;
	//    }

	//    public DataFlowPatch(IDataFlowComponent component) {
	//        if(component == null) throw new ArgumentNullException("component");

	//        ComponentId = component.DataFlowComponentId;
	//        if(component.Source != null) {
	//            Source = component.Source;
	//        }
	//    }

	//    public Guid ComponentId { get; private set; }
	//    public IDataFlowComponentReference Source { get; private set; }
	//}
}