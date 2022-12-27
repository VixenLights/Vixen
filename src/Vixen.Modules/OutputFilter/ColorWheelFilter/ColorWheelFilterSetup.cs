using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.OutputFilter.ColorWheelFilter
{
    /// <summary>
    /// Dialog for configuring Color Wheel filter.
    /// </summary>
    public partial class ColorWheelFilterSetup : BaseForm
	{
        #region Fields

		/// <summary>
		/// Data associated with the filter.
		/// </summary>
        private ColorWheelFilterData _data;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data associated with the filter</param>
		public ColorWheelFilterSetup(ColorWheelFilterData data)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			// Store off the data associated with the filter
			_data = data;

			// If converting color intensity into index commands then...
			if (_data.ConvertColorIntentsIntoIndexCommands)
			{
				// Initialize the check box state
				checkBoxConvert.CheckState = CheckState.Checked;
			}

			// Initialize the tag text box
			textBoxTag.Text = _data.Tag;
		}

        #endregion

        #region Private Event Handlers

        private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		/// <summary>
		/// OK button clicked event handler.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void buttonOk_Click(object sender, EventArgs e)
		{
			// Store off whether to convert color into color wheel index commands 
			_data.ConvertColorIntentsIntoIndexCommands = (checkBoxConvert.CheckState == CheckState.Checked);

			// Store off the tag
			_data.Tag = textBoxTag.Text;
		}

		/// <summary>
		/// Event handler for when the Tag text box changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
        private void textBoxTag_TextChanged(object sender, EventArgs e)
        {
			// If the tag is empty then...
			if (string.IsNullOrEmpty(textBoxTag.Text))
            {
				// Disable the OK button
				buttonOk.Enabled = false;
            }
			// Otherwise the tag is populated
			else
            {
				// Enable the OK button
				buttonOk.Enabled = true;
			}
        }

		#endregion
	}
}
