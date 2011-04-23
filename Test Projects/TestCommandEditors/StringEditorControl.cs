using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Interface;
using Vixen.Common;

namespace TestCommandEditors {
    public partial class StringEditorControl : UserControl, ICommandEditorControl {
        public StringEditorControl() {
            InitializeComponent();
        }

        //The control is created and displayed by the editor which then passes
        //this collection to the parameterized command.
        public ParameterValue[] CommandParameterValues {
            get {
                return new ParameterValue[] {
                    new StringParameterValue(textBoxValue.Text)
                };
            }
        }

    }
}
