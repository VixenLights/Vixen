using Common.Controls.Theme;

namespace VixenModules.App.Shows
{

	public partial class PauseTypeEditor : TypeEditorBase
	{
		private readonly ShowItem _showItem;

		public PauseTypeEditor(ShowItem showItem)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_showItem = showItem;
		}

		private void SequenceTypeEditor_Load(object sender, EventArgs e)
		{
			numericUpDownPauseSeconds.Value = _showItem.Pause_Seconds;
			UpdateItemName();
		}

		private void UpdateItemName()
		{
			_showItem.Name = Text = $@"Pause for {_showItem.Pause_Seconds}";
			FireChanged(_showItem.Name);
		}

		private void numericUpDownPauseSeconds_ValueChanged(object sender, EventArgs e)
		{
			_showItem.Pause_Seconds = Convert.ToInt32(numericUpDownPauseSeconds.Value);
			UpdateItemName();
		}

	}
}
