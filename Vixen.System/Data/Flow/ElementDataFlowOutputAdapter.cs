using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Data.Flow
{
	internal class ElementDataFlowOutputAdapter : IDataFlowOutput<IntentsDataFlowData>
	{
		private readonly Element _element;
		private readonly IntentsDataFlowData _data = new IntentsDataFlowData(Enumerable.Empty<IIntentState>().ToList());

		public ElementDataFlowOutputAdapter(Element element)
		{
			_element = element;
		}

		public IntentsDataFlowData Data
		{
			get
			{
				_data.Value = _element.State.AsList();
				return _data;
			}
		}

		public string Name
		{
			get { return _element.Name; }
		}

		IDataFlowData IDataFlowOutput.Data
		{
			get { return Data; }
		}
	}
}