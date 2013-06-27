namespace VixenModules.Preview.DisplayPreview.Views
{
	public partial class VisualizerView
	{
		public bool SystemClosing { get; set; }


		public VisualizerView()
		{
			InitializeComponent();
		}

		private void DisplayPreview_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!SystemClosing) {
				System.Windows.MessageBox.Show("To close the Display Preview, click the 'Configure Previews' " +
				                               "button on the main Vixen Administration window and then uncheck the preview in the list.",
				                               "Close Display Preview");
				e.Cancel = true;
			}
		}
	}
}