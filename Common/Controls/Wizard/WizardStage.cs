using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls.Wizard
{
	// a Wizard Stage is essentially a control, with a few extensions to determine if the stage can
	// move forward (or back), and an event to indicate when it has updated (and thus, the forward/back
	// permissions may have changed).
	public class WizardStage : UserControl
	{
		public virtual bool CanMoveNext { get { return true; } }
		public virtual bool CanMovePrevious { get { return true; } }

		public virtual void StageStart() { }
		public virtual void StageEnd() { }

		public event EventHandler WizardStageChanged;
		protected void _WizardStageChanged()
		{
			if (WizardStageChanged != null)
				WizardStageChanged(this, EventArgs.Empty);
		}
	}
}
