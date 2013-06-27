using System;
using Vixen.Sys;

namespace Vixen.Data.Flow
{
	/// <summary>
	/// Facilitates allowing elements to participate in the data flow system.
	/// </summary>
	internal class ElementDataFlowAdapter : IDataFlowComponent<IntentsDataFlowData>
	{
		private Element _element;
		private IDataFlowOutput<IntentsDataFlowData>[] _outputs;

		public ElementDataFlowAdapter(Element element)
		{
			_element = element;
		}

		public IDataFlowOutput<IntentsDataFlowData>[] Outputs
		{
			get
			{
				if (_outputs == null) {
					_outputs = new[] {new ElementDataFlowOutputAdapter(_element)};
				}
				return _outputs;
			}
		}

		IDataFlowOutput[] IDataFlowComponent.Outputs
		{
			get { return Outputs; }
		}

		public IDataFlowComponentReference<IntentsDataFlowData> Source
		{
			get
			{
				// No input data type, so this is meaningless.
				return null;
			}
			set
			{
				/* Can't set this for a element (in its role as a data flow participant) */
			}
		}

		IDataFlowComponentReference IDataFlowComponent.Source
		{
			get { return Source; }
			set { Source = (IDataFlowComponentReference<IntentsDataFlowData>) value; }
		}

		public DataFlowType InputDataType
		{
			get { return DataFlowType.None; }
		}

		public DataFlowType OutputDataType
		{
			get { return DataFlowType.MultipleIntents; }
		}

		public Guid DataFlowComponentId
		{
			get { return _element.Id; }
		}

		public string Name
		{
			get { return _element.Name; }
		}
	}
}