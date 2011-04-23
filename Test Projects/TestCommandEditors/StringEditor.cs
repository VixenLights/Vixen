using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Interface;
using CommandStandard;

namespace TestCommandEditors {
    public class StringEditor : ICommandEditorModuleInstance {
        public IApplication Application { private get; set; }

        public Guid TypeId {
            get { return StringEditorModule._typeId; }
        }

        public Guid InstanceId { get; set; }

        public object ModuleData { get; set; }

        public ICommandEditorControl CreateEditorControl() {
            return new StringEditorControl();
        }

        public CommandIdentifier TargetCommand {
            get { return StringEditorModule._targetCommand; }
        }

        public void Dispose() {
        }
    }
}
