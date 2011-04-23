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
using CommandStandard.Types;

namespace TestCommandEditors {
    public partial class PercentEditorControl : UserControl, ICommandEditorControl {
        public PercentEditorControl() {
            InitializeComponent();
        }

        //The control is created and displayed by the editor which then passes
        //this collection to the parameterized command.
		//public ParameterValue[] CommandParameterValues {
        public object[] CommandParameterValues {
            get {
                return new object[] {
				//return new ParameterValue[] {
					//new DoubleParameterValue((double)numericUpDownValue.Value)
                    (Level)(double)numericUpDownValue.Value
                };
            }
        }


    }
}
