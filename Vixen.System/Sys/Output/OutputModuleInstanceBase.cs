using Vixen.Commands;
using Vixen.Module;

namespace Vixen.Sys.Output {
	abstract public class OutputModuleInstanceBase : ModuleInstanceBase, IOutputModule {
		//private ModuleLocalDataSet _moduleDataSet;

		//protected OutputModuleInstanceBase() {
		//    _moduleDataSet = new ModuleLocalDataSet();
		//}

		//virtual public ModuleLocalDataSet ModuleDataSet {
		//    get { return _moduleDataSet; }
		//    set {
		//        _moduleDataSet = value;
		//        _moduleDataSet.AssignModuleTypeData(this);
		//    }
		//}

		abstract public IDataPolicy DataPolicy { get; }

		//virtual public void UpdateState(ICommand[] outputStates) {
		//    _UpdateState(outputStates);
		//}

		//abstract protected void _UpdateState(ICommand[] outputStates);

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
			get { return ((IOutputModuleDescriptor)Descriptor).UpdateInterval; }
		}
	}
}
