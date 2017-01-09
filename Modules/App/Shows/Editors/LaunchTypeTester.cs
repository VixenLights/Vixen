using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;

namespace VixenModules.App.Shows
{
	public partial class LaunchTypeTester : BaseForm
	{
		public ShowItem ShowItem { get; set; }
		public LaunchAction action;

		public LaunchTypeTester(ShowItem showItem)
		{
			InitializeComponent();
			ShowItem = showItem;
			action = new LaunchAction(ShowItem);
			action.ActionComplete += OnActionComplete;
			action.Execute();
		}

		private delegate void OnActionCompleteDelegate(Object sender, EventArgs e);
		public void OnActionComplete(Object sender, EventArgs e) 
		{
			if (this.InvokeRequired)
				this.Invoke(new OnActionCompleteDelegate(OnActionComplete), sender, e);
			else
			{
				textBoxOutput.Text = action.ResultString;
				labelStatus.Text = "Status: Run Complete";
			}
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			action.ActionComplete -= OnActionComplete;
			action.Stop();
			Close();
		}

	}
}
