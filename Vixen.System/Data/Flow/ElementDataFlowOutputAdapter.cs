using Vixen.Sys;

namespace Vixen.Data.Flow
{
	internal class ElementDataFlowOutputAdapter : IDataFlowOutput<IntentsDataFlowData>
	{
		private Element _element;

		public ElementDataFlowOutputAdapter(Element element)
		{
			_element = element;
		}

		public IntentsDataFlowData Data
		{
			get { return new IntentsDataFlowData(_element.State); }
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