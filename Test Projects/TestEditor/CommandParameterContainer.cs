using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.EffectEditor;
using Vixen.Sys;

namespace TestEditor {
    public partial class EffectParameterContainer : Form {
		private IEffectEditorControl _editorControl;

		public EffectParameterContainer(Guid effectId) {
			InitializeComponent();
			// Need to go through ApplicationServices because external code can't get
			// a typed reference to module implementation components (i.e. module
			// manager).
			_EffectEditorControl = ApplicationServices.GetEffectEditorControls(effectId).First();
		}
		//Needs updating
		//public CommandParameterContainer(string commandName) {
		//    InitializeComponent();
		//    CommandEditorControl = Vixen.Sys.ApplicationServices.GetCommandEditorControl(commandName);
		//}

        public bool HasParameters { get; private set; }

		private IEffectEditorControl _EffectEditorControl {
            get { return _editorControl; }
            set {
                if(HasParameters = (value != null)) {
                    Control control = value as Control;
                    panel1.Controls.Add(control);
                    ClientSize = new Size(Math.Max(ClientSize.Width, control.Width), control.Height + panel2.Height);
                    control.Dock = DockStyle.Fill;
                }
                _editorControl = value;
            }
        }

        public object[] Values {
            get {
                if(HasParameters) {
					return _editorControl.EffectParameterValues;
                }
                return null;
            }
		}
    }
}
