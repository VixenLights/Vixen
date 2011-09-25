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
			comboBoxData.Add(new RGBSetupChannelListing(Guid.Empty, "None"));
			foreach (ChannelNode node in nodes)
				comboBoxData.Add(new RGBSetupChannelListing(node.Id, node.Name));

			comboBoxR.DisplayMember = "Name";
			comboBoxR.ValueMember = "ID";
			comboBoxR.DataSource = comboBoxData;
			if (data.RedChannelNode != Guid.Empty) comboBoxR.SelectedValue = data.RedChannelNode;

			comboBoxG.DisplayMember = "Name";
			comboBoxG.ValueMember = "ID";
			comboBoxG.DataSource = comboBoxData;
			if (data.GreenChannelNode != Guid.Empty) comboBoxG.SelectedValue = data.GreenChannelNode;

			comboBoxB.DisplayMember = "Name";
			comboBoxB.ValueMember = "ID";
			comboBoxB.DataSource = comboBoxData;
			if (data.BlueChannelNode != Guid.Empty) comboBoxB.SelectedValue = data.BlueChannelNode;

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
		public Guid ID;
		public string Name;

		public RGBSetupChannelListing(Guid id, string name)
		{
			ID = id;
			Name = name;
		}
	}
}
