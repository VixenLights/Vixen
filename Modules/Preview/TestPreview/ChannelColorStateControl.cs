using System.Drawing;
using System.Windows.Forms;

namespace TestPreview {
	public partial class ChannelColorStateControl : UserControl {
		public ChannelColorStateControl(string channelName) {
			InitializeComponent();
			ChannelName = channelName;
		}

		public string ChannelName {
			set { label.Text = value; }
		}

		public Color ChannelColor {
			set { pictureBox.BackColor = value; }
		}
	}
}
