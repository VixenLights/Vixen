// =====================================================================
// SetupForm.cs - the setup dialog form
// version 1.0.0.1 - 2 june 2010
// =====================================================================

// =====================================================================
// Copyright (c) 2010 Joshua 1 Systems Inc. All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//    1. Redistributions of source code must retain the above copyright notice, this list of
//       conditions and the following disclaimer.
//    2. Redistributions in binary form must reproduce the above copyright notice, this list
//       of conditions and the following disclaimer in the documentation and/or other materials
//       provided with the distribution.
// THIS SOFTWARE IS PROVIDED BY JOSHUA 1 SYSTEMS INC. "AS IS" AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// The views and conclusions contained in the software and documentation are those of the
// authors and should not be interpreted as representing official policies, either expressed
// or implied, of Joshua 1 Systems Inc.
// =====================================================================

using Common.Controls;
using Common.Controls.Theme;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Windows.Forms;
    using Common.Resources;
    using Common.Resources.Properties;
    using VixenModules.Controller.E131;
    using VixenModules.Controller.E131.J1Sys;
    using System.Drawing;
    using System.Linq;
using Common.Controls.Scaling;
using Vixen.Sys;

namespace VixenModules.Output.E131
{
	public partial class SetupForm : BaseForm
    {
        // column indexes - must be changed if column addrange code is changed
        // could refactor to a variable and initialize it at column add time
        // but then it wouldn't work well with switch/case code
        private const int START_COLUMN = 0; 

        private const int ACTIVE_COLUMN = 1;

        private const int UNIVERSE_COLUMN = 2;

        private const int SIZE_COLUMN = 3;

        // plugin channel count as set by vixen
        private readonly SortedList<string, int> badIDs = new SortedList<string, int>();

        private readonly SortedList<string, int> multicasts = new SortedList<string, int>();

        private readonly SortedDictionary<string, string> nicIDs = new SortedDictionary<string, string>();

        private readonly SortedDictionary<string, string> nicNames = new SortedDictionary<string, string>();

        private bool shownSortError = false;
        /// <summary>
        ///   Initializes a new instance of the <see cref = "SetupForm" /> class. 
        ///   Build some nic tables and initialize the component
        /// </summary>
        public SetupForm()
        {
			
            // get all the nics
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                if (networkInterface == null)
                {
                    continue;
                }

                // if not a tunnel
               if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.SupportsMulticast)
                {
                    // then add it to multicasts table by name
                    this.multicasts.Add(networkInterface.Name, 0);

                    // add it to available nicIDs table
                    this.nicIDs.Add(networkInterface.Id, networkInterface.Name);

                    // add it to available nicNames table
                    this.nicNames.Add(networkInterface.Name, networkInterface.Id);
                }
            }

            // finally initialize the form
            InitializeComponent();
	        int iconSize = (int)(16*ScalingTools.GetScaleFactor());
			lblDestination.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 14.25F);
            btnAddUniverse.Text = "";
            btnAddUniverse.Image = Tools.GetIcon(Resources.add, iconSize);
            btnDeleteUniverse.Text = "";
            btnDeleteUniverse.Image = Tools.GetIcon(Resources.delete, iconSize);

            btnAddUnicast.Text = "";
            btnAddUnicast.Image = Tools.GetIcon(Resources.add, iconSize);
            btnDeleteUnicast.Text = "";
            btnDeleteUnicast.Image = Tools.GetIcon(Resources.delete, iconSize);
			SetDestinations();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this, new List<Control>(new []{univDGVN}));
			foreach (Control tab in tabControlEX1.TabPages)
			{
				tab.BackColor = ThemeColorTable.BackgroundColor;
				tab.ForeColor = ThemeColorTable.ForeColor;
			}
			tabControlEX1.SelectedTabColor = ThemeColorTable.BackgroundColor;
			tabControlEX1.TabColor = ThemeColorTable.BackgroundColor;
	        tabControlEX1.SelectedTab = tabPageEX1;
	        tabControlEX1.SizeMode = TabSizeMode.Fixed;
	        SizeF size = ScalingTools.MeasureString(Font, "Advanced Optionsss");
	        tabControlEX1.ItemSize = size.ToSize();
			univDGVN.EnableHeadersVisualStyles = false;
			univDGVN.BackgroundColor = ThemeColorTable.BackgroundColor;
			univDGVN.ForeColor = ThemeColorTable.ForeColor;
	        univDGVN.DefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
	        univDGVN.DefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
	        univDGVN.DefaultCellStyle.SelectionBackColor = ThemeColorTable.ListBoxHighLightColor;
			univDGVN.DefaultCellStyle.SelectionForeColor = ThemeColorTable.ForeColor;
			univDGVN.RowsDefaultCellStyle.BackColor = Color.Empty;
			univDGVN.RowsDefaultCellStyle.ForeColor = Color.Empty;
			univDGVN.ColumnHeadersDefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
			univDGVN.ColumnHeadersDefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
	        univDGVN.RowHeadersDefaultCellStyle.BackColor = Color.Empty;
	        univDGVN.RowHeadersDefaultCellStyle.ForeColor = Color.Empty;
			univDGVN.RowHeadersDefaultCellStyle.SelectionForeColor = Color.Empty;
			univDGVN.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty;
			univDGVN.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			autoPopulateStateUpdate();

        }

		
        public int EventRepeatCount
        {
            get
            {
                int count;

                if (!int.TryParse(this.eventRepeatCountTextBox.Text, out count))
                {
                    count = 0;
                }

                return count;
            }

            set { this.eventRepeatCountTextBox.Text = value.ToString(); }
        }

		public int EventSuppressCount
		{
			get
			{
				int count;

				if (!int.TryParse(this.eventSuppressCountTextBox.Text, out count))
				{
					count = 0;
				}

				return count;
			}

			set { this.eventSuppressCountTextBox.Text = value.ToString(); }
		}

        public bool AutoPopulateStart
        {
            get { return !autoPopulateStart.Checked; }
            set { autoPopulateStart.Checked = !value; autoPopulateStateUpdate(); }
        }

        public bool Blind
        {
            get { return chkBoxTransmitBlind.Checked; }
            set { chkBoxTransmitBlind.Checked = value; }
        }

        public int Priority
        {
            get { return Convert.ToInt16(numericPriority.Value); }
            set { numericPriority.Value = value; }
        }

		public int PluginChannelCount
        {
            set { this.pluginChannelCount = value; }
        }

        public bool StatisticsOption
        {
            get { return this.statisticsCheckBox.Checked; }

            set { this.statisticsCheckBox.Checked = value; }
        }

        public int UniverseCount
        {
            get { return this.univDGVN.Rows.Count; }
        }

        public bool WarningsOption
        {
            get { return this.warningsCheckBox.Checked; }

            set { this.warningsCheckBox.Checked = value; }
        }

        public bool UniverseAdd(
            bool active, int universe, int start, int size)
        {
            // all set, add the row - convert int's to strings ourselves
            this.univDGVN.Rows.Add(
                new object[]
					{start.ToString(), active, universe.ToString(), size.ToString()});
            updateDgvnStartValues();
            return true;
        }

        public void SetDestination(string multicast, string unicast)
        {
            string destination = null;

            // if it is unicast we add the destination to the
            // drop down list if it isn't already there
            // and we 'reformat' to text for display
            if (unicast != null)
            {
                if (!E131OutputPlugin.unicasts.ContainsKey(unicast))
                {
                    E131OutputPlugin.unicasts.Add(unicast, 0);
                }
                destination = "Unicast " + unicast;
            }

            // if it is multicast we check for the id to match
            // a nic. if it doesn't we warn of interface changes
            // and store in bad id's so we only warn once
            if (multicast != null)
            {
                if (this.nicIDs.ContainsKey(multicast))
                {
                    destination = "Multicast " + this.nicIDs[multicast];
                }
                else
                {

                    // then add it to multicasts table by name
                    multicasts.Add("UNKNOWN", 0);

                    // add it to available nicIDs table
                    this.nicIDs.Add(multicast, "UNKNOWN");

                    // add it to available nicNames table
                    this.nicNames.Add("UNKNOWN", multicast);

					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("The selected multicast interface was not found, be sure to verify your destination.", "", false, false);
					messageBox.ShowDialog();

                    destination = "Multicast UNKNOWN";
                }
            }

            SetDestinations();

            if (destination == null)
            {
                if (comboDestination.Items.Contains("Multicast Local Area Connection"))
                    comboDestination.Text = "Multicast Local Area Connection";
            }
            else
            {
                comboDestination.Text = destination;
            }
        }

        /// <summary>
        /// Returns the selected destination as a Tuple <unicast, multicast>
        /// </summary>
        public Tuple<string, string> GetDestination()
        {

            if (comboDestination.Text.StartsWith("Unicast "))
            {
                return Tuple.Create(comboDestination.Text.Substring(8), null as string);
            }
            else if (comboDestination.Text.StartsWith("Multicast "))
            {
                return Tuple.Create(null as string, this.nicNames[comboDestination.Text.Substring(10)]);
            }
            return Tuple.Create(null as string, null as string);
        }

        public void UniverseClear()
        {
            this.univDGVN.Rows.Clear();
        }

        public bool UniverseGet(
            int index,
            ref bool active,
            ref int universe,
            ref int start,
            ref int size)
        {
            var row = this.univDGVN.Rows[index];

            if (row.IsNewRow)
            {
                return false;
            }

            if (row.Cells[ACTIVE_COLUMN].Value == null)
            {
                active = false;
            }
            else
            {
                active = (bool)row.Cells[ACTIVE_COLUMN].Value;
            }

            // all numeric columns are stored as strings
            universe = ((string)row.Cells[UNIVERSE_COLUMN].Value).TryParseInt32(1);
            start = ((string)row.Cells[START_COLUMN].Value).TryParseInt32(1);
            size = ((string)row.Cells[SIZE_COLUMN].Value).TryParseInt32(1);

            return true;
        }

        /*private void UnivDgvnDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[ACTIVE_COLUMN].Value = false;
            e.Row.Cells[START_COLUMN].Value = "1";

            int maxUniverse = 1;

            //try to supply a more useful start value
            foreach(DataGridViewRow r in univDGVN.Rows) {
                if (r.Cells[UNIVERSE_COLUMN].Value != null)
                    if (Convert.ToInt16(e.Row.Cells[UNIVERSE_COLUMN].Value.ToString()) > maxUniverse)
                        maxUniverse = Convert.ToInt16(e.Row.Cells[UNIVERSE_COLUMN].Value.ToString());
            }

            e.Row.Cells[UNIVERSE_COLUMN].Value = maxUniverse;

            e.Row.Cells[SIZE_COLUMN].Value = "1";
        }*/

        private void AddUnicastIp()
        {
            var unicastForm = new UnicastForm();

            if (this.univDGVN.CurrentCell != null)
            {
                if (this.univDGVN.CurrentCell.IsInEditMode)
                {
                    this.univDGVN.EndEdit();
                }
            }

            if (unicastForm.ShowDialog() == DialogResult.OK)
            {

                var ipAddressText = unicastForm.IpAddrText;

                if (E131OutputPlugin.unicasts.ContainsKey(ipAddressText))
                {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Error - Duplicate IP Address",
						"IP Address Validation", false, false);
					messageBox.ShowDialog();
                }
                else
                {
                    E131OutputPlugin.unicasts.Add(ipAddressText, 0);
                    this.SetDestinations();
                }
                comboDestination.SelectedItem = "Unicast " + ipAddressText;
            }
        }

        private void DestinationContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.AddUnicastIp();
        }

        private void eventRepeatCountTextBoxValidating(object sender, CancelEventArgs e)
        {
            int count;

            if (!int.TryParse(((TextBox)sender).Text, out count))
            {
                count = -1;
            }

            if (count < 0 || 99 < count)
            {
                e.Cancel = true;
            }

            if (e.Cancel)
            {
                MessageBeepClass.MessageBeep(MessageBeepClass.BeepType.SimpleBeep);
				Focus();
            }
        }

		private void eventSuppressCountTextBoxValidating(object sender, CancelEventArgs e)
		{
			int count;

			if (!int.TryParse(((TextBox)sender).Text, out count))
			{
				count = -1;
			}

			if (count < 0 || 10000 < count)
			{
				e.Cancel = true;
			}

			if (e.Cancel)
			{
				Focus();
				MessageBeepClass.MessageBeep(MessageBeepClass.BeepType.SimpleBeep);
			}
		}

		/// <summary>
        ///   event handler for a numeric textbox this handler is used by the univDVGN editing control for the numeric columns and by a simple textbox control for numeric only input controls
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void NumTextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }

            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }

            if (e.Handled)
            {
                MessageBeepClass.MessageBeep(MessageBeepClass.BeepType.SimpleBeep);
            }
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            var valid = true;
            var errorList = new StringBuilder();
            var universeDestinations = new SortedList<string, int>();

            // first buid a table of active universe/destination combos
            foreach (DataGridViewRow row in this.univDGVN.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                if (row.Cells[ACTIVE_COLUMN].Value != null)
                {
                    if ((bool)row.Cells[ACTIVE_COLUMN].Value)
                    {
                        var universeDestination = (string)row.Cells[UNIVERSE_COLUMN].Value + ":"
                                                    + comboDestination.Text;
                        if (universeDestinations.ContainsKey(universeDestination))
                        {
                            universeDestinations[universeDestination] = 1;
                        }
                        else
                        {
                            universeDestinations.Add(universeDestination, 0);
                        }
                    }
                }
            }

            // now scan for empty destinations, duplicate universe/destination combos, channels errors, etc.
            foreach (DataGridViewRow row in this.univDGVN.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                // only test if row is active
                if (row.Cells[ACTIVE_COLUMN].Value != null)
                {
                    if ((bool)row.Cells[ACTIVE_COLUMN].Value)
                    {

                        // otherwise, test for duplicate universe/destination combos
                        var universeDestination = (string)row.Cells[UNIVERSE_COLUMN].Value + ":"
                                                    + comboDestination.Text;

                        if (universeDestinations[universeDestination] != 0)
                        {
                            if (!valid)
                            {
                                errorList.Append("\r\n");
                            }

                            errorList.Append("Row ");
                            errorList.Append((row.Index + 1).ToString());
                            errorList.Append(": Duplicate Universe/Destination Combination");
                            valid = false;
                        }
                        

                        // only test for range if more than 0 channels, otherwise wait for runtime
                        if (this.pluginChannelCount > 0)
                        {
                            // now test for valid channel start
                            if (((string)row.Cells[START_COLUMN].Value).TryParseInt32(1) > this.pluginChannelCount)
                            {
                                if (!valid)
                                {
                                    errorList.Append("\r\n");
                                }

                                errorList.Append("Row ");
                                errorList.Append((row.Index + 1).ToString());
                                errorList.Append(": Start Channel Out Of Range");
                                valid = false;
                            }

                            // now test for valid channel size
                            if (((string)row.Cells[START_COLUMN].Value).TryParseInt32(1)
                                + ((string)row.Cells[SIZE_COLUMN].Value).TryParseInt32(1) - 1 > this.pluginChannelCount)
                            {
                                if (!valid)
                                {
                                    errorList.Append("\r\n");
                                }

                                errorList.Append("Row ");
                                errorList.Append((row.Index + 1).ToString());
                                errorList.Append(": Start Channel + Size Out Of Range");
                                valid = false;
                            }
                        }

                    }
                }
            }

            if (!valid)
            {
                if (
                    J1MsgBox.ShowMsg(
                        "Your configurations contains active entries that may cause run time errors.\r\n\r\nHit OK to continue and save your configuration. Hit Cancel to re-edit before saving.",
                        errorList.ToString(),
                        "Configuration Validation",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Error) == DialogResult.OK)
                {
                    valid = true;
                }
            }

            if (valid)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        // -------------------------------------------------------------
        // rowManipulationContextMenuStrip_Opening()
        // we need to gray out a few items based on row, or if
        // it is the 'adding' row cancel the menu
        // -------------------------------------------------------------
        private void RowManipulationContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            var contextMenuStrip = sender as ContextMenuStrip;

            if (contextMenuStrip != null)
            {
                if (this.univDGVNCellEventArgs != null)
                {
                    var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];

                    if (row.IsNewRow)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        // enable/disable move row up
                        contextMenuStrip.Items[3].Enabled = row.Index != 0;

                        // disable move row down
                        contextMenuStrip.Items[4].Enabled = false;

                        // enable move row down if able
                        if (row.Index < this.univDGVN.Rows.Count - 1)
                        {
                            if (!this.univDGVN.Rows[row.Index + 1].IsNewRow)
                            {
                                contextMenuStrip.Items[4].Enabled = true;
                            }
                        }
                    }
                }
            }
        }

        
        private void SetDestinations()
        {
            comboDestination.Items.Clear();

            foreach (var destination in this.multicasts.Keys)
            {
                comboDestination.Items.Add("Multicast " + destination);
             }

            foreach (var ipAddr in E131OutputPlugin.unicasts.Keys)
            {
                comboDestination.Items.Add("Unicast " + ipAddr);
            }
        }

        // -------------------------------------------------------------
        // univDGVN_CellEndEdit() - clear the errortext
        // -------------------------------------------------------------
        private void UnivDgvnCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.univDGVN.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        private void UnivDgvnCellEnter(object sender, DataGridViewCellEventArgs e)
        {
            /*if (e.ColumnIndex == DESTINATION_COLUMN)
            {
                this.univDGVN.BeginEdit(false);
            }*/
        }

        // -------------------------------------------------------------
        // univDGVN_CellMouseClick() - cell mouse click event
        // -------------------------------------------------------------
        private void UnivDgvnCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // if it's the headers - sort 'em
            if (e.RowIndex == -1)
            {
                if (!autoPopulateStart.Checked)
                {
	                if (!shownSortError)
	                {
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm("Sorting is not available while Vixen manages the start values.",
							"", false, false);
						messageBox.ShowDialog();
	                }
                    shownSortError = true;
                } else
                {
                    if (e.ColumnIndex < 5)
                    {
                        var lsd = ListSortDirection.Ascending;

                        if (e.Button == MouseButtons.Right)
                        {
                            lsd = ListSortDirection.Descending;
                        }

                        this.univDGVN.Sort(this.univDGVN.Columns[e.ColumnIndex], lsd);
                    }
                }
            }
        }

        private void UnivDgvnCellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            this.univDGVNCellEventArgs = e;
        }

        private void updateDgvnStartValues()
        {
            int nextStart = 0;
            if (AutoPopulateStart)
            {
                foreach (DataGridViewRow r in univDGVN.Rows)
                {
                    if (r.Index == 0)
                    {
                        r.Cells[START_COLUMN].Value = "1";
                        nextStart = 1 + Convert.ToInt32(r.Cells[SIZE_COLUMN].Value as string);
                    }
                    else
                    {
                        r.Cells[START_COLUMN].Value = nextStart.ToString();
                        nextStart += Convert.ToInt32(r.Cells[SIZE_COLUMN].Value as string);
                    }
                }
            }
        }

        private void UnivDgvnCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var cellValue = e.FormattedValue;
            var cellValueText = cellValue as string;
            var cellValueInt = 0;

            if (cellValueText != null)
            {
                if (!int.TryParse(cellValueText, out cellValueInt))
                {
                    cellValueInt = 0;
                }
            }

            switch (e.ColumnIndex)
            {
                case UNIVERSE_COLUMN:
                    if (cellValueText == null)
                    {
                        e.Cancel = true;
                    }
                    else if (cellValueInt < 1 || 64000 < cellValueInt)
                    {
                        e.Cancel = true;
                    }

                    if (e.Cancel)
                    {
                        this.univDGVN.Rows[e.RowIndex].ErrorText = "Universe must be between 1 and 64000 inclusive";
                    }


                    break;

                case START_COLUMN:
                    if (cellValueText == null)
                    {
                        e.Cancel = true;
                    }
                    else if (cellValueInt < 1 || 99999 < cellValueInt)
                    {
                        e.Cancel = true;
                    }

                    if (e.Cancel)
                    {
                        this.univDGVN.Rows[e.RowIndex].ErrorText = "Start must be between 1 and 99999 inclusive";
                    }

                    break;

                case SIZE_COLUMN:
                    if (cellValueText == null)
                    {
                        e.Cancel = true;
                    }
                    else if (cellValueInt < 1 || 512 < cellValueInt)
                    {
                        e.Cancel = true;
                    }

                    if (e.Cancel)
                    {
                        this.univDGVN.Rows[e.RowIndex].ErrorText = "Size must be between 1 and 512 inclusive";
                    }

                    break;
            }

            if (e.Cancel)
            {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(
					this.univDGVN.Rows[e.RowIndex].ErrorText,
					"Cell Validation", false, false);
				messageBox.ShowDialog();
            }
        }

        private void UnivDgvnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var columnIndex = univDGVN.CurrentCell.ColumnIndex;

            if (columnIndex == UNIVERSE_COLUMN || columnIndex == START_COLUMN || columnIndex == SIZE_COLUMN)
            {
                // first remove the event handler (if previously added)
                e.Control.KeyPress -= this.NumTextBoxKeyPress;

                // now add our event handler
                e.Control.KeyPress += this.NumTextBoxKeyPress;
            }
        }

        private void UnivDgvnInsertRow(object sender, EventArgs e)
        {
            if (this.univDGVNCellEventArgs != null)
            {
                var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];

                if (!row.IsNewRow)
                {
                    this.univDGVN.Rows.Insert(row.Index, new object[] { "1", false, "1", "1", null, "1" });
                }
            }
        }

        private void UnivDgvnMoveRowDown(object sender, EventArgs e)
        {
            if (this.univDGVNCellEventArgs != null)
            {
                var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];
                var rowIndex = row.Index;

                if (!row.IsNewRow)
                {
                    if (rowIndex < this.univDGVN.Rows.Count - 1)
                    {
                        if (!this.univDGVN.Rows[rowIndex + 1].IsNewRow)
                        {
                            this.univDGVN.Rows.RemoveAt(rowIndex);
                            this.univDGVN.Rows.Insert(rowIndex + 1, row);
                        }
                    }
                }
            }
        }

        private void UnivDgvnMoveRowUp(object sender, EventArgs e)
        {
            if (this.univDGVNCellEventArgs != null)
            {
                var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];
                var rowIndex = row.Index;

                if (!row.IsNewRow && rowIndex > 0)
                {
                    this.univDGVN.Rows.RemoveAt(rowIndex);
                    this.univDGVN.Rows.Insert(rowIndex - 1, row);
                }
            }
        }

        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (DialogResult == DialogResult.OK)
            {
	            // only test for range if more than 0 channels, otherwise wait for runtime
				if (pluginChannelCount > 0)
				{
					var valid = true;
					var errorList = new StringBuilder();
					var totalChannels = 0;
					for(int x = 0; x < UniverseCount; x++ )
					{
						bool active = false;
						int start = 0;
						int size = 0;
						int universe = 0;
						UniverseGet(x, ref active, ref universe, ref start, ref size);
						// now test for valid channel start
						if (start > pluginChannelCount)
						{
							if (!valid)
							{
								errorList.Append("\n\n");
							}

							errorList.Append($"Universe {universe} start channel exceeds controller output count of {pluginChannelCount}");
							valid = false;
						}

						// now test for valid channel size
						if (start + size - 1 > pluginChannelCount)
						{
							if (!valid)
							{
								errorList.Append("\n\n");
							}

							errorList.Append($"Universe {universe} {start} + {size} exceeds controller output count of {pluginChannelCount}");
							valid = false;
						}

						totalChannels += size;
				    
					}

					if (totalChannels != pluginChannelCount)
					{
						if (!valid)
						{
							errorList.Append("\n\n");
						}

						errorList.Append($"Total universe channel count of {totalChannels} does not match the controller output count of {pluginChannelCount}.\n\n" +
						                 "Check your universe setup or adjust the amount of output channels to match.");
						valid = false;
					}

					if (!valid)
					{
						var messageBox = new MessageBoxForm($"{errorList}", "Universe Channel / Output count conflict.", MessageBoxButtons.OKCancel, SystemIcons.Warning);
						messageBox.ShowDialog();
						if (messageBox.DialogResult == DialogResult.Cancel)
						{
							e.Cancel = true;
							return;
						}
					}
				}

				bool overlapWarning = false;

				//Validate that a given Vixen input channel doesn't go to multiple sACN output channels
				//Technically nothing bad will happen, but the user should be aware as it is most likely an accident.
				foreach(DataGridViewRow r1 in univDGVN.Rows){
					foreach (DataGridViewRow r2 in univDGVN.Rows)
					{
						if (r1 != r2)
						{
							int r1LowerBound = ((string)r1.Cells[START_COLUMN].Value).TryParseInt32(0);
							int r1UpperBound  = (((string)r1.Cells[START_COLUMN].Value).TryParseInt32(0) + ((string)r1.Cells[SIZE_COLUMN].Value).TryParseInt32(0)-1);
							int r2LowerBound = ((string)r2.Cells[START_COLUMN].Value).TryParseInt32(0);
							int r2UpperBound  = (((string)r2.Cells[START_COLUMN].Value).TryParseInt32(0) + ((string)r2.Cells[SIZE_COLUMN].Value).TryParseInt32(0)-1);

							if ((r1LowerBound >= r2LowerBound && r1LowerBound <= r2UpperBound) || (r1UpperBound >= r2LowerBound && r1UpperBound <= r2UpperBound))
							{

								var messageBox = new MessageBoxForm("The start values seem to be setup in an unusual way. You are sending identical lighting values to multiple sACN outputs. The start value column refers to where a given universe starts reading values in from the list of output channel data from Vixen. For example, setting Universe 1's start value to 5 will map Channel 1 in Universe 1 to output channel #5 in the Vixen controller setup. Would you like to review your settings?", "Warning", MessageBoxButtons.OKCancel, SystemIcons.Warning);
								messageBox.ShowDialog();
								if (messageBox.DialogResult == DialogResult.OK)
								{
									e.Cancel = false;
									return;
								}
								overlapWarning = true;
								break;
							}
						}
					}
					if (overlapWarning) break;
				}

				var destination = new Tuple<string, string>(null, null);
				destination = GetDestination(); //Item1 Unicast, Item2 Multicast

				//prevent duplicates in this plugin instance
				foreach (DataGridViewRow r1 in univDGVN.Rows)
				{
					int univ = ((string)r1.Cells[UNIVERSE_COLUMN].Value).TryParseInt32(1);
					foreach(DataGridViewRow r2 in univDGVN.Rows){
						if(r1 != r2 && univ == ((string)r2.Cells[UNIVERSE_COLUMN].Value).TryParseInt32(1)){
							//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
							MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
							var messageBox = new MessageBoxForm("Universe numbers must be unique.", "Error", false, false);
							messageBox.ShowDialog();
							e.Cancel = true;
							return;
						}
					}
				}

				//Validate that the same universe isn't being broadcasted to the same devices from multiple
				//instances of the plugin
				foreach (E131OutputPlugin p in E131OutputPlugin.PluginInstances)
				{
					if (p.isSetupOpen) //don't validate against this instance of the plugin
						continue;

					//Conditions which we need to validate for overlap
					if(
						!(((p.ModuleData as E131ModuleDataModel).Unicast != null && destination.Item1 != null && (p.ModuleData as E131ModuleDataModel).Unicast != destination.Item1) //unicasting to different IPs
						  || ((p.ModuleData as E131ModuleDataModel).Multicast != null && destination.Item2 != null && (p.ModuleData as E131ModuleDataModel).Multicast != destination.Item2))) //Multicasting to different networks
					{
						int[] usedUniverses = (p.ModuleData as E131ModuleDataModel).Universes.Select(x => x.Universe).ToArray();

						for (int i = 0; i < UniverseCount; i++)
						{
							bool active = true;
							int universe = 0;
							int start = 0;
							int size = 0;

							if (UniverseGet(
								i, ref active, ref universe, ref start, ref size))
							{
								if (usedUniverses.Contains(universe))
								{
									//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
									MessageBoxForm.msgIcon = SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
									var messageBox = new MessageBoxForm(string.Format("Universe {0} already exists on another controller transmitting to the same device or network. Please configure a different universe to prevent hardware errors.", universe), "Existing Universe", false, false);
									messageBox.ShowDialog();
									e.Cancel = true;
									return;
								}
							}
						}
					}
				}
            }
		}

        private void autoPopulateStart_CheckedChanged(object sender, EventArgs e)
        {
            autoPopulateStateUpdate();
        }

        private void univDGVN_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case START_COLUMN:
                case SIZE_COLUMN:
                    updateDgvnStartValues();
                    break;
            }
        }

        private void autoPopulateStateUpdate()
        {
            univDGVN.Columns[START_COLUMN].ReadOnly = !autoPopulateStart.Checked;
            foreach (DataGridViewColumn c in univDGVN.Columns)
                if (!autoPopulateStart.Checked)
                    c.ToolTipText = "";
                else
                    c.ToolTipText = "Sort (LeftClick = Ascending, RightClick = Descending)";

            if (!autoPopulateStart.Checked)
            {
	            var style = univDGVN.Columns[START_COLUMN].DefaultCellStyle;
				style.BackColor = ThemeColorTable.ListBoxHighLightColor;
				style.Font = new Font(univDGVN.Columns[START_COLUMN].DefaultCellStyle.Font ?? SystemFonts.MessageBoxFont, FontStyle.Italic);
	            univDGVN.Columns[START_COLUMN].DefaultCellStyle = style;
            }
            else
            {
				var style = univDGVN.Columns[START_COLUMN].DefaultCellStyle;
				style.BackColor = ThemeColorTable.BackgroundColor;
                style.Font = new Font(univDGVN.Columns[START_COLUMN].DefaultCellStyle.Font ?? SystemFonts.MessageBoxFont, FontStyle.Regular);
				univDGVN.Columns[START_COLUMN].DefaultCellStyle = style;
            }

            updateDgvnStartValues();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int maxUniverse = 0;
			object universeSize = univDGVN.RowCount > 0
		        ? univDGVN.Rows[univDGVN.Rows.Count - 1].Cells[SIZE_COLUMN].Value
		        : "510";
	        //try to supply a more useful start value
            foreach (DataGridViewRow r in univDGVN.Rows)
            {
                if (r.Cells[UNIVERSE_COLUMN].Value != null)
                    if (Convert.ToInt16(r.Cells[UNIVERSE_COLUMN].Value.ToString()) > maxUniverse)
                        maxUniverse = Convert.ToInt16(r.Cells[UNIVERSE_COLUMN].Value.ToString());
            }
            maxUniverse++;
            this.univDGVN.Rows.Add(
			new object[] { 0, true, maxUniverse.ToString(), universeSize });
            updateDgvnStartValues();

            foreach (DataGridViewRow r in univDGVN.Rows)
                r.Selected = false;
            univDGVN.Rows[univDGVN.Rows.Count-1].Selected = true;
			univDGVN.FirstDisplayedScrollingRowIndex = univDGVN.RowCount -1;
        }

        private void UnivDgvnDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            updateDgvnStartValues();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> toDelete = new List<DataGridViewRow>();
            foreach (DataGridViewRow r in univDGVN.Rows)
                foreach (DataGridViewCell c in r.Cells)
                    if (c.Selected)
                    {
                        toDelete.Add(r);
                        break;
                    }

            foreach (DataGridViewRow r in toDelete)
                univDGVN.Rows.Remove(r);
            updateDgvnStartValues();
        }

        private void UnivDgvnInsertRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        private void univDGVN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Delete)
                btnDelete_Click(null, null);
        }

        private void univDGVN_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                btnDelete_Click(null, null);
        }

        private void btnAddUnicast_Click_1(object sender, EventArgs e)
        {
            AddUnicastIp();
        }

        private void btnRemoveUnicast_Click(object sender, EventArgs e)
        {
            if (comboDestination.SelectedItem.ToString().StartsWith("Multicast"))
            {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Multicast destinations cannot be removed.", "Streaming ACN (E1.31)", false, false);
				messageBox.ShowDialog();
                return;
            }

            //Validate that the unicast destination isn't in use by another instance of the plugin

            var destination = new Tuple<string, string>(null, null);
            destination = GetDestination(); //Item1 Unicast, Item2 Multicast

            foreach (E131OutputPlugin p in E131OutputPlugin.PluginInstances)
            {
                if (p.isSetupOpen) //don't validate against this instance of the plugin
                    continue;

                if ((p.ModuleData as E131ModuleDataModel).Unicast == destination.Item1)
                {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("This destination is in use by another instance of the plugin and cannot be removed.", "Streaming ACN (E1.31)", false, false);
					messageBox.ShowDialog();
                    return;
                }
            }

            E131OutputPlugin.unicasts.Remove(destination.Item1);
            SetDestinations();
            comboDestination.SelectedIndex = 0;
        }

        private void comboDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboDestination.SelectedItem.ToString().StartsWith("Multicast"))
                btnDeleteUnicast.Enabled = false;
            else
                btnDeleteUnicast.Enabled = true;
        }

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

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
    }
}