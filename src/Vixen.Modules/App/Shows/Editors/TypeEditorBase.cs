namespace VixenModules.App.Shows
{
	public partial class TypeEditorBase : UserControl
	{
		public delegate void OnTextChangedHandler(object sender, EventArgs e);
		public new virtual event OnTextChangedHandler OnTextChanged;

		public TypeEditorBase()
		{
			InitializeComponent();
		}

		//public string Text { get; set; }

		public void FireChanged(string text)
		{
			Text = text;
			if (OnTextChanged != null)
			{
				OnTextChanged(this, EventArgs.Empty);
			}
		}
	}
}
