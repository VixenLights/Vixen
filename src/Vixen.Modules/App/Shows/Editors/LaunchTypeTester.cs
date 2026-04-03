using Common.Controls;

namespace VixenModules.App.Shows
{
	public partial class LaunchTypeTester : BaseForm
	{
		public ShowItem ShowItem { get; }
		private readonly LaunchAction _action;

		public LaunchTypeTester(ShowItem showItem)
		{
			InitializeComponent();
			ShowItem = showItem;
			_action = new LaunchAction(ShowItem);
			_action.ActionComplete += OnActionComplete;
			_action.Execute();
		}

		private delegate void OnActionCompleteDelegate(Object sender, EventArgs e);
		public void OnActionComplete(Object sender, EventArgs e) 
		{
			if (this.InvokeRequired)
				this.Invoke(new OnActionCompleteDelegate(OnActionComplete), sender, e);
			else
			{
				textBoxOutput.Text = _action.ResultString;
				labelStatus.Text = "Status: Run Complete";
			}
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			_action.ActionComplete -= OnActionComplete;
			_action.Stop();
			Close();
		}

	}
}
