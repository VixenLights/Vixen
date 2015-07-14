using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;

namespace Common.Controls
{
	public partial class MessageBoxForm : Form
	{
		public MessageBoxForm(string messageBoxData, string messageBoxTitle)
		{
			InitializeComponent();
			buttonOk.BackgroundImage = Resources.Properties.Resources.HeadingBackgroundImage;
			labelPrompt.Text = messageBoxData;
			this.Text = messageBoxTitle;
		}
	}
}