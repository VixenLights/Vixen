namespace Common.Controls.NameGeneration
{
	public class NameGeneratorEditor : UserControl
	{
		public event EventHandler DataChanged;

		protected void OnDataChanged()
		{
			if (DataChanged != null)
				DataChanged(this, EventArgs.Empty);
		}
	}
}