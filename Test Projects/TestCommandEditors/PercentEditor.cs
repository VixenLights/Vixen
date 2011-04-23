using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Interface;
using CommandStandard;

namespace TestCommandEditors {
    // Can't have this be the editor because the editor would need to
    // subclass both CommandEditorBase and the control.
    public class PercentEditor : ICommandEditorModuleInstance {
		public IApplication Application { private get; set; }

		public Guid TypeId {
			get { return PercentEditorModule._typeId; }
		}
		
		public Guid InstanceId { get; set; }

        public IModuleDataModel ModuleData { get; set; }

        public ICommandEditorControl CreateEditorControl()
		{
            return new PercentEditorControl();
        }

		public CommandIdentifier TargetCommand
		{
            get { return PercentEditorModule._targetCommand; }
		}

        public void Dispose() { }

	}
}
