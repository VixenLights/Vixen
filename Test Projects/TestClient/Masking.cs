using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace TestClient {
    public partial class Masking : Form {
        private TimedSequence _sequence;

        public Masking(TimedSequence sequence) {
            InitializeComponent();
            _sequence = sequence;
            _AddFixturesToView(_sequence.Fixtures.ToArray());
        }

        private void _AddFixturesToView(Fixture[] fixtures) {
            // Update the fixture display.
            TreeNode fixtureNode;
            TreeNode channelNode;
            List<TreeNode> nodes = new List<TreeNode>();
            foreach(Fixture fixture in fixtures) {
                fixtureNode = new TreeNode(fixture.ChannelCount.ToString() + ((fixture.ChannelCount > 1) ? " channels" : " channel"));
                fixtureNode.Tag = fixture;
                fixtureNode.Checked = !fixture.Masked;
                foreach(Channel channel in fixture.Channels) {
                    channelNode = new TreeNode("Channel");
                    channelNode.Tag = channel;
                    channelNode.Checked = !channel.Masked;
                    fixtureNode.Nodes.Add(channelNode);
                }

                nodes.Add(fixtureNode);
            }
            // By adding the nodes to the tree at the end, the AfterCheck event isn't fired.
            treeView.Nodes.AddRange(nodes.ToArray());
        }

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e) {
            if(e.Node.Tag is Channel) {
                Channel channel = e.Node.Tag as Channel;
                channel.Masked = !e.Node.Checked;
            } else if(e.Node.Tag is Fixture) {
                Fixture fixture = e.Node.Tag as Fixture;
                fixture.Masked = !e.Node.Checked;
            }
        }

        private void Masking_FormClosing(object sender, FormClosingEventArgs e) {
            if(e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
