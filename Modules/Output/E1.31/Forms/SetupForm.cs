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

namespace VixenModules.Output.E131
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Windows.Forms;
    using VixenModules.Output.E131.J1Sys;

    public partial class SetupForm : Form
    {
        // column indexes - must be changed if column addrange code is changed
        // could refactor to a variable and initialize it at column add time
        // but then it wouldn't work well with switch/case code
        private const int ACTIVE_COLUMN = 0;

        private const int DESTINATION_COLUMN = 4;

        private const int SIZE_COLUMN = 3;

        private const int START_COLUMN = 2;

        private const int TTL_COLUMN = 5;

        private const int UNIVERSE_COLUMN = 1;

        // plugin channel count as set by vixen
        private readonly SortedList<string, int> _badIds = new SortedList<string, int>();

        private readonly SortedList<string, int> _multicasts = new SortedList<string, int>();

        private readonly SortedDictionary<string, string> _nicIds = new SortedDictionary<string, string>();

        private readonly SortedDictionary<string, string> _nicNames = new SortedDictionary<string, string>();

        private readonly SortedList<string, int> _unicasts = new SortedList<string, int>();
        private readonly Data _data;
        private IEnumerable<NetworkInterface> _networkInterfaces;

        public SetupForm(Data data, IEnumerable<NetworkInterface> networkInterfaces)
        {
            _data = data;
            _networkInterfaces = networkInterfaces;

            // finally initialize the form
            InitializeComponent();
        }

        public int EventRepeatCount
        {
            get
            {
                int count;

                if (!int.TryParse(eventRepeatCountTextBox.Text, out count))
                {
                    count = 0;
                }

                return count;
            }

            set
            {
                eventRepeatCountTextBox.Text = value.ToString();
            }
        }

        public int PluginChannelCount
        {
            set
            {
                pluginChannelCount = value;
            }
        }

        public bool StatisticsOption
        {
            get
            {
                return statisticsCheckBox.Checked;
            }

            set
            {
                statisticsCheckBox.Checked = value;
            }
        }

        public int UniverseCount
        {
            get
            {
                return univDGVN.Rows.Count;
            }
        }

        public bool WarningsOption
        {
            get
            {
                return warningsCheckBox.Checked;
            }

            set
            {
                warningsCheckBox.Checked = value;
            }
        }

        public bool UniverseAdd(bool active, int universe, int start, int size, string unicast, string multicast, int ttl)
        {
            string destination = null;

            // if it is unicast we add the destination to the
            // drop down list if it isn't already there
            // and we 'reformat' to text for display
            if (unicast != null)
            {
                if (!_unicasts.ContainsKey(unicast))
                {
                    _unicasts.Add(unicast, 0);
                    destinationColumn.Items.Add("Unicast " + unicast);
                }

                destination = "Unicast " + unicast;
            }

            // if it is multicast we check for the id to match
            // a nic. if it doesn't we warn of interface changes
            // and store in bad id's so we only warn once
            if (multicast != null)
            {
                if (_nicIds.ContainsKey(multicast))
                {
                    destination = "Multicast " + _nicIds[multicast];
                }
                else
                {
                    if (!_badIds.ContainsKey(multicast))
                    {
                        _badIds.Add(multicast, 0);
                        MessageBox.Show(
                                        "Warning - Interface IDs have changed. Please reselect all empty destinations.", 
                                        "Network Interface Mapping", 
                                        MessageBoxButtons.OK, 
                                        MessageBoxIcon.Warning);
                    }
                }
            }

            // all set, add the row - convert int's to strings ourselves
            univDGVN.Rows.Add(
                              new object[]
                              {
                                 active, universe.ToString(), start.ToString(), size.ToString(), destination, ttl.ToString() 
                              });
            return true;
        }

        public void UniverseClear()
        {
            univDGVN.Rows.Clear();
        }

        public bool UniverseGet(
            int index, 
            ref bool active, 
            ref int universe, 
            ref int start, 
            ref int size, 
            ref string unicast, 
            ref string multicast, 
            ref int ttl)
        {
            var row = univDGVN.Rows[index];
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
            ttl = ((string)row.Cells[TTL_COLUMN].Value).TryParseInt32(1);

            // first set both unicast and multicast results to null
            unicast = null;
            multicast = null;

            // then set the selected unicast/multicast destination
            if (row.Cells[DESTINATION_COLUMN].Value != null)
            {
                var destination = (string)row.Cells[DESTINATION_COLUMN].Value;
                if (destination.StartsWith("Unicast "))
                {
                    unicast = destination.Substring(8);
                }
                else if (destination.StartsWith("Multicast "))
                {
                    multicast = _nicNames[destination.Substring(10)];
                }
            }

            return true;
        }

        private static void UnivDgvnDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[ACTIVE_COLUMN].Value = false;
            e.Row.Cells[UNIVERSE_COLUMN].Value = "1";
            e.Row.Cells[START_COLUMN].Value = "1";
            e.Row.Cells[SIZE_COLUMN].Value = "1";
            e.Row.Cells[TTL_COLUMN].Value = "1";
        }

        private void AddUnicastIp()
        {
            using (var unicastForm = new UnicastForm())
            {
                if (univDGVN.CurrentCell != null)
                {
                    if (univDGVN.CurrentCell.IsInEditMode)
                    {
                        univDGVN.EndEdit();
                    }
                }

                if (unicastForm.ShowDialog()
                    == DialogResult.OK)
                {
                    IPAddress ipAddress;
                    var valid = IPAddress.TryParse(unicastForm.IpAddrText, out ipAddress);

                    if (valid)
                    {
                        var ipBytes = ipAddress.GetAddressBytes();

                        if (ipBytes[0] == 0 && ipBytes[1] == 0 && ipBytes[2] == 0
                            && ipBytes[3] == 0)
                        {
                            valid = false;
                        }

                        if (ipBytes[0] == 255 && ipBytes[1] == 255 && ipBytes[2] == 255
                            && ipBytes[3] == 255)
                        {
                            valid = false;
                        }
                    }

                    if (!valid)
                    {
                        MessageBox.Show(
                                        "Error - Invalid IP Address", 
                                        "IP Address Validation", 
                                        MessageBoxButtons.OK, 
                                        MessageBoxIcon.Error);
                    }
                    else
                    {
                        var ipAddressText = ipAddress.ToString();

                        if (_unicasts.ContainsKey(ipAddressText))
                        {
                            MessageBox.Show(
                                            "Error - Duplicate IP Address", 
                                            "IP Address Validation", 
                                            MessageBoxButtons.OK, 
                                            MessageBoxIcon.Error);
                        }
                        else
                        {
                            _unicasts.Add(ipAddressText, 0);
                            SetDestinations();
                        }
                    }
                }
            }
        }

        private void DestinationContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            AddUnicastIp();
        }

        private void EventRepeatCountTextBoxValidating(object sender, CancelEventArgs e)
        {
            int count;

            if (!int.TryParse(((TextBox)sender).Text, out count))
            {
                count = 0;
            }

            if (count < 0
                || 99 < count)
            {
                e.Cancel = true;
            }

            if (e.Cancel)
            {
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
            foreach (DataGridViewRow row in univDGVN.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                if (row.Cells[ACTIVE_COLUMN].Value != null)
                {
                    if ((bool)row.Cells[ACTIVE_COLUMN].Value)
                    {
                        if (row.Cells[DESTINATION_COLUMN].Value != null)
                        {
                            var universeDestination = (string)row.Cells[UNIVERSE_COLUMN].Value + ":"
                                                      + (string)row.Cells[DESTINATION_COLUMN].Value;
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
            }

            // now scan for empty destinations, duplicate universe/destination combos, channels errors, etc.
            foreach (DataGridViewRow row in univDGVN.Rows)
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
                        // test for null destinations
                        if (row.Cells[DESTINATION_COLUMN].Value == null)
                        {
                            if (!valid)
                            {
                                errorList.Append("\r\n");
                            }

                            errorList.Append("Row ");
                            errorList.Append((row.Index + 1).ToString());
                            errorList.Append(": No Destination Selected");
                            valid = false;
                        }
                        else
                        {
                            // otherwise, test for duplicate universe/destination combos
                            var universeDestination = (string)row.Cells[UNIVERSE_COLUMN].Value + ":"
                                                      + (string)row.Cells[DESTINATION_COLUMN].Value;

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
                        }

                        // only test for range if more than 0 channels, otherwise wait for runtime
                        if (pluginChannelCount > 0)
                        {
                            // now test for valid channel start
                            if (((string)row.Cells[START_COLUMN].Value).TryParseInt32(1) > pluginChannelCount)
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
                                + ((string)row.Cells[SIZE_COLUMN].Value).TryParseInt32(1) - 1 > pluginChannelCount)
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

                        // now test for ttl value
                        if (((string)row.Cells[TTL_COLUMN].Value).TryParseInt32(1) == 0)
                        {
                            if (!valid)
                            {
                                errorList.Append("\r\n");
                            }

                            errorList.Append("Row ");
                            errorList.Append((row.Index + 1).ToString());
                            errorList.Append(": Warning - Zero TTL");
                            valid = false;
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
                                     MessageBoxIcon.Error)
                    == DialogResult.OK)
                {
                    valid = true;
                }
            }

            if (valid)
            {
                DialogResult = DialogResult.OK;
                Close();
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
                if (univDGVNCellEventArgs != null)
                {
                    var row = univDGVN.Rows[univDGVNCellEventArgs.RowIndex];

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
                        if (row.Index
                            < univDGVN.Rows.Count - 1)
                        {
                            if (!univDGVN.Rows[row.Index + 1].IsNewRow)
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
            destinationColumn.Items.Clear();

            foreach (var destination in _multicasts.Keys)
            {
                destinationColumn.Items.Add("Multicast " + destination);
            }

            foreach (var ipAddr in _unicasts.Keys)
            {
                destinationColumn.Items.Add("Unicast " + ipAddr);
            }
        }

        // -------------------------------------------------------------
        // univDGVN_CellEndEdit() - clear the errortext
        // -------------------------------------------------------------
        private void UnivDgvnCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            univDGVN.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        private void UnivDgvnCellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == DESTINATION_COLUMN)
            {
                univDGVN.BeginEdit(false);
            }
        }

        // -------------------------------------------------------------
        // univDGVN_CellMouseClick() - cell mouse click event
        // -------------------------------------------------------------
        private void UnivDgvnCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // if it's the headers - sort 'em
            if (e.RowIndex
                == -1)
            {
                if (0 < e.ColumnIndex
                    && e.ColumnIndex < 5)
                {
                    var lsd = ListSortDirection.Ascending;

                    if (e.Button
                        == MouseButtons.Right)
                    {
                        lsd = ListSortDirection.Descending;
                    }

                    univDGVN.Sort(univDGVN.Columns[e.ColumnIndex], lsd);
                }
            }

                // if it's the rows - handle specials
            else
            {
                // if it's the right button
                if (e.Button
                    == MouseButtons.Right)
                {
                    // if it's the destination column - they want to add a unicast ip
                    if (e.ColumnIndex == DESTINATION_COLUMN)
                    {
                        AddUnicastIp();
                    }
                }
            }
        }

        private void UnivDgvnCellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            univDGVNCellEventArgs = e;
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
                    else if (cellValueInt < 1
                             || 64000 < cellValueInt)
                    {
                        e.Cancel = true;
                    }

                    if (e.Cancel)
                    {
                        univDGVN.Rows[e.RowIndex].ErrorText = "Universe must be between 1 and 64000 inclusive";
                    }

                    break;

                case START_COLUMN:
                    if (cellValueText == null)
                    {
                        e.Cancel = true;
                    }
                    else if (cellValueInt < 1
                             || 99999 < cellValueInt)
                    {
                        e.Cancel = true;
                    }

                    if (e.Cancel)
                    {
                        univDGVN.Rows[e.RowIndex].ErrorText = "Start must be between 1 and 99999 inclusive";
                    }

                    break;

                case SIZE_COLUMN:
                    if (cellValueText == null)
                    {
                        e.Cancel = true;
                    }
                    else if (cellValueInt < 1
                             || 512 < cellValueInt)
                    {
                        e.Cancel = true;
                    }

                    if (e.Cancel)
                    {
                        univDGVN.Rows[e.RowIndex].ErrorText = "Size must be between 1 and 512 inclusive";
                    }

                    break;

                case TTL_COLUMN:
                    if (cellValueText == null)
                    {
                        e.Cancel = true;
                    }
                    else if (cellValueInt < 0
                             || 99 < cellValueInt)
                    {
                        e.Cancel = true;
                    }

                    if (e.Cancel)
                    {
                        univDGVN.Rows[e.RowIndex].ErrorText = "TTL must be between 0 and 99 inclusive";
                    }

                    break;
            }

            if (e.Cancel)
            {
                MessageBox.Show(
                                univDGVN.Rows[e.RowIndex].ErrorText, 
                                "Cell Validation", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Exclamation);
            }
        }

        private void UnivDgvnDeleteRow(object sender, EventArgs e)
        {
            if (univDGVNCellEventArgs != null)
            {
                var row = univDGVN.Rows[univDGVNCellEventArgs.RowIndex];

                if (!row.IsNewRow)
                {
                    univDGVN.Rows.RemoveAt(row.Index);
                }
            }
        }

        private void UnivDgvnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var columnIndex = univDGVN.CurrentCell.ColumnIndex;

            if (columnIndex == UNIVERSE_COLUMN || columnIndex == START_COLUMN || columnIndex == SIZE_COLUMN
                || columnIndex == TTL_COLUMN)
            {
                // first remove the event handler (if previously added)
                e.Control.KeyPress -= NumTextBoxKeyPress;

                // now add our event handler
                e.Control.KeyPress += NumTextBoxKeyPress;
            }

            if (columnIndex == DESTINATION_COLUMN)
            {
                var control = e.Control as DataGridViewComboBoxEditingControl;

                if (control != null)
                {
                    if (destinationToolTip == null)
                    {
                        destinationToolTip = new ToolTip();
                    }

                    destinationToolTip.SetToolTip(e.Control, "RightClick to add a new Unicast IP Address");

                    if (destinationContextMenuStrip == null)
                    {
                        destinationContextMenuStrip = new ContextMenuStrip();
                        destinationContextMenuStrip.Opening += DestinationContextMenuStripOpening;
                    }

                    control.ContextMenuStrip = destinationContextMenuStrip;
                }
            }
        }

        private void UnivDgvnInsertRow(object sender, EventArgs e)
        {
            if (univDGVNCellEventArgs != null)
            {
                var row = univDGVN.Rows[univDGVNCellEventArgs.RowIndex];

                if (!row.IsNewRow)
                {
                    univDGVN.Rows.Insert(row.Index, new object[] { false, "1", "1", "1", null, "1" });
                }
            }
        }

        private void UnivDgvnMoveRowDown(object sender, EventArgs e)
        {
            if (univDGVNCellEventArgs != null)
            {
                var row = univDGVN.Rows[univDGVNCellEventArgs.RowIndex];
                var rowIndex = row.Index;

                if (!row.IsNewRow)
                {
                    if (rowIndex < univDGVN.Rows.Count - 1)
                    {
                        if (!univDGVN.Rows[rowIndex + 1].IsNewRow)
                        {
                            univDGVN.Rows.RemoveAt(rowIndex);
                            univDGVN.Rows.Insert(rowIndex + 1, row);
                        }
                    }
                }
            }
        }

        private void UnivDgvnMoveRowUp(object sender, EventArgs e)
        {
            if (univDGVNCellEventArgs != null)
            {
                var row = univDGVN.Rows[univDGVNCellEventArgs.RowIndex];
                var rowIndex = row.Index;
                if (!row.IsNewRow
                    && rowIndex > 0)
                {
                    univDGVN.Rows.RemoveAt(rowIndex);
                    univDGVN.Rows.Insert(rowIndex - 1, row);
                }
            }
        }
    }
}
