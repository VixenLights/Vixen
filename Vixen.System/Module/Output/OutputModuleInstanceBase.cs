using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Transform;
using Vixen.Commands;

namespace Vixen.Module.Output {
	abstract public class OutputModuleInstanceBase : ModuleInstanceBase, IOutputModuleInstance, IEqualityComparer<IOutputModuleInstance>, IEquatable<IOutputModuleInstance>, IEqualityComparer<OutputModuleInstanceBase>, IEquatable<OutputModuleInstanceBase> {
		private IModuleDataSet _moduleDataSet;
		private List<List<ITransformModuleInstance>> _outputTransforms = new List<List<ITransformModuleInstance>>();
		private int _outputCount;

		protected OutputModuleInstanceBase() {
			_moduleDataSet = new ModuleLocalDataSet();
			BaseTransforms = new ITransformModuleInstance[0];
		}

		public int OutputCount {
			get { return _outputCount; }
			set {
				_UpdateOutputTransforms(_outputCount, value);
				_outputCount = value;
				_SetOutputCount(value);
			}
		}

		abstract protected void _SetOutputCount(int outputCount);

		virtual public IModuleDataSet ModuleDataSet {
			get { return _moduleDataSet; }
			set {
				_moduleDataSet = value;
				_moduleDataSet.GetModuleTypeData(this);
			}
		}

		/// <summary>
		/// Transforms that are applied to any outputs created.
		/// </summary>
		virtual public IEnumerable<ITransformModuleInstance> BaseTransforms { get; set; }

		public virtual int ChainIndex { get; set; }

		virtual public void SetTransforms(int outputIndex, IEnumerable<ITransformModuleInstance> transforms) {
			List<ITransformModuleInstance> transformList = _GetTransformList(outputIndex);
			transformList.Clear();
			_AddTransformsToOutput(transforms, outputIndex);
		}

		virtual public IEnumerable<ITransformModuleInstance> GetTransforms(int outputIndex) {
			return _GetTransforms(outputIndex);
		}


		private void _AddTransformsToOutput(IEnumerable<ITransformModuleInstance> transforms, int outputIndex) {
			List<ITransformModuleInstance> outputTransforms = _GetTransformList(outputIndex);
			foreach(ITransformModuleInstance transform in transforms) {
				// Allowing multiple instances of a transform type.
				// Create a new instance, but use the same data (clone).
				//*Onus is on the caller to make sure they have a unique instance*
				// Otherwise, if this does it, a new instance is created and the caller has a different reference
				// than what the system has.
				//ITransformModuleInstance newInstance = Modules.ModuleManagement.CloneTransform(transform);
				// If data is already assigned to the module, it will be added to the data set.
				// If not, it will be created and added.
				ModuleDataSet.GetModuleInstanceData(transform);
				outputTransforms.Add(transform);
			}
		}

		private void _UpdateOutputTransforms(int oldOutputCount, int newOutputCount) {
			if(oldOutputCount < newOutputCount) {
				// Adding
				while(oldOutputCount < newOutputCount) {
					// Create a list of transforms for the output.
					_outputTransforms.Add(new List<ITransformModuleInstance>());
					// Give the new output the base set of transforms.
					_AddTransformsToOutput(BaseTransforms, oldOutputCount);
					oldOutputCount++;
				}
			} else if(oldOutputCount > newOutputCount) {
				// Removing
				_outputTransforms.RemoveRange(newOutputCount, oldOutputCount - newOutputCount);
			}
		}

		virtual public void UpdateState(Command[] outputStates) {
			// Transform...
			for(int i = 0; i < outputStates.Length; i++) {
				Command outputState = outputStates[i];
				if(outputState != null) {
					// Make a copy of the state so that we're not modifying the actual state
					// with the transforms.
					// Otherwise, we're going to end up transforming a previously tranformed value.
					outputState = outputState.Clone();
					outputStates[i] = outputState;

					List<ITransformModuleInstance> outputTransforms = _outputTransforms[i];
					foreach(ITransformModuleInstance transform in outputTransforms) {
						transform.Transform(outputState);
					}
				}
			}

			// Send on to the output module.
			_UpdateState(outputStates);
		}

		abstract protected void _UpdateState(Command[] outputStates);

		/// <summary>
		/// If overriding this, please also override Start and Stop.
		/// </summary>
		virtual public bool IsRunning { get; private set; }

		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() {
			return false;
		}

		/// <summary>
		/// If overriding this, please also override Stop and IsRunning.
		/// </summary>
		virtual public void Start() {
			IsRunning = true;
		}

		/// <summary>
		/// If overriding this, please also override Start and IsRunning.
		/// </summary>
		virtual public void Stop() {
			IsRunning = false;
		}

		virtual public void Pause() {
		}

		virtual public void Resume() {
		}

		virtual public int UpdateInterval {
			get { return (Descriptor as IOutputModuleDescriptor).UpdateInterval; }
		}

		virtual public void AddTransform(int outputIndex, ITransformModuleInstance transformModule) {
			// Add/Create the instance data in the dataset.
			ModuleDataSet.GetModuleInstanceData(transformModule);

			// Add the instance.
			List<ITransformModuleInstance> outputTransforms = _GetTransformList(outputIndex);
			outputTransforms.Add(transformModule);
		}

		virtual public void RemoveTransform(int outputIndex, Guid transformTypeId, Guid transformInstanceId) {
			List<ITransformModuleInstance> outputTransforms = _GetTransformList(outputIndex);
			ITransformModuleInstance instance = outputTransforms.FirstOrDefault(x => x.Descriptor.TypeId == transformTypeId && x.InstanceId == transformInstanceId);
			if(instance != null) {
				// Remove from the transform list.
				outputTransforms.Remove(instance);
				// Remove from the transform module data.
				ModuleDataSet.RemoveModuleInstanceData(transformTypeId, transformInstanceId);
			}
		}

		private List<ITransformModuleInstance> _GetTransformList(int outputIndex) {
			return _outputTransforms[outputIndex];
		}

		private IEnumerable<ITransformModuleInstance> _GetTransforms(int outputIndex) {
			return BaseTransforms.Concat(_outputTransforms[outputIndex]);
		}

		public bool Equals(IOutputModuleInstance x, IOutputModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IOutputModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(OutputModuleInstanceBase x, OutputModuleInstanceBase y) {
			return Equals(x as IOutputModuleInstance, y as IOutputModuleInstance);
		}

		public int GetHashCode(OutputModuleInstanceBase obj) {
			return GetHashCode(obj as IOutputModuleInstance);
		}

		public bool Equals(OutputModuleInstanceBase other) {
			return Equals(other as IOutputModuleInstance);
		}
	}
}
