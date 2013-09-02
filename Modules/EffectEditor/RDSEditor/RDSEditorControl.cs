using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;

namespace VixenModules.EffectEditor.RDSEditor {
	public partial class RDSEditorControl : UserControl, IEffectEditorControl {
		public RDSEditorControl() {
			InitializeComponent();
		}

		public object[] EffectParameterValues {
			get {
				return new object[] {Title,Artist };
			}
			set {
				Text = value[0] as string;
				Artist= value[1] as string;
			}
		}

		private IEffect _targetEffect;

		public IEffect TargetEffect {
			get { return _targetEffect; }
			set {
				_targetEffect = value;
				//Ensure target effect is passed through as these editors need it.
			}
		}
		public string Title { get { return this.textRDSTitle.Text; } set { this.textRDSTitle.Text = value; } }
		public string Artist { get { return this.textRDSArtist.Text; } set { this.textRDSArtist.Text=value; } }
	}
}
