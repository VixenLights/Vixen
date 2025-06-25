using System.Collections.ObjectModel;

namespace Vixen.Sys.Props.Components
{
	public interface IPropComponentNode<T, out Z> where Z : IPropComponent where T : IPropComponentNode<T, Z>
	{
		Guid Id { get; }

		string Name { get; set; }

		Z? PropComponent { get; }

		ObservableCollection<T> Parents { get; }

		ObservableCollection<T> Children { get; }

		bool IsPropComponent { get; }

		bool IsGroupNode { get; }

		bool IsRootNode { get; }

		IEnumerable<T> GetIPropComponentNodeLeafEnumerator();
	}
}