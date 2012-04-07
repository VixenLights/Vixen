using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.ModuleTemplate;
using Vixen.Module.Transform;
using Vixen.Module.Controller;
using Vixen.Services;
using Vixen.Sys;

namespace TestTemplate {
	public class OutputControllerTemplate : ModuleTemplateModuleInstanceBase {
		private OutputControllerTemplateData _data;

		public override void Project(IModuleInstance target) {
			IControllerModuleInstance outputModule = target as IControllerModuleInstance;
			// Get instances of the transforms we reference.
			ITransformModuleInstance[] transforms = _GetTransforms();
			
			outputModule.BaseTransforms = transforms;
		}

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
				transform = ApplicationServices.Get<ITransformModuleInstance>(transformReference.TypeId);
				transform.InstanceId = transformReference.InstanceId;
				// Get data for each instance from our transform module data set.
				// Get as instance data, not type data.
				_data.TransformData.AssignModuleInstanceData(transform);
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

		override public IModuleDataModel StaticModuleData {
			get { return _data; }
			set { _data = value as OutputControllerTemplateData; }
		}

		override public void Setup() {
			// The setup dialog needs the transform datum because it's going to be creating
			// new instances and allowing the user to set them up.
			if(_data == null) throw new Exception("No data object.");
			using(OutputControllerTemplateSetup templateSetup = new OutputControllerTemplateSetup(_data.TransformData)) {
				templateSetup.Transforms = _GetTransforms();
				if(templateSetup.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					_SetTransforms(templateSetup.Transforms);
					//Template data is now stored in application module data in UserData
					//ApplicationServices.CommitTemplate(this);
				}
			}
		}
	}
}
