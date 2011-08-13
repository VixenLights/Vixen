using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.FileTemplate;
using Vixen.Module.Transform;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Hardware;

namespace TestTemplate {
	public class OutputControllerTemplate : FileTemplateModuleInstanceBase {
		private OutputControllerTemplateData _data;

		override public void Project(object newInstance) {
			OutputControllerTemplateData data = ModuleData as OutputControllerTemplateData;
			OutputController controller = newInstance as OutputController;

			// Get instances of the transforms we reference.
			ITransformModuleInstance[] transforms = _GetTransforms();

			//foreach(OutputController.Output output in controller.Outputs) {
			//    // This must be done in two steps so that we have the original
			//    // instances (appropriate ones, that is) that belong to this output
			//    // and instances of the missing ones from the template.
			//    var alreadyHave = output.DataTransforms.Intersect(transforms, controller);
			//    // Add any transforms in the template but not in the output.
			//    foreach(ITransformModuleInstance templateTransform in transforms.Except(alreadyHave)) {
			//        output.AddTransform(templateTransform);
			//    }
			//}

			// Two dangers in doing this:
			//1. It only happens when they intially create the controller, contrary to
			//   their possible expectations.
			//2. Renaming an output will not affect the channel name, contrary to
			//   their possible expectations.
			// Do they want seed channels created?
			//if(_data.AutoCreateOutputChannels) {
			//    foreach(OutputController.Output output in controller.Outputs) {
			//        Vixen.Sys.Execution.AddChannel(output.Name);
			//    }
			//}
		}

		//*** replace _get/_set with a property?
		/// <summary>
		/// Instantiates a collection of transforms according to our module data and
		/// provides each with data from our transform module dataset.
		/// </summary>
		/// <returns></returns>
		private ITransformModuleInstance[] _GetTransforms() {
			// Get instances of the transforms.
			List<ITransformModuleInstance> transforms = new List<ITransformModuleInstance>();
			ITransformModuleInstance transform;
			foreach(InstanceReference transformReference in _data.Transforms) {
				//transform = VixenSystem.ModuleManagement.GetTransform(transformReference.TypeId) as ITransformModuleInstance;
				transform = ApplicationServices.Get<ITransformModuleInstance>(transformReference.TypeId);
				transform.InstanceId = transformReference.InstanceId;
				// Get data for each instance from our transform module data set.
				// Get as instance data, not type data.
				_data.TransformData.GetModuleInstanceData(transform);
				transforms.Add(transform);
			}
			return transforms.ToArray();
		}

		/// <summary>
		/// Persists a collection of transforms to the module dataset.
		/// </summary>
		/// <param name="transforms"></param>
		private void _SetTransforms(ITransformModuleInstance[] transforms) {
			_data.Transforms.Clear();
			_data.Transforms.AddRange(transforms.Select(x => new InstanceReference(x)));
		}

		override public IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as OutputControllerTemplateData; }
		}

		override public void Setup() {
			// The setup dialog needs the transform datum because it's going to be creating
			// new instances and allowing the user to set them up.
			using(OutputControllerTemplateSetup templateSetup = new OutputControllerTemplateSetup(_data.TransformData)) {
				templateSetup.Transforms = _GetTransforms();
				if(templateSetup.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					_SetTransforms(templateSetup.Transforms);
					ApplicationServices.CommitTemplate(this);
				}
			}
		}
	}
}
