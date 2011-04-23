using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Interface;
using CommandStandard;

namespace TestCommandEditors {
    public class LoadPositionAndPlayEditor : ICommandEditorModuleInstance {
        public IApplication Application { private get; set; }

        public Guid TypeId {
            get { return LoadPositionAndPlayModule._typeId; }
        }

        public Guid InstanceId { get; set; }

        public object ModuleData { get; set; }

        public ICommandEditorControl CreateEditorControl() {
            return new LoadPositionAndPlayControl();
        }

        public CommandIdentifier TargetCommand {
            get { return LoadPositionAndPlayModule._targetCommand; }
        }

        public void Dispose() {
        }
    }
}
