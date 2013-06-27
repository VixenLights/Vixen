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
	using VixenModules.Controller.E131;
	using VixenModules.Controller.E131.J1Sys;

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
		private readonly SortedList<string, int> badIDs = new SortedList<string, int>();

		private readonly SortedList<string, int> multicasts = new SortedList<string, int>();

		private readonly SortedDictionary<string, string> nicIDs = new SortedDictionary<string, string>();

		private readonly SortedDictionary<string, string> nicNames = new SortedDictionary<string, string>();

		private readonly SortedList<string, int> unicasts = new SortedList<string, int>();

		/// <summary>
		///   Initializes a new instance of the <see cref = "SetupForm" /> class. 
		///   Build some nic tables and initialize the component
		/// </summary>
		public SetupForm()
		{
			// first build some sorted lists and dictionaries for the nics

			// get all the nics
			var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (var networkInterface in networkInterfaces) {
				if (networkInterface == null) {
					continue;
				}

				// if not a tunnel
				if (networkInterface.NetworkInterfaceType.CompareTo(NetworkInterfaceType.Tunnel) != 0) {
					// and supports multicast
					if (networkInterface.SupportsMulticast) {
						// then add it to multicasts table by name
						this.multicasts.Add(networkInterface.Name, 0);

						// add it to available nicIDs table
						this.nicIDs.Add(networkInterface.Id, networkInterface.Name);

						// add ot to available nicNames table
						this.nicNames.Add(networkInterface.Name, networkInterface.Id);
					}
				}
			}

			// finally initialize the form
			this.InitializeComponent();
			this.SetDestinations();
		}

		public int EventRepeatCount
		{
			get
			{
				int count;

				if (!int.TryParse(this.eventRepeatCountTextBox.Text, out count)) {
					count = 0;
				}

				return count;
			}

			set { this.eventRepeatCountTextBox.Text = value.ToString(); }
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
			bool active, int universe, int start, int size, string unicast, string multicast, int ttl)
		{
			string destination = null;

			// if it is unicast we add the destination to the
			// drop down list if it isn't already there
			// and we 'reformat' to text for display
			if (unicast != null) {
				if (!this.unicasts.ContainsKey(unicast)) {
					this.unicasts.Add(unicast, 0);
					this.destinationColumn.Items.Add("Unicast " + unicast);
				}

				destination = "Unicast " + unicast;
			}

			// if it is multicast we check for the id to match
			// a nic. if it doesn't we warn of interface changes
			// and store in bad id's so we only warn once
			if (multicast != null) {
				if (this.nicIDs.ContainsKey(multicast)) {
					destination = "Multicast " + this.nicIDs[multicast];
				}
				else {
					if (!this.badIDs.ContainsKey(multicast)) {
						this.badIDs.Add(multicast, 0);
						MessageBox.Show(
							"Warning - Interface IDs have changed. Please reselect all empty destinations.",
							"Network Interface Mapping",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning);
					}
				}
			}

			// all set, add the row - convert int's to strings ourselves
			this.univDGVN.Rows.Add(
				new object[]
					{
						active, universe.ToString(), start.ToString(), size.ToString(), destination, ttl.ToString()
					});
			return true;
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
			ref int size,
			ref string unicast,
			ref string multicast,
			ref int ttl)
		{
			var row = this.univDGVN.Rows[index];

			if (row.IsNewRow) {
				return false;
			}

			if (row.Cells[ACTIVE_COLUMN].Value == null) {
				active = false;
			}
			else {
				active = (bool) row.Cells[ACTIVE_COLUMN].Value;
			}

			// all numeric columns are stored as strings
			universe = ((string) row.Cells[UNIVERSE_COLUMN].Value).TryParseInt32(1);
			start = ((string) row.Cells[START_COLUMN].Value).TryParseInt32(1);
			size = ((string) row.Cells[SIZE_COLUMN].Value).TryParseInt32(1);
			ttl = ((string) row.Cells[TTL_COLUMN].Value).TryParseInt32(1);

			// first set both unicast and multicast results to null
			unicast = null;
			multicast = null;

			// then set the selected unicast/multicast destination
			if (row.Cells[DESTINATION_COLUMN].Value != null) {
				var destination = (string) row.Cells[DESTINATION_COLUMN].Value;
				if (destination.StartsWith("Unicast ")) {
					unicast = destination.Substring(8);
				}
				else if (destination.StartsWith("Multicast ")) {
					multicast = this.nicNames[destination.Substring(10)];
				}
			}

			return true;
		}

		private void UnivDgvnDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
		{
			e.Row.Cells[ACTIVE_COLUMN].Value = false;
			e.Row.Cells[UNIVERSE_COLUMN].Value = "1";
			e.Row.Cells[START_COLUMN].Value = "1";
			e.Row.Cells[SIZE_COLUMN].Value = "1";
			e.Row.Cells[TTL_COLUMN].Value = "1";
		}

		private void AddUnicastIp()
		{
			var unicastForm = new UnicastForm();

			if (this.univDGVN.CurrentCell != null) {
				if (this.univDGVN.CurrentCell.IsInEditMode) {
					this.univDGVN.EndEdit();
				}
			}

			if (unicastForm.ShowDialog() == DialogResult.OK) {
				IPAddress ipAddress;
				bool valid = IPAddress.TryParse(unicastForm.IpAddrText, out ipAddress);

				if (valid) {
					var ipBytes = ipAddress.GetAddressBytes();

					if (ipBytes[0] == 0 && ipBytes[1] == 0 && ipBytes[2] == 0 && ipBytes[3] == 0) {
						valid = false;
					}

					if (ipBytes[0] == 255 && ipBytes[1] == 255 && ipBytes[2] == 255 && ipBytes[3] == 255) {
						valid = false;
					}
				}

				if (!valid) {
					MessageBox.Show(
						"Error - Invalid IP Address",
						"IP Address Validation",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
				else {
					var ipAddressText = ipAddress.ToString();

					if (this.unicasts.ContainsKey(ipAddressText)) {
						MessageBox.Show(
							"Error - Duplicate IP Address",
							"IP Address Validation",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
					else {
						this.unicasts.Add(ipAddressText, 0);
						this.SetDestinations();
					}
				}
			}
		}

		private void DestinationContextMenuStripOpening(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			this.AddUnicastIp();
		}

		private void EventRepeatCountTextBoxValidating(object sender, CancelEventArgs e)
		{
			int count;

			if (!int.TryParse(((TextBox) sender).Text, out count)) {
				count = 0;
			}

			if (count < 0 || 99 < count) {
				e.Cancel = true;
			}

			if (e.Cancel) {
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

			if (char.IsControl(e.KeyChar)) {
				e.Handled = false;
			}

			if (char.IsDigit(e.KeyChar)) {
				e.Handled = false;
			}

			if (e.Handled) {
				MessageBeepClass.MessageBeep(MessageBeepClass.BeepType.SimpleBeep);
			}
		}

		private void OkButtonClick(object sender, EventArgs e)
		{
			var valid = true;
			var errorList = new StringBuilder();
			var universeDestinations = new SortedList<string, int>();

			// first buid a table of active universe/destination combos
			foreach (DataGridViewRow row in this.univDGVN.Rows) {
				if (row.IsNewRow) {
					continue;
				}

				if (row.Cells[ACTIVE_COLUMN].Value != null) {
					if ((bool) row.Cells[ACTIVE_COLUMN].Value) {
						if (row.Cells[DESTINATION_COLUMN].Value != null) {
							var universeDestination = (string) row.Cells[UNIVERSE_COLUMN].Value + ":"
							                          + (string) row.Cells[DESTINATION_COLUMN].Value;
							if (universeDestinations.ContainsKey(universeDestination)) {
								universeDestinations[universeDestination] = 1;
							}
							else {
								universeDestinations.Add(universeDestination, 0);
							}
						}
					}
				}
			}

			// now scan for empty destinations, duplicate universe/destination combos, channels errors, etc.
			foreach (DataGridViewRow row in this.univDGVN.Rows) {
				if (row.IsNewRow) {
					continue;
				}

				// only test if row is active
				if (row.Cells[ACTIVE_COLUMN].Value != null) {
					if ((bool) row.Cells[ACTIVE_COLUMN].Value) {
						// test for null destinations
						if (row.Cells[DESTINATION_COLUMN].Value == null) {
							if (!valid) {
								errorList.Append("\r\n");
							}

							errorList.Append("Row ");
							errorList.Append((row.Index + 1).ToString());
							errorList.Append(": No Destination Selected");
							valid = false;
						}
						else {
							// otherwise, test for duplicate universe/destination combos
							var universeDestination = (string) row.Cells[UNIVERSE_COLUMN].Value + ":"
							                          + (string) row.Cells[DESTINATION_COLUMN].Value;

							if (universeDestinations[universeDestination] != 0) {
								if (!valid) {
									errorList.Append("\r\n");
								}

								errorList.Append("Row ");
								errorList.Append((row.Index + 1).ToString());
								errorList.Append(": Duplicate Universe/Destination Combination");
								valid = false;
							}
						}

						// only test for range if more than 0 channels, otherwise wait for runtime
						if (this.pluginChannelCount > 0) {
							// now test for valid channel start
							if (((string) row.Cells[START_COLUMN].Value).TryParseInt32(1) > this.pluginChannelCount) {
								if (!valid) {
									errorList.Append("\r\n");
								}

								errorList.Append("Row ");
								errorList.Append((row.Index + 1).ToString());
								errorList.Append(": Start Channel Out Of Range");
								valid = false;
							}

							// now test for valid channel size
							if (((string) row.Cells[START_COLUMN].Value).TryParseInt32(1)
							    + ((string) row.Cells[SIZE_COLUMN].Value).TryParseInt32(1) - 1 > this.pluginChannelCount) {
								if (!valid) {
									errorList.Append("\r\n");
								}

								errorList.Append("Row ");
								errorList.Append((row.Index + 1).ToString());
								errorList.Append(": Start Channel + Size Out Of Range");
								valid = false;
							}
						}

						// now test for ttl value
						if (((string) row.Cells[TTL_COLUMN].Value).TryParseInt32(1) == 0) {
							if (!valid) {
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

			if (!valid) {
				if (
					J1MsgBox.ShowMsg(
						"Your configurations contains active entries that may cause run time errors.\r\n\r\nHit OK to continue and save your configuration. Hit Cancel to re-edit before saving.",
						errorList.ToString(),
						"Configuration Validation",
						MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error) == DialogResult.OK) {
					valid = true;
				}
			}

			if (valid) {
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

			if (contextMenuStrip != null) {
				if (this.univDGVNCellEventArgs != null) {
					var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];

					if (row.IsNewRow) {
						e.Cancel = true;
					}
					else {
						// enable/disable move row up
						contextMenuStrip.Items[3].Enabled = row.Index != 0;

						// disable move row down
						contextMenuStrip.Items[4].Enabled = false;

						// enable move row down if able
						if (row.Index < this.univDGVN.Rows.Count - 1) {
							if (!this.univDGVN.Rows[row.Index + 1].IsNewRow) {
								contextMenuStrip.Items[4].Enabled = true;
							}
						}
					}
				}
			}
		}

		private void SetDestinations()
		{
			this.destinationColumn.Items.Clear();

			foreach (var destination in this.multicasts.Keys) {
				this.destinationColumn.Items.Add("Multicast " + destination);
			}

			foreach (var ipAddr in this.unicasts.Keys) {
				this.destinationColumn.Items.Add("Unicast " + ipAddr);
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
			if (e.ColumnIndex == DESTINATION_COLUMN) {
				this.univDGVN.BeginEdit(false);
			}
		}

		// -------------------------------------------------------------
		// univDGVN_CellMouseClick() - cell mouse click event
		// -------------------------------------------------------------
		private void UnivDgvnCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			// if it's the headers - sort 'em
			if (e.RowIndex == -1) {
				if (0 < e.ColumnIndex && e.ColumnIndex < 5) {
					var lsd = ListSortDirection.Ascending;

					if (e.Button == MouseButtons.Right) {
						lsd = ListSortDirection.Descending;
					}

					this.univDGVN.Sort(this.univDGVN.Columns[e.ColumnIndex], lsd);
				}
			}

				// if it's the rows - handle specials
			else {
				// if it's the right button
				if (e.Button == MouseButtons.Right) {
					// if it's the destination column - they want to add a unicast ip
					if (e.ColumnIndex == DESTINATION_COLUMN) {
						this.AddUnicastIp();
					}
				}
			}
		}

		private void UnivDgvnCellMouseEnter(object sender, DataGridViewCellEventArgs e)
		{
			this.univDGVNCellEventArgs = e;
		}

		private void UnivDgvnCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			var cellValue = e.FormattedValue;
			var cellValueText = cellValue as string;
			var cellValueInt = 0;

			if (cellValueText != null) {
				if (!int.TryParse(cellValueText, out cellValueInt)) {
					cellValueInt = 0;
				}
			}

			switch (e.ColumnIndex) {
				case UNIVERSE_COLUMN:
					if (cellValueText == null) {
						e.Cancel = true;
					}
					else if (cellValueInt < 1 || 64000 < cellValueInt) {
						e.Cancel = true;
					}

					if (e.Cancel) {
						this.univDGVN.Rows[e.RowIndex].ErrorText = "Universe must be between 1 and 64000 inclusive";
					}

					break;

				case START_COLUMN:
					if (cellValueText == null) {
						e.Cancel = true;
					}
					else if (cellValueInt < 1 || 99999 < cellValueInt) {
						e.Cancel = true;
					}

					if (e.Cancel) {
						this.univDGVN.Rows[e.RowIndex].ErrorText = "Start must be between 1 and 99999 inclusive";
					}

					break;

				case SIZE_COLUMN:
					if (cellValueText == null) {
						e.Cancel = true;
					}
					else if (cellValueInt < 1 || 512 < cellValueInt) {
						e.Cancel = true;
					}

					if (e.Cancel) {
						this.univDGVN.Rows[e.RowIndex].ErrorText = "Size must be between 1 and 512 inclusive";
					}

					break;

				case TTL_COLUMN:
					if (cellValueText == null) {
						e.Cancel = true;
					}
					else if (cellValueInt < 0 || 99 < cellValueInt) {
						e.Cancel = true;
					}

					if (e.Cancel) {
						this.univDGVN.Rows[e.RowIndex].ErrorText = "TTL must be between 0 and 99 inclusive";
					}

					break;
			}

			if (e.Cancel) {
				MessageBox.Show(
					this.univDGVN.Rows[e.RowIndex].ErrorText,
					"Cell Validation",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
			}
		}

		private void UnivDgvnDeleteRow(object sender, EventArgs e)
		{
			if (this.univDGVNCellEventArgs != null) {
				var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];

				if (!row.IsNewRow) {
					this.univDGVN.Rows.RemoveAt(row.Index);
				}
			}
		}

		private void UnivDgvnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			var columnIndex = univDGVN.CurrentCell.ColumnIndex;

			if (columnIndex == UNIVERSE_COLUMN || columnIndex == START_COLUMN || columnIndex == SIZE_COLUMN
			    || columnIndex == TTL_COLUMN) {
				// first remove the event handler (if previously added)
				e.Control.KeyPress -= this.NumTextBoxKeyPress;

				// now add our event handler
				e.Control.KeyPress += this.NumTextBoxKeyPress;
			}

			if (columnIndex == DESTINATION_COLUMN) {
				var control = e.Control as DataGridViewComboBoxEditingControl;

				if (control != null) {
					if (this.destinationToolTip == null) {
						this.destinationToolTip = new ToolTip();
					}

					this.destinationToolTip.SetToolTip(e.Control, "RightClick to add a new Unicast IP Address");

					if (this.destinationContextMenuStrip == null) {
						this.destinationContextMenuStrip = new ContextMenuStrip();
						this.destinationContextMenuStrip.Opening += this.DestinationContextMenuStripOpening;
					}

					control.ContextMenuStrip = this.destinationContextMenuStrip;
				}
			}
		}

		private void UnivDgvnInsertRow(object sender, EventArgs e)
		{
			if (this.univDGVNCellEventArgs != null) {
				var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];

				if (!row.IsNewRow) {
					this.univDGVN.Rows.Insert(row.Index, new object[] {false, "1", "1", "1", null, "1"});
				}
			}
		}

		private void UnivDgvnMoveRowDown(object sender, EventArgs e)
		{
			if (this.univDGVNCellEventArgs != null) {
				var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];
				var rowIndex = row.Index;

				if (!row.IsNewRow) {
					if (rowIndex < this.univDGVN.Rows.Count - 1) {
						if (!this.univDGVN.Rows[rowIndex + 1].IsNewRow) {
							this.univDGVN.Rows.RemoveAt(rowIndex);
							this.univDGVN.Rows.Insert(rowIndex + 1, row);
						}
					}
				}
			}
		}

		private void UnivDgvnMoveRowUp(object sender, EventArgs e)
		{
			if (this.univDGVNCellEventArgs != null) {
				var row = this.univDGVN.Rows[this.univDGVNCellEventArgs.RowIndex];
				var rowIndex = row.Index;

				if (!row.IsNewRow && rowIndex > 0) {
					this.univDGVN.Rows.RemoveAt(rowIndex);
					this.univDGVN.Rows.Insert(rowIndex - 1, row);
				}
			}
		}

		private void UnivDgvnInsertRow(object sender, DataGridViewRowEventArgs e)
		{
		}

		private void UnivDgvnDeleteRow(object sender, DataGridViewRowEventArgs e)
		{
		}

		private void univDGVN_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
		}

		private void SetupForm_Load(object sender, EventArgs e)
		{
		}
	}
}