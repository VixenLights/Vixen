using Vixen.Module;

namespace Vixen.Sys.Output {
	abstract public class OutputModuleInstanceBase : ModuleInstanceBase, IOutputModule, IHasSetup, IHardware {
		/// <summary>
		/// If overriding this, please also override Start and Stop.
		/// </summary>
		virtual public bool IsRunning { get; private set; }

		/// <summary>
		/// If overriding this, please also override Pause and Resume.
		/// </summary>
		virtual public bool IsPaused { get; private set; }

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

		/// <summary>
		/// If overriding this, please also override Resume and IsPaused.
		/// </summary>
		virtual public void Pause() {
			IsPaused = true;
		}

		/// <summary>
		/// If overriding this, please also override Pause and IsPaused.
		/// </summary>
		virtual public void Resume() {
			IsPaused = false;
		}

		virtual public int UpdateInterval {
			get { return ((IOutputModuleDescriptor)Descriptor).UpdateInterval; }
		}
	}
}
