using System.Collections.Generic;
using System.Linq;
using Vixen.Intent.IntersectionStrategy;

namespace Vixen.Sys {
	class IntentReplaceWithOtherStrategy : IIntentAddStrategy {
		private IIntersectionStrategy<IIntentNode> _strategy;

		private enum IntersectionType { Left, LeftAlignedLongBase, LeftAlignedShortBase, Middle, Right, RightAlignedLongBase, RightAlignedShortBase, Enveloped, Exact };

		public IntentReplaceWithOtherStrategy() {
			_strategy = new ReplaceWithOther();
		}

		public AddStrategyDelegate<IIntentNode> AddStrategy {
			get { return AddIntentNode; }
		}

		public void AddIntentNode(IIntentNode itemAdding, IList<IIntentNode> list) {
			// Get the intents that intersect the intent being added.
			IIntentNode[] intersectingIntents = _GetIntersectingIntents(itemAdding, list);

			if(intersectingIntents.Length > 0) {
				// Remove them.
				foreach(IIntentNode intentNode in intersectingIntents) {
					list.Remove(intentNode);
				}

				// Generate a new set of intents.
				List<IIntentNode> newNodes = _GenerateCombinedNodesUsingStrategy(itemAdding, intersectingIntents);

				// Add the new set of intents.
				for(int i = 0; i < newNodes.Count; i++) {
					list.Add(newNodes[i]);
				}
			} else {
				list.Add(itemAdding);
			}
		}

		private IIntentNode[] _GetIntersectingIntents(IIntentNode itemAdding, IList<IIntentNode> list) {
			return list.Where(x => TimeNode.IntersectsExclusively(x, itemAdding)).ToArray();
		}

		private List<IIntentNode> _GenerateCombinedNodesUsingStrategy(IIntentNode itemAdding, IIntentNode[] intersectingNodes) {
			List<IIntentNode> resultNodes = new List<IIntentNode>();

			// Put the intersecting nodes in time order.
			// This step is very important!  The behavior of this object is built on the assumption
			// that the intersecting nodes are in time order.
			IEnumerable<IIntentNode> timeOrderedIntersectingNodes = intersectingNodes.OrderBy(x => x.StartTime);

			IIntentNode rightRemainder = null;
			foreach(IIntentNode intersectingNode in timeOrderedIntersectingNodes) {
				IntersectionType intersectionType = _GetIntersectionType(itemAdding, intersectingNode);
				IIntentNode strategyTargetNode = null;
				IIntentNode leftRemainder = null;
				switch(intersectionType) {
					case IntersectionType.LeftAlignedLongBase:
						_LeftAlignedLongBaseIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
					case IntersectionType.LeftAlignedShortBase:
						_LeftAlignedShortBaseIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
					case IntersectionType.Middle:
						_MiddleIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
					case IntersectionType.RightAlignedLongBase:
						_RightAlignedLongBaseIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
					case IntersectionType.RightAlignedShortBase:
						_RightAlignedShortBaseIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
					case IntersectionType.Left:
						_LeftIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
					case IntersectionType.Right:
						_RightIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
					case IntersectionType.Enveloped:
						_EnvelopedIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
					case IntersectionType.Exact:
						_ExactIntersection(itemAdding, intersectingNode, out strategyTargetNode, out leftRemainder, out rightRemainder);
						break;
				}

				if(leftRemainder != null) {
					resultNodes.Add(leftRemainder);
				}

				resultNodes.Add(strategyTargetNode);

				// It is not possible to have two right remainders -- one from the base node, needing further intersections,
				// and one from the intersecting node -- if the collection that the intersecting nodes were pulled from
				// has been properly maintained.  There should be no overlapping nodes in that case.
				//
				// Nor is it possible for the remainder to be the remainder of the base node if it's the remainder of the
				// intersecting node.  In that case, they two would have been right-aligned and there would be no remainder.
				if(_RightRemainderIsIntersectionTruncation(rightRemainder, intersectingNode)) {
					resultNodes.Add(rightRemainder);
				} else {
					// Otherwise the right remainder is the rest of the base node that needs further intersection.
					itemAdding = rightRemainder;
				}
			}

			// If there is a right remainder and it's right-aligned with the base node, it can't be an intersecting-node
			// remainder because then that intersecting node would have been right-aligned and there would be no
			// right remainder.
			if(_RightRemainderIsBaseTruncation(rightRemainder, itemAdding)) {
				resultNodes.Add(rightRemainder);
			}

			return resultNodes;
		}

		private IntersectionType _GetIntersectionType(IIntentNode baseNode, IIntentNode intersectingNode) {
			if(intersectingNode.StartTime == baseNode.StartTime && intersectingNode.EndTime == baseNode.EndTime) return IntersectionType.Exact;

			if(intersectingNode.StartTime == baseNode.StartTime) {
				if(intersectingNode.TimeSpan < baseNode.TimeSpan) {
					return IntersectionType.LeftAlignedLongBase;
				} else {
					return IntersectionType.LeftAlignedShortBase;
				}
			}
			if(intersectingNode.EndTime == baseNode.EndTime) {
				if(intersectingNode.TimeSpan < baseNode.TimeSpan) {
					return IntersectionType.RightAlignedLongBase;
				} else {
					return IntersectionType.RightAlignedShortBase;
				}
			}
			if(intersectingNode.StartTime > baseNode.StartTime && intersectingNode.EndTime < baseNode.EndTime) return IntersectionType.Middle;
			if(intersectingNode.StartTime < baseNode.StartTime && intersectingNode.EndTime > baseNode.EndTime) return IntersectionType.Enveloped;
			if(intersectingNode.StartTime < baseNode.StartTime && intersectingNode.EndTime > baseNode.StartTime) return IntersectionType.Left;
			if(intersectingNode.StartTime < baseNode.EndTime && intersectingNode.EndTime > baseNode.EndTime) return IntersectionType.Right;
			return IntersectionType.Enveloped;
		}

		private void _LeftAlignedLongBaseIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			IIntentNode[] intentNodes = baseNode.DivideAt(intersectingNode.EndTime);
			intersectionResult = _strategy.GetIntersectionOf(intentNodes[0], intersectingNode);
			leftRemainder = null;
			rightRemainder = intentNodes[1];
		}

		private void _LeftAlignedShortBaseIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			IIntentNode[] intentNodes = intersectingNode.DivideAt(baseNode.EndTime);
			intersectionResult = _strategy.GetIntersectionOf(baseNode, intentNodes[0]);
			leftRemainder = null;
			rightRemainder = intentNodes[1];
		}

		private void _MiddleIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			IIntentNode[] leftNodes = baseNode.DivideAt(intersectingNode.StartTime);
			leftRemainder = leftNodes[0];

			IIntentNode[] rightNodes = leftNodes[1].DivideAt(intersectingNode.EndTime);
			rightRemainder = rightNodes[1];

			intersectionResult = _strategy.GetIntersectionOf(rightNodes[0], intersectingNode);
		}

		private void _EnvelopedIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			IIntentNode[] leftNodes = intersectingNode.DivideAt(baseNode.StartTime);
			leftRemainder = leftNodes[0];

			IIntentNode[] rightNodes = leftNodes[1].DivideAt(baseNode.EndTime);
			rightRemainder = rightNodes[1];

			intersectionResult = _strategy.GetIntersectionOf(baseNode, rightNodes[0]);
		}

		private void _RightAlignedLongBaseIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			IIntentNode[] intentNodes = baseNode.DivideAt(intersectingNode.StartTime);
			intersectionResult = _strategy.GetIntersectionOf(intentNodes[1], intersectingNode);
			leftRemainder = intentNodes[0];
			rightRemainder = null;
		}

		private void _RightAlignedShortBaseIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			IIntentNode[] intentNodes = intersectingNode.DivideAt(baseNode.StartTime);
			intersectionResult = _strategy.GetIntersectionOf(baseNode, intentNodes[1]);
			leftRemainder = intentNodes[0];
			rightRemainder = null;
		}

		private void _LeftIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			IIntentNode[] intentNodes = intersectingNode.DivideAt(baseNode.StartTime);
			_LeftAlignedLongBaseIntersection(baseNode, intentNodes[1], out intersectionResult, out leftRemainder, out rightRemainder);
			leftRemainder = intentNodes[0];
		}

		private void _RightIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			IIntentNode[] intentNodes = intersectingNode.DivideAt(baseNode.StartTime);
			_RightAlignedLongBaseIntersection(baseNode, intentNodes[0], out intersectionResult, out leftRemainder, out rightRemainder);
			rightRemainder = intentNodes[1];
		}

		private void _ExactIntersection(IIntentNode baseNode, IIntentNode intersectingNode, out IIntentNode intersectionResult, out IIntentNode leftRemainder, out IIntentNode rightRemainder) {
			intersectionResult = _strategy.GetIntersectionOf(baseNode, intersectingNode);
			leftRemainder = null;
			rightRemainder = null;
		}

		private bool _RightRemainderIsIntersectionTruncation(IIntentNode rightRemainder, IIntentNode intersectingNode) {
			return rightRemainder != null && rightRemainder.EndTime == intersectingNode.EndTime;
		}

		private bool _RightRemainderIsBaseTruncation(IIntentNode rightRemainder, IIntentNode baseNode) {
			return rightRemainder != null && rightRemainder.EndTime == baseNode.EndTime;
		}

	}
}
