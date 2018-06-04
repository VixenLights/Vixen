using System.Collections.ObjectModel;
using Vixen.Marks;

namespace Vixen.Sys
{
	public interface IHasMarks
	{
		ObservableCollection<IMarkCollection> LabeledMarkCollections { get; }
	}
}
