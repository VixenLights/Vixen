#nullable enable
using System.Collections.ObjectModel;

namespace Vixen.Sys.Props
{
	public interface IPropNode<T, out Z> where Z : IProp where T : IPropNode<T, Z>
	{
		Guid Id { get; }

		string Name { get; set; }
		
		Z? Prop { get; }

		ObservableCollection<T> Parents { get; }

		ObservableCollection<T> Children { get; }

		bool IsProp { get; }
		
		bool IsGroupNode { get; }

		bool IsRootNode { get; }

		IEnumerable<T> GetIPropNodeLeafEnumerator();
	}
}