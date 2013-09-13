using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;

namespace LauncherEditor
{
	public partial class LauncherEditorControl : UserControl, IEffectEditorControl
	{
		public LauncherEditorControl()
		{
			InitializeComponent();
		}

		#region IEffectEditorControl Members

		public object[] EffectParameterValues
		{
			get
			{
				return new object[] {Description, Executable, Arguments };
			}
			set
			{
				Description = value[0] as string;
				Executable = value[1] as string;
				Arguments= value[2] as string;
			}
		}
		private IEffect _targetEffect;

		public IEffect TargetEffect
		{
			get { return _targetEffect; }
			set
			{
				_targetEffect = value;
				//Ensure target effect is passed through as these editors need it.
			}
		}

		#endregion

		private void btnOpenFile_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Multiselect=false;
				var res=	dlg.ShowDialog(this);
				if (res == DialogResult.OK) {
					this.txtFileName.Text = dlg.FileName;
				}
			}
		}

		public string Executable { get { return this.txtFileName.Text; } set { this.txtFileName.Text = value; } }
		public string Arguments { get { return this.txtArguments.Text; } set { this.txtArguments.Text=value; } }
		public string Description { get { return this.txtDescription.Text; } set { this.txtDescription.Text=value; } }

	}
}
