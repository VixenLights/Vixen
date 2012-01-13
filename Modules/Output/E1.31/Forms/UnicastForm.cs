namespace VixenModules.Output.E131
{
    using System.Windows.Forms;

    public partial class UnicastForm : Form
    {
        public UnicastForm()
        {
            this.InitializeComponent();
        }

        public string IpAddrText
        {
            get
            {
                return this.ipTextBox.Text;
            }

            set
            {
                this.ipTextBox.Text = value;
            }
        }
    }
}