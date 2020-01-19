using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VersionControl
{
	public partial class Status : BaseForm
    {
        public Status()
        {
            InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
        }

        #region Delegate Methods
        private delegate void SetStatusTextDelegate(string text);
        private delegate void SetIntValueDelegate(int value);

        public void SetStatusText(string text)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetStatusTextDelegate(SetStatusText), text);
            else StatusText = text;
        }

	    public void SetMinimum(int value)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetIntValueDelegate(SetMinimum), value);
            else Minimum = value;
        }

        public void SetMaximum(int value)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetIntValueDelegate(SetMaximum), value);
            else Maximum = value;
        }

        public void SetValue(int value)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetIntValueDelegate(SetValue), value);
            else Value = value;
        }
        #endregion
   
        #region Properties
        public string StatusText
        {
            get { return this.label2.Text; }
            set
            {
                this.label2.Text = value;
                Application.DoEvents();
            }
        }

        public int Minimum
        {
            get { return this.progressBar1.Minimum; }
            set
            {
                this.progressBar1.Minimum = value;
                Application.DoEvents();

            }
        }
        public int Maximum
        {
            get { return this.progressBar1.Maximum; }
            set
            {
                this.progressBar1.Maximum = value;
                Application.DoEvents();

            }
        }
        public int Value
        {
            get { return this.progressBar1.Value; }
            set
            {
                this.progressBar1.Value = value;
                Application.DoEvents();

            }
        }
        #endregion
    }
}
