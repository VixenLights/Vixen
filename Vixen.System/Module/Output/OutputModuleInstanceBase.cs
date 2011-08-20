using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Transform;

namespace Vixen.Module.Output {
	abstract public class OutputModuleInstanceBase : ModuleInstanceBase, IOutputModuleInstance, IEqualityComparer<IOutputModuleInstance>, IEquatable<IOutputModuleInstance>, IEqualityComparer<OutputModuleInstanceBase>, IEquatable<OutputModuleInstanceBase> {
		private List<ITransformModuleInstance> _baseTransforms = new List<ITransformModuleInstance>();
		private List<List<ITransformModuleInstance>> _outputTransforms = new List<List<ITransformModuleInstance>>();
		private int _outputCount;

		protected OutputModuleInstanceBase() {
			TransformModuleData = new ModuleDataSet();
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

		public IModuleDataSet TransformModuleData { get; set; }

		public IEnumerable<ITransformModuleInstance> BaseTransforms {
			get { return _baseTransforms; }
			set {
				_baseTransforms = new List<ITransformModuleInstance>(value);
				// Affect any outputs that are lacking these.
				int index = 0;
				foreach(List<ITransformModuleInstance> outputTransformList in _outputTransforms) {
					IEnumerable<ITransformModuleInstance> missingTransforms = value.Except(outputTransformList);
					_AddTransformsToOutput(missingTransforms, index);
					index++;
				}
			}
		}

		private void _AddTransformsToOutput(IEnumerable<ITransformModuleInstance> transforms, int outputIndex) {
			List<ITransformModuleInstance> outputTransforms = _GetOutputTransforms(outputIndex);
			foreach(ITransformModuleInstance transform in transforms) {
				// Allowing multiple instances of a transform type.
				// Create a new instance, but use the same data (clone).
				ITransformModuleInstance newInstance = Modules.ModuleManagement.CloneTransform(transform);
				if(newInstance.ModuleData != null) {
					// Add the data to our transform dataset.
					TransformModuleData.Add(newInstance.ModuleData);
				} else {
					// Create data for the instance.
					TransformModuleData.GetModuleInstanceData(newInstance);
				}
				outputTransforms.Add(newInstance);
			}
		}

		private void _UpdateOutputTransforms(int oldOutputCount, int newOutputCount) {
			if(oldOutputCount < newOutputCount) {
				// Adding
				while(oldOutputCount < newOutputCount) {
					// Create a list of transforms for the output.
					_outputTransforms.Add(new List<ITransformModuleInstance>());
					// Give the new output the base set of transforms.
					_AddTransformsToOutput(_baseTransforms, oldOutputCount);
					//_outputTransforms.Add(new List<ITransformModuleInstance>(_baseTransforms));
					oldOutputCount++;
				}
			} else if(oldOutputCount > newOutputCount) {
				// Removing
				_outputTransforms.RemoveRange(newOutputCount, oldOutputCount - newOutputCount);
			}
		}

		public IEnumerable<ITransformModuleInstance> GetOutputTransforms(int outputIndex) {
			return _GetOutputTransforms(outputIndex);
		}

		public void UpdateState(CommandData[] outputStates) {
			// Make a copy of the state so that we're not modifying the actual state
			// with the transforms.
			outputStates = outputStates.Select(x => new CommandData(x.StartTime, x.EndTime, x.CommandIdentifier, x.ParameterValues.ToArray())).ToArray();

			// Transform...
			for(int i = 0; i < outputStates.Length; i++) {
				List<ITransformModuleInstance> outputTransforms = _outputTransforms[i];
				foreach(ITransformModuleInstance transform in outputTransforms) {
					transform.Transform(outputStates[i]);
				}
			}

			// Send on to the output module.
			_UpdateState(outputStates);
		}

		abstract protected void _UpdateState(CommandData[] outputStates);

		/// <summary>
		/// If overriding this, please also override Start and Stop.
		/// </summary>
		virtual public bool IsRunning { get; private set; }

		abstract public bool Setup();

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

		virtual public void AddTransform(int outputIndex, Guid transformTypeId, Guid transformInstanceId = default(Guid)) {
			// Create a new instance.
			ITransformModuleInstance instance = Modules.ModuleManagement.GetTransform(transformTypeId);
			
			// If an instance id is provided (creating an instance for existing module data),
			// assign it.
			if(transformInstanceId != default(Guid)) {
				instance.InstanceId = transformInstanceId;
			}
			
			// Create data for the instance.
			TransformModuleData.GetModuleInstanceData(instance);
			
			// Add the instance.
			List<ITransformModuleInstance> outputTransforms = _GetOutputTransforms(outputIndex);
			outputTransforms.Add(instance);
		}

		virtual public void RemoveTransform(int outputIndex, Guid transformTypeId, Guid transformInstanceId) {
			List<ITransformModuleInstance> outputTransforms = _GetOutputTransforms(outputIndex);
			ITransformModuleInstance instance = outputTransforms.FirstOrDefault(x => x.Descriptor.TypeId == transformTypeId && x.InstanceId == transformInstanceId);
			if(instance != null) {
				// Remove from the transform list.
				outputTransforms.Remove(instance);
				// Remove from the transform module data.
				TransformModuleData.Remove(transformTypeId, transformInstanceId);
			}
		}

		private List<ITransformModuleInstance> _GetOutputTransforms(int outputIndex) {
			return _outputTransforms[outputIndex];
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
