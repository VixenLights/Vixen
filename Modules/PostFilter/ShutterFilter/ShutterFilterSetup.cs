using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using System;
using System.Windows.Forms;


namespace VixenModules.OutputFilter.ShutterFilter
{
	/// <summary>
	/// Dialog for configuring Dimming filter.
	/// </summary>
    public partial class ShutterFilterSetup : BaseForm
	{
        #region Fields

		/// <summary>
		/// Data associated with the filter.
		/// </summary>
        private ShutterFilterData _data;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data associated with the filter</param>
		public ShutterFilterSetup(ShutterFilterData data)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			// Store off the data associated with the filter
			_data = data;

			// If converting color into shutter intents then...
			if (_data.ConvertRGBIntoShutterIntents)
			{
				// Initialize the check box state
				checkBoxConvert.CheckState = CheckState.Checked;

				// Initialize the Open Shutter value
				textOpenShutter.Text = _data.OpenShutterIndexValue.ToString();	
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
			// Store off whether to convert color into dimming 
			_data.ConvertRGBIntoShutterIntents = (checkBoxConvert.CheckState == CheckState.Checked);

			// Store off the tag
			_data.Tag = textBoxTag.Text;

			// Store off the open shutter idex value
			_data.OpenShutterIndexValue = byte.Parse(textOpenShutter.Text);
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
			else if (!string.IsNullOrEmpty(textOpenShutter.Text))
            {
				// Enable the OK button
				buttonOk.Enabled = true;
			}
        }
        
		/// <summary>
		/// Event handler for when the Open Shutter Value changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
        private void textOpenShutter_TextChanged(object sender, EventArgs e)
        {
			// If the tag is empty then...
			if (string.IsNullOrEmpty(textOpenShutter.Text))
			{
				// Disable the OK button
				buttonOk.Enabled = false;
			}
			// Otherwise if the tag is populated
			else if (!string.IsNullOrEmpty(textBoxTag.Text))
			{
				// Enable the OK button
				buttonOk.Enabled = true;
			}
		}

		/// <summary>
		/// Event handler for when the 'Automatically Open and Close Shutter' check box toggles.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arugments</param>
		private void checkBoxConvert_CheckedChanged(object sender, EventArgs e)
		{
			// If shutter is being automated then...
			if (checkBoxConvert.Checked)
			{
				// Enable the Open Shutter text box
				textOpenShutter.Enabled = true;
			}
			// Otherwise shutter is NOT being automated
			else
			{
				// Disable the Open Shutter text box
				textOpenShutter.Enabled = false;
			}
		}

		#endregion
	}
}
