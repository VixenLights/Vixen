using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.CustomProp
{
	public partial class AddMultipleChannels : Form
	{
		public AddMultipleChannels()
		{
			InitializeComponent();
		}
		public string TemplateName { get { return this.txtTemplate.Text.Contains("{0}") ? this.txtTemplate.Text : this.txtTemplate.Text + "{0}"; } }
		public int ChannelCount { get { return (int)this.numChannelsToAdd.Value; } }
	}
}
