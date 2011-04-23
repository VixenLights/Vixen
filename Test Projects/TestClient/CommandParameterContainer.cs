using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Interface;
using Vixen.Common;
using CommandStandard;

namespace TestClient {
    public partial class CommandParameterContainer : Form {
        //private List<ICommandEditorControl> _editors = new List<ICommandEditorControl>();
        private ICommandEditorControl _editorControl;

        public CommandParameterContainer(CommandSignature commandSignature) {
            InitializeComponent();
            CommandEditorControl = Vixen.Sys.ApplicationServices.GetCommandEditorControl(commandSignature.Identifier);
        }

        public bool HasParameters { get; private set; }

        private ICommandEditorControl CommandEditorControl {
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

		//public ParameterValue[] Values {
        public object[] Values {
            get {
                if(HasParameters) {
                    return _editorControl.CommandParameterValues;
                }
                return null;
            }
        }

        //private void _AddCommandEditor(Control control) {
        //    _editors.Add(control as ICommandEditorControl);
        //    panel1.Controls.Add(control);
        //    control.Dock = DockStyle.Top;
        //}

		//public Parameter[] GetParameters() {
		//    //*** How will the user's values into the Parameters
		//    return
		//        (from editor in _editors
		//         select editor.CommandParameters).SelectMany(x => x).ToArray();
		//}

        private void button1_Click(object sender, EventArgs e) {

        }
    }
}
