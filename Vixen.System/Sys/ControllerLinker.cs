using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	public class ControllerLinker : IEnumerable<ControllerLink>
	{
		private HashSet<ControllerLink> _links;

		public ControllerLinker()
		{
			_links = new HashSet<ControllerLink>();
		}

		public void AddRange(IEnumerable<ControllerLink> links)
		{
			_links.AddRange(links);
		}

		public void LinkController(Guid childControllerId, Guid? parentControllerId)
		{
			_AddController(childControllerId);
			_AddController(parentControllerId);

			ControllerLink childControllerLink = _GetLinksFor(childControllerId);
			if (childControllerLink != null) {
				if (_CanLinkTo(parentControllerId)) {
					RemoveControllerFromParent(childControllerId);

					_SetPrior(childControllerId, parentControllerId);

					if (parentControllerId != null) {
						_SetNext(parentControllerId.Value, childControllerId);
					}
				}
			}
		}

		public void RemoveController(Guid controllerId)
		{
			ControllerLink controllerLink = _GetLinksFor(controllerId);
			if (controllerLink != null) {
				ControllerLink priorControllerLink = _PriorOf(controllerId);
				ControllerLink nextControllerLink = _NextOf(controllerId);
				if (priorControllerLink != null) {
					priorControllerLink.NextId = null;
				}
				if (nextControllerLink != null) {
					nextControllerLink.PriorId = null;
				}
				controllerLink.NextId = null;
				controllerLink.PriorId = null;
			}
		}

		public void RemoveControllerFromParent(Guid controllerId)
		{
			ControllerLink controllerLink = _GetLinksFor(controllerId);
			if (controllerLink != null) {
				ControllerLink priorControllerLink = _PriorOf(controllerId);
				if (priorControllerLink != null) {
					controllerLink.PriorId = null;
					priorControllerLink.NextId = null;
				}
			}
		}

		public int GetChainIndex(Guid controllerId)
		{
			int count = 0;
			ControllerLink priorLink = _PriorOf(controllerId);
			while (priorLink != null) {
				count++;
				priorLink = _PriorOf(priorLink.ControllerId);
			}
			return count;
		}

		public Guid? GetNext(Guid controllerId)
		{
			ControllerLink controllerLink = _NextOf(controllerId);
			return (controllerLink != null) ? controllerLink.ControllerId : (Guid?) null;
		}

		public Guid? GetPrior(Guid controllerId)
		{
			ControllerLink controllerLink = _PriorOf(controllerId);
			return (controllerLink != null) ? controllerLink.ControllerId : (Guid?) null;
		}

		private void _AddController(Guid? controllerId)
		{
			if (controllerId.HasValue && _GetLinksFor(controllerId.Value) == null) {
				_links.Add(new ControllerLink(controllerId.Value));
			}
		}

		private bool _CanLinkTo(Guid? parentControllerId)
		{
			// Can link to the parent if it doesn't already have a next link.
			return !parentControllerId.HasValue || _NextOf(parentControllerId.Value) == null;
		}

		private ControllerLink _PriorOf(Guid controllerId)
		{
			ControllerLink link = _GetLinksFor(controllerId);
			return (link != null && link.PriorId.HasValue) ? _GetLinksFor(link.PriorId.Value) : null;
		}

		private ControllerLink _NextOf(Guid controllerId)
		{
			ControllerLink link = _GetLinksFor(controllerId);
			return (link != null && link.NextId.HasValue) ? _GetLinksFor(link.NextId.Value) : null;
		}

		private void _SetPrior(Guid controllerId, Guid? priorControllerId)
		{
			ControllerLink link = _GetLinksFor(controllerId);
			if (link != null) {
				link.PriorId = priorControllerId;
			}
		}

		private void _SetNext(Guid controllerId, Guid? nextControllerId)
		{
			ControllerLink link = _GetLinksFor(controllerId);
			if (link != null) {
				link.NextId = nextControllerId;
			}
		}

		public bool IsRootController(OutputController controller)
		{
			ControllerLink link = _GetLinksFor(controller.Id);
			return link == null || link.PriorId != null;
		}

		private ControllerLink _GetLinksFor(Guid controllerId)
		{
			return _links.FirstOrDefault(x => x.ControllerId == controllerId);
		}


		public IEnumerator<ControllerLink> GetEnumerator()
		{
			return _links.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}