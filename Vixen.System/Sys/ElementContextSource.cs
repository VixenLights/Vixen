using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys
{
	internal class ElementContextSource //: IStateSourceCollectionAdapter<Guid, IEnumerable<IIntentState>>
	{
		public ElementContextSource(Guid elementId)
		{
			Key = elementId;
		}

		public Guid Key { get; set; }

		//public IEnumerator<IStateSource<IEnumerable<IIntentState>>> GetEnumerator()
		//{
		//	return VixenSystem.Contexts.Select(x => x.GetState(Key)).Where(x => x != null).GetEnumerator();
		//}

		//IEnumerator IEnumerable.GetEnumerator()
		//{
		//	return GetEnumerator();
		//}
	}
}