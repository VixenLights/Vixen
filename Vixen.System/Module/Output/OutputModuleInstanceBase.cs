using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.Module.Output {
	abstract public class OutputModuleInstanceBase : ModuleInstanceBase, IOutputModuleInstance, IEqualityComparer<IOutputModuleInstance>, IEquatable<IOutputModuleInstance>, IEqualityComparer<OutputModuleInstanceBase>, IEquatable<OutputModuleInstanceBase> {
		abstract public void SetOutputCount(int outputCount);

		abstract public void UpdateState(CommandData[] outputStates);

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
