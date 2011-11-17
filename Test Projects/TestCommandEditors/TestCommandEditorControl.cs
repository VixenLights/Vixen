using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.EffectEditor;
using Vixen.Commands.KnownDataTypes;

namespace TestCommandEditors {
    using Vixen.Module.Effect;

    public partial class TestCommandEditorControl : UserControl, IEffectEditorControl {
		public TestCommandEditorControl() {
			InitializeComponent();
		}

		public object[] EffectParameterValues {
			get { return new object[] { (Level)(double)numericUpDownLevel.Value }; }
			set
			{
				// TODO
			}
		}

        public IEffect TargetEffect
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
