using Vixen.Sys;

namespace Vixen.Intent.IntersectionStrategy {
	class ReplaceWithOther : IIntersectionStrategy<IIntentNode> {
		public IIntentNode GetIntersectionOf(IIntentNode baseObject, IIntentNode otherObject) {
			return otherObject;
		}
	}
}
