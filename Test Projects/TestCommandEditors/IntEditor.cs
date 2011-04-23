using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Interface;
using CommandStandard;

namespace TestCommandEditors {
    public class IntEditor : ICommandEditorModuleInstance {
        public IApplication Application { private get; set; }

        public Guid TypeId {
            get { return IntEditorModule._typeId; }
        }

        public Guid InstanceId { get; set; }

        public object ModuleData { get; set; }

        public ICommandEditorControl CreateEditorControl() {
            return new IntEditorControl();
        }

        public CommandIdentifier TargetCommand {
            get { return IntEditorModule._targetCommand; }
        }

        public void Dispose() {
        }
    }
}
