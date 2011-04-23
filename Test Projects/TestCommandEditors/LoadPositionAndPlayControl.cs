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
    public partial class LoadPositionAndPlayControl : UserControl, ICommandEditorControl {
        public LoadPositionAndPlayControl() {
            InitializeComponent();
        }

        public ParameterValue[] CommandParameterValues {
            get {
                return new ParameterValue[] {
                    new StringParameterValue(textBoxFileName.Text),
                    new IntParameterValue(int.Parse(textBoxStartTime.Text))
                };
            }
        }
    }
}
