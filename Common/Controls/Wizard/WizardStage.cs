using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace Common.Controls.Wizard
{
	// a Wizard Stage is essentially a control, with a few extensions to determine if the stage can
	// move forward (or back), and an event to indicate when it has updated (and thus, the forward/back
	// permissions may have changed).
	public class WizardStage : UserControl
	{
		public WizardStage()
		{
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}

		public virtual bool CanMoveNext
		{
			get { return true; }
		}

		public virtual bool CanMovePrevious
		{
			get { return true; }
		}

		public virtual bool IsPreviousVisible
		{
			get { return true; }
		}

		public virtual bool IsCancelVisible
		{
			get { return true; }
		}

		public virtual void StageStart()
		{
		}

		public virtual async Task StageEnd()
		{
		}

		public virtual void StageCancelled()
		{
			
		}

		public event EventHandler WizardStageChanged;

		protected void _WizardStageChanged()
		{
			if (WizardStageChanged != null)
				WizardStageChanged(this, EventArgs.Empty);
		}
	}
}