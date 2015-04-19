using System.Windows.Forms;

namespace VixenModules.EffectEditor.EffectTypeEditors
{
	public partial class RangeTypeEditorControl : UserControl
	{
		public RangeTypeEditorControl()
		{
			InitializeComponent();
		}

		public int Value
		{
			get { return trackBarRange.Value; }
			set { trackBarRange.Value = value; }
		}

		public int UpperBounds
		{
			get { return trackBarRange.Maximum; }
			set { trackBarRange.Maximum = value; }
		}

		public int LowerBounds
		{
			get { return trackBarRange.Minimum; }
			set { trackBarRange.Minimum = value; }
		}
	}
}
