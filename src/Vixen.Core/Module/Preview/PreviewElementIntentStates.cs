using Vixen.Sys;

namespace Vixen.Preview
{
	public class PreviewElementIntentStates : Dictionary<Element, IIntentStates>
	{
		public PreviewElementIntentStates()
		{
		}

		public PreviewElementIntentStates(IDictionary<Element, IIntentStates> values)
			: base(values)
		{
		}
	}
}