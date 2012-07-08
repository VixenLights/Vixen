using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenModules.Property.RGB
{
	public partial class RGBSetup : Form
	{
		private RGBData _data;

		public RGBSetup(RGBData data, ChannelNode[] nodes)
		{
			_data = data;
			InitializeComponent();

			List<RGBSetupChannelListing> comboBoxData = new List<RGBSetupChannelListing>();
			RGBSetupChannelListing emptyItem = new RGBSetupChannelListing(Guid.Empty, "None");
			comboBoxData.Add(emptyItem);
			foreach (ChannelNode node in nodes)
				comboBoxData.Add(new RGBSetupChannelListing(node.Id, node.Name));

			comboBoxR.DataSource = comboBoxData.ToArray();
			comboBoxR.DisplayMember = "Name";
			comboBoxR.ValueMember = "ID";
			comboBoxR.SelectedItem = emptyItem;
			foreach (RGBSetupChannelListing item in comboBoxData) {
				if (item.ID == data.RedChannelNode) {
					comboBoxR.SelectedItem = item;
					break;
				}
			}

			comboBoxG.DataSource = comboBoxData.ToArray();
			comboBoxG.DisplayMember = "Name";
			comboBoxG.ValueMember = "ID";
			comboBoxG.SelectedItem = emptyItem;
			foreach (RGBSetupChannelListing item in comboBoxData) {
				if (item.ID == data.GreenChannelNode) {
					comboBoxG.SelectedItem = item;
					break;
				}
			}

			comboBoxB.DataSource = comboBoxData.ToArray();
			comboBoxB.DisplayMember = "Name";
			comboBoxB.ValueMember = "ID";
			comboBoxB.SelectedItem = emptyItem;
			foreach (RGBSetupChannelListing item in comboBoxData) {
				if (item.ID == data.BlueChannelNode) {
					comboBoxB.SelectedItem = item;
					break;
				}
			}

			if (data.RGBType == RGBModelType.eIndividualRGBChannels)
				radioButtonMultiChannel.Checked = true;
			else
				radioButtonSingleChannel.Checked = true;
		}

		private void radioButton_CheckedChanged(object sender, EventArgs e)
		{
			groupBoxComponents.Enabled = radioButtonMultiChannel.Checked;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			_data.RedChannelNode = (comboBoxR.SelectedItem as RGBSetupChannelListing).ID;
			_data.GreenChannelNode = (comboBoxG.SelectedItem as RGBSetupChannelListing).ID;
			_data.BlueChannelNode = (comboBoxB.SelectedItem as RGBSetupChannelListing).ID;
			_data.RGBType = (radioButtonMultiChannel.Checked) ? RGBModelType.eIndividualRGBChannels : RGBModelType.eSingleRGBChannel;
		}
	}

	public class RGBSetupChannelListing
	{
		public RGBSetupChannelListing(Guid id, string name)
		{
			ID = id;
			Name = name;
		}

		public Guid ID { get; set; }
		public string Name { get; set; }
	}
}
