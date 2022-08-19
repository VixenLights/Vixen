using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using System;
using System.Windows.Forms;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Dialog for configuring Tagged filter.
	/// </summary>
    public partial class TaggedFilterSetup : BaseForm
	{
        #region Fields

		/// <summary>
		/// Data associated with the filter.
		/// </summary>
        private TaggedFilterDataBase _data;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data associated with the filter</param>
		public TaggedFilterSetup(TaggedFilterDataBase data)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			// Store off the data associated with the filter
			_data = data;
		
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
