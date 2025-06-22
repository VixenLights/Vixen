#nullable enable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Vixen.Sys.Props.Components
{
	public interface IPropComponent: INotifyPropertyChanged
	{
		Guid Id { get; }
		
		string Name { get; set; }
		
		PropComponentType ComponentType { get; }
		
		IEnumerable<IElementNode> TargetNodes { get; }
		
		void AddElementNodes(IEnumerable<IElementNode> nodes);

		bool TryAdd(IElementNode node);

		bool TryRemove(Guid id, [MaybeNullWhen(false)]out IElementNode node);

		bool TryGet(Guid id, [MaybeNullWhen(false)] out IElementNode node);

		void Clear();
		
		bool IsUserDefined { get; }

	}
}