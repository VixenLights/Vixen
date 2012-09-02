using System;
using Vixen.Sys.Output;

namespace Vixen.Sys.Managers {
	class ControllerLinkingManagement<T> : IControllerLinkManager<T>
		where T : class, IOutputDevice, IHasOutputs {
		public T GetController(Guid id) {
			return (T)VixenSystem.ControllerManagement.GetController(id);
		}

		public T GetNext(T controller) {
			Guid? nextControllerId = VixenSystem.ControllerLinking.GetNext(controller.Id);
			return (nextControllerId.HasValue) ? GetController(nextControllerId.Value) : null;
		}

		public T GetPrior(T controller) {
			Guid? priorControllerId = VixenSystem.ControllerLinking.GetPrior(controller.Id);
			return (priorControllerId.HasValue) ? GetController(priorControllerId.Value) : null;
		}
	}
}
