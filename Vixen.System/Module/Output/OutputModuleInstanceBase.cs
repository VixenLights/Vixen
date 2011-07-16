using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.Module.Output {
	abstract public class OutputModuleInstanceBase : ModuleInstanceBase, IOutputModuleInstance, IEqualityComparer<IOutputModuleInstance> {
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

		public bool Equals(IOutputModuleInstance x, IOutputModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputModuleInstance obj) {
			return base.GetHashCode(obj);
		}
	}
}
