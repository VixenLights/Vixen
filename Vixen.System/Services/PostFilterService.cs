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

		public void ApplyTemplateOnce(string templateFileName, IPatchingRule mappingRule) {
			var serializer = SerializerFactory.Instance.CreatePostFilterTemplateSerializer();
			var result = serializer.Read(templateFileName);
			if(!result.Success) return;

			//*** file reader needs to populate the modules with instance data from the template's
			//    dataset
			ControllerReference[] controllerReferences = mappingRule.GenerateControllerReferences(1);
			_ApplyTemplateToOutputs(result.Object, controllerReferences);
		}

		public void ApplyTemplateMany(string templateFileName, IPatchingRule mappingRule, int count) {
			var serializer = SerializerFactory.Instance.CreatePostFilterTemplateSerializer();
			var result = serializer.Read(templateFileName);
			if(!result.Success) return;

			PostFilterTemplate template = result.Object;

			ControllerReference[] controllerReferences = mappingRule.GenerateControllerReferences(count);
			int referencesPerOutput = controllerReferences.Length / count;
			int countToAffect = _GetCountToAffect(controllerReferences, template.OutputFilters);

			int referenceStartIndex = 0;
			while(count-- > 0) {
				ControllerReference[] referencesForApplicationOfTemplate = controllerReferences.SubArray(referenceStartIndex, countToAffect);
				_ApplyTemplateToOutputs(template, referencesForApplicationOfTemplate);
				referenceStartIndex += referencesPerOutput;
			}
		}

		private void _ApplyTemplateToOutputs(PostFilterTemplate template, ControllerReference[] controllerReferences) {
			int countToAffect = _GetCountToAffect(controllerReferences, template.OutputFilters);

			for(int i = 0; i < countToAffect; i++) {
				// Apply the appropriate list of filters to the appropriate output.
				_ApplyFilterCollectionToOutput(template[i], controllerReferences[i]);
			}
		}

		private int _GetCountToAffect(IEnumerable<ControllerReference> controllerReferences, IEnumerable<PostFilterCollection> filterCollections) {
			return Math.Min(controllerReferences.Count(), filterCollections.Count());
		}

		private void _ApplyFilterCollectionToOutput(IEnumerable<IPostFilterModuleInstance> filters, ControllerReference controllerReference) {
			OutputController controller = (OutputController)VixenSystem.Controllers.Get(controllerReference.ControllerId);
			if(controller != null) {
				foreach(IPostFilterModuleInstance filter in filters) {
					controller.AddPostFilter(controllerReference.OutputIndex, filter);
				}
			}
		}
	}
}
