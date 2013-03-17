using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenModules.SequenceType.Vixen2x
{
	public partial class Vixen2xSequenceImporterChannelMapper : Form
	{
		private List<ChannelMapping> _mappings;

		public Vixen2xSequenceImporterChannelMapper(List<ChannelMapping> mappings)
		{
			InitializeComponent();

			_mappings = mappings;
		}

		private void Vixen2xSequenceImporterChannelMapper_Load(object sender, EventArgs e)
		{
			listViewMapping.BeginUpdate();

			listViewMapping.Items.Clear();
			
			foreach (ChannelMapping mapping in _mappings) {
				ListViewItem item = new ListViewItem(mapping.ChannelName);
				if (mapping.ElementNode != null)
					item.SubItems.Add(mapping.ElementNode.Name);
				listViewMapping.Items.Add(item);
			}

			listViewMapping.EndUpdate();
		}
	}


	public class ChannelMapping
	{
		public string ChannelName;
		public ElementNode ElementNode;

		public ChannelMapping(string channelName, ElementNode element)
		{
			ChannelName = channelName;
			ElementNode = element;
		}
	}
}
