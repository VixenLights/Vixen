using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys.Output;

namespace Vixen.Module.Controller {
	abstract public class ControllerModuleInstanceBase : OutputModuleInstanceBase, IControllerModuleInstance, IEqualityComparer<IControllerModuleInstance>, IEquatable<IControllerModuleInstance>, IEqualityComparer<ControllerModuleInstanceBase>, IEquatable<ControllerModuleInstanceBase> {
		private int _outputCount;

		public int OutputCount {
			get { return _outputCount; }
			set {
				_outputCount = value;
				_SetOutputCount(value);
			}
		}

		abstract protected void _SetOutputCount(int outputCount);

		public virtual int ChainIndex { get; set; }

		abstract public void Start(int outputCount);

		abstract public void UpdateState(ICommand[] outputStates);

		public bool Equals(IControllerModuleInstance x, IControllerModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IControllerModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IControllerModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(ControllerModuleInstanceBase x, ControllerModuleInstanceBase y) {
			return Equals(x as IControllerModuleInstance, y as IControllerModuleInstance);
		}

		public int GetHashCode(ControllerModuleInstanceBase obj) {
			return GetHashCode(obj as IControllerModuleInstance);
		}

		public bool Equals(ControllerModuleInstanceBase other) {
			return Equals(other as IControllerModuleInstance);
		}
	}
	//abstract public class ControllerModuleInstanceBase : ModuleInstanceBase, IControllerModuleInstance, IEqualityComparer<IControllerModuleInstance>, IEquatable<IControllerModuleInstance>, IEqualityComparer<ControllerModuleInstanceBase>, IEquatable<ControllerModuleInstanceBase> {
	//    private ModuleLocalDataSet _moduleDataSet;
	//    private int _outputCount;

	//    protected ControllerModuleInstanceBase() {
	//        _moduleDataSet = new ModuleLocalDataSet();
	//    }

	//    public int OutputCount {
	//        get { return _outputCount; }
	//        set {
	//            _outputCount = value;
	//            _SetOutputCount(value);
	//        }
	//    }

	//    abstract protected void _SetOutputCount(int outputCount);

	//    virtual public ModuleLocalDataSet ModuleDataSet {
	//        get { return _moduleDataSet; }
	//        set {
	//            _moduleDataSet = value;
	//            _moduleDataSet.AssignModuleTypeData(this);
	//        }
	//    }

	//    public virtual int ChainIndex { get; set; }

	//    abstract public IDataPolicy DataPolicy { get; }

	//    virtual public void UpdateState(ICommand[] outputStates) {
	//        // Send on to the output module.
	//        _UpdateState(outputStates);
	//    }

	//    abstract protected void _UpdateState(ICommand[] outputStates);

	//    /// <summary>
	//    /// If overriding this, please also override Start and Stop.
	//    /// </summary>
	//    virtual public bool IsRunning { get; private set; }

	//    virtual public bool HasSetup {
	//        get { return false; }
	//    }

	//    virtual public bool Setup() {
	//        return false;
	//    }

	//    /// <summary>
	//    /// If overriding this, please also override Stop and IsRunning.
	//    /// </summary>
	//    virtual public void Start() {
	//        IsRunning = true;
	//    }

	//    abstract public void Start(int outputCount);

	//    /// <summary>
	//    /// If overriding this, please also override Start and IsRunning.
	//    /// </summary>
	//    virtual public void Stop() {
	//        IsRunning = false;
	//    }

	//    virtual public void Pause() {
	//    }

	//    virtual public void Resume() {
	//    }

	//    virtual public int UpdateInterval {
	//        get { return ((IControllerModuleDescriptor)Descriptor).UpdateInterval; }
	//    }

	//    public bool Equals(IControllerModuleInstance x, IControllerModuleInstance y) {
	//        return base.Equals(x, y);
	//    }

	//    public int GetHashCode(IControllerModuleInstance obj) {
	//        return base.GetHashCode(obj);
	//    }

	//    public bool Equals(IControllerModuleInstance other) {
	//        return base.Equals(other);
	//    }

	//    public bool Equals(ControllerModuleInstanceBase x, ControllerModuleInstanceBase y) {
	//        return Equals(x as IControllerModuleInstance, y as IControllerModuleInstance);
	//    }

	//    public int GetHashCode(ControllerModuleInstanceBase obj) {
	//        return GetHashCode(obj as IControllerModuleInstance);
	//    }

	//    public bool Equals(ControllerModuleInstanceBase other) {
	//        return Equals(other as IControllerModuleInstance);
	//    }
	//}
}
