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
			if (_data.ConvertColorIntoShutterIntents)
			{
				// Initialize the check box state
				checkBoxConvert.CheckState = CheckState.Checked;				
			}

			// Initialize the Open Shutter value
			textOpenShutter.Text = _data.OpenShutterIndexValue.ToString();

			// Initialize the Close Shutter value
			textCloseShutter.Text = _data.CloseShutterIndexValue.ToString();

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
			_data.ConvertColorIntoShutterIntents = (checkBoxConvert.CheckState == CheckState.Checked);

			// Store off the tag
			_data.Tag = textBoxTag.Text;

			// Store off the open shutter idex value
			_data.OpenShutterIndexValue = byte.Parse(textOpenShutter.Text);

			// Store off the close shutter idex value
			_data.CloseShutterIndexValue = byte.Parse(textCloseShutter.Text);
		}

		/// <summary>
		/// Returns true if all the required fields are populated.
		/// </summary>
		/// <returns>True if all the required fields are populated</returns>
		private bool AllRequiredFieldsArePopulated()
        {
			// Return true if all required fields are populated
			return
				!string.IsNullOrEmpty(textBoxTag.Text) &&
				!string.IsNullOrEmpty(textOpenShutter.Text) &&
				!string.IsNullOrEmpty(textCloseShutter.Text);
		}

		/// <summary>
		/// Event handler for when the Tag text box changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
        private void textBoxTag_TextChanged(object sender, EventArgs e)
        {
			// Enable/Disable the OK button
			buttonOk.Enabled = AllRequiredFieldsArePopulated();
		}
        
		/// <summary>
		/// Event handler for when the open shutter value changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
        private void textOpenShutter_TextChanged(object sender, EventArgs e)
        {			
			// Enable/Disable the OK button
			buttonOk.Enabled = AllRequiredFieldsArePopulated();						
		}

		/// <summary>
		/// Event handler for when the close shutter value changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void textCloseShutter_TextChanged(object sender, EventArgs e)
		{
			// Enable/Disable the OK button
			buttonOk.Enabled = AllRequiredFieldsArePopulated();
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

				// Enable the Close Shutter text box
				textCloseShutter.Enabled = true;
			}
			// Otherwise shutter is NOT being automated
			else
			{
				// Disable the Open Shutter text box
				textOpenShutter.Enabled = false;

				// Disable the Close Shutter text box
				textCloseShutter.Enabled = false;
			}
		}

        #endregion        
    }
}
