using Vixen.Sys;

namespace Vixen.Intent.IntersectionStrategy {
	class ReplaceWithBase : IIntersectionStrategy<IIntentNode> {
		public IIntentNode GetIntersectionOf(IIntentNode baseObject, IIntentNode otherObject) {
			return baseObject;
		}
	}
}
