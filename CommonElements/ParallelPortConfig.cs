using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CommonElements
{
    public partial class ParallelPortConfig : Form
    {
        private int _OtherAddressIndex;
        public ParallelPortConfig(int portAddress)
        {
            InitializeComponent();
            _OtherAddressIndex = 3;
            switch (portAddress)
            {
                case 0x278:
                    portComboBox.SelectedIndex = 1;
                    break;

                case 0x378:
                    portComboBox.SelectedIndex = 0;
                    break;

                case 0x3bc:
                    portComboBox.SelectedIndex = 2;
                    break;

                case 0:
                    portComboBox.SelectedIndex = 0;
                    break;

                default:
                    portTextBox.Text = portAddress.ToString("X4");
                    portComboBox.SelectedIndex = 3;
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            portTextBox.Enabled = portComboBox.SelectedIndex == _OtherAddressIndex;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (portComboBox.SelectedIndex == _OtherAddressIndex)
            {
                try
                {
                    Convert.ToUInt16(portTextBox.Text, 0x10);
                }
                catch
                {
                    MessageBox.Show("The port number is not a valid hexadecimal number.", "ParallelPortConfig", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    base.DialogResult = DialogResult.None;
                }
            }
            
        }

        public ushort PortAddress
        {
            get
            {
                switch (portComboBox.SelectedIndex)
                {
                    case 0:
                        return 0x378;

                    case 1:
                        return 0x278;

                    case 2:
                        return 0x3bc;
                }
                return Convert.ToUInt16(portTextBox.Text, 0x10);
            }
        }
    }
}
