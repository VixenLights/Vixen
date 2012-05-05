using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.IO;
using Vixen.Module.PostFilter;
using Vixen.Rule;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Services {
	public class PostFilterService {
		static private PostFilterService _instance;

		private PostFilterService() {
		}

		public static PostFilterService Instance {
			get { return _instance ?? (_instance = new PostFilterService()); }
		}

		public void ApplyTemplateOnce(string templateFileName, IPatchingRule patchingRule, bool clearExisting = false) {
			var serializer = SerializerFactory.Instance.CreatePostFilterTemplateSerializer();
			var result = serializer.Read(templateFileName);
			if(!result.Success) return;

			ControllerReference[] controllerReferences = patchingRule.GenerateControllerReferenceCollections(1).First().ToArray();
			IEnumerable<OutputController> referencedControllers = VixenSystem.Controllers.GetControllers(controllerReferences).ToArray();

			VixenSystem.Controllers.Pause(referencedControllers);
	
			_ApplyTemplateToOutputs(result.Object, controllerReferences, clearExisting);
			
			VixenSystem.Controllers.Resume(referencedControllers);
		}

		public void ApplyTemplateMany(string templateFileName, IPatchingRule patchingRule, int channelCount, bool clearExisting = false) {
			var serializer = SerializerFactory.Instance.CreatePostFilterTemplateSerializer();
			var result = serializer.Read(templateFileName);
			if(!result.Success) return;

			PostFilterTemplate template = result.Object;

			IEnumerable<ControllerReferenceCollection> controllerReferenceCollections = patchingRule.GenerateControllerReferenceCollections(channelCount).ToArray();

			IEnumerable<OutputController> referencedControllers = VixenSystem.Controllers.GetControllers(controllerReferenceCollections.SelectMany(x => x)).ToArray();
			VixenSystem.Controllers.Pause(referencedControllers);

			foreach(ControllerReferenceCollection controllerReferences in controllerReferenceCollections) {
				_ApplyTemplateToOutputs(template, controllerReferences.ToArray(), clearExisting);
			}
			
			VixenSystem.Controllers.Resume(referencedControllers);
		}

		private void _ApplyTemplateToOutputs(PostFilterTemplate template, ControllerReference[] controllerReferences, bool clearExisting = false) {
			int countToAffect = _GetCountToAffect(controllerReferences, template.OutputFilters);

			for(int i = 0; i < countToAffect; i++) {
				// Apply the appropriate list of filters to the appropriate output.
				_ApplyFilterCollectionToOutput(_CloneFilters(template[i]), controllerReferences[i], clearExisting);
			}
		}

		private IEnumerable<IPostFilterModuleInstance> _CloneFilters(IEnumerable<IPostFilterModuleInstance> postFilterModules) {
			return postFilterModules.Select(x => (IPostFilterModuleInstance)x.Clone());
		}

		private int _GetCountToAffect(IEnumerable<ControllerReference> controllerReferences, IEnumerable<PostFilterCollection> filterCollections) {
			return Math.Min(controllerReferences.Count(), filterCollections.Count());
		}

		private void _ApplyFilterCollectionToOutput(IEnumerable<IPostFilterModuleInstance> filters, ControllerReference controllerReference, bool clearExisting = false) {
			OutputController controller = (OutputController)VixenSystem.Controllers.Get(controllerReference.ControllerId);
			if(controller != null) {
				if(clearExisting) {
					controller.ClearPostFilters(controllerReference.OutputIndex);
				}
				foreach(IPostFilterModuleInstance filter in filters) {
					controller.AddPostFilter(controllerReference.OutputIndex, filter);
				}
			}
		}
	}
}
