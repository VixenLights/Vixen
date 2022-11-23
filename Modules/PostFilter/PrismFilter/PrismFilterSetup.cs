using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using System;
using System.Windows.Forms;


namespace VixenModules.OutputFilter.PrismFilter
{
	/// <summary>
	/// Dialog for configuring prism filter.
	/// </summary>
    public partial class PrismFilterSetup : BaseForm
	{
        #region Fields

		/// <summary>
		/// Data associated with the filter.
		/// </summary>
        private PrismFilterData _data;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data associated with the filter</param>
		public PrismFilterSetup(PrismFilterData data)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			// Store off the data associated with the filter
			_data = data;

			// If converting prism intents into open prism intents then...
			if (_data.ConvertPrismIntentsIntoOpenPrismIntents)
			{
				// Initialize the check box state
				checkBoxConvert.CheckState = CheckState.Checked;				
			}

			// Initialize the Open Prism value
			textOpenPrism.Text = _data.OpenPrismIndexValue.ToString();

			// Initialize the Close Prism value
			textClosePrism.Text = _data.ClosePrismIndexValue.ToString();

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
			// Store off whether to convert prism intents into open prism commands
			_data.ConvertPrismIntentsIntoOpenPrismIntents= (checkBoxConvert.CheckState == CheckState.Checked);

			// Store off the tag
			_data.Tag = textBoxTag.Text;

			// Store off the open prism index value
			_data.ClosePrismIndexValue= byte.Parse(textOpenPrism.Text);

			// Store off the close prism index value
			_data.ClosePrismIndexValue = byte.Parse(textClosePrism.Text);
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
				!string.IsNullOrEmpty(textOpenPrism.Text) &&
				!string.IsNullOrEmpty(textClosePrism.Text);
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
		/// Event handler for when the open prism value changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
        private void textOpenPrism_TextChanged(object sender, EventArgs e)
        {			
			// Enable/Disable the OK button
			buttonOk.Enabled = AllRequiredFieldsArePopulated();						
		}

		/// <summary>
		/// Event handler for when the close prism value changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void textClosePrism_TextChanged(object sender, EventArgs e)
		{
			// Enable/Disable the OK button
			buttonOk.Enabled = AllRequiredFieldsArePopulated();
		}

		/// <summary>
		/// Event handler for when the 'Automatically Open and Close Prism' check box toggles.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arugments</param>
		private void checkBoxConvert_CheckedChanged(object sender, EventArgs e)
		{
			// If Prism is being automated then...
			if (checkBoxConvert.Checked)
			{
				// Enable the Open Prism text box
				textOpenPrism.Enabled = true;

				// Enable the Close Prism text box
				textClosePrism.Enabled = true;
			}
			// Otherwise prism is NOT being automated
			else
			{
				// Disable the Open Prism text box
				textOpenPrism.Enabled = false;

				// Disable the Close Prism text box
				textClosePrism.Enabled = false;
			}
		}

        #endregion        
    }
}
