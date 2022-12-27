namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class ParameterEditor : UserControl
	{
		public ParameterEditor()
		{
			InitializeComponent();
		}

		public Panel editorPanel
		{
			get { return tableLayoutPanel; }
		}

		public string labelText
		{
			get { return labelParameterType.Text; }
			set { labelParameterType.Text = value; }
		}
	}
}